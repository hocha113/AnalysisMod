using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using System.IO;
using AnalysisMod.AnalysisContent.NPCs;
using AnalysisMod.AnalysisCommon.Configs;

//Related to GlobalProjectile: ProjectileWithGrowingDamage
//与GlobalProjectile相关：具有逐渐增加伤害的投射物
namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    public class WeaponWithGrowingDamage : GlobalItem
    {
        public int experience;
        public static int experiencePerLevel = 100;
        private int bonusValuePerItem;
        public int level => experience / experiencePerLevel;

        public override bool InstancePerEntity => true;

        public override bool IsLoadingEnabled(Mod mod)
        {
            // To experiment with this Analysis, you'll need to enable it in the config.
            // 要尝试这个分析，您需要在配置中启用它。
            return ModContent.GetInstance<AnalysisModConfig>().WeaponWithGrowingDamageToggle;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            //Apply to weapons
            // 应用于武器
            return lateInstantiation && entity.damage > 0;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            experience = 0;
            GainExperience(item, tag.Get<int>("experience"));//Load experience tag
                                                             //加载经验标签
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            tag["experience"] = experience;//Save experience tag
                                           //保存经验标签
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(experience);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            experience = 0;
            GainExperience(item, reader.ReadInt32());
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitNPCGeneral(player, target, hit, item);
        }

        public void OnHitNPCGeneral(Player player, NPC target, NPC.HitInfo hit, Item item = null, Projectile projectile = null)
        {
            //The weapon gains experience when hitting an npc.
            // 武器攻击npc时获得经验。
            int xp = hit.Damage;
            if (projectile != null)
            {
                xp /= 2;
            }

            GainExperience(item, xp);
        }

        public void GainExperience(Item item, int xp)
        {
            experience += xp;

            UpdateValue(item);
        }

        public void UpdateValue(Item item, int stackChange = 0)
        {
            if (item == null)
            {
                return;
            }

            item.value -= bonusValuePerItem;
            int stack = item.stack + stackChange;
            if (stack == 0)
            {
                bonusValuePerItem = 0;
            }
            else
            {
                bonusValuePerItem = experience * 5 / stack;
            }

            item.value += bonusValuePerItem;
        }

        public override void UpdateInventory(Item item, Player player)
        {
            UpdateValue(item);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //Gain 1% multiplicative damage for every level on the weapon.
            // 每级武器增加1％的乘法伤害。
            damage *= 1f + level / 100f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (experience > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "level", $"Level: {level}") { OverrideColor = Color.LightGreen });
                string levelString = $" ({(level + 1) * experiencePerLevel - experience} to next level)";
                tooltips.Add(new TooltipLine(Mod, "experience", $"Experience: {experience}{levelString}") { OverrideColor = Color.White });
            }
        }

        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (item.type == ItemID.Snowball)
            {
                GainExperience(item, item.stack); // snowballs come with 1xp, for testing :)
                                                  // 雪球带有1xp，供测试使用 :)
            }

            if (context is RecipeItemCreationContext rContext)
            {
                foreach (Item ingredient in rContext.ConsumedItems)
                {
                    if (ingredient.TryGetGlobalItem(out WeaponWithGrowingDamage ingredientGlobal))
                    {
                        //Transfer all experience from consumed items to the crafted item.
                        // 将所有消耗品的经验转移到制作的物品上。
                        GainExperience(item, ingredientGlobal.experience);
                    }
                }
            }
        }

        public override void OnStack(Item destination, Item source, int numToTransfer)
        {
            if (!source.TryGetGlobalItem(out WeaponWithGrowingDamage weapon2))
            {
                return;
            }

            TransferExperience(destination, source, weapon2, numToTransfer);
        }

        public override void SplitStack(Item destination, Item source, int numToTransfer)
        {
            if (!source.TryGetGlobalItem(out WeaponWithGrowingDamage weapon2))
            {
                return;
            }

            //Prevent duplicating the experience on the new item, increase, which is a clone of decrease.  experience should not be cloned, so set it to 0.
            // 防止在新物品上复制经验，而是将其克隆到减少。
            experience = 0;

            TransferExperience(destination, source, weapon2, numToTransfer);
        }

        private void TransferExperience(Item destination, Item source, WeaponWithGrowingDamage weapon2, int numToTransfer)
        {
            //Transfer experience and value to increase.
            // 将经验和价值转移给增加项。
            experience += weapon2.experience;
            UpdateValue(destination, numToTransfer);

            if (source.stack > numToTransfer)
            {
                //Prevent duplicating the experience by clearing it on decrease if decrease will still exist.
                // 如果仍然存在，则通过清除减少来防止复制体验。
                weapon2.experience = 0;
                weapon2.UpdateValue(source, -numToTransfer);
            }
        }
    }

    public class DoubleXPSnowBallInAnalysisPersonShop : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            // To experiment with this Analysis, you'll need to enable it in the config.
            // 要尝试这个分析，您需要在配置中启用它。
            return ModContent.GetInstance<AnalysisModConfig>().WeaponWithGrowingDamageToggle;
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType != ModContent.NPCType<AnalysisPerson>())
            {
                return;
            }

            var snowball = new Item(ItemID.Snowball);
            if (snowball.TryGetGlobalItem(out WeaponWithGrowingDamage weapon))
            {
                weapon.GainExperience(snowball, 2);
            }
            shop.Add(snowball);
        }
    }
}
