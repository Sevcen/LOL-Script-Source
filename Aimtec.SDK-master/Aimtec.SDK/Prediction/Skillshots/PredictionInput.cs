namespace Aimtec.SDK.Prediction.Skillshots
{
    using Aimtec.SDK.Prediction.Collision;

    public class PredictionInput
    {
        #region Fields

        private Vector3 @from;

        private Vector3 rangeCheckFrom;

        #endregion

        #region Public Properties

        public bool AoE { get; set; }

        public bool Collision { get; set; }

        public CollisionableObjects CollisionObjects { get; set; } =
            CollisionableObjects.Minions | CollisionableObjects.YasuoWall;

        public float Delay { get; set; }

        public Vector3 From
        {
            get => this.from.IsZero ? ObjectManager.GetLocalPlayer().Position : this.@from;

            set => this.@from = value;
        }

        public float Radius { get; set; } = 1f;

        public float Range { get; set; } = float.MaxValue;

        public Vector3 RangeCheckFrom
        {
            get => this.rangeCheckFrom.IsZero ? this.From : this.rangeCheckFrom;

            set => this.rangeCheckFrom = value;
        }

        public float Speed { get; set; } = float.MaxValue;

        public SkillshotType Type { get; set; } = SkillshotType.Line;

        public Obj_AI_Base Unit { get; set; } = ObjectManager.GetLocalPlayer();

        public bool UseBoundingRadius { get; set; } = true;

        #endregion

        #region Properties

        internal float RealRadius => this.UseBoundingRadius ? this.Radius + this.Unit.BoundingRadius : this.Radius;

        #endregion
    }
}