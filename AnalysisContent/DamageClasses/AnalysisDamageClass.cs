using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.DamageClasses
{//【红茶注：这个项目的注释量很大，翻译压力也较大，欢迎纠错】
	public class AnalysisDamageClass : DamageClass
	{
        // This is an Analysis damage class designed to demonstrate all the current functionality of the feature and explain how to create one of your own, should you need one.
        // For information about how to apply stat bonuses to specific damage classes, please instead refer to AnalysisMod/AnalysisContent/Items/Accessories/AnalysisStatBonusAccessory.

        // 这是一个分析伤害类，旨在展示该功能的所有当前功能，并解释如何创建自己的伤害类（如果需要）。
        // 有关如何将统计加成应用于特定的伤害类，请参阅AnalysisMod / AnalysisContent / Items / Accessories / AnalysisStatBonusAccessory。
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
            // This method lets you make your damage class benefit from other classes' stat bonuses by default, as well as universal stat bonuses.
            // To briefly summarize the two nonstandard damage class names used by DamageClass:
            // Default is, you guessed it, the default damage class. It doesn't scale off of any class-specific stat bonuses or universal stat bonuses.
            // There are a number of items and projectiles that use this, such as thrown waters and the Bone Glove's bones.
            // Generic, on the other hand, scales off of all universal stat bonuses and nothing else; it's the base damage class upon which all others that aren't Default are built.

            // 此方法使您可以默认从其他类别的统计加成以及通用统计加成中受益。
            // 简要概括DamageClass使用的两个非标准伤害类型名称：
            // 默认值是，你猜对了，默认损坏等级。 它不会按任何特定于班级或通用统计奖金缩放。
            // 有许多物品和抛射物使用此选项，例如投掷水和骨手套的骨头。
            // 另一方面，Generic基于所有通用状态奖励进行缩放； 它是构建除Default之外所有其他内容所依赖的基本损坏等级。
            if (damageClass == DamageClass.Generic)
				return StatInheritanceData.Full;

			return new StatInheritanceData(
				damageInheritance: 0f,
				critChanceInheritance: 0f,
				attackSpeedInheritance: 0f,
				armorPenInheritance: 0f,
				knockbackInheritance: 0f
			);
            // Now, what exactly did we just do, you might ask? Well, let's see here...
            // StatInheritanceData is a struct which you'll need to return one of for any given outcome this method.
            // Normally, the latter of these two would be written as "StatInheritanceData.None", rather than being typed out by hand...
            // ...but for the sake of clarity, we've written it out and labeled each parameter in order; they should be self-explanatory.
            // To explain how these return values work, each one behaves like a percentage, with 0f being 0%, 1f being 100%, and so on.
            // The return value indicates how much your class will scale off of the stat in question for whatever damage class(es) you've returned it for.
            // If you create a StatInheritanceData without any parameters, all of them will be set to 1f.
            // For Analysis, if we propose a hypothetical alternate return for DamageClass.Ranged...

            // 那么我们刚才做了什么呢？ 好吧，让我们看看这里...
            // StatInheritanceData是一个结构体，在任何给定结果上都需要返回其中之一。
            // 通常情况下，后者将被写为“StatInheritanceData.None”，而不是手动输入...
            // ...但出于清晰起见，我们已经将其写出并按顺序标记了每个参数； 它们应该是不言自明的。
            // 为解释这些返回值如何工作，每个返回值都像一个百分比一样运行，其中0f表示0％，1f表示100％等等。
            // 返回值指示您的类别将根据所选伤害类型缩放有关统计信息的程度。
            // 如果创建StatInheritanceData而没有任何参数，则所有参数都将设置为1f。
            // 对于Analysis来说，如果我们提出DamageClass.Ranged的假设性替代回报...

            /*
			if (damageClass == DamageClass.Ranged)
				return new StatInheritanceData(
					damageInheritance: 1f,
					critChanceInheritance: -1f,
					attackSpeedInheritance: 0.4f,
					armorPenInheritance: 2.5f,
					knockbackInheritance: 0f
				);
			*/
            // This would allow our custom class to benefit from the following ranged stat bonuses:
            // - Damage, at 100% effectiveness
            // - Attack speed, at 40% effectiveness
            // - Crit chance, at -100% effectiveness (this means anything that raises ranged crit chance specifically will lower the crit chance of our custom class by the same amount)
            // - Armor penetration, at 250% effectiveness

            // 这将允许我们自定义类受益于以下远程状态奖励：
            // - 100％有效率下的伤害
            // - 40％有效速度
            //-100％效果（这意味着任何特定提高远程暴击几率的东西都会以相同数量降低自定义班级的暴击几率）
            // -250%穿透护甲

            // CAUTION: There is no hardcap on what you can set these to. Please be aware and advised that whatever you set them to may have unintended consequences,
            // and that we are NOT responsible for any temporary or permanent damage caused to you, your character, or your world as a result of your morbid curiosity.
            // To refer to a non-vanilla damage class for these sorts of things, use "ModContent.GetInstance<TargetDamageClassHere>()" instead of "DamageClass.XYZ".

            // 注意：您可以设置这些内容上限。请注意，并且无论您设置什么可能会产生意外后果，
            // 我们对由于您病态好奇心造成的任何临时或永久损坏以及对您、角色或世界造成损坏概不负责。
            // 对于这些事情的非香草伤害类，请使用“ModContent.GetInstance <TargetDamageClassHere>（）”而不是“DamageClass.XYZ”。
        }

        public override bool GetEffectInheritance(DamageClass damageClass) {
            // This method allows you to make your damage class benefit from and be able to activate other classes' effects (e.g. Spectre bolts, Magma Stone) based on what returns true.
            // Note that unlike our stat inheritance methods up above, you do not need to account for universal bonuses in this method.
            // For this Analysis, we'll make our class able to activate melee- and magic-specifically effects.

            // 此方法允许您使您的损坏等级受益并能够激活其他班级的效果（例如Spectre bolts，Magma Stone），具体取决于返回true的内容。
            // 请注意，与我们上面的统计继承方法不同，您无需在此方法中考虑通用奖金。
            // 对于此分析，我们将使我们的班级能够激活特定于近战和魔法的效果。
            if (damageClass == DamageClass.Melee)
				return true;
			if (damageClass == DamageClass.Magic)
				return true;

			return false;
		}

		public override void SetDefaultStats(Player player) {
            // This method lets you set default statistical modifiers for your Analysis damage class.
            // Here, we'll make our Analysis damage class have more critical strike chance and armor penetration than normal.

            // 这个方法允许你为分析伤害类设置默认的统计修饰符。
            // 在这里，我们将使我们的分析伤害类比普通情况下具有更高的暴击率和穿甲。
            player.GetCritChance<AnalysisDamageClass>() += 4;
			player.GetArmorPenetration<AnalysisDamageClass>() += 10;
            // These sorts of modifiers also exist for damage (GetDamage), knockback (GetKnockback), and attack speed (GetAttackSpeed).
            // You'll see these used all around in referencce to vanilla classes and our Analysis class here. Familiarize yourself with them.

            // 这些类型的修饰符也适用于伤害（GetDamage）、击退（GetKnockback）和攻击速度（GetAttackSpeed）。
            // 你会在各种参考文献中看到这些，包括原版职业和我们这里的分析职业。请熟悉他们。
        }

        // This property lets you decide whether or not your damage class can use standard critical strike calculations.
        // Note that setting it to false will also prevent the critical strike chance tooltip line from being shown.
        // This prevention will overrule anything set by ShowStatTooltipLine, so be careful!

        // 此属性允许您决定您的伤害类是否可以使用标准暴击计算。
        // 请注意，将其设置为false还将防止显示关于暴击率提示行。
        // 这种预防措施将覆盖ShowStatTooltipLine所设定的任何内容，请小心！
        public override bool UseStandardCritCalcs => true;

		public override bool ShowStatTooltipLine(Player player, string lineName) {
            // This method lets you prevent certain common statistical tooltip lines from appearing on items associated with this DamageClass.
            // The four line names you can use are "Damage", "CritChance", "Speed", and "Knockback". All four cases default to true, and thus will be shown. For Analysis...

            // 此方法可让您防止与此DamageClass相关联物品上出现某些常见统计工具提示行。
            // 您可以使用四个线路名称："Damage"、"CritChance"、"Speed" 和 "Knockback"。所有四种情况都默认为true，并且因此会被显示。对于Analysis...
            if (lineName == "Speed")
				return false;

			return true;
            // PLEASE BE AWARE that this hook will NOT be here forever; only until an upcoming revamp to tooltips as a whole comes around.
            // Once this happens, a better, more versatile explanation of how to pull this off will be showcased, and this hook will be removed.

            // 请注意，该钩子不会永远存在；只有在整体工具提示即将进行更新时才存在。
            // 一旦发生这种情况，将展示更好、更多功能的解释如何完成此操作，并且该钩子将被删除。
        }
    }
}