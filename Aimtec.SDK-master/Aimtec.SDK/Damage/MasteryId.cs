namespace Aimtec.SDK.Damage
{
    using System.Linq;

    /// <summary>
    ///     Class MasteryId.
    /// </summary>
    public static class MasteryId
    {
        #region Enums

        public enum Ferocity
        {
            Fury = 80,

            Sorcery = 38,

            FreshBlood = 39,

            Feast = 108,

            ExposeWeakness = 18,

            Vampirism = 60,

            NaturalTalent = 7,

            BountyHunter = 90,

            DoubleEdgedSword = 6,

            BattleTrance = 11,

            BatteringBlows = 33,

            PiercingThoughts = 72,

            WarlordsBloodlust = 111,

            FervorofBattle = 65,

            DeathfireTouch = 112
        }

        public enum Cunning
        {
            Wanderer = 80,

            Savagery = 43,

            RunicAffinity = 39,

            SecretStash = 108,

            Assassin = 18,

            Merciless = 60,

            Meditation = 74,

            GreenFathersGift = 90,

            Bandit = 6,

            DangerousGame = 11,

            Precision = 33,

            Intelligence = 76,

            StormraidersSurge = 111,

            ThunderlordsDecree = 65,

            WindspeakersBlessing = 118
        }

        //untested cause resolve page broken
        public enum Resolve
        {
            Recovery = 80,

            Unyielding = 43,

            Explorer = 0,

            ToughSkin = 18,

            Siegemaster = 0,

            RunicArmor = 60,

            VeteransScars = 74,

            Insight = 90,

            Perseverance = 0,

            Fearless = 0,

            Swiftness = 0,

            LegendaryGuardian = 0,

            GraspoftheUndying = 0,

            CourageOfTheColossus = 0,

            StonebornPact = 0
        }

        #endregion

        #region Public Methods and Operators

        public static Mastery GetCunningPage(this Obj_AI_Hero hero, Cunning cunning)
        {
            return hero?.GetMastery(MasteryPage.Cunning, (uint)cunning);
        }

        public static Mastery GetFerocityPage(this Obj_AI_Hero hero, Ferocity ferocity)
        {
            return hero?.GetMastery(MasteryPage.Ferocity, (uint)ferocity);
        }

        public static Mastery GetResolvePage(this Obj_AI_Hero hero, Resolve resolve)
        {
            return hero?.GetMastery(MasteryPage.Resolve, (uint)resolve);
        }

        public static Mastery GetMastery(this Obj_AI_Hero hero, MasteryPage page, uint id)
        {
            return hero?.Masteries.FirstOrDefault(m => m != null && m.Page == page && m.Id == id);
        }

        public static bool IsUsingMastery(this Obj_AI_Hero hero, Mastery mastery)
        {
            return mastery?.Points > 0;
        }

        public static bool IsUsingMastery(this Obj_AI_Hero hero, MasteryPage page, uint mastery)
        {
            return hero?.GetMastery(page, mastery) != null;
        }

        #endregion
    }
}