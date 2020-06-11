﻿using System;

namespace LeagueSharp.Common.Data
{
    using System.Linq;

    public static class MasteryData
    {
        #region Enums
        // updated 7 december 2016
        public enum Cunning
        {
            Wanderer = 65,

            Savagery = 66,

            RunicAffinity = 81,

            SecretStash = 82,

            Assasin = 83,

            Meditation = 98,

            Merciless = 97,

            GreenfathersGift = 113,

            Bandit = 114,

            DangerousGame = 115,

            Precision = 129,

            Intelligence = 130,

            StormraidersSurge = 145,

            ThunderlordsDecree = 146,

            WindspeakerBlessing = 147
        }

        // updated 7 december 2016
        public enum Ferocity
        {
            Fury = 65,

            Sorcery = 68,

            FreshBlood = 81,

            ExposeWeakness = 83,

            DoubleEdgedSword = 114,

            [Obsolete("Obsolete! Please update your assembly, oppresor called doubleedgedsword now")]
            Oppresor = 114,

            Vampirism = 97,

            NaturalTalent = 100,

            Feast = 82,

            BountyHunter = 113,

            BattleTrance = 115,

            BatteringBlows = 129,

            PiercingThoughts = 132,

            WarlordsBloodlust = 145,

            FervorofBattle = 146,

            DeathFireTouch = 148
        }

        //updated 7 december 2016
        public enum Resolve
        {
            Recovery = 65,

            Unyielding = 66,

            Explorer = 81,

            SiegeMaster = 82,

            ToughSkin = 83,

            RunicArmor = 97,

            VeteransScar = 98,

            Insight = 113,

            Perseverance = 114,

            Fearless = 115,

            Swiftness = 129,

            LegendaryGuardian = 130,

            GraspoftheUndying = 145,

            CourageoftheColossus = 146,

            [Obsolete("Obsolete! Please update your assembly! StrengthoftheAges is called CourageoftheColossus now.")]
            StrengthoftheAges = 146,

            BondofStones = 147
        }

        #endregion

        #region Public Methods and Operators

        public static Mastery FindMastery(this Obj_AI_Hero @hero, MasteryPage page, int id)
        {
            var mastery = @hero.Masteries.FirstOrDefault(m => m.Page == page && m.Id == id);
            return mastery;
        }

        public static Mastery GetMastery(this Obj_AI_Hero hero, Ferocity ferocity)
        {
            return FindMastery(hero, MasteryPage.Ferocity, (int)ferocity);
        }

        public static Mastery GetMastery(this Obj_AI_Hero hero, Cunning cunning)
        {
            return FindMastery(hero, MasteryPage.Cunning, (int)cunning);
        }

        public static Mastery GetMastery(this Obj_AI_Hero hero, Resolve resolve)
        {
            return FindMastery(hero, MasteryPage.Resolve, (int)resolve);
        }

        public static bool IsActive(this Mastery mastery)
        {
            return mastery.Points >= 1;
        }

        #endregion
    }
}