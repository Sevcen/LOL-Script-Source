namespace Aimtec.SDK
{
    using System;

    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu.Config;
    using Aimtec.SDK.Prediction.Skillshots;

    using NLog;

    /// <summary>
    ///     Class Spell.
    /// </summary>
    public class Spell
    {
        #region Fields

        private int chargedCastedT;

        private int chargeReqSentT;

        private Vector3 from;

        private float range = float.MaxValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spell" /> class.
        /// </summary>
        /// <param name="slot">The slot.</param>
        public Spell(SpellSlot slot)
        {
            this.Slot = slot;
        }

        /// <inheritdoc />
        public Spell(SpellSlot slot, float range)
            : this(slot)
        {
            this.Range = range;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the charged buff.
        /// </summary>
        /// <value>The name of the charged buff.</value>
        public string ChargedBuffName { get; set; }

        /// <summary>
        ///     Gets or sets the charged maximum range.
        /// </summary>
        /// <value>The charged maximum range.</value>
        public int ChargedMaxRange { get; set; }

        /// <summary>
        ///     Gets or sets the charged minimum range.
        /// </summary>
        /// <value>The charged minimum range.</value>
        public int ChargedMinRange { get; set; }

        /// <summary>
        ///     Gets or sets the name of the charged spell.
        /// </summary>
        /// <value>The name of the charged spell.</value>
        public string ChargedSpellName { get; set; }

        /// <summary>
        ///     Gets or sets the duration of the charge.
        /// </summary>
        /// <value>The duration of the charge.</value>
        public float ChargeDuration { get; set; }

        /// <summary>
        ///     Gets the percentage the spell is charged up
        /// </summary>
        public float ChargePercent
        {
            get
            {
                if (!this.IsChargedSpell)
                {
                    return 100;
                }

                if (!this.IsCharging)
                {
                    return 0;
                }

                var start = Math.Max(this.chargeReqSentT, this.chargedCastedT);
                var chargePercent = (Game.TickCount - start) / (start + this.ChargeDuration - 150 - start);

                return chargePercent * 100;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Spell" /> has collision.
        /// </summary>
        /// <value><c>true</c> if the spell has collision; otherwise, <c>false</c>.</value>
        public bool Collision { get; set; }

        /// <summary>
        ///     Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the from.
        /// </summary>
        public Vector3 From
        {
            get => this.from.IsZero ? Player.ServerPosition : this.from;
            set => this.from = value;
        }

        /// <summary>
        ///     Gets or sets the hit chance.
        /// </summary>
        /// <value>The hit chance.</value>
        public HitChance HitChance { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is charged spell.
        /// </summary>
        /// <value><c>true</c> if this instance is charged spell; otherwise, <c>false</c>.</value>
        public bool IsChargedSpell { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is charing.
        /// </summary>
        /// <value><c>true</c> if this instance is charing; otherwise, <c>false</c>.</value>
        public bool IsCharging => this.Ready && (Player.HasBuff(this.ChargedBuffName)
            || Game.TickCount - this.chargedCastedT < 300 + Game.Ping);

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is skill shot.
        /// </summary>
        /// <value><c>true</c> if this instance is skill shot; otherwise, <c>false</c>.</value>
        public bool IsSkillShot { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is a vector skillshot
        /// </summary>
        /// <value><c>true</c> if this instance is a vector skill shot; otherwise, <c>false</c>.</value>
        public bool IsVectorSkillShot { get; set; }

        /// <summary>
        ///     Gets the last time the spell was attempted to be casted.
        /// </summary>
        public int LastCastAttemptT { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public float Range
        {
            get
            {
                if (!this.IsChargedSpell)
                {
                    return this.range;
                }

                if (this.IsCharging)
                {
                    return this.ChargedMinRange + Math.Min(
                        this.ChargedMaxRange - this.ChargedMinRange,
                        (Game.TickCount - this.chargedCastedT) * (this.ChargedMaxRange - this.ChargedMinRange)
                        / this.ChargeDuration - 150);
                }

                return this.ChargedMaxRange;
            }

            set => this.range = value;
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Spell" /> is ready.
        /// </summary>
        /// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
        public bool Ready => Player.SpellBook.GetSpellState(this.Slot) == SpellState.Ready;

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>The slot.</value>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the speed.
        /// </summary>
        /// <value>The speed.</value>
        public float Speed { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public SkillshotType Type { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>The player.</value>
        private static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Casts the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="aoe"><c>true</c> if the spell should hit as many enemies as possible.</param>
        /// <param name="minTargets">The minimum number of targets needed to hit to cast the spell.</param>
        /// <returns><c>true</c> if the spell was casted, <c>false</c> otherwise.</returns>
        public bool Cast(Obj_AI_Base target, bool aoe = false, int minTargets = 0)
        {
            if (!this.Ready)
            {
                return false;
            }

            if (this.IsVectorSkillShot)
            {
                if (AimtecMenu.DebugEnabled)
                {
                    Logger.Error("Vector skillshot should be casted using two positions");
                }

                return false;
            }

            if (!this.IsSkillShot && !this.IsChargedSpell)
            {
                this.LastCastAttemptT = Game.TickCount;

                return Player.SpellBook.CastSpell(this.Slot, target);
            }

            if (minTargets > 0)
            {
                aoe = true;
            }

            var input = this.GetPredictionInput(target);
            input.AoE = aoe;

            var prediction = Prediction.Skillshots.Prediction.Instance.GetPrediction(input);

            if (prediction.HitChance < this.HitChance)
            {
                return false;
            }

            if (prediction.CollisionObjects.Count > 0 && this.Collision)
            {
                return false;
            }

            if (this.IsChargedSpell)
            {
                return this.IsCharging
                    ? this.ShootChargedSpell(prediction.CastPosition)
                    : this.StartCharging(prediction.CastPosition);
            }

            if (minTargets > 0 && prediction.AoeTargetsHitCount <= minTargets)
            {
                return false;
            }

            this.LastCastAttemptT = Game.TickCount;

            return Player.SpellBook.CastSpell(this.Slot, prediction.CastPosition);
        }

        /// <summary>
        ///     Casts this instance.
        /// </summary>
        /// <returns><c>true</c> if the spell was casted, <c>false</c> otherwise.</returns>
        public bool Cast()
        {
            return this.CastOnUnit(ObjectManager.GetLocalPlayer());
        }

        /// <summary>
        ///     Casts the spell at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Cast(Vector2 position)
        {
            if (!this.Ready || MenuGUI.IsChatOpen() || MenuGUI.IsShopOpen())
            {
                return false;
            }

            var castPosition = new Vector3(position.X, NavMesh.GetHeightForWorld(position.X, position.Y), position.Y);

            if (this.IsChargedSpell)
            {
                return this.IsCharging ? this.ShootChargedSpell(castPosition) : this.StartCharging(castPosition);
            }

            return Player.SpellBook.CastSpell(this.Slot, castPosition);
        }

        /// <summary>
        ///     Casts the spell at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Cast(Vector3 position)
        {
            if (!this.Ready || MenuGUI.IsChatOpen() || MenuGUI.IsShopOpen())
            {
                return false;
            }

            if (this.IsChargedSpell)
            {
                return this.IsCharging ? this.ShootChargedSpell(position) : this.StartCharging(position);
            }

            return Player.SpellBook.CastSpell(this.Slot, position);
        }

        /// <summary>
        ///     Casts a Vector spell at a specified start position to a specified end position.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Cast(Vector3 start, Vector3 end)
        {
            if (!this.Ready || MenuGUI.IsChatOpen() || MenuGUI.IsShopOpen())
            {
                return false;
            }

            return Player.SpellBook.CastSpell(this.Slot, start, end);
        }

        /// <summary>
        ///     Casts the spell if it will hit at least <paramref name="minTargets" />.
        /// </summary>
        /// <param name="unit">The unit to cast on.</param>
        /// <param name="minTargets">The minimum number of targets to hit.</param>
        /// <returns><c>true</c> if the spell was casted; otherwise, <c>false</c>.</returns>
        public bool CastIfWillHit(Obj_AI_Base unit, int minTargets = 2)
        {
            return this.Cast(unit, true, minTargets);
        }

        /// <summary>
        ///     Casts the on unit.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CastOnUnit(GameObject obj)
        {
            if (!this.Ready || MenuGUI.IsChatOpen() || MenuGUI.IsShopOpen()
                || this.From.Distance(obj.ServerPosition) > this.Range)
            {
                return false;
            }

            this.LastCastAttemptT = Game.TickCount;

            return Player.SpellBook.CastSpell(this.Slot, obj);
        }

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="fromPos">The position to cast the spell from.</param>
        /// <param name="rangeCheckFrom">The position to check the range from.</param>
        /// <returns>PredictionOutput.</returns>
        public PredictionOutput GetPrediction(
            Obj_AI_Base target,
            Vector3 fromPos = new Vector3(),
            Vector3 rangeCheckFrom = new Vector3())
        {
            return Prediction.Skillshots.Prediction.Instance.GetPrediction(
                this.GetPredictionInput(target, fromPos, rangeCheckFrom));
        }

        /// <summary>
        ///     Gets the prediction input.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="fromPosition">The position to cast the spell from.</param>
        /// <param name="rangeCheckFromPosition">The position to check the range from.</param>
        /// <returns>PredictionInput.</returns>
        public PredictionInput GetPredictionInput(
            Obj_AI_Base target,
            Vector3 fromPosition = new Vector3(),
            Vector3 rangeCheckFromPosition = new Vector3())
        {
            var input = new PredictionInput()
            {
                Delay = this.Delay,
                Radius = this.Width,
                Speed = this.Speed,
                Range = this.Range,
                Unit = target,
                From = this.From,
                Collision = this.Collision
            };

            if (!fromPosition.IsZero)
            {
                input.From = fromPosition;
            }

            if (!rangeCheckFromPosition.IsZero)
            {
                input.RangeCheckFrom = rangeCheckFromPosition;
            }

            return input;
        }

        /// <summary>
        ///     Sets the charged.
        /// </summary>
        /// <param name="spellName">Name of the spell.</param>
        /// <param name="buffName">Name of the buff.</param>
        /// <param name="minRange">The minimum range.</param>
        /// <param name="maxRange">The maximum range.</param>
        /// <param name="chargeDurationSeconds">The charge duration in seconds.</param>
        public void SetCharged(
            string spellName,
            string buffName,
            int minRange,
            int maxRange,
            float chargeDurationSeconds)
        {
            this.IsChargedSpell = true;
            this.ChargedSpellName = spellName;
            this.ChargedBuffName = buffName;
            this.ChargedMaxRange = maxRange;
            this.ChargedMinRange = minRange;
            this.ChargeDuration = chargeDurationSeconds * 1000;
            this.chargedCastedT = 0;

            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                if (sender.IsMe && args.SpellData.Name == this.ChargedSpellName)
                {
                    this.chargedCastedT = Game.TickCount;
                }
            };

            SpellBook.OnUpdateChargedSpell += (sender, args) =>
            {
                if (sender.IsMe && Game.TickCount - this.chargeReqSentT < 3000 && args.ReleaseCast)
                {
                    //args.Process = false;
                }
            };

            SpellBook.OnCastSpell += (sender, args) =>
            {
                if (args.Slot == this.Slot && Game.TickCount - this.chargeReqSentT > 500 && this.IsCharging)
                {
                    this.Cast((Vector2) args.End);
                }
            };

            if (AimtecMenu.DebugEnabled)
            {
                Logger.Debug(
                    "{0} Set as Charged -> Spell Name: {1}, Buff Name: {2}, Min Range: {3}, Max Range: {4}, Charge Duration: {5}s",
                    this.Slot,
                    spellName,
                    buffName,
                    minRange,
                    maxRange,
                    chargeDurationSeconds);
            }
        }

        /// <summary>
        ///     Sets the skillshot.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="width">The width.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="collision">if set to <c>true</c> the spell has collision.</param>
        /// <param name="type">The type.</param>
        /// <param name="vectorSkillshot">if set to <c>true</c> the spell has a start and end position. Ex. Viktor E and Rumble R</param>
        /// <param name="hitchance">The hitchance.</param>
        public void SetSkillshot(
            float delay,
            float width,
            float speed,
            bool collision,
            SkillshotType type,
            bool vectorSkillshot = false,
            HitChance hitchance = HitChance.High)
        {
            this.Delay = delay;
            this.Width = width;
            this.Speed = speed;
            this.Type = type;
            this.Collision = collision;
            this.IsSkillShot = true;
            this.IsVectorSkillShot = vectorSkillshot;
            this.HitChance = hitchance;

            if (AimtecMenu.DebugEnabled)
            {
                Logger.Debug(
                    "{0} Set as SkillShot -> Range: {1}, Delay: {2},  Width: {3}, Speed: {4}, Collision: {5}, Type: {6}, MinHitChance: {7}",
                    this.Slot,
                    this.Range,
                    delay,
                    width,
                    speed,
                    collision,
                    type,
                    hitchance);
            }
        }

        /// <summary>
        ///     Shoots the charged spell
        /// </summary>
        public bool ShootChargedSpell(Vector3 position, bool releaseCast = true)
        {
            var resultUpdate = Player.SpellBook.UpdateChargedSpell(this.Slot, position, releaseCast);

            if (!releaseCast)
            {
                var resultCast = Player.SpellBook.CastSpell(this.Slot, position);
                return resultUpdate && resultCast;
            }

            return resultUpdate;
        }

        /// <summary>
        ///     Starts charging the spell
        /// </summary>
        public bool StartCharging(Vector3 position)
        {
            if (this.IsCharging || Game.TickCount - this.chargeReqSentT <= 400 + Game.Ping)
            {
                return false;
            }

            this.chargeReqSentT = Game.TickCount;
            return Player.SpellBook.CastSpell(this.Slot, position);
        }

        #endregion
    }
}