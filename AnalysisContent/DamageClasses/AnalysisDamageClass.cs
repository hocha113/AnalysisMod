﻿using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.DamageClasses
{//【红茶注：这个项目的注释量很大，翻译压力也较大，欢迎纠错】
	public class AnalysisDamageClass : DamageClass
	{
        // 这是一个分析伤害类，旨在展示该功能的所有当前功能，并解释如何创建自己的伤害类（如果需要）。
        // 有关如何将统计加成应用于特定的伤害类，请参阅AnalysisMod / AnalysisContent / Items / Accessories / AnalysisStatBonusAccessory。
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
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
            // 这将允许我们自定义类受益于以下远程状态奖励：
            // - 100％有效率下的伤害
            // - 40％有效速度
            // - 100％效果（这意味着任何特定提高远程暴击几率的东西都会以相同数量降低自定义班级的暴击几率）
            // - 250%穿透护甲

            // 注意：您可以设置这些内容上限。请注意，并且无论您设置什么可能会产生意外后果，
            // 我们对由于您病态好奇心造成的任何临时或永久损坏以及对您、角色或世界造成损坏概不负责。
            // 对于这些事情的非香草伤害类，请使用“ModContent.GetInstance <TargetDamageClassHere>（）”而不是“DamageClass.XYZ”。
        }

        public override bool GetEffectInheritance(DamageClass damageClass) {
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
            // 这个方法允许你为分析伤害类设置默认的统计修饰符。
            // 在这里，我们将使我们的分析伤害类比普通情况下具有更高的暴击率和穿甲。
            player.GetCritChance<AnalysisDamageClass>() += 4;
			player.GetArmorPenetration<AnalysisDamageClass>() += 10;
            // 这些类型的修饰符也适用于伤害（GetDamage）、击退（GetKnockback）和攻击速度（GetAttackSpeed）。
            // 你会在各种参考文献中看到这些，包括原版职业和我们这里的分析职业。请熟悉他们。
        }
        // 此属性允许您决定您的伤害类是否可以使用标准暴击计算。
        // 请注意，将其设置为false还将防止显示关于暴击率提示行。
        // 这种预防措施将覆盖ShowStatTooltipLine所设定的任何内容，请小心！
        public override bool UseStandardCritCalcs => true;

		public override bool ShowStatTooltipLine(Player player, string lineName) {
            // 此方法可让您防止与此DamageClass相关联物品上出现某些常见统计工具提示行。
            // 您可以使用四个线路名称："Damage"、"CritChance"、"Speed" 和 "Knockback"。所有四种情况都默认为true，并且因此会被显示。对于Analysis...
            if (lineName == "Speed")
				return false;

			return true;
            // 请注意，该钩子不会永远存在；只有在整体工具提示即将进行更新时才存在。
            // 一旦发生这种情况，将展示更好、更多功能的解释如何完成此操作，并且该钩子将被删除。
        }
    }
}