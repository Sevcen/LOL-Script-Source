namespace Aimtec.SDK.Orbwalking
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Menu.Config;
    using Aimtec.SDK.Prediction.Health;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util;
    using Aimtec.SDK.Events;

    internal class OrbwalkingImpl : AOrbwalker
    {
        #region Fields

        /// <summary>
        ///     The time the last attack command was sent (determined locally)
        /// </summary>
        protected float LastAttackCommandSentTime;

        #endregion

        #region Constructors and Destructors

        internal OrbwalkingImpl()
        {
            this.Initialize();
        }

        #endregion

        #region Public Properties

        public float AnimationTime => Player.AttackCastDelay * 1000;

        public float AttackCoolDownTime
            =>
                (Player.ChampionName.Equals("Graves")
                     ? 1.07402968406677f * Player.AttackDelay - 0.716238141059875f
                     : Player.AttackDelay) * 1000 - this.AttackDelayReduction;

        public override bool IsWindingUp
        {
            get
            {
                var detectionTime = Math.Max(this.ServerAttackDetectionTick, this.LastAttackCommandSentTime);
                return Game.TickCount + Game.Ping / 2 - detectionTime <= this.WindUpTime;
            }
        }

        public override float WindUpTime => this.AnimationTime + this.ExtraWindUp;

        #endregion

        #region Properties

        protected bool AttackReady => Game.TickCount + Game.Ping / 2 - this.ServerAttackDetectionTick
            >= this.AttackCoolDownTime;

        private bool Attached { get; set; }

        private int AttackDelayReduction => this.Config["Advanced"]["attackDelayReduction"].Value;

        private bool DrawAttackRange => this.Config["Drawings"]["drawAttackRange"].Enabled;

        private bool DrawHoldPosition => this.Config["Drawings"]["drawHoldRadius"].Enabled;

        private int ExtraWindUp => this.Config["Misc"]["extraWindup"].Value;

        /// <summary>
        ///     Special auto attack names that do not trigger OnProcessAutoAttack
        /// </summary>
        private string[] SpecialAttacks =
        {
            "caitlynheadshotmissile",
            "goldcardpreattack",
            "redcardpreattack",
            "bluecardpreattack",
            "viktorqbuff"
        };

        /// <summary>
        ///     Gets or sets the Forced Target
        /// </summary>
        private AttackableUnit ForcedTarget { get; set; }

        // TODO this completely breaks the modular design, Orbwalker and health prediction shouldnt be tightly coupled!
        private HealthPredictionImplB HealthPred { get; set;  } 

        //Menu Getters
        private int HoldPositionRadius => this.Config["Misc"]["holdPositionRadius"].Value;

        private AttackableUnit LastTarget { get; set; }

        //Members
        private float ServerAttackDetectionTick { get; set; }

        private Obj_AI_Hero GangPlank { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Attach(IMenu menu)
        {
            if (!this.Attached)
            {
                this.Attached = true;
                menu.Add(this.Config);
                Obj_AI_Base.OnProcessAutoAttack += this.ObjAiHeroOnProcessAutoAttack;
                Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
                Game.OnUpdate += this.Game_OnUpdate;
                SpellBook.OnStopCast += this.SpellBook_OnStopCast;
                Render.OnRender += this.RenderManager_OnRender;
            }

            else
            {
                this.Logger.Info("This Orbwalker instance is already attached to a Menu.");
            }
        }

        public override bool Attack(AttackableUnit target)
        {
            var preAttackargs = this.FirePreAttack(target);

            if (!preAttackargs.Cancel)
            {
                AttackableUnit targetToAttack = preAttackargs.Target;

                if (this.ForcedTarget != null)
                {
                    targetToAttack = this.ForcedTarget;
                }

                if (Player.IssueOrder(OrderType.AttackUnit, targetToAttack))
                {
                    this.LastAttackCommandSentTime = Game.TickCount;
                    return true;
                }
            }

            return false;
        }

        public bool BlindCheck()
        {
            if (!this.Config["Misc"]["noBlindAA"].Enabled)
            {
                return true;
            }

            if (!Player.ChampionName.Equals("Kalista") && !Player.ChampionName.Equals("Twitch"))
            {
                if (Player.BuffManager.HasBuffOfType(BuffType.Blind))
                {
                    return false;
                }
            }

            return true;
        }

    
        public bool IsValidAttackableObject(AttackableUnit unit)
        {
            //Valid check
            if (!unit.IsValidAutoRange())
            {
                return false;
            }

            if (unit is Obj_AI_Hero || unit is Obj_AI_Turret || unit.Type == GameObjectType.obj_BarracksDampener || unit.Type == GameObjectType.obj_HQ)
            {
                return true;
            }

            //J4 flag
            if (unit.Name.Contains("Beacon"))
            {
                return false;
            }

            var minion = unit as Obj_AI_Minion;

            if (minion == null)
            {
                return false;
            }

            var name = minion.UnitSkinName.ToLower();

            if (!this.Config["Misc"]["attackPlants"].Enabled && name.Contains("sru_plant_"))
            {
                return false;
            }

            if (!this.Config["Misc"]["attackWards"].Enabled && name.Contains("ward"))
            {
                return false;
            }

            if (this.GangPlank != null)
            {
                if (name.Contains("gangplankbarrel"))
                {
                    if (!this.Config["Misc"]["attackBarrels"].Enabled)
                    {
                        return false;
                    }

                    //dont attack ally barrels
                    if (this.GangPlank.IsAlly)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool CanAttack()
        {
            return this.CanAttack(this.GetActiveMode());
        }

        public bool CanAttack(OrbwalkerMode mode)
        {
            if (mode == null)
            {
                return false;
            }

            if (!this.AttackingEnabled || !mode.AttackingEnabled)
            {
                return false;
            }

            if (Player.HasBuffOfType(BuffType.Polymorph))
            {
                return false;
            }

            if (!this.BlindCheck())
            {
                return false;
            }

            if (Player.ChampionName.Equals("Jhin") && Player.HasBuff("JhinPassiveReload"))
            {
                return false;
            }
            
            if (Player.ChampionName.Equals("Graves") && !Player.HasBuff("GravesBasicAttackAmmo1"))
            {
                return false;
            }

            if (this.NoCancelChamps.Contains(Player.ChampionName))
            {
                return true;
            }

            if (this.IsWindingUp)
            {
                return false;
            }

            return this.AttackReady;
        }

        public override bool CanMove()
        {
            return this.CanMove(this.GetActiveMode());
        }

        public bool CanMove(OrbwalkerMode mode)
        {
            if (mode == null)
            {
                return false;
            }

            if (!this.MovingEnabled || !mode.MovingEnabled)
            {
                return false;
            }

            if (Player.Distance(Game.CursorPos) < this.HoldPositionRadius)
            {
                return false;
            }

            if (this.NoCancelChamps.Contains(Player.ChampionName))
            {
                return true;
            }

            if (this.IsWindingUp)
            {
                return false;
            }

            return true;
        }

        public override void Dispose()
        {
            this.Config.Dispose();
            Obj_AI_Base.OnProcessAutoAttack -= this.ObjAiHeroOnProcessAutoAttack;
            Obj_AI_Base.OnProcessSpellCast -= this.Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate -= this.Game_OnUpdate;
            SpellBook.OnStopCast -= this.SpellBook_OnStopCast;
            Render.OnRender -= this.RenderManager_OnRender;
            this.Attached = false;
        }

        public override void ForceTarget(AttackableUnit unit)
        {
            this.ForcedTarget = unit;
        }

        public override AttackableUnit GetOrbwalkingTarget()
        {
            return this.LastTarget;
        }

        public override AttackableUnit FindTarget(OrbwalkerMode mode)
        {
            if (this.ForcedTarget != null && this.ForcedTarget.IsValidAutoRange())
            {
                return this.ForcedTarget;
            }

            return mode?.GetTarget();
        }

        public override bool Move(Vector3 movePosition)
        {
            var preMoveArgs = this.FirePreMove(movePosition);

            if (!preMoveArgs.Cancel)
            {
                if (Player.IssueOrder(OrderType.MoveTo, preMoveArgs.MovePosition))
                {
                    return true;
                }
            }

            return false;
        }

        public override void Orbwalk()
        {
            var mode = this.GetActiveMode();

            if (mode == null)
            {
                return;
            }

            /// <summary>
            ///     Execute the specific logic for this mode if any
            /// </summary>
            mode.Execute();

            if (!mode.BaseOrbwalkingEnabled)
            {
                return;
            }

            if (this.CanAttack(mode))
            {
                var target = this.LastTarget = this.FindTarget(mode);
                if (target != null)
                {
                    this.Attack(target);
                }
            }

            if (this.CanMove(mode))
            {
                this.Move(Game.CursorPos);
            }
        }

        public override void ResetAutoAttackTimer()
        {
            this.ServerAttackDetectionTick = 0;
            this.LastAttackCommandSentTime = 0;
        }

        #endregion

        #region Methods

        protected void Game_OnUpdate()
        {
            if (Player.IsDead)
            {
                return;
            }

            this.Orbwalk();
        }

        protected void ObjAiHeroOnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender.IsMe)
            {
                var targ = args.Target as AttackableUnit;
                if (targ != null)
                {
                    this.ServerAttackDetectionTick = Game.TickCount - Game.Ping / 2;
                    this.LastTarget = targ;
                    this.ForcedTarget = null;
                    DelayAction.Queue((int) this.WindUpTime, () => this.FirePostAttack(targ));
                }
            }
        }

        bool CanKillMinion(Obj_AI_Base minion, int time = 0)
        {
            var rtime = time == 0 ? this.TimeForAutoToReachTarget(minion) : time;
            var pred = this.GetPredictedHealth(minion, rtime);

            //The minions health will already be 0 by the time our auto attack reaches it, so no point attacking it...
            if (pred <= 0)
            {
                this.FireNonKillableMinion(minion);
                return false;
            }

            var dmg = Player.GetAutoAttackDamage(minion);

            var result = dmg - pred >= 0;

            return result;
        }

        float DamageDealtInTime(Obj_AI_Base sender, Obj_AI_Base minion, int time)
        {
            var autos = this.NumberOfAutoAttacksInTime(sender, minion, time);
            var dmg = autos * sender.GetAutoAttackDamage(minion);

            return (float) (autos * dmg);
        }

        AttackableUnit GetHeroTarget()
        {
            return TargetSelector.Implementation.GetTarget(0, true);
        }

        AttackableUnit GetLaneClearTarget()
        {
            var attackable = ObjectManager.Get<AttackableUnit>().Where(x => this.IsValidAttackableObject(x));

            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();

            IEnumerable<Obj_AI_Base> minions = attackableUnits
                .Where(x => x is Obj_AI_Base).Cast<Obj_AI_Base>().OrderByDescending(x => x.MaxHealth);

            var minionTurretAggro = minions.FirstOrDefault(x => this.HealthPred.HasTurretAggro(x));

            if (minionTurretAggro != null)
            {
                var data = this.HealthPred.GetAggroData(minionTurretAggro);

                var timeToReach = this.TimeForAutoToReachTarget(minionTurretAggro) + 50;

                var predHealth1Auto = this.HealthPred.GetPredictedDamage(minionTurretAggro, timeToReach);

                var dmgauto = Player.GetAutoAttackDamage(minionTurretAggro);

                var turretDmg = data.LastTurretAttack.Sender.GetAutoAttackDamage(minionTurretAggro);

                //If it won't be dead already...
                if (predHealth1Auto > 0)
                {
                    if (Game.TickCount + timeToReach > Game.TickCount + data.LastTurretAttack.ETA)
                    {
                        if (dmgauto >= predHealth1Auto)
                        {
                            return minionTurretAggro;
                        }
                    }

                    //Our auto can reach sooner than the turret auto
                    else
                    {
                        if (Math.Ceiling(dmgauto - minionTurretAggro.Health) <= 0 || dmgauto > predHealth1Auto)
                        {
                            return minionTurretAggro;
                        }
                    }
                }

                var afterAutoHealth = predHealth1Auto - dmgauto;
                var afterTurretHealth = predHealth1Auto - turretDmg;

                if (afterAutoHealth > 0 && turretDmg >= afterAutoHealth)
                {
                    return null;
                }

                var numautos =
                    this.NumberOfAutoAttacksInTime(Player, minionTurretAggro, data.TimeUntilNextTurretAttack);

                var tdmg = dmgauto * numautos;

                var hNextTurretShot =
                    this.HealthPred.GetPredictedDamage(minionTurretAggro, data.TimeUntilNextTurretAttack - 100);

                if (tdmg >= minionTurretAggro.Health)
                {
                    return minionTurretAggro;
                }

                //Killable
                AttackableUnit killableMinion0 = minions.FirstOrDefault(x => this.CanKillMinion(x));

                if (killableMinion0 != null)
                {
                    return killableMinion0;
                }

                return null;
            }

            //Killable
            AttackableUnit killableMinion = minions.FirstOrDefault(x => this.CanKillMinion(x));

            if (killableMinion != null)
            {
                return killableMinion;
            }

            var waitableMinion = minions.Any(this.ShouldWaitMinion);
            if (waitableMinion)
            {
                return null;
            }

            var structure = this.GetStructureTarget(attackableUnits);

            if (structure != null)
            {
                return structure;
            }

            foreach (var minion in minions.OrderBy(
                x => Math.Ceiling(this.GetPredictedHealth(x) / Player.GetAutoAttackDamage(x))))
            {
                var predHealth = this.GetPredictedHealth(minion);

                var dmg = Player.GetAutoAttackDamage(minion);

                var data = this.HealthPred.GetAggroData(minion);

                //if our damage is enough to kill it
                if (dmg >= predHealth)
                {
                    return minion;
                }

                if (data != null)
                {
                    if (predHealth > dmg && predHealth < dmg * 1.5 && data.TimeElapsedSinceLastMinionAttack < 1300)
                    {
                        Player.IssueOrder(OrderType.Stop, Player.Position);
                        return null;
                    }
                }

                return minion;
            }

            //Heros
            var hero = this.GetHeroTarget();
            if (hero != null)
            {
                return hero;
            }

            return null;
        }

        AttackableUnit GetLastHitTarget()
        {
            return this.GetLastHitTarget(null);
        }

        AttackableUnit GetLastHitTarget(IEnumerable<AttackableUnit> attackable)
        {
            if (attackable == null)
            {
                attackable = ObjectManager.Get<AttackableUnit>().Where(x => this.IsValidAttackableObject(x));
            }

            var availableMinionTargets = attackable
                .OfType<Obj_AI_Base>().Where(x => this.CanKillMinion(x));

            var bestMinionTarget = availableMinionTargets
                .OrderByDescending(x => x.MaxHealth).ThenBy(x => this.HealthPred.GetAggroData(x)?.HasTurretAggro)
                .ThenBy(x => x.Health).FirstOrDefault();

            return bestMinionTarget;
        }

        //In mixed mode we prioritize killable units, then structures, then heros. If none are found, then we don't attack anything.
        AttackableUnit GetMixedModeTarget()
        {
            var attackable = ObjectManager.Get<AttackableUnit>().Where(x => this.IsValidAttackableObject(x));

            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();

            var killable = this.GetLastHitTarget(attackableUnits);

            //Killable unit 
            if (killable != null)
            {
                return killable;
            }

            //Structures
            var structure = this.GetStructureTarget(attackableUnits);
            if (structure != null)
            {
                return structure;
            }

            //Heros
            var hero = this.GetHeroTarget();
            if (hero != null)
            {
                return hero;
            }

            return null;
        }

        int GetPredictedHealth(Obj_AI_Base minion, int time = 0)
        {
            var rtime = time == 0 ? this.TimeForAutoToReachTarget(minion) : time;
            return (int) Math.Ceiling(this.HealthPred.GetPrediction(minion, rtime));
        }

        //Gets a structure target based on the following order (Nexus, Turret, Inihibitor)
        AttackableUnit GetStructureTarget(IEnumerable<AttackableUnit> attackable)
        {
            //Nexus
            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();
            var nexus = attackableUnits.Where(x => x.Type == GameObjectType.obj_HQ).MinBy(x => x.Distance(Player));
            if (nexus != null && nexus.IsValidAutoRange())
            {
                return nexus;
            }

            //Turret
            var turret = attackableUnits.Where(x => x is Obj_AI_Turret).MinBy(x => x.Distance(Player));
            if (turret != null && turret.IsValidAutoRange())
            {
                return turret;
            }

            //Inhib
            var inhib = attackableUnits.Where(x => x.Type == GameObjectType.obj_BarracksDampener)
                                       .MinBy(x => x.Distance(Player));
            if (inhib != null && inhib.IsValidAutoRange())
            {
                return inhib;
            }

            return null;
        }

        private void Initialize()
        {
            this.HealthPred = (HealthPredictionImplB) HealthPrediction.Implementation;

            this.Config = new Menu("Orbwalker", "Orbwalker")
            {
                new Menu("Advanced", "Advanced")
                {
                    new MenuSlider("attackDelayReduction", "Attack Delay Reduction", 90, 0, 180, true),
                },
                new Menu("Drawings", "Drawings")
                {
                    new MenuBool("drawAttackRange", "Draw Attack Range", true),
                    new MenuBool("drawHoldRadius", "Draw Hold Radius", false),
                },
                new Menu("Misc", "Misc")
                {
                    new MenuSlider("holdPositionRadius", "Hold Radius", 50, 0, 400, true),
                    new MenuSlider("extraWindup", "Additional Windup", 30, 0, 200, true),
                    new MenuBool("noBlindAA", "No AA when Blind", true, true),
                    new MenuBool("attackPlants", "Attack Plants", false, true),
                    new MenuBool("attackWards", "Attack Wards", true, true),
                    new MenuBool("attackBarrels", "Attack Barrels", true, true)
                }
            };

            this.AddMode(this.Combo = new OrbwalkerMode("Combo", GlobalKeys.ComboKey, this.GetHeroTarget, null));
            this.AddMode(this.LaneClear = new OrbwalkerMode("Laneclear", GlobalKeys.WaveClearKey, this.GetLaneClearTarget, null));
            this.AddMode(this.LastHit = new OrbwalkerMode("Lasthit", GlobalKeys.LastHitKey, this.GetLastHitTarget, null));
            this.AddMode(this.Mixed = new OrbwalkerMode("Mixed", GlobalKeys.MixedKey, this.GetMixedModeTarget, null));

            GPCheck();

            GameEvents.GameStart += GameEvents_GameStart;
        }

        private void GPCheck()
        {
            var gp = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(x => x.ChampionName.ToLower().Equals("gangplank"));
            if (gp != null)
            {
                this.GangPlank = gp;
            }
        }

        private void GameEvents_GameStart()
        {
            this.GPCheck();
        }


        int NumberOfAutoAttacksInTime(Obj_AI_Base sender, AttackableUnit minion, int time)
        {
            var basetimePerAuto = this.TimeForAutoToReachTarget(minion);

            var numberOfAutos = 0;
            var adjustedTime = 0;

            if (basetimePerAuto > time)
            { 
                return 0;
            }

            if (this.AttackReady)
            {
                numberOfAutos++;
                adjustedTime = time - basetimePerAuto;
            }

            var fullTimePerAuto = basetimePerAuto + sender.AttackDelay * 1000;
            var additionalAutos = (int) Math.Ceiling(adjustedTime / fullTimePerAuto);

            numberOfAutos += additionalAutos;

            return numberOfAutos;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs e)
        {
            if (sender.IsMe)
            {
                var name = e.SpellData.Name.ToLower();

                if (this.SpecialAttacks.Contains(name))
                {
                    this.ObjAiHeroOnProcessAutoAttack(sender, e);
                }

                if (this.IsReset(name))
                {
                    this.ResetAutoAttackTimer();
                }
            }
        }

        private void RenderManager_OnRender()
        {
            if (this.DrawAttackRange)
            {
                Render.Circle(Player.Position, Player.AttackRange + Player.BoundingRadius, 30, Color.DeepSkyBlue);
            }

            if (this.DrawHoldPosition)
            {
                Render.Circle(Player.Position, this.HoldPositionRadius, 30, Color.White);
            }
        }

        bool ShouldWaitMinion(Obj_AI_Base minion)
        {
            var pred = this.GetPredictedHealth(
                minion,
                this.TimeForAutoToReachTarget(minion) + (int) this.AttackCoolDownTime + (int) this.WindUpTime);
            return Player.GetAutoAttackDamage(minion) - pred >= 0;
        }

        private void SpellBook_OnStopCast(Obj_AI_Base sender, SpellBookStopCastEventArgs e)
        {
            if (sender.IsMe && (e.DestroyMissile || e.ForceStop || e.StopAnimation))
            {
                this.ResetAutoAttackTimer();
            }
        }

        int TimeForAutoToReachTarget(AttackableUnit minion)
        {
            var dist = Player.Distance(minion) - Player.BoundingRadius - minion.BoundingRadius;
            var ms = Player.IsMelee ? int.MaxValue : Player.BasicAttack.MissileSpeed;
            var attackTravelTime = dist / ms * 1000f;
            var totalTime = (int) (this.AnimationTime + attackTravelTime + 70 + Game.Ping / 2);
            return totalTime;
        }

        #endregion
    }
}
