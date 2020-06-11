﻿namespace EnsoulSharp.SDK
{
    using System;

    using SharpDX;

    using Color = System.Drawing.Color;

    public abstract class BaseSpell
    {
        #region Constructors and Destructors

        public BaseSpell(string spellName)
        {
            this.SData = SpellDatabase.GetByName(spellName);
        }

        public BaseSpell(SpellDatabaseEntry entry)
        {
            this.SData = entry;
        }

        #endregion

        #region Public Properties

        public SpellDatabaseEntry SData;

        public virtual SkillshotDetectionType DetectionType { get; set; }

        public virtual AIBaseClient Caster { get; set; }

        public virtual Vector2 StartPosition { get; set; }

        public virtual Vector2 EndPosition { get; set; }

        public virtual int StartTime { get; set; }

        public bool HasMissile => this is SkillshotMissile;

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return "BaseSpell: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        public void PrintSpellData()
        {
            Console.WriteLine(@"=================");
            var properties = new[]
                                 {
                                     "ChampionName", "SpellType", "SpellName", "Range", "Radius", "Delay", "MissileSpeed",
                                     "CanBeRemoved", "Angle", "FixedRange"
                                 };
            properties.ForEach(
                property =>
                    {
                        Console.WriteLine(
                            "{0} => {1}",
                            property,
                            this.SData.GetType().GetProperty(property).GetValue(this.SData, null));
                    });
        }

        public virtual bool HasExpired()
        {
            if (this.SData.MissileAccel != 0)
            {
                return Variables.TickCount >= this.StartTime + 5000;
            }

            return Variables.TickCount
                   > this.StartTime + this.SData.Delay
                   + /* this.ExtraDuration + */
                   1000 * (this.StartPosition.Distance(this.EndPosition) / this.SData.MissileSpeed);
        }

        //TODO: ADD DAMAGE STUFF

        public virtual bool IsAboutToHit(AIBaseClient unit, int afterTime)
        {
            //TODO
            return false;
        }

        public virtual bool IsAboutToHit(Vector3 position, int afterTime)
        {
            //TODO
            return false;
        }

        internal virtual void Game_OnUpdate() { }

        public virtual void Draw(Color color, Color missileColor, int borderWidth = 1) { }

        #endregion
    }
}