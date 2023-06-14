using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    public class AnalysisStatBonusAccessory : ModItem
    {
        // By declaring these here, changing the values will alter the effect, and the tooltip
        public static readonly int AdditiveDamageBonus = 25;
        public static readonly int MultiplicativeDamageBonus = 12;
        public static readonly int BaseDamageBonus = 4;
        public static readonly int FlatDamageBonus = 5;
        public static readonly int MeleeCritBonus = 10;
        public static readonly int RangedAttackSpeedBonus = 15;
        public static readonly int MagicArmorPenetration = 5;
        public static readonly int AnalysisKnockback = 100;
        public static readonly int AdditiveCritDamageBonus = 20;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 5));
        }

        // Insert the modifier values into the tooltip localization
        // 通过在这里声明，更改值将改变效果和工具提示
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, MultiplicativeDamageBonus, BaseDamageBonus, FlatDamageBonus, MeleeCritBonus, RangedAttackSpeedBonus, MagicArmorPenetration, AnalysisKnockback, AdditiveCritDamageBonus);

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // GetDamage returns a reference to the specified damage class' damage StatModifier.
            // Since it doesn't return a value, but a reference to it, you can freely modify it with mathematics operators (+, -, *, /, etc.).
            // StatModifier is a structure that separately holds float additive and multiplicative modifiers, as well as base damage and flat damage.
            // When StatModifier is applied to a value, its additive modifiers are applied before multiplicative ones.
            // Base damage is added directly to the weapon's base damage and is affected by damage bonuses, while flat damage is applied after all other calculations.
            // In this case, we're doing a number of things:
            // - Adding 25% damage, additively. This is the typical "X% damage increase" that accessories use, use this one.
            // - Adding 12% damage, multiplicatively. This effect is almost never useds in Terraria, typically you want to use the additive multiplier above. It is extremely hard to correctly balance the game with multiplicative bonuses.
            // - Adding 4 base damage.
            // - Adding 5 flat damage.
            // Since we're using DamageClass.Generic, these bonuses apply to ALL damage the player deals.

            // GetDamage返回指定伤害类别的伤害StatModifier的引用。
            // 由于它不返回一个值，而是一个引用，因此您可以自由地使用数学运算符（+、-、*、/等）进行修改。
            // StatModifier是一种结构体，分别包含浮点加法和乘法修饰符以及基础伤害和平坦伤害。
            // 当StatModifier应用于一个值时，在乘法修饰符之前应用其加法修饰符。
            // 基础伤害直接添加到武器的基础伤害中，并受到伤害奖励的影响，而平坦伤害则在所有其他计算之后应用。
            // 在这种情况下，我们正在做以下几件事：
            // - 添加25%的额外伤害。这是配件通常使用的“X％增加攻击力”的典型方式，请使用此选项。
            // - 添加12% 的额外倍率。Terraria几乎从未使用过该效果,通常希望使用上面提到的附加乘数。 使用多重奖金极难正确平衡游戏
            // - 添加4个基础伤害。
            // - 添加5个平坦伤害。
            // 由于我们使用DamageClass.Generic，因此这些奖励适用于玩家造成的所有伤害。
            player.GetDamage(DamageClass.Generic) += AdditiveDamageBonus / 100f;
            player.GetDamage(DamageClass.Generic) *= 1 + MultiplicativeDamageBonus / 100f;
            player.GetDamage(DamageClass.Generic).Base += BaseDamageBonus;
            player.GetDamage(DamageClass.Generic).Flat += FlatDamageBonus;

            // GetCrit, similarly to GetDamage, returns a reference to the specified damage class' crit chance.
            // In this case, we're adding 10% crit chance, but only for the melee DamageClass (as such, only melee weapons will receive this bonus).
            // NOTE: Once all crit calculations are complete, a weapon or class' total crit chance is typically cast to an int. Plan accordingly.

            // GetCrit与GetDamage类似，返回指定伤害类别的暴击几率引用。
            // 在这种情况下，我们为近战DamageClass添加了10％的暴击几率（因此只有近战武器会获得此奖金）。
            // 注意：完成所有爆击计算后，通常将武器或职业的总爆击几率转换为int。请做好相应规划。
            player.GetCritChance(DamageClass.Melee) += MeleeCritBonus;

            // GetAttackSpeed is functionally identical to GetDamage and GetKnockback; it's for attack speed.
            // In this case, we'll make ranged weapons 15% faster to use overall.
            // NOTE: Zero or a negative value as the result of these calculations will throw an exception. Plan accordingly.

            // GetAttackSpeed在功能上与GetDamage和GetKnockback相同； 它是攻击速度。
            // 在这种情况下，我们将使远程武器整体使用速度快15％。
            // 注意：这些计算结果为零或负值将抛出异常。请做好相应规划。
            player.GetAttackSpeed(DamageClass.Ranged) += RangedAttackSpeedBonus / 100f;

            // GetArmorPenetration is functionally identical to GetCritChance, but for the armor penetration stat instead.
            // In this case, we'll add 5 armor penetration to magic weapons.
            // NOTE: Once all armor pen calculations are complete, the final armor pen amount is cast to an int. Plan accordingly.

            // GetArmorPenetration在功能上与GetCritChance相同，但针对装甲穿透统计数据而不是暴击率
            // 在本例中, 我们将魔法武器增加5点穿透力.
            // 注意: 所有装甲穿透力计算完成后, 最终数值会被转换成一个整数. 请做好相关规划.
            player.GetArmorPenetration(DamageClass.Magic) += MagicArmorPenetration;

            // GetKnockback is functionally identical to GetDamage, but for the knockback stat instead.
            // In this case, we're adding 100% knockback additively, but only for our custom Analysis DamageClass (as such, only our Analysis class weapons will receive this bonus).

            // GetKnockback在功能上与GetDamage相同，但针对击退统计数据而不是伤害。
            // 在这种情况下，我们正在添加100％的额外击退力，但仅适用于我们自定义的Analysis DamageClass（因此只有我们的Analysis类武器会获得此奖金）。
            player.GetKnockback<AnalysisDamageClass>() += AnalysisKnockback / 100f;

            player.GetModPlayer<AnalysisDamageModificationPlayer>().AdditiveCritDamageBonus += AdditiveCritDamageBonus / 100f;
        }
    }
}