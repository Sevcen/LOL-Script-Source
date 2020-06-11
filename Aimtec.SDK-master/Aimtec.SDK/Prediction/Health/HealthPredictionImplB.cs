namespace Aimtec.SDK.Prediction.Health
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Menu.Config;

    class HealthPredictionImplB : IHealthPrediction
    {
        #region Fields

        internal readonly Dictionary<int, List<Attack>> incomingAttacks = new Dictionary<int, List<Attack>>();

        internal readonly Dictionary<int, AggroData> MinionAggroData = new Dictionary<int, AggroData>();

        internal Menu.Menu Config { get; set; }

        #endregion

        #region Constructors and Destructors

        internal HealthPredictionImplB()
        {
            Game.OnUpdate += this.GameOnUpdate;
            Obj_AI_Base.OnProcessAutoAttack += this.ObjAiBaseOnOnProcessAutoAttack;
            GameObject.OnDestroy += this.GameObjectOnOnDestroy;
            Obj_AI_Base.OnPerformCast += this.Obj_AI_Base_OnPerformCast;
            SpellBook.OnStopCast += this.SpellBook_OnStopCast;

            Config = new Menu.Menu("HealthPred", "HealthPrediction");
            Config.Add(new MenuSeperator("seperator", "Default value should be 180"));
            Config.Add(new MenuSlider("ExtraDelay", "Extra Delay", 180, 0, 250));

            AimtecMenu.Instance.Add(this.Config);
        }

        #endregion

        #region Properties

        private Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        #endregion

        #region Public Methods and Operators

        public AggroData GetAggroData(Obj_AI_Base unit)
        {
            if (this.MinionAggroData.ContainsKey(unit.NetworkId))
            {
                return this.MinionAggroData[unit.NetworkId];
            }

            return null;
        }

        public float GetPredictedDamage(Obj_AI_Base unit, int time)
        {
            //If there is no incoming auto attacks detected for this unit, then it is not taking damage, so return 0.
            if (!this.incomingAttacks.ContainsKey(unit.NetworkId))
            {
                return 0;
            }

            var incAttacksUnit = this.incomingAttacks[unit.NetworkId];

            float predictedDmg = 0;

            foreach (var attack in incAttacksUnit)
            {
                //if this attack will take longer than the specified time to reach the target, then ignore it
                if (attack.ETA + this.Config["ExtraDelay"].Value > time)
                {
                    continue;
                }

                float dmg = 0;

                if (attack is AutoAttack)
                {
                    dmg = (float) attack.Sender.GetAutoAttackDamage(unit); //calc dmg from source to this unit
                }

                //Must be turret attack
                else
                {
                    var tAttack = attack as TurretAttack;
                    if (tAttack != null)
                    {
                        dmg = (1 + tAttack.DmgPercentageIncrease) * (float) attack.Sender.GetAutoAttackDamage(unit);
                    }
                }

                predictedDmg += dmg;
            }

            return predictedDmg;
        }

        public float GetPrediction(Obj_AI_Base unit, int time)
        {
            var pred = Math.Max(0, unit.Health - this.GetPredictedDamage(unit, time));

            return pred;
        }

        public bool HasTurretAggro(Obj_AI_Base unit)
        {
            var data = this.GetAggroData(unit);
            if (data != null)
            {
                return data.HasTurretAggro;
            }

            return false;
        }

        #endregion

        #region Methods

        private void GameObjectOnOnDestroy(GameObject sender)
        {
            var mc = sender as MissileClient;

            if (mc != null)
            {
                var source = mc.SpellCaster;
                var targ = mc.Target as Obj_AI_Base;

                if (source != null && targ != null)
                {
                    foreach (var value in this.incomingAttacks.Values)
                    {
                        value.RemoveAll(
                            x => x.Sender.NetworkId == source.NetworkId && x.AttackName == mc.SpellData.Name
                                && x.Target.NetworkId == targ.NetworkId);
                    }

                    if (source is Obj_AI_Turret)
                    {
                        if (this.MinionAggroData.ContainsKey(targ.NetworkId))
                        {
                            var lastattack = this.MinionAggroData[targ.NetworkId].LastTurretAttack;
                            if (lastattack != null)
                                lastattack.Destroyed = true;
                        }
                    }
                }
            }
        }

        private void GameOnUpdate()
        {
            foreach (var value in this.incomingAttacks.Values)
            {
                value.RemoveAll(x => Game.TickCount - x.CastTime > 1800);
            }

            foreach (var value in this.MinionAggroData.Values.ToList())
            {
                if (value.Unit == null || !value.Unit.IsValid || value.Unit.IsDead)
                {
                    this.MinionAggroData.Remove(value.NetworkID);
                }
            }
        }

        private void Obj_AI_Base_OnPerformCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs e)
        {
            if (sender != null && sender.IsValid && sender.IsMelee)
            {
                foreach (var value in this.incomingAttacks.Values)
                {
                    value.RemoveAll(
                        x => x.Sender == null || x.Target == null
                            || x.Sender.IsMelee && x.Sender.NetworkId == sender.NetworkId);
                }
            }
        }

        private void ObjAiBaseOnOnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            //Only detect attacks from and to minions close to us
            if (sender == null || sender.Distance(this.Player) > 3500 || sender is Obj_AI_Hero)
            {
                return;
            }

            var target = args.Target as Obj_AI_Base;

            if (target != null)
            {
                if (!this.MinionAggroData.ContainsKey(target.NetworkId))
                {
                    this.MinionAggroData.Add(target.NetworkId, new AggroData(target, this));
                }

                if (sender.IsTurret)
                {
                    var data = this.MinionAggroData[target.NetworkId];

                    if (data != null)
                    {
                        if (data.LastTurretAttack != null)
                        {
                            if (Game.TickCount - data.LastTurretAttack.CastTime < 2500)
                            {
                                data.TurretShotNumber++;
                            }

                            else
                            {
                                data.TurretShotNumber = 1;
                            }
                        }

                        var auto = new TurretAttack(
                            sender,
                            target,
                            args,
                            data.LastTurretAttack == null ? 1 : data.TurretShotNumber,
                            args.SpellData.Name,
                            sender.Inventory.HasItem(1337420)); //todo: add actual item id.

                        if (!this.incomingAttacks.ContainsKey(auto.Target.NetworkId))
                        {
                            this.incomingAttacks.Add(auto.Target.NetworkId, new List<Attack>());
                        }

                        this.incomingAttacks[auto.Target.NetworkId].Add(auto);

                        data.LastTurretAttack = auto;
                    }
                }

                else
                {
                    var auto = new AutoAttack(sender, target, args, args.SpellData.Name);

                    if (!this.incomingAttacks.ContainsKey(auto.Target.NetworkId))
                    {
                        this.incomingAttacks.Add(auto.Target.NetworkId, new List<Attack>());
                    }

                    this.incomingAttacks[auto.Target.NetworkId].Add(auto);

                    if (this.MinionAggroData.ContainsKey(auto.Target.NetworkId))
                    {
                        this.MinionAggroData[auto.Target.NetworkId].LastMinionAttack = auto;
                    }
                }
            }
        }

        private void SpellBook_OnStopCast(Obj_AI_Base sender, SpellBookStopCastEventArgs e)
        {
            if (sender.IsValid)
            {
                if (sender is Obj_AI_Turret)
                {
                    return;
                }

                if (sender is Obj_AI_Base && (e.DestroyMissile || e.StopAnimation))
                {
                    foreach (var value in this.incomingAttacks.Values)
                    {
                        value.RemoveAll(x => x.Sender.NetworkId == sender.NetworkId);
                    }
                }
            }
        }

        #endregion

        public class AggroData
        {
            #region Constructors and Destructors

            public AggroData(Obj_AI_Base unit, HealthPredictionImplB hpred)
            {
                this.HealthPredInstance = hpred;
                this.Unit = unit;
                this.NetworkID = unit.NetworkId;
            }

            #endregion

            #region Public Properties

            public bool HasMinionAggro => this.TimeElapsedSinceLastMinionAttack < 500;

            public bool HasTurretAggro => this.TimeElapsedSinceLastTurretAttack < 2500;

            public List<Attack> IncomingAttacks =>
                this.HealthPredInstance.incomingAttacks.ContainsKey(this.Unit.NetworkId)
                    ? this.HealthPredInstance.incomingAttacks[this.Unit.NetworkId]
                    : null;

            public IEnumerable<AutoAttack> IncomingMinionAttacks => this
                .IncomingAttacks.Where(x => x is AutoAttack).Cast<AutoAttack>();

            public IEnumerable<TurretAttack> IncomingTurretAttacks => this
                .IncomingAttacks.Where(x => x is TurretAttack).Cast<TurretAttack>();

            public Attack LastMinionAttack { get; set; }

            public TurretAttack LastTurretAttack { get; set; }

            public int NetworkID { get; set; }

            public int NextTurretAttackTime
            {
                get
                {
                    if (this.LastTurretAttack != null)
                    {
                        var sender = this.LastTurretAttack.Sender;
                        return this.LastTurretAttack.CastTime + (int) (sender.AttackDelay * 1000)
                            + (int) (sender.AttackCastDelay * 1000)
                            + (int) (sender.Distance(this.Unit.Position) - sender.BoundingRadius
                                - this.Unit.BoundingRadius) / (int) sender.BasicAttack.MissileSpeed + 50;
                    }

                    return 0;
                }
            }

            public int TimeElapsedSinceLastMinionAttack => Game.TickCount + Game.Ping / 2
                - (this.LastMinionAttack?.CastTime ?? 0);

            public int TimeElapsedSinceLastTurretAttack => Game.TickCount - (this.LastTurretAttack?.CastTime ?? 0);

            public int TimeUntilNextTurretAttack
            {
                get
                {
                    return this.NextTurretAttackTime - Game.TickCount;
                }
            }

            public int TurretShotNumber { get; set; }

            public Obj_AI_Base Unit { get; set; }

            #endregion

            #region Properties

            private HealthPredictionImplB HealthPredInstance { get; set; }

            #endregion
        }

        public class Attack
        {
            #region Fields

            public Obj_AI_BaseMissileClientDataEventArgs args;

            #endregion

            #region Constructors and Destructors

            public Attack(
                Obj_AI_Base sender,
                Obj_AI_Base target,
                Obj_AI_BaseMissileClientDataEventArgs args,
                string name)
            {
                this.Sender = sender;
                this.Target = target;
                this.StartPosition = sender.Position;
                this.Missilespeed = args.SpellData.MissileSpeed;
                this.args = args;
                this.CastTime = Game.TickCount - Game.Ping / 2;
                this.AttackName = name;
            }

            #endregion

            #region Public Properties

            //If there is time left until arrival, this auto has not arrived yet and is active, otherwise this auto attack has already reached the target and is inactive
            public virtual bool Active => this.ETA >= 0;

            public string AttackName { get; set; }

            public int CastTime { get; set; }

            public bool Destroyed { get; set; }

            public float Distance => this.StartPosition.Distance(this.Target.Position) - this.Sender.BoundingRadius - this.Target.BoundingRadius;

            //Gets the time left until this auto reaches target by subtracting the time elapsed from total travel time
            public float ETA => this.TravelTime - this.TimeElapsed;

            public virtual float MeleeTravelTime { get; set; }

            public float Missilespeed { get; set; }

            public virtual float RangedTravelTime { get; set; }

            public Obj_AI_Base Sender { get; set; }

            public Vector3 StartPosition { get; set; }

            public Obj_AI_Base Target { get; set; }

            //Gets the time passed by since this auto attack was detected
            public float TimeElapsed => Game.TickCount - this.CastTime;

            public virtual float TravelTime => this.Sender.IsMelee ? this.MeleeTravelTime : this.RangedTravelTime;

            #endregion
        }

        public class AutoAttack : Attack
        {
            #region Fields

            private float extraDelay = 0;

            #endregion

            #region Constructors and Destructors

            public AutoAttack(
                Obj_AI_Base sender,
                Obj_AI_Base target,
                Obj_AI_BaseMissileClientDataEventArgs args,
                string name)
                : base(sender, target, args, name)
            {
                this.Melee = sender.IsMelee;
            }

            #endregion

            #region Public Properties

            public bool Melee { get; set; }

            public override float MeleeTravelTime => this.Sender.AttackCastDelay * 1000f + this.extraDelay;

            public override float RangedTravelTime => this.Distance / this.Missilespeed * 1000f
                + this.Sender.AttackCastDelay * 1000f + this.extraDelay;

            #endregion
        }

        public class TurretAttack : Attack
        {
            #region Constructors and Destructors

            public TurretAttack(
                Obj_AI_Base sender,
                Obj_AI_Base target,
                Obj_AI_BaseMissileClientDataEventArgs args,
                int shotNumber,
                string name,
                bool hasLightning)
                : base(sender, target, args, name)
            {
                this.ShotNumber = shotNumber;
                this.HasLightningRod = hasLightning;
            }

            #endregion

            #region Public Properties

            public float Damage => (1 + this.DmgPercentageIncrease)
                * (float) this.Sender.GetAutoAttackDamage(this.Target);

            public int DestroyTime { get; set; }

            public float DmgPercentageIncrease => this.HasLightningRod
                ? Math.Min(125f, this.HeatAmount * 1.05f) / 100
                : 0;

            public bool HasLightningRod { get; set; }

            public int HeatAmount => Math.Min(this.ShotNumber * 6, 120);

            public int ShotNumber { get; set; }

            public override float TravelTime => this.Distance / this.Missilespeed * 1000
                + this.Sender.AttackCastDelay * 1000 + 50;

            #endregion
        }
    }
}