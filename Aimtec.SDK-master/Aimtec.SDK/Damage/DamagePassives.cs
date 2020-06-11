
// ReSharper disable ConvertToLambdaExpression
// ReSharper disable LoopCanBeConvertedToQuery

namespace Aimtec.SDK.Damage
{
    using Aimtec.SDK.Extensions;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Util.Cache;

    internal class DamagePassives
    {
        static DamagePassives()
        {
            #region Physical Damage Passives

            Passives.Add(new DamagePassive
                             {
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         var ardentCenserBuff = source.GetBuff("itemangelhandbuff");
                                         if (ardentCenserBuff != null)
                                         {
                                             var ardentCenserBuffCaster = ardentCenserBuff.Caster as Obj_AI_Hero;
                                             if (ardentCenserBuffCaster != null)
                                             {
                                                 return 19.12 + 0.88 * ardentCenserBuffCaster.Level;
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         var namiE = source.GetBuff("NamiE");
                                         if (namiE != null)
                                         {
                                             var namiECaster = namiE.Caster as Obj_AI_Hero;
                                             if (namiECaster != null)
                                             {
                                                 return namiECaster.GetSpellDamage(target, SpellSlot.E);
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         var leonaPassive = target.GetBuff("LeonaSunlight");
                                         if (leonaPassive != null)
                                         {
                                             var leonaPassiveCaster = leonaPassive.Caster as Obj_AI_Hero;
                                             if (leonaPassiveCaster != null)
                                             {
                                                 return 15 + 5 * leonaPassiveCaster.Level;
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.GetBuffCount("braummarkcounter") == 3)
                                         {
                                             return 16 + 10 * source.Level;
                                         }

                                         return 0;
                                     }
                             });


            Passives.Add(new DamagePassive
                             {
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         var buff = target.GetBuff("kalistacoopstrikemarkbuff");
                                         if (buff != null &&
                                             source.HasBuff("kalistacoopstrikeally"))
                                         {
                                             var buffCaster = buff.Caster as Obj_AI_Hero;
                                             if (buffCaster != null)
                                             {
                                                 if (target.Type == GameObjectType.obj_AI_Minion && target.Health < 125)
                                                 {
                                                     return target.Health;
                                                 }

                                                 return buffCaster.GetSpellDamage(target, SpellSlot.W);
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Aatrox",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("aatroxwpower") &&
                                             source.HasBuff("aatroxwonhpowerbuff"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Akali",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("AkaliMota"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Detonation);
                                         }

                                         if (source.HasBuff("akalishadowstate"))
                                         {
                                             return
                                                 0.5 * source.FlatPhysicalDamageMod +
                                                 0.75 * source.TotalAbilityDamage +
                                                 new[] { 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 40, 50, 60, 70, 80, 90, 100 }[source.Level - 1];
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Alistar",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("alistartrample"))
                                         {
                                             return 40 + 15 * source.Level;
                                         }

                                         return 0;
                                     }
                             });

            /*
             Passives.Add(new DamagePassive
                             {
                                 Name = "Ashe",
                                 DamageType = DamagePassive.PassiveDamageType.PercentPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("asheqattack"))
                                         {
                                             return new[] { 0.20, 0.21, 0.22, 0.23, 0.24, 0.25 }[source.SpellBook.GetSpell(SpellSlot.Q).Level - 1] * source.TotalAttackDamage;
                                         }

                                         return 1;
                                     }
                             });
            */

            Passives.Add(new DamagePassive
                             {
                                 Name = "Ashe",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("ashepassiveslow"))
                                         {
                                             return (0.1 + source.Crit / 100 * (1 + (source.HasItem(ItemId.InfinityEdge) ? 0.5 : 0))) * source.TotalAttackDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Bard",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.GetRealBuffCount("bardpspiritammocount") > 0)
                                         {
                                             return 30 + 15 * (source.GetRealBuffCount("bardpdisplaychimecount") / 5) + 0.3 * source.TotalAbilityDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Blitzcrank",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("powerfist"))
                                         {
                                             return source.TotalAttackDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Braum",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("braummarkstunreduction"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Buff);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Caitlyn",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("caitlynheadshot") ||
                                             source.HasBuff("caitlynheadshotrangecheck") && target.HasBuff("caitlynyordletrapinternal"))
                                         {
                                             var damage = 0d;
                                             switch (target.Type)
                                             {
                                                 case GameObjectType.obj_AI_Minion:
                                                     damage = 1.5 * source.TotalAttackDamage;
                                                     break;

                                                 case GameObjectType.obj_AI_Hero:
                                                     var critDamageMultiplier = source.HasItem(ItemId.InfinityEdge) ? 2.5 : 2;

                                                     damage = (50 + 0.5 * critDamageMultiplier * source.Crit * 100) / 100 * source.TotalAttackDamage;

                                                     if (target.HasBuff("caitlynyordletrapinternal"))
                                                     {
                                                         damage += new[] { 30, 70, 110, 150, 190 }[source.SpellBook.GetSpell(SpellSlot.W).Level - 1] + 0.7 * source.TotalAttackDamage;
                                                     }
                                                     break;
                                             }

                                             return damage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Camille",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("camiller"))
                                         {
                                             return
                                                 new[] { 5, 10, 15 }[source.SpellBook.GetSpell(SpellSlot.R).Level - 1] +
                                                 new[] { 0.04, 0.06, 0.08 }[source.SpellBook.GetSpell(SpellSlot.R).Level - 1] * (target.MaxHealth - target.Health);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "ChoGath",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("VorpalSpikes"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.E);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Corki",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         return 0.8 * source.TotalAttackDamage;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Corki",
                                 DamageType = DamagePassive.PassiveDamageType.PercentPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         return 0.2;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Darius",
                                 DamageType = DamagePassive.PassiveDamageType.PercentPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("dariusnoxiantacticsonh"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W) * source.TotalAttackDamage;
                                         }

                                         return 1;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Diana",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.GetBuffCount("dianapassivemarker") == 2)
                                         {
                                             return new[] { 20, 25, 30, 35, 40, 50, 60, 70, 80, 90, 105, 120, 135, 155, 175, 200, 225, 250 }[source.Level - 1] + 0.8 * source.TotalAbilityDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Draven",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("dravenspinningattack"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "DrMundo",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("masochism"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.E);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Ekko",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.GetBuffCount("ekkostacks") == 2)
                                         {
                                             return new[] { 30, 40, 50, 60, 70, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140 }[source.Level - 1] + 0.8 * source.TotalAbilityDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Ekko",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (!source.SpellBook.GetSpell(SpellSlot.W).State.HasFlag(SpellState.NotLearned) && target.HealthPercent() < 30)
                                         {
                                             double damage = 0;
                                             switch (target.Type)
                                             {
                                                 case GameObjectType.obj_AI_Hero:
                                                     damage = Math.Max(0.03 + 0.03 * (source.TotalAbilityDamage / 100), 15);
                                                     break;

                                                 case GameObjectType.obj_AI_Minion:
                                                     damage = Math.Min(Math.Max(0.03 + 0.03 * (source.TotalAbilityDamage / 100), 15), 150);
                                                     break;
                                             }

                                             return damage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Ekko",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("ekkoeattackbuff"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.E);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Fizz",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("FizzW"))
                                         {
                                             if (ObjectManager.Get<GameObject>().Any(o => o.Distance(target) <= 50 && o.Name == "Fizz_Base_W_DmgMarker_champion.troy"))
                                             {
                                                 return source.GetSpellDamage(target, SpellSlot.W);
                                             }
                                             if (ObjectManager.Get<GameObject>().Any(o => o.Distance(target) <= 50 && o.Name == "Fizz_Base_W_DmgMarkerMaintain.troy"))
                                             {
                                                 return source.GetSpellDamage(target, SpellSlot.W, DamageStage.Empowered);
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Galio",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("galiopassivebuff"))
                                         {
                                             return 8 + 4 * source.Level + source.TotalAttackDamage + (source.TotalAbilityDamage + 0.4) + source.BonusSpellBlock * 0.4;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Garen",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("garenq"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Gnar",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.GetBuffCount("gnarwproc") == 2)
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Gragas",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("gragaswattackbuff"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Hecarim",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("hecarimrampspeed"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.E);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Illaoi",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("IllaoiW"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W)
                                                    + source.GetSpellDamage(target, SpellSlot.Q)
                                                    * GameObjects.Minions.Count(
                                                        i =>
                                                            i.Team == source.Team &&
                                                            i.UnitSkinName == "illaoiminion" &&
                                                            i.Distance(target) < 800 && (i.IsFloatingHealthBarActive || i.HasBuff("illaoir2")));
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Irelia",
                                 DamageType = DamagePassive.PassiveDamageType.FlatTrue,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("ireliahitenstylecharged"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "JarvanIV",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (!target.HasBuff("jarvanivmartialcadencecheck"))
                                         {
                                             switch (target.Type)
                                             {
                                                 case GameObjectType.obj_AI_Hero:
                                                     return target.Health * 0.1;

                                                 case GameObjectType.obj_AI_Minion:
                                                     return Math.Min(target.Health * 0.1, 400);
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Jax",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("JaxEmpowerTwo"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Jayce",
                                 DamageType = DamagePassive.PassiveDamageType.PercentPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("jaycehypercharge"))
                                         {
                                             return new[] { 0.7, 0.78, 0.86, 0.94, 1.02, 1.1 }[source.SpellBook.GetSpell(SpellSlot.W).Level - 1] * source.TotalAttackDamage;
                                         }

                                         return 1;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Jhin",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (Math.Abs(source.Crit - 1) < float.Epsilon)
                                         {
                                             return
                                                 (source.TotalAttackDamage
                                                        + Math.Round((new[] { 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 40 }[source.Level - 1]
                                                                        + Math.Round(source.Crit * 100 / 10 * 4)
                                                                        + Math.Round((source.AttackSpeedMod - 1) * 100 / 10) * 2.5) / 100 * source.TotalAttackDamage)) * (source.HasItem(ItemId.InfinityEdge) ? 0.875 : 0.5);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Jhin",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("JhinPassiveAttackBuff"))
                                         {
                                             return (source.Level < 6
                                                        ? 0.15 :
                                                        source.Level < 11
                                                           ? 0.2
                                                           : 0.25) * (target.MaxHealth - target.Health) * (source.HasItem(ItemId.InfinityEdge) ? 1.875 : 1.5);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Jinx",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("JinxQ"))
                                         {
                                             return 1.1 * source.TotalAttackDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Kalista",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("kalistacoopstrikemarkally"))
                                         {
                                             if (target.Type == GameObjectType.obj_AI_Minion && target.Health < 125)
                                             {
                                                 return target.Health;    
                                             }

                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Kalista",
                                 DamageType = DamagePassive.PassiveDamageType.PercentPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         return 0.9;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Kassadin",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (!source.SpellBook.GetSpell(SpellSlot.W).State.HasFlag(SpellState.NotLearned))
                                         {
                                             return 20 + 0.1 * source.TotalAbilityDamage + (source.HasBuff("NetherBlade")
                                                                                                ? source.GetSpellDamage(target, SpellSlot.W)
                                                                                                : 0);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "KogMaw",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("kogmawbioarcanebarrage"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Lucian",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("lucianpassivebuff"))
                                         {
                                             double multiplier;
                                             switch (target.Type)
                                             {
                                                 case GameObjectType.obj_AI_Minion:
                                                     multiplier = 1;
                                                     break;

                                                 default:
                                                     multiplier = source.Level < 6
                                                                     ? 0.3 :
                                                                     source.Level < 11
                                                                        ? 0.4 :
                                                                        source.Level < 16
                                                                           ? 0.5
                                                                           : 0.6;
                                                     break;
                                             }

                                             return source.TotalAttackDamage * multiplier;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Lux",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("luxilluminatingfraulein"))
                                         {
                                             return 10 + 10 * source.Level + 0.2 * source.TotalAbilityDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "MasterYi",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("doublestrike"))
                                         {
                                             return 0.5 * source.TotalAttackDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "MasterYi",
                                 DamageType = DamagePassive.PassiveDamageType.FlatTrue,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("wujustylesuperchargedvisual"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.E);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "MissFortune",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         var passiveObject = ObjectManager.Get<GameObject>()
                                             .FirstOrDefault(o => o.IsValid && o.Name == "MissFortune_Base_P_Mark.troy");
                                         if (passiveObject != null)
                                         {
                                             var passiveUnit = ObjectManager.Get<AttackableUnit>()
                                                 .Where(m => m.IsValidTarget())
                                                 .MinBy(o => o.Distance(passiveObject));

                                             if (passiveUnit != null &&
                                                 target != passiveUnit)
                                             {
                                                 var damage = source.TotalAttackDamage * (source.Level < 4
                                                                                              ? 0.25 :
                                                                                              source.Level < 7
                                                                                                  ? 0.3 :
                                                                                                  source.Level < 9
                                                                                                      ? 0.35 :
                                                                                                      source.Level < 11
                                                                                                          ? 0.4 :
                                                                                                          source.Level < 13
                                                                                                              ? 0.45
                                                                                                              : 0.5);
                                                 switch (target.Type)
                                                 {
                                                     case GameObjectType.obj_AI_Minion:
                                                         return damage;

                                                     default:
                                                         return damage * 2;
                                                 }
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Nasus",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("NasusQ"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q) + source.GetRealBuffCount("nasusqstacks");
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Orianna",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         var baseDamage = 0.15 * source.TotalAbilityDamage +
                                                          (source.Level < 4
                                                              ? 10 :
                                                              source.Level < 7
                                                                  ? 18 :
                                                                  source.Level < 10
                                                                      ? 26 :
                                                                      source.Level < 13
                                                                          ? 34 :
                                                                          source.Level < 16
                                                                              ? 42
                                                                              : 50);

                                         return baseDamage * (1 + 0.20 * source.GetRealBuffCount("orianapowerdaggerdisplay"));
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Quinn",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("quinnw"))
                                         {
                                             return 10 + 5 * source.Level + (0.14 + 0.02 * source.Level) * source.TotalAttackDamage;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Sejuani",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.HasBuff("sejuanistun"))
                                         {
                                             switch (target.Type)
                                             {
                                                 case GameObjectType.obj_AI_Hero:
                                                     if (source.Level < 7)
                                                     {
                                                         return 0.1 * target.MaxHealth;
                                                     }
                                                     else if (source.Level < 14)
                                                     {
                                                         return 0.15 * target.MaxHealth;
                                                     }
                                                     else
                                                     {
                                                         return 0.2 * target.MaxHealth;
                                                     }

                                                 case GameObjectType.obj_AI_Minion:
                                                     return 400;
                                             }
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Teemo",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (!source.SpellBook.GetSpell(SpellSlot.E).State.HasFlag(SpellState.NotLearned))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.E);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Thresh",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (!source.SpellBook.GetSpell(SpellSlot.E).State.HasFlag(SpellState.NotLearned))
                                         {
                                             var multiplier = (double)(source.GetRealBuffCount("threshpassivesoulsgain") + (0.5f + 0.3f * source.SpellBook.GetSpell(SpellSlot.E).Level) * source.TotalAttackDamage);
                                             if (source.HasBuff("threshepassive3"))
                                             {
                                                 multiplier *= 0.5;
                                             }
                                             else if (source.HasBuff("threshepassive2"))
                                             {
                                                 multiplier *= 0.333;
                                             }
                                             else if (source.HasBuff("threshepassive1"))
                                             {
                                                 multiplier *= 0.25;
                                             }

                                             return multiplier;
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Vayne",
                                 DamageType = DamagePassive.PassiveDamageType.FlatPhysical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("vaynetumblebonus"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Vayne",
                                 DamageType = DamagePassive.PassiveDamageType.FlatTrue,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (target.GetBuffCount("vaynesilvereddebuff") == 2)
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.W);
                                         }

                                         return 0;
                                     }
                             });

            Passives.Add(new DamagePassive
                             {
                                 Name = "Viktor",
                                 DamageType = DamagePassive.PassiveDamageType.FlatMagical,
                                 PassiveDamage = (source, target) =>
                                     {
                                         if (source.HasBuff("ViktorPowerTransferReturn"))
                                         {
                                             return source.GetSpellDamage(target, SpellSlot.Q, DamageStage.SecondForm);
                                         }

                                         return 0;
                                     }
                             });

            #endregion
        }

        public static PassiveDamageResult ComputePassiveDamages(Obj_AI_Hero source, Obj_AI_Base target)
        {
            double totalPhysicalDamage = 0;
            double totalMagicalDamage = 0;
            double totalTrueDamage = 0;

            double totalPhysicalDamageMod = 1;
            double totalMagicalDamageMod = 1;
            double totalTrueDamageMod = 1;

            foreach (var passive in Passives)
            {
                if (passive.Name != null &&
                    source.ChampionName != passive.Name)
                {
                    continue;
                }

                var passiveDamage = passive.GetDamage(source, target);
                switch (passive.DamageType)
                {
                    case DamagePassive.PassiveDamageType.FlatPhysical:
                        totalPhysicalDamage += passiveDamage;
                        break;

                    case DamagePassive.PassiveDamageType.FlatMagical:
                        totalMagicalDamage += passiveDamage;
                        break;

                    case DamagePassive.PassiveDamageType.FlatTrue:
                        totalTrueDamage += passiveDamage;
                        break;

                    case DamagePassive.PassiveDamageType.PercentPhysical:
                        totalPhysicalDamageMod *= passiveDamage;
                        break;

                    case DamagePassive.PassiveDamageType.PercentMagical:
                        totalMagicalDamageMod *= passiveDamage;
                        break;

                    case DamagePassive.PassiveDamageType.PercentTrue:
                        totalTrueDamageMod *= passiveDamage;
                        break;
                }
            }

            return new PassiveDamageResult(totalPhysicalDamage, totalMagicalDamage, totalTrueDamage, totalPhysicalDamageMod, totalMagicalDamageMod, totalTrueDamageMod);
        }

        public static List<DamagePassive> Passives { get; set; } = new List<DamagePassive>();

        public class PassiveDamageResult
        {
            public PassiveDamageResult(double physicalDamage, double magicalDamage, double trueDamage, double physicalDamagePercent, double magicalDamagePercent, double trueDamagePercent)
            {
                this.PhysicalDamage = Math.Floor(physicalDamage);
                this.MagicalDamage = Math.Floor(magicalDamage);
                this.TrueDamage = Math.Floor(trueDamage);

                this.PhysicalDamagePercent = physicalDamagePercent;
                this.MagicalDamagePercent = magicalDamagePercent;
                this.TrueDamagePercent = trueDamagePercent;
            }

            public double PhysicalDamage { get; set; }
            public double MagicalDamage { get; set; }
            public double TrueDamage { get; set; }
            public double PhysicalDamagePercent { get; set; }
            public double MagicalDamagePercent { get; set; }
            public double TrueDamagePercent { get; set; }
        }

        public class DamagePassive
        {
            public string Name { get; set; }

            public delegate double PassiveDamageDelegateHandler(Obj_AI_Hero source, Obj_AI_Base target);

            public PassiveDamageDelegateHandler PassiveDamage { get; set; }

            public double GetDamage(Obj_AI_Hero source, Obj_AI_Base target)
            {
                if (source != null && target != null && this.PassiveDamage != null)
                {
                    return this.PassiveDamage(source, target);
                }

                return 0;
            }

            public PassiveDamageType DamageType { get; set; }

            public enum PassiveDamageType
            {
                FlatPhysical,
                FlatMagical,
                FlatTrue,
                PercentPhysical,
                PercentMagical,
                PercentTrue
            }
        }
    }
}