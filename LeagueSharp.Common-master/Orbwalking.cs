namespace LeagueSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     This class offers everything related to auto-attacks and orbwalking.
    /// </summary>
    public static class Orbwalking
    {
        #region Static Fields

        /// <summary>
        /// An array of the last 3 targets as NetworkIDs, useful for 3-hit passives or thunderlord
        /// </summary>
        public static int[] LastTargets = new int[] {0,0,0};
        /// <summary>
        ///     <c>true</c> if the orbwalker will attack.
        /// </summary>
        public static bool Attack = true;

        /// <summary>
        ///     <c>true</c> if the orbwalker will skip the next attack.
        /// </summary>
        public static bool DisableNextAttack;

        /// <summary>
        ///     The last auto attack tick
        /// </summary>
        public static int LastAATick;

        /// <summary>
        ///     The tick the most recent attack command was sent.
        /// </summary>
        public static int LastAttackCommandT;

        /// <summary>
        ///     The last move command position
        /// </summary>
        public static Vector3 LastMoveCommandPosition = Vector3.Zero;

        /// <summary>
        ///     The tick the most recent move command was sent.
        /// </summary>
        public static int LastMoveCommandT;

        /// <summary>
        ///     <c>true</c> if the orbwalker will move.
        /// </summary>
        public static bool Move = true;

        /// <summary>
        ///     The champion name
        /// </summary>
        private static readonly string _championName;

        /// <summary>
        ///     The random
        /// </summary>
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        ///     Spells that reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
            {
                "dariusnoxiantacticsonh", "fiorae", "garenq", "gravesmove",
                "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge",
                "leonashieldofdaybreak", "luciane", "monkeykingdoubleattack",
                "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze",
                "netherblade", "gangplankqwrapper", "powerfist",
                "renektonpreexecute", "rengarq", "shyvanadoubleattack",
                "sivirw", "takedown", "talonnoxiandiplomacy",
                "trundletrollsmash", "vaynetumble", "vie", "volibearq",
                "xenzhaocombotarget", "yorickspectral", "reksaiq",
                "itemtitanichydracleave", "masochism", "illaoiw",
                "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds",
                "asheq", "camilleq", "camilleq2"
            };

        /// <summary>
        ///     Spells that are attacks even if they dont have the "attack" word in their name.
        /// </summary>
        private static readonly string[] Attacks =
            {
                "caitlynheadshotmissile", "frostarrow", "garenslash2",
                "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced",
                "renektonexecute", "renektonsuperexecute",
                "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust",
                "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff",
                "lucianpassiveshot"
            };

        /// <summary>
        ///     Spells that are not attacks even if they have the "attack" word in their name.
        /// </summary>
        private static readonly string[] NoAttacks =
            {
                "volleyattack", "volleyattackwithsound",
                "jarvanivcataclysmattack", "monkeykingdoubleattack",
                "shyvanadoubleattack", "shyvanadoubleattackdragon",
                "zyragraspingplantattack", "zyragraspingplantattack2",
                "zyragraspingplantattackfire", "zyragraspingplantattack2fire",
                "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
                "elisespiderlingbasicattack", "heimertyellowbasicattack",
                "heimertyellowbasicattack2", "heimertbluebasicattack",
                "annietibbersbasicattack", "annietibbersbasicattack2",
                "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
                "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
                "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
                "kindredwolfbasicattack", "gravesautoattackrecoil"
            };

        /// <summary>
        ///     Champs whose auto attacks can't be cancelled
        /// </summary>
        private static readonly string[] NoCancelChamps = { "Kalista" };

        /// <summary>
        ///     The player
        /// </summary>
        private static readonly Obj_AI_Hero Player;

        private static int _autoattackCounter;

        /// <summary>
        ///     The delay
        /// </summary>
        private static int _delay;

        /// <summary>
        ///     The last target
        /// </summary>
        private static AttackableUnit _lastTarget;

        /// <summary>
        ///     The minimum distance
        /// </summary>
        private static float _minDistance = 400;

        /// <summary>
        ///     <c>true</c> if the auto attack missile was launched from the player.
        /// </summary>
        private static bool _missileLaunched;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Orbwalking" /> class.
        /// </summary>
        static Orbwalking()
        {
            Player = ObjectManager.Player;
            _championName = Player.ChampionName;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnDoCast += Obj_AI_Base_OnDoCast;
            Spellbook.OnStopCast += SpellbookOnStopCast;

            if (_championName == "Rengar")
            {
                Obj_AI_Base.OnPlayAnimation += delegate(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
                    {
                        if (sender.IsMe && args.Animation == "Spell5")
                        {
                            var t = 0;

                            if (_lastTarget != null && _lastTarget.IsValid)
                            {
                                t += (int)Math.Min(ObjectManager.Player.Distance(_lastTarget) / 1.5f, 0.6f);
                            }

                            LastAATick = Utils.GameTimeTickCount - Game.Ping / 2 + t;
                        }
                    };
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate AfterAttackEvenH
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);

        /// <summary>
        ///     Delegate BeforeAttackEvenH
        /// </summary>
        /// <param name="args">The <see cref="BeforeAttackEventArgs" /> instance containing the event data.</param>
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);

        /// <summary>
        ///     Delegate OnAttackEvenH
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);

        /// <summary>
        ///     Delegate OnNonKillableMinionH
        /// </summary>
        /// <param name="minion">The minion.</param>
        public delegate void OnNonKillableMinionH(AttackableUnit minion);

        /// <summary>
        ///     Delegate OnTargetChangeH
        /// </summary>
        /// <param name="oldTarget">The old target.</param>
        /// <param name="newTarget">The new target.</param>
        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        #endregion

        #region Public Events

        /// <summary>
        ///     This event is fired after a unit finishes auto-attacking another unit (Only works with player for now).
        /// </summary>
        public static event AfterAttackEvenH AfterAttack;

        /// <summary>
        ///     This event is fired before the player auto attacks.
        /// </summary>
        public static event BeforeAttackEvenH BeforeAttack;

        /// <summary>
        ///     This event is fired when a unit is about to auto-attack another unit.
        /// </summary>
        public static event OnAttackEvenH OnAttack;

        /// <summary>
        ///     Occurs when a minion is not killable by an auto attack.
        /// </summary>
        public static event OnNonKillableMinionH OnNonKillableMinion;

        /// <summary>
        ///     Gets called on target changes
        /// </summary>
        public static event OnTargetChangeH OnTargetChange;

        #endregion

        #region Enums

        /// <summary>
        ///     The orbwalking mode.
        /// </summary>
        public enum OrbwalkingMode
        {
            /// <summary>
            ///     The orbwalker will only last hit minions.
            /// </summary>
            LastHit,

            /// <summary>
            ///     The orbwalker will alternate between last hitting and auto attacking champions.
            /// </summary>
            Mixed,

            /// <summary>
            ///     The orbwalker will clear the lane of minions as fast as possible while attempting to get the last hit.
            /// </summary>
            LaneClear,

            /// <summary>
            ///     The orbwalker will only attack the target.
            /// </summary>
            Combo,

            /// <summary>
            ///     The orbwalker will only last hit minions as late as possible.
            /// </summary>
            Freeze,

            /// <summary>
            ///     The orbwalker will only move.
            /// </summary>
            CustomMode,

            /// <summary>
            ///     The orbwalker does nothing.
            /// </summary>
            None
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns if the player's auto-attack is ready.
        /// </summary>
        /// <returns><c>true</c> if this instance can attack; otherwise, <c>false</c>.</returns>
        public static bool CanAttack()
        {
            if (Player.IsCastingInterruptableSpell())
            {
                return false;
            }
            
            if (Player.HasBuffOfType(BuffType.Blind) && Player.CharData.BaseSkinName != "Kalista")
            {
                return false;
            }

            if (Player.ChampionName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;
                if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + attackDelay
                    && Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return true;
                }

                return false;
            }

            if (Player.ChampionName == "Jhin")
            {
                if (Player.HasBuff("JhinPassiveReload"))
                {
                    return false;
                }
            }

            return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + Player.AttackDelay * 1000;
        }

        /// <summary>
        ///     Returns true if moving won't cancel the auto-attack.
        /// </summary>
        /// <param name="extraWindup">The extra windup.</param>
        /// <returns><c>true</c> if this instance can move the specified extra windup; otherwise, <c>false</c>.</returns>
        public static bool CanMove(float extraWindup, bool disableMissileCheck = false)
        {
            if (_missileLaunched && Orbwalker.MissileCheck && !disableMissileCheck)
            {
                return true;
            }

            var localExtraWindup = 0;
            if (_championName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
            {
                localExtraWindup = 200;
            }

            return NoCancelChamps.Contains(_championName)
                   || (Utils.GameTimeTickCount + Game.Ping / 2
                       >= LastAATick + Player.AttackCastDelay * 1000 + extraWindup + localExtraWindup);
        }

        /// <summary>
        ///     Returns the auto-attack range of the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.Single.</returns>
        public static float GetAttackRange(Obj_AI_Hero target)
        {
            var result = target.AttackRange + target.BoundingRadius;
            return result;
        }

        /// <summary>
        ///     Gets the last move position.
        /// </summary>
        /// <returns>Vector3.</returns>
        public static Vector3 GetLastMovePosition()
        {
            return LastMoveCommandPosition;
        }

        /// <summary>
        ///     Gets the last move time.
        /// </summary>
        /// <returns>System.Single.</returns>
        public static float GetLastMoveTime()
        {
            return LastMoveCommandT;
        }

        /// <summary>
        ///     Returns player auto-attack missile speed.
        /// </summary>
        /// <returns>System.Single.</returns>
        public static float GetMyProjectileSpeed()
        {
            return IsMelee(Player) || _championName == "Azir" || _championName == "Velkoz"
                   || _championName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                       ? float.MaxValue
                       : Player.BasicAttack.MissileSpeed;
        }

        /// <summary>
        ///     Returns the auto-attack range of local player with respect to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.Single.</returns>
        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null && Player.ChampionName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }
            var myRange = GetRealAutoAttackRange(target);
            return
                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(),
                    Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        /// <summary>
        ///     Returns true if the spellname is an auto-attack.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the name is an auto attack; otherwise, <c>false</c>.</returns>
        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower()))
                   || Attacks.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns true if the spellname resets the attack timer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is an auto attack reset; otherwise, <c>false</c>.</returns>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns true if the unit is melee
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns><c>true</c> if the specified unit is melee; otherwise, <c>false</c>.</returns>
        public static bool IsMelee(this Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        /// <summary>
        ///     Moves to the position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="holdAreaRadius">The hold area radius.</param>
        /// <param name="overrideTimer">if set to <c>true</c> [override timer].</param>
        /// <param name="useFixedDistance">if set to <c>true</c> [use fixed distance].</param>
        /// <param name="randomizeMinDistance">if set to <c>true</c> [randomize minimum distance].</param>
        public static void MoveTo(
            Vector3 position,
            float holdAreaRadius = 0,
            bool overrideTimer = false,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            var playerPosition = Player.ServerPosition;

            if (playerPosition.Distance(position, true) < holdAreaRadius * holdAreaRadius)
            {
                if (Player.Path.Length > 0)
                {
                    Player.IssueOrder(GameObjectOrder.Stop, playerPosition);
                    LastMoveCommandPosition = playerPosition;
                    LastMoveCommandT = Utils.GameTimeTickCount - 70;
                }
                return;
            }

            var point = position;

            if (Player.Distance(point, true) < 150 * 150)
            {
                point = playerPosition.Extend(
                    position,
                    randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance);
            }
            var angle = 0f;
            var currentPath = Player.GetWaypoints();
            if (currentPath.Count > 1 && currentPath.PathLength() > 100)
            {
                var movePath = Player.GetPath(point);

                if (movePath.Length > 1)
                {
                    var v1 = currentPath[1] - currentPath[0];
                    var v2 = movePath[1] - movePath[0];
                    angle = v1.AngleBetween(v2.To2D());
                    var distance = movePath.Last().To2D().Distance(currentPath.Last(), true);

                    if ((angle < 10 && distance < 500 * 500) || distance < 50 * 50)
                    {
                        return;
                    }
                }
            }

            if (Utils.GameTimeTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer
                && angle < 60)
            {
                return;
            }

            if (angle >= 60 && Utils.GameTimeTickCount - LastMoveCommandT < 60)
            {
                return;
            }

            Player.IssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
            LastMoveCommandT = Utils.GameTimeTickCount;
        }

        /// <summary>
        ///     Orbwalks a target while moving to Position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="position">The position.</param>
        /// <param name="extraWindup">The extra windup.</param>
        /// <param name="holdAreaRadius">The hold area radius.</param>
        /// <param name="useFixedDistance">if set to <c>true</c> [use fixed distance].</param>
        /// <param name="randomizeMinDistance">if set to <c>true</c> [randomize minimum distance].</param>
        public static void Orbwalk(
            AttackableUnit target,
            Vector3 position,
            float extraWindup = 90,
            float holdAreaRadius = 0,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            if (Utils.GameTimeTickCount - LastAttackCommandT < 70 + Math.Min(60, Game.Ping))
            {
                return;
            }

            try
            {
                if (target.IsValidTarget() && CanAttack() && Attack)
                {
                    DisableNextAttack = false;
                    FireBeforeAttack(target);

                    if (!DisableNextAttack)
                    {
                        if (!NoCancelChamps.Contains(_championName))
                        {
                            _missileLaunched = false;
                        }

                        if (Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                        {
                            LastAttackCommandT = Utils.GameTimeTickCount;
                            _lastTarget = target;
                        }

                        return;
                    }
                }

                if (CanMove(extraWindup) && Move)
                {
                    MoveTo(position, Math.Max(holdAreaRadius, 30), false, useFixedDistance, randomizeMinDistance);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        ///     Resets the Auto-Attack timer.
        /// </summary>
        public static void ResetAutoAttackTimer()
        {
            LastAATick = 0;
        }

        /// <summary>
        ///     Sets the minimum orbwalk distance.
        /// </summary>
        /// <param name="d">The d.</param>
        public static void SetMinimumOrbwalkDistance(float d)
        {
            _minDistance = d;
        }

        /// <summary>
        ///     Sets the movement delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public static void SetMovementDelay(int delay)
        {
            _delay = delay;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Pushes a target to the <see cref="LastTargets"/> list.
        /// </summary>
        /// <param name="networkId"></param>
        private static void PushLastTargets(int networkId)
        {
            LastTargets[2] = LastTargets[1];
            LastTargets[1] = LastTargets[0];
            LastTargets[0] = networkId;
        }

        /// <summary>
        ///     Fires the after attack event.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

        /// <summary>
        ///     Fires the before attack event.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            }
            else
            {
                DisableNextAttack = false;
            }
        }

        /// <summary>
        ///     Fires the on attack event.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        private static void FireOnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (OnAttack != null)
            {
                OnAttack(unit, target);
            }
        }

        /// <summary>
        ///     Fires the on non killable minion event.
        /// </summary>
        /// <param name="minion">The minion.</param>
        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        /// <summary>
        ///     Fires the on target switch event.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        /// <summary>
        ///     Fired when an auto attack is fired.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;
                if (ping <= 30) //First world problems kappa
                {
                    Utility.DelayAction.Add(30 - ping, () => Obj_AI_Base_OnDoCast_Delayed(sender, args));
                    return;
                }

                Obj_AI_Base_OnDoCast_Delayed(sender, args);
            }
        }

        /// <summary>
        ///     Fired 30ms after an auto attack is launched.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnDoCast_Delayed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (IsAutoAttackReset(args.SData.Name))
            {
                ResetAutoAttackTimer();
            }

            if (IsAutoAttack(args.SData.Name))
            {
                FireAfterAttack(sender, args.Target as AttackableUnit);
                _missileLaunched = true;
            }
        }

        /// <summary>
        ///     Handles the <see cref="E:ProcessSpell" /> event.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="Spell">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs Spell)
        {
            try
            {
                if (unit.IsMe)
                {
                    var spellName = Spell.SData.Name;

                    if (IsAutoAttackReset(spellName) && Spell.SData.SpellCastTime == 0)
                        ResetAutoAttackTimer();

                    if (!IsAutoAttack(spellName))
                        return;
                    
                    if (Spell.Target is Obj_AI_Base || Spell.Target is Obj_BarracksDampener || Spell.Target is Obj_HQ)
                    {
                        PushLastTargets(Spell.Target.NetworkId);

                        LastAATick = Utils.GameTimeTickCount - Game.Ping / 2;
                        _missileLaunched = false;
                        LastMoveCommandT = 0;
                        _autoattackCounter++;

                        if (Spell.Target is Obj_AI_Base)
                        {
                            var target = (Obj_AI_Base)Spell.Target;
                            if (target.IsValid)
                            {
                                FireOnTargetSwitch(target);
                                _lastTarget = target;
                            }
                        }
                    }
                }
                FireOnAttack(unit, _lastTarget);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Fired when the spellbook stops casting a spell.
        /// </summary>
        /// <param name="spellbook">The spellbook.</param>
        /// <param name="args">The <see cref="SpellbookStopCastEventArgs" /> instance containing the event data.</param>
        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && spellbook.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                ResetAutoAttackTimer();
            }
        }

        #endregion

        /// <summary>
        ///     The before attack event arguments.
        /// </summary>
        public class BeforeAttackEventArgs : EventArgs
        {
            #region Fields

            /// <summary>
            ///     The target
            /// </summary>
            public AttackableUnit Target;

            /// <summary>
            ///     The unit
            /// </summary>
            public Obj_AI_Base Unit = ObjectManager.Player;

            /// <summary>
            ///     <c>true</c> if the orbwalker should continue with the attack.
            /// </summary>
            private bool _process = true;

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="BeforeAttackEventArgs" /> should continue with the attack.
            /// </summary>
            /// <value><c>true</c> if the orbwalker should continue with the attack; otherwise, <c>false</c>.</value>
            public bool Process
            {
                get
                {
                    return this._process;
                }
                set
                {
                    DisableNextAttack = !value;
                    this._process = value;
                }
            }

            #endregion
        }

        /// <summary>
        ///     This class allows you to add an instance of "Orbwalker" to your assembly in order to control the orbwalking in an
        ///     easy way.
        /// </summary>
        public class Orbwalker : IDisposable
        {
            #region Constants

            /// <summary>
            ///     The lane clear wait time modifier.
            /// </summary>
            private const float LaneClearWaitTimeMod = 2f;

            #endregion

            #region Static Fields

            /// <summary>
            ///     The instances of the orbwalker.
            /// </summary>
            public static List<Orbwalker> Instances = new List<Orbwalker>();

            /// <summary>
            ///     The configuration
            /// </summary>
            private static Menu _config;

            #endregion

            #region Fields

            /// <summary>
            ///     The player
            /// </summary>
            private readonly Obj_AI_Hero Player;

            /// <summary>
            ///     The forced target
            /// </summary>
            private Obj_AI_Base _forcedTarget;

            /// <summary>
            ///     The orbalker mode
            /// </summary>
            private OrbwalkingMode _mode = OrbwalkingMode.None;

            /// <summary>
            ///     The orbwalking point
            /// </summary>
            private Vector3 _orbwalkingPoint;

            /// <summary>
            ///     The previous minion the orbwalker was targeting.
            /// </summary>
            private Obj_AI_Minion _prevMinion;

            /// <summary>
            ///     The name of the CustomMode if it is set.
            /// </summary>
            private string CustomModeName;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Orbwalker" /> class.
            /// </summary>
            /// <param name="attachToMenu">The menu the orbwalker should attach to.</param>
            public Orbwalker(Menu attachToMenu)
            {
                _config = attachToMenu;
                /* Drawings submenu */
                var drawings = new Menu("Drawings", "drawings");
                drawings.AddItem(
                    new MenuItem("AACircle", "AACircle").SetShared()
                        .SetValue(new Circle(true, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("AACircle2", "Enemy AA circle").SetShared()
                        .SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("HoldZone", "HoldZone").SetShared()
                        .SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(new MenuItem("AALineWidth", "Line Width")).SetShared().SetValue(new Slider(2, 1, 6));
                drawings.AddItem(new MenuItem("LastHitHelper", "Last Hit Helper").SetShared().SetValue(false));
                _config.AddSubMenu(drawings);

                /* Misc options */
                var misc = new Menu("Misc", "Misc");
                misc.AddItem(
                    new MenuItem("HoldPosRadius", "Hold Position Radius").SetShared().SetValue(new Slider(50, 50, 250)));
                misc.AddItem(new MenuItem("PriorizeFarm", "Prioritize farm over harass").SetShared().SetValue(true));
                misc.AddItem(new MenuItem("PrioritizeCasters", "Attack caster minions first").SetShared().SetValue(false));
                misc.AddItem(new MenuItem("AttackWards", "Auto attack wards").SetShared().SetValue(false));
                misc.AddItem(new MenuItem("AttackPetsnTraps", "Auto attack pets & traps").SetShared().SetValue(true));
                misc.AddItem(
                    new MenuItem("AttackGPBarrel", "Auto attack gangplank barrel").SetShared()
                        .SetValue(new StringList(new[] { "Combo and Farming", "Farming", "No" }, 1)));
                misc.AddItem(new MenuItem("Smallminionsprio", "Jungle clear small first").SetShared().SetValue(false));
                misc.AddItem(
                    new MenuItem("FocusMinionsOverTurrets", "Focus minions over objectives").SetShared()
                        .SetValue(new KeyBind('M', KeyBindType.Toggle)));

                _config.AddSubMenu(misc);

                /* Missile check */
                _config.AddItem(new MenuItem("MissileCheck", "Use Missile Check").SetShared().SetValue(true));

                /* Delay sliders */
                _config.AddItem(
                    new MenuItem("ExtraWindup", "Extra windup time").SetShared().SetValue(new Slider(80, 0, 200)));
                _config.AddItem(new MenuItem("FarmDelay", "Farm delay").SetShared().SetValue(new Slider(0, 0, 200)));

                /*Load the menu*/
                _config.AddItem(
                    new MenuItem("LastHit", "Last hit").SetShared().SetValue(new KeyBind('X', KeyBindType.Press)));

                _config.AddItem(new MenuItem("Farm", "Mixed").SetShared().SetValue(new KeyBind('C', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("Freeze", "Freeze").SetShared().SetValue(new KeyBind('N', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("LaneClear", "LaneClear").SetShared().SetValue(new KeyBind('V', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("Orbwalk", "Combo").SetShared().SetValue(new KeyBind(32, KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("StillCombo", "Combo without moving").SetShared()
                        .SetValue(new KeyBind('N', KeyBindType.Press)));
                _config.Item("StillCombo").ValueChanged +=
                    (sender, args) => { Move = !args.GetNewValue<KeyBind>().Active; };

                this.Player = ObjectManager.Player;
                Game.OnUpdate += this.GameOnOnGameUpdate;
                Drawing.OnDraw += this.DrawingOnOnDraw;
                Instances.Add(this);
            }

            #endregion

            #region Public Properties
            
            /// <summary>
            ///     Gets a value indicating whether the orbwalker is orbwalking by checking the missiles.
            /// </summary>
            /// <value><c>true</c> if the orbwalker is orbwalking by checking the missiles; otherwise, <c>false</c>.</value>
            public static bool MissileCheck
            {
                get
                {
                    return _config.Item("MissileCheck").GetValue<bool>();
                }
            }

            /// <summary>
            ///     Gets or sets the active mode.
            /// </summary>
            /// <value>The active mode.</value>
            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (this._mode != OrbwalkingMode.None)
                    {
                        return this._mode;
                    }

                    if (_config.Item("Orbwalk").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("StillCombo").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("LaneClear").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LaneClear;
                    }

                    if (_config.Item("Farm").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Mixed;
                    }

                    if (_config.Item("Freeze").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Freeze;
                    }

                    if (_config.Item("LastHit").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LastHit;
                    }

                    if (_config.Item(this.CustomModeName) != null
                        && _config.Item(this.CustomModeName).GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }

                    return OrbwalkingMode.None;
                }
                set
                {
                    this._mode = value;
                }
            }

            #endregion

            #region Properties

            /// <summary>
            ///     Gets the farm delay.
            /// </summary>
            /// <value>The farm delay.</value>
            private int FarmDelay
            {
                get
                {
                    return _config.Item("FarmDelay").GetValue<Slider>().Value;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Menu.Remove(_config);
                Game.OnUpdate -= this.GameOnOnGameUpdate;
                Drawing.OnDraw -= this.DrawingOnOnDraw;
                Instances.Remove(this);
            }

            /// <summary>
            ///     Forces the orbwalker to attack the set target if valid and in range.
            /// </summary>
            /// <param name="target">The target.</param>
            public void ForceTarget(Obj_AI_Base target)
            {
                this._forcedTarget = target;
            }

            /// <summary>
            ///     Gets the target.
            /// </summary>
            /// <returns>AttackableUnit.</returns>
            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;
                var mode = this.ActiveMode;

                if ((mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LaneClear)
                    && !_config.Item("PriorizeFarm").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target != null && this.InAutoAttackRange(target))
                    {
                        return target;
                    }
                }

                //GankPlank barrels
                var attackGankPlankBarrels = _config.Item("AttackGPBarrel").GetValue<StringList>().SelectedIndex;
                if (attackGankPlankBarrels != 2
                    && (attackGankPlankBarrels == 0
                        || (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed
                            || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)))
                {
                    var enemyGangPlank =
                        HeroManager.Enemies.FirstOrDefault(
                            e => e.ChampionName.Equals("gangplank", StringComparison.InvariantCultureIgnoreCase));

                    if (enemyGangPlank != null)
                    {
                        var barrels =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    minion =>
                                    minion.Team == GameObjectTeam.Neutral
                                    && minion.CharData.BaseSkinName == "gangplankbarrel" && minion.IsHPBarRendered
                                    && minion.IsValidTarget() && this.InAutoAttackRange(minion));

                        foreach (var barrel in barrels)
                        {
                            if (barrel.Health <= 1f)
                            {
                                return barrel;
                            }

                            var t = (int)(this.Player.AttackCastDelay * 1000) + Game.Ping / 2
                                    + 1000 * (int)Math.Max(0, this.Player.Distance(barrel) - this.Player.BoundingRadius)
                                    / (int)GetMyProjectileSpeed();

                            var barrelBuff =
                                barrel.Buffs.FirstOrDefault(
                                    b =>
                                    b.Name.Equals("gangplankebarrelactive", StringComparison.InvariantCultureIgnoreCase));

                            if (barrelBuff != null && barrel.Health <= 2f)
                            {
                                var healthDecayRate = enemyGangPlank.Level >= 13
                                                          ? 0.5f
                                                          : (enemyGangPlank.Level >= 7 ? 1f : 2f);
                                var nextHealthDecayTime = Game.Time < barrelBuff.StartTime + healthDecayRate
                                                              ? barrelBuff.StartTime + healthDecayRate
                                                              : barrelBuff.StartTime + healthDecayRate * 2;

                                if (nextHealthDecayTime <= Game.Time + t / 1000f)
                                {
                                    return barrel;
                                }
                            }
                        }

                        if (barrels.Any())
                        {
                            return null;
                        }
                    }
                }

                /*Killable Minion*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit
                    || mode == OrbwalkingMode.Freeze)
                {
                    var MinionList =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(minion => minion.IsValidTarget() && this.InAutoAttackRange(minion))
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenBy(minion => minion.Health)
                            .ThenByDescending(minion => minion.MaxHealth);

                    foreach (var minion in MinionList)
                    {
                        var t = (int)(this.Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2
                                + 1000 * (int)Math.Max(0, this.Player.Distance(minion) - this.Player.BoundingRadius)
                                / (int)GetMyProjectileSpeed();

                        if (mode == OrbwalkingMode.Freeze)
                        {
                            t += 200 + Game.Ping / 2;
                        }

                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, this.FarmDelay);

                        if (minion.Team != GameObjectTeam.Neutral && this.ShouldAttackMinion(minion))
                        {
                            var damage = this.Player.GetAutoAttackDamage(minion, true);
                            var killable = predHealth <= damage;

                            if (mode == OrbwalkingMode.Freeze)
                            {
                                if (minion.Health < 50 || predHealth <= 50)
                                {
                                    return minion;
                                }
                            }
                            else
                            {
                                if (predHealth <= 0)
                                {
                                    FireOnNonKillableMinion(minion);
                                }

                                if (killable)
                                {
                                    return minion;
                                }
                            }
                        }
                    }
                }

                //Forced target
                if (this._forcedTarget.IsValidTarget() && this.InAutoAttackRange(this._forcedTarget))
                {
                    return this._forcedTarget;
                }

                /* turrets / inhibitors / nexus */
                if (mode == OrbwalkingMode.LaneClear
                    && (!_config.Item("FocusMinionsOverTurrets").GetValue<KeyBind>().Active
                        || !MinionManager.GetMinions(
                            ObjectManager.Player.Position,
                            GetRealAutoAttackRange(ObjectManager.Player)).Any()))
                {
                    /* turrets */
                    foreach (var turret in
                        ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* inhibitor */
                    foreach (var turret in
                        ObjectManager.Get<Obj_BarracksDampener>()
                            .Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* nexus */
                    foreach (var nexus in
                        ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                    {
                        return nexus;
                    }
                }

                /*Champions*/
                if (mode != OrbwalkingMode.LastHit)
                {
                    if (mode != OrbwalkingMode.LaneClear || !this.ShouldWait())
                    {
                        var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                        if (target.IsValidTarget() && this.InAutoAttackRange(target))
                        {
                            return target;
                        }
                    }
                }

                /*Jungle minions*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    var jminions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                mob =>
                                mob.IsValidTarget() && mob.Team == GameObjectTeam.Neutral && this.InAutoAttackRange(mob)
                                && mob.CharData.BaseSkinName != "gangplankbarrel" && mob.Name != "WardCorpse");

                    result = _config.Item("Smallminionsprio").GetValue<bool>()
                                 ? jminions.MinOrDefault(mob => mob.MaxHealth)
                                 : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                    {
                        return result;
                    }
                }

                /* UnderTurret Farming */
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit
                    || mode == OrbwalkingMode.Freeze)
                {
                    var closestTower =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .MinOrDefault(t => t.IsAlly && !t.IsDead ? this.Player.Distance(t, true) : float.MaxValue);

                    if (closestTower != null && this.Player.Distance(closestTower, true) < 1500 * 1500)
                    {
                        Obj_AI_Minion farmUnderTurretMinion = null;
                        Obj_AI_Minion noneKillableMinion = null;
                        // return all the minions underturret in auto attack range
                        var minions =
                            MinionManager.GetMinions(this.Player.Position, this.Player.AttackRange + 200)
                                .Where(
                                    minion =>
                                    this.InAutoAttackRange(minion) && closestTower.Distance(minion, true) < 900 * 900)
                                .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                                .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                                .ThenByDescending(minion => minion.MaxHealth)
                                .ThenByDescending(minion => minion.Health);
                        if (minions.Any())
                        {
                            // get the turret aggro minion
                            var turretMinion =
                                minions.FirstOrDefault(
                                    minion =>
                                    minion is Obj_AI_Minion && HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                            if (turretMinion != null)
                            {
                                var hpLeftBeforeDie = 0;
                                var hpLeft = 0;
                                var turretAttackCount = 0;
                                var turretStarTick = HealthPrediction.TurretAggroStartTick(
                                    turretMinion as Obj_AI_Minion);
                                // from healthprediction (don't blame me :S)
                                var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000)
                                                     + 1000
                                                     * Math.Max(
                                                         0,
                                                         (int)
                                                         (turretMinion.Distance(closestTower)
                                                          - closestTower.BoundingRadius))
                                                     / (int)(closestTower.BasicAttack.MissileSpeed + 70);
                                // calculate the HP before try to balance it
                                for (float i = turretLandTick + 50;
                                     i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50;
                                     i = i + closestTower.AttackDelay * 1000)
                                {
                                    var time = (int)i - Utils.GameTimeTickCount + Game.Ping / 2;
                                    var predHP =
                                        (int)
                                        HealthPrediction.LaneClearHealthPrediction(turretMinion, time > 0 ? time : 0);
                                    if (predHP > 0)
                                    {
                                        hpLeft = predHP;
                                        turretAttackCount += 1;
                                        continue;
                                    }
                                    hpLeftBeforeDie = hpLeft;
                                    hpLeft = 0;
                                    break;
                                }
                                // calculate the hits is needed and possibilty to balance
                                if (hpLeft == 0 && turretAttackCount != 0 && hpLeftBeforeDie != 0)
                                {
                                    var damage = (int)this.Player.GetAutoAttackDamage(turretMinion, true);
                                    var hits = hpLeftBeforeDie / damage;
                                    var timeBeforeDie = turretLandTick
                                                        + (turretAttackCount + 1)
                                                        * (int)(closestTower.AttackDelay * 1000)
                                                        - Utils.GameTimeTickCount;
                                    var timeUntilAttackReady = LastAATick + (int)(this.Player.AttackDelay * 1000)
                                                               > Utils.GameTimeTickCount + Game.Ping / 2 + 25
                                                                   ? LastAATick + (int)(this.Player.AttackDelay * 1000)
                                                                     - (Utils.GameTimeTickCount + Game.Ping / 2 + 25)
                                                                   : 0;
                                    var timeToLandAttack = this.Player.IsMelee
                                                               ? this.Player.AttackCastDelay * 1000
                                                               : this.Player.AttackCastDelay * 1000
                                                                 + 1000
                                                                 * Math.Max(
                                                                     0,
                                                                     turretMinion.Distance(this.Player)
                                                                     - this.Player.BoundingRadius)
                                                                 / this.Player.BasicAttack.MissileSpeed;
                                    if (hits >= 1
                                        && hits * this.Player.AttackDelay * 1000 + timeUntilAttackReady
                                        + timeToLandAttack < timeBeforeDie)
                                    {
                                        farmUnderTurretMinion = turretMinion as Obj_AI_Minion;
                                    }
                                    else if (hits >= 1
                                             && hits * this.Player.AttackDelay * 1000 + timeUntilAttackReady
                                             + timeToLandAttack > timeBeforeDie)
                                    {
                                        noneKillableMinion = turretMinion as Obj_AI_Minion;
                                    }
                                }
                                else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                                {
                                    noneKillableMinion = turretMinion as Obj_AI_Minion;
                                }
                                // should wait before attacking a minion.
                                if (this.ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }
                                if (farmUnderTurretMinion != null)
                                {
                                    return farmUnderTurretMinion;
                                }
                                // balance other minions
                                foreach (var minion in
                                    minions.Where(
                                        x =>
                                        x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion
                                        && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)))
                                {
                                    var playerDamage = (int)this.Player.GetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                    var leftHP = (int)minion.Health % turretDamage;
                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }
                                // late game
                                var lastminion =
                                    minions.LastOrDefault(
                                        x =>
                                        x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion
                                        && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (1f / this.Player.AttackDelay >= 1f
                                        && (int)(turretAttackCount * closestTower.AttackDelay / this.Player.AttackDelay)
                                        * this.Player.GetAutoAttackDamage(lastminion) > lastminion.Health)
                                    {
                                        return lastminion;
                                    }
                                    if (minions.Count() >= 5 && 1f / this.Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            else
                            {
                                if (this.ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }
                                // balance other minions
                                foreach (var minion in
                                    minions.Where(
                                        x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion))
                                    )
                                {
                                    if (closestTower != null)
                                    {
                                        var playerDamage = (int)this.Player.GetAutoAttackDamage(minion);
                                        var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                        var leftHP = (int)minion.Health % turretDamage;
                                        if (leftHP > playerDamage)
                                        {
                                            return minion;
                                        }
                                    }
                                }
                                //late game
                                var lastminion =
                                    minions.LastOrDefault(
                                        x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (minions.Count() >= 5 && 1f / this.Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            return null;
                        }
                    }
                }

                /*Lane Clear minions*/
                if (mode == OrbwalkingMode.LaneClear)
                {
                    if (!this.ShouldWait())
                    {
                        if (this._prevMinion.IsValidTarget() && this.InAutoAttackRange(this._prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(
                                this._prevMinion,
                                (int) (this.Player.AttackDelay*1000*LaneClearWaitTimeMod),
                                this.FarmDelay);
                            if (predHealth >= 2*this.Player.GetAutoAttackDamage(this._prevMinion)
                                || Math.Abs(predHealth - this._prevMinion.Health) < float.Epsilon)
                            {
                                return this._prevMinion;
                            }
                        }

                        var results = (from minion in
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    minion =>
                                        minion.IsValidTarget() && this.InAutoAttackRange(minion)
                                        && this.ShouldAttackMinion(minion))
                            let predHealth =
                                HealthPrediction.LaneClearHealthPrediction(
                                    minion,
                                    (int) (this.Player.AttackDelay*1000*LaneClearWaitTimeMod),
                                    this.FarmDelay)
                            where
                                predHealth >= 2*this.Player.GetAutoAttackDamage(minion)
                                || Math.Abs(predHealth - minion.Health) < float.Epsilon
                            select minion);

                        result = results.MaxOrDefault(m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);

                        if (_config.Item("PrioritizeCasters").GetValue<bool>())
                        {
                            result =
                                results.OrderByDescending(
                                    m =>
                                        m.CharData.BaseSkinName.Contains("Ranged"))
                                    .FirstOrDefault();
                        }

                        if (result != null)
                        {
                            this._prevMinion = (Obj_AI_Minion) result;
                        }
                    }
                }

                return result;
            }

            /// <summary>
            ///     Determines if a target is in auto attack range.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <returns><c>true</c> if a target is in auto attack range, <c>false</c> otherwise.</returns>
            public virtual bool InAutoAttackRange(AttackableUnit target)
            {
                return Orbwalking.InAutoAttackRange(target);
            }

            /// <summary>
            ///     Registers the Custom Mode of the Orbwalker. Useful for adding a flee mode and such.
            /// </summary>
            /// <param name="name">The name of the mode Ex. "Myassembly.FleeMode" </param>
            /// <param name="displayname">The name of the mode in the menu. Ex. Flee</param>
            /// <param name="key">The default key for this mode.</param>
            public virtual void RegisterCustomMode(string name, string displayname, uint key)
            {
                this.CustomModeName = name;
                if (_config.Item(name) == null)
                {
                    _config.AddItem(
                        new MenuItem(name, displayname).SetShared().SetValue(new KeyBind(key, KeyBindType.Press)));
                }
            }

            /// <summary>
            ///     Enables or disables the auto-attacks.
            /// </summary>
            /// <param name="b">if set to <c>true</c> the orbwalker will attack units.</param>
            public void SetAttack(bool b)
            {
                Attack = b;
            }

            /// <summary>
            ///     Enables or disables the movement.
            /// </summary>
            /// <param name="b">if set to <c>true</c> the orbwalker will move.</param>
            public void SetMovement(bool b)
            {
                Move = b;
            }

            /// <summary>
            ///     Forces the orbwalker to move to that point while orbwalking (Game.CursorPos by default).
            /// </summary>
            /// <param name="point">The point.</param>
            public void SetOrbwalkingPoint(Vector3 point)
            {
                this._orbwalkingPoint = point;
            }

            /// <summary>
            ///     Determines if the orbwalker should wait before attacking a minion.
            /// </summary>
            /// <returns><c>true</c> if the orbwalker should wait before attacking a minion, <c>false</c> otherwise.</returns>
            public bool ShouldWait()
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral
                            && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false)
                            && HealthPrediction.LaneClearHealthPrediction(
                                minion,
                                (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod),
                                this.FarmDelay) <= this.Player.GetAutoAttackDamage(minion));
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Fired when the game is drawn.
            /// </summary>
            /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
            private void DrawingOnOnDraw(EventArgs args)
            {
                if (_config.Item("AACircle").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        this.Player.Position,
                        GetRealAutoAttackRange(null) + 65,
                        _config.Item("AACircle").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value);
                }
                if (_config.Item("AACircle2").GetValue<Circle>().Active)
                {
                    foreach (var target in
                        HeroManager.Enemies.FindAll(target => target.IsValidTarget(1175)))
                    {
                        Render.Circle.DrawCircle(
                            target.Position,
                            GetAttackRange(target),
                            _config.Item("AACircle2").GetValue<Circle>().Color,
                            _config.Item("AALineWidth").GetValue<Slider>().Value);
                    }
                }

                if (_config.Item("HoldZone").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        this.Player.Position,
                        _config.Item("HoldPosRadius").GetValue<Slider>().Value,
                        _config.Item("HoldZone").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value,
                        true);
                }
                _config.Item("FocusMinionsOverTurrets")
                    .Permashow(_config.Item("FocusMinionsOverTurrets").GetValue<KeyBind>().Active);

                if (_config.Item("LastHitHelper").GetValue<bool>())
                {
                    foreach (var minion in
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x => x.Name.ToLower().Contains("minion") && x.IsHPBarRendered && x.IsValidTarget(1000)))
                    {
                        if (minion.Health < ObjectManager.Player.GetAutoAttackDamage(minion, true))
                        {
                            Render.Circle.DrawCircle(minion.Position, 50, Color.LimeGreen);
                        }
                    }
                }
            }

            /// <summary>
            ///     Fired when the game is updated.
            /// </summary>
            /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
            private void GameOnOnGameUpdate(EventArgs args)
            {
                try
                {
                    if (this.ActiveMode == OrbwalkingMode.None)
                    {
                        return;
                    }

                    //Prevent canceling important spells
                    if (this.Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }

                    var target = this.GetTarget();
                    Orbwalk(
                        target,
                        this._orbwalkingPoint.To2D().IsValid() ? this._orbwalkingPoint : Game.CursorPos,
                        _config.Item("ExtraWindup").GetValue<Slider>().Value,
                        Math.Max(_config.Item("HoldPosRadius").GetValue<Slider>().Value, 30));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            /// <summary>
            ///     Returns if a minion should be attacked
            /// </summary>
            /// <param name="minion">The <see cref="Obj_AI_Minion" /></param>
            /// <param name="includeBarrel">Include Gangplank Barrel</param>
            /// <returns><c>true</c> if the minion should be attacked; otherwise, <c>false</c>.</returns>
            private bool ShouldAttackMinion(Obj_AI_Minion minion)
            {
                if (minion.Name == "WardCorpse" || minion.CharData.BaseSkinName == "jarvanivstandard")
                {
                    return false;
                }

                if (MinionManager.IsWard(minion))
                {
                    return _config.Item("AttackWards").IsActive();
                }

                return (_config.Item("AttackPetsnTraps").GetValue<bool>() || MinionManager.IsMinion(minion))
                       && minion.CharData.BaseSkinName != "gangplankbarrel";
            }

            private bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion)
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                            (noneKillableMinion != null ? noneKillableMinion.NetworkId != minion.NetworkId : true)
                            && minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral
                            && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false)
                            && HealthPrediction.LaneClearHealthPrediction(
                                minion,
                                (int)
                                (this.Player.AttackDelay * 1000
                                 + (this.Player.IsMelee
                                        ? this.Player.AttackCastDelay * 1000
                                        : this.Player.AttackCastDelay * 1000
                                          + 1000 * (this.Player.AttackRange + 2 * this.Player.BoundingRadius)
                                          / this.Player.BasicAttack.MissileSpeed)),
                                this.FarmDelay) <= this.Player.GetAutoAttackDamage(minion));
            }

            #endregion
        }
    }
}
