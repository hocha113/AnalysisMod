using AnalysisMod.AnalysisContent.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // This file showcases the concept of piercing.
    // The code of the item that spawns it is located at the bottom.

    // 这个文件展示了穿透与无敌帧相关的机制的概念。
    // 生成它的物品代码位于底部。

    // NPC.immune determines if an npc can be hit by a item or projectile owned by a particular player (it is an array, each slot corresponds to different players (whoAmI))
    // NPC.immune is decremented towards 0 every update
    // Melee items set NPC.immune to player.itemAnimation, which starts at item.useAnimation and decrements towards 0
    // Projectiles, however, provide mechanisms for custom immunity.
    // 1. penetrate == 1: A projectile with penetrate set to 1 in SetDefaults will hit regardless of the npc's immunity counters (The penetrate from SetDefaults is remembered in maxPenetrate)
    //	Ex: Wooden Arrow.
    // 2. No code and penetrate > 1, penetrate == -1, or (appliesImmunityTimeOnSingleHits && penetrate == 1): npc.immune[owner] will be set to 10.
    // 	The NPC will be hit if not immune and will become immune to all damage for 10 ticks
    // 	Ex: Unholy Arrow
    // 3. Override OnHitNPC: If not immune, when it hits it manually set an immune other than 10
    // 	Ex: Arkhalis: Sets it to 5
    // 	Ex: Sharknado Minion: Sets to 20
    // 	Video: https://gfycat.com/DisloyalImprobableHoatzin Notice how Sharknado minion hits prevent Arhalis hits for a brief moment.
    // 4. Projectile.usesIDStaticNPCImmunity and Projectile.idStaticNPCHitCooldown: Specifies that a type of projectile has a shared immunity timer for each npc.
    // 	Use this if you want other projectiles a chance to damage, but don't want the same projectile type to hit an npc rapidly.
    // 	Ex: Ghastly Glaive is the only one who uses this.
    // 5. Projectile.usesLocalNPCImmunity and Projectile.localNPCHitCooldown: Specifies the projectile manages it's own immunity timers for each npc
    // 	Use this if you want the multiple projectiles of the same type to have a chance to attack rapidly, but don't want a single projectile to hit rapidly. A -1 value prevents the same projectile from ever hitting the npc again.
    // 	Ex: Lightning Aura sentries use this. (localNPCHitCooldown = 3, but other code controls how fast the projectile itself hits)
    // 		Overlapping Auras all have a chance to hit after each other even though they share the same ID.
    // Try the above by uncommenting out the respective bits of code in the projectile below.

    // NPC.immune 确定NPC是否可以被特定玩家拥有的物品或抛射物击中（它是一个数组，每个插槽对应不同的玩家（whoAmI））
    // NPC.immune 每次更新都会减少到0
    // 近战武器将NPC.immune设置为player.itemAnimation，该值从item.useAnimation开始递减至0
    // 抛射物提供自定义免疫机制。
    // 1. penetrate == 1: 在SetDefaults中penetrate设置为1的抛射物将无视npc的免疫计数器进行攻击（SetDefaults中记住了maxPenetrate）
    // 例如：木箭头。
    // 2. 没有代码且penetrate>1、penetrate==-1或(appliesImmunityTimeOnSingleHits&&penetrate==1)：npc.immune[owner]将被设为10。
    // 如果没有免疫，则会受到攻击，并在10个tick内对所有伤害产生免疫效果
    // 例如：邪恶箭头
    // 3. 覆盖OnHitNPC：如果未受影响，则手动设置除10以外其他类型的免疫效果
    // 例如：Arkhalis：将其设置为5
    // 例如：鲨卷风仆从：设置为20
    // 视频: https://gfycat.com/DisloyalImprobableHoatzin 注意Sharknado仆从的攻击如何在短暂的时间内阻止Arhalis的攻击。
    // 4. Projectile.usesIDStaticNPCImmunity和Projectile.idStaticNPCHitCooldown: 指定一种抛射物对每个npc具有共享免疫计时器。
    // 如果您希望其他抛射物有机会造成伤害，但不希望同一类型的抛射物快速打击npc，则使用此选项。
    // 例如：幽灵长戟是唯一使用这种方式的武器。
    // 5. Projectile.usesLocalNPCImmunity和Projectile.localNPCHitCooldown: 指定该抛射物管理其自己的每个npc免疫计时器
    // 如果您想让多个相同类型的抛射物有机会快速攻击，但不希望单个抛射物快速打击，则使用此选项。 -1值可以防止同一投掷武器再次命中npc。
    // 例如：闪电光环哨站使用此功能。（localNPCHitCooldown =3，但其他代码控制了弹丸本身命中目标的速度）
    // 重叠光环都有机会在彼此之后命中。
    // 取消下面抛射物代码中相应部分的注释以进行测试。


    public class AnalysisPiercingProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12; // The width of projectile hitbox
            Projectile.height = 12; // The height of projectile hitbox

            // Ccopy the ai of any given projectile using AIType, since we want
            // the projectile to essentially behave the same way as the vanilla projectile.
            AIType = ProjectileID.Bullet;

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 60; // Each update timeLeft is decreased by 1. Once timeLeft hits 0, the Projectile will naturally despawn. (60 ticks = 1 second)

            Projectile.penetrate = -1;
            // 1: Projectile.penetrate = 1; // Will hit even if npc is currently immune to player
            // 2a: Projectile.penetrate = -1; // Will hit and unless 3 is use, set 10 ticks of immunity
            // 2b: Projectile.penetrate = 3; // Same, but max 3 hits before dying
            // 5: Projectile.usesLocalNPCImmunity = true;
            // 5a: Projectile.localNPCHitCooldown = -1; // 1 hit per npc max
            // 5b: Projectile.localNPCHitCooldown = 20; // 20 ticks before the same npc can be hit again

            // 1: Projectile.penetrate = 1; // 即使npc当前对玩家免疫，也会命中
            // 2a: Projectile.penetrate = -1; // 命中并且除非使用3，否则设置10个ticks的免疫时间
            // 2b: Projectile.penetrate = 3; // 相同，但最多只能命中3次就死亡
            // 5: Projectile.usesLocalNPCImmunity = true;
            // 5a: Projectile.localNPCHitCooldown = -1; // 每个npc最多被击中一次
            // 5b: Projectile.localNPCHitCooldown = 20; // 同一个npc再次受到攻击前需要等待20 ticks
        }

        // See comments at the beginning of the class
        // 参见类开头的注释
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 3a: target.immune[Projectile.owner] = 20;
            // 3b: target.immune[Projectile.owner] = 5;
        }
    }

    // This is a simple item that is based on the FlintlockPistol and shoots AnalysisPiercingProjectile to showcase it.
    // 这是一个简单的物品，基于FlintlockPistol，并射出AnalysisPiercingProjectile以展示它。
    internal class AnalysisPiercingProjectileItem : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.FlintlockPistol}";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FlintlockPistol);
            Item.useAmmo = AmmoID.None;
            Item.shoot = ModContent.ProjectileType<AnalysisPiercingProjectile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
