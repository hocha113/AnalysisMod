using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.EntitySources
{
    // The following classes showcases pattern matching of IEntitySource instances to make things happen only in specific contexts.
    // 以下类展示了如何对IEntitySource实例进行模式匹配，以便只在特定上下文中发生操作。
    public sealed class AnalysisSourceDependentProjectileTweaks : GlobalProjectile
    {
        // Always override AppliesToEntity when you can!
        // 尽可能覆盖 AppliesToEntity！
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type is ProjectileID.BulletDeadeye;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // Make bullets shot by tactical skeletons do less damage
            // 降低战术骷髅发射的子弹伤害
            if (source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.type == NPCID.TacticalSkeleton)
            {
                projectile.damage /= 2;
            }
        }
    }

    public sealed class AnalysisSourceDependentItemTweaks : GlobalItem
    {
        public override void OnSpawn(Item item, IEntitySource source)
        {
            // Accompany all loot from trees with a slime.
            // 伴随着从树上获得的所有战利品都有一只史莱姆掉下来。
            if (source is EntitySource_ShakeTree)
            {
                NPC.NewNPC(source, (int)item.position.X, (int)item.position.Y, NPCID.BlueSlime);
            }
        }
    }

    public sealed class AnalysisSourceDependentItemTweaks2 : GlobalItem
    {
        // Always override AppliesToEntity when you can!
        // 尽可能覆盖 AppliesToEntity！
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type is ItemID.CopperCoin or ItemID.SilverCoin or ItemID.GoldCoin or ItemID.PlatinumCoin;
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            // make coins spawned from the lucky coin accessory fly into the air
            // 让幸运硬币饰品生成的硬币飞向空中
            if (source.Context == "LuckyCoin")
            {
                item.velocity.Y -= 20;
            }
        }
    }
}
