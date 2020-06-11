
// ReSharper disable ConvertToLambdaExpression
// ReSharper disable LoopCanBeConvertedToQuery

namespace Aimtec.SDK.Damage
{
    using Aimtec.SDK.Extensions;

    using System;
    using System.Collections.Generic;

    internal class DamageItems
    {
        static DamageItems()
        {
            #region Physical Damage Items

            Items.Add(new DamageItem
                          {
                              Id = ItemId.BladeoftheRuinedKing,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      switch (target.Type)
                                      {
                                          case GameObjectType.obj_AI_Minion:
                                              return Math.Min(0.08 * target.Health, 60);

                                          case GameObjectType.obj_AI_Hero:
                                              return Math.Max(0.08 * target.Health, 15);
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.EnchantmentBloodrazor,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      switch (target.Type)
                                      {
                                          case GameObjectType.obj_AI_Minion:
                                              return Math.Min(0.04 * target.MaxHealth, 75);
                                          
                                          case GameObjectType.obj_AI_Hero:
                                              return 0.04 * target.MaxHealth;
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.RecurveBow,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      return 15;
                                  }
                          });


            Items.Add(new DamageItem
                          {
                              Id = ItemId.TitanicHydra,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.HasBuff("itemtitanichydracleavebuff"))
                                      {
                                          return 40 + 0.1 * source.MaxHealth;
                                      }

                                      return 5 + 0.01 * source.MaxHealth;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.TrinityForce,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.HasBuff("sheen"))
                                      {
                                          return source.BaseAttackDamage * 2;
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.IcebornGauntlet,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.HasBuff("sheen"))
                                      {
                                          return source.BaseAttackDamage;
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.Sheen,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.HasBuff("sheen"))
                                      {
                                          return source.BaseAttackDamage;
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.Muramana,
                              DamageType = DamageItem.ItemDamageType.Physical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.ManaPercent() > 20)
                                      {
                                          return 0.06 * source.Mana;
                                      }

                                      return 0;
                                  }
                          });

            #endregion

            #region Magical Damage Items

            Items.Add(new DamageItem
                          {
                              Id = ItemId.GuinsoosRageblade,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      return 15;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.NashorsTooth,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      return 15 + 0.15 * source.TotalAbilityDamage;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.WitsEnd,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      return 40;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.LichBane,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.HasBuff("LichBane"))
                                      {
                                          return 0.75 * source.BaseAttackDamage + 0.50 * source.TotalAbilityDamage;
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.KircheisShard,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.GetBuffCount("ItemStatikShankCharge") == 100)
                                      {
                                          return 50;
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.RapidFirecannon,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.GetBuffCount("ItemStatikShankCharge") == 100)
                                      {
                                          return new[] { 50, 50, 50, 50, 50, 50, 56, 67, 72, 77, 83, 88, 94, 99, 104, 110, 115, 120 }[source.Level - 1];
                                      }

                                      return 0;
                                  }
                          });

            Items.Add(new DamageItem
                          {
                              Id = ItemId.StatikkShiv,
                              DamageType = DamageItem.ItemDamageType.Magical,
                              ItemDamage = (source, target) =>
                                  {
                                      if (source.GetBuffCount("ItemStatikShankCharge") == 100)
                                      {
                                          return new[] { 60, 60, 60, 60, 60, 68, 76, 84, 91, 99, 107, 114, 122, 130, 137, 145, 153, 160 }[source.Level - 1];
                                      }

                                      return 0;
                                  }
                          });

            #endregion
        }

        public static ItemDamageResult ComputeItemDamages(Obj_AI_Base source, Obj_AI_Base target)
        {
            double totalPhysicalDamage = 0;
            double totalMagicalDamage = 0;
            double totalTrueDamage = 0;
            
            foreach (var item in Items)
            {
                if (!source.HasItem(item.Id))
                {
                    continue;
                }

                switch (item.DamageType)
                {
                    case DamageItem.ItemDamageType.Physical:
                        totalPhysicalDamage += item.GetDamage(source, target);
                        break;

                    case DamageItem.ItemDamageType.Magical:
                        totalMagicalDamage += item.GetDamage(source, target);
                        break;

                    case DamageItem.ItemDamageType.True:
                        totalTrueDamage += item.GetDamage(source, target);
                        break;
                }
            }

            return new ItemDamageResult(totalPhysicalDamage, totalMagicalDamage, totalTrueDamage);
        }

        public static List<DamageItem> Items { get; set; } = new List<DamageItem>();

        public class ItemDamageResult
        {
            public ItemDamageResult(double physicalDamage, double magicalDamage, double trueDamage)
            {
                this.PhysicalDamage = Math.Floor(physicalDamage);
                this.MagicalDamage = Math.Floor(magicalDamage);
                this.TrueDamage = Math.Floor(trueDamage);
            }

            public double PhysicalDamage { get; set; }
            public double MagicalDamage { get; set; }
            public double TrueDamage { get; set; }
        }

        public class DamageItem
        {
            public uint Id { get; set; }

            public delegate double ItemDamageDelegateHandler(Obj_AI_Base source, Obj_AI_Base target);

            public ItemDamageDelegateHandler ItemDamage { get; set; }

            public double GetDamage(Obj_AI_Base source, Obj_AI_Base target)
            {
                if (source != null && target != null && this.ItemDamage != null)
                {
                    return this.ItemDamage(source, target);
                }

                return 0;
            }

            public ItemDamageType DamageType { get; set; }

            public enum ItemDamageType
            {
                Physical,
                Magical,
                True
            }
        }
    }
}
