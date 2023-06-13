using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using AnalysisMod.AnalysisCommon.ItemDropRules.DropConditions;
using System.Linq;
using AnalysisMod.AnalysisContent.Items;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    // This file shows numerous Analysiss of what you can do with the extensive NPC Loot lootable system.
    // You can find more info on the wiki: https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
    // Despite this file being GlobalNPC, everything here can be used with a ModNPC as well! See Analysiss of this in the Content/NPCs folder.

    // 这个文件展示了使用广泛的NPC战利品系统可以做什么样的分析。
    // 您可以在维基上找到更多信息：https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
    // 尽管这个文件是GlobalNPC，但这里的所有内容也适用于ModNPC！请参见Content/NPCs文件夹中的分析。
    public class AnalysisNPCLoot : GlobalNPC
    {
        // ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
        // Here we go through all of them, and how they can be used.
        // There are tons of other Analysiss in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.

        // ModifyNPCLoot使用一个称为ItemDropDatabase的独特系统，该系统具有许多不同规则以满足许多不同掉落情况。
        // 在这里我们将介绍它们全部以及如何使用它们。
        // 有大量其他分析可供选择！在反编译vanilla构建中，GameContent/ItemDropRules/ItemDropDatabase会向每个vanilla NPC添加物品掉落，这可能是一个很好的资源。
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!NPCID.Sets.CountsAsCritter[npc.type])
            { // If npc is not a critter
              // Make it drop AnalysisItem.

                // 如果npc不是动物，则使其掉落AnalysisItem。
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnalysisItem>(), 1));

                // Drop an AnalysisResearchPresent in journey mode with 2/7ths base chance, but only in journey mode
                // 在旅行模式下以2/7的基础几率掉落AnalysisResearchPresent，但仅在旅行模式下.
                npcLoot.Add(ItemDropRule.ByCondition(new AnalysisJourneyModeDropCondition(), ModContent.ItemType<AnalysisResearchPresent>(), chanceDenominator: 7, chanceNumerator: 2));
            }

            // We will now use the Guide to explain many of the other types of drop rules.
            // 我们现在将使用指南来解释许多其他类型的掉落规则。
            if (npc.type == NPCID.Guide)
            {
                // RemoveWhere will remove any drop rule that matches the provided expression.
                // To make your own expressions to remove vanilla drop rules, you'll usually have to study the original source code that adds those rules.

                // RemoveWhere将删除与提供的表达式匹配的任何掉落规则。
                // 要创建自己的表达式以删除香草掉落规则，通常必须研究添加这些规则的原始源代码。
                npcLoot.RemoveWhere(
                    // The following expression returns true if the following conditions are met:
                    // 以下表达式返回true如果满足以下条件：
                    rule => rule is ItemDropWithConditionRule drop // If the rule is an ItemDropWithConditionRule instance
                                                                   // 如果该规则是ItemDropWithConditionRule实例
                        && drop.itemId == ItemID.GreenCap // And that instance drops a green cap
                                                          // 并且该实例会掉落一个绿色帽子
                        && drop.condition is Conditions.NamedNPC npcNameCondition // ..And if its condition is that an npc name must match some string
                                                                                  // ..并且它的条件是npc名称必须匹配某个字符串
                        && npcNameCondition.neededName == "Andrew" // And the condition's string is "Andrew".
                                                                   // 条件字符串为“Andrew”。
                );

                npcLoot.Add(ItemDropRule.Common(ItemID.GreenCap, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
                                                                      // 结合上述移除操作，这使得任何名称为Guide的角色都可以获得Green Cap。
            }

            // Editing an existing drop rule
            // 编辑现有掉落规则
            if (npc.type == NPCID.BloodNautilus)
            {
                // Dreadnautilus, known as BloodNautilus in the code, drops SanguineStaff. The drop rate is 100% in Expert mode and 50% in Normal mode. This Analysis will change that rate.
                // The vanilla code responsible for this drop is: ItemDropRule.NormalvsExpert(4269, 2, 1)
                // The NormalvsExpert method creates a DropBasedOnExpertMode rule, and that rule is made up of 2 CommonDrop rules. We'll need to use this information in our casting to properly identify the recipe to edit.

                // Dreadnautilus，也称为BloodNautilus，掉落SanguineStaff。在专家模式下掉率为100％，普通模式下为50％。此分析将更改该比率。
                // 负责此掉落的基础代码是：ItemDropRule.NormalvsExpert（4269, 2, 1）
                // NormalvsExpert方法创建了一个基于专家模式的规则，并且该规则由2个CommonDrop规则组成。我们需要使用这些信息来正确识别要编辑的配方。

                // There are 2 options. One option is remove the original rule and then add back a similar rule. The other option is to modify the existing rule.
                // It is preferred to modify the existing rule to preserve compatibility with other mods.

                // 有两种选择。一种选择是删除原始规则，然后添加类似的规则。另一种选择是修改现有规则。
                // 修改现有规则以保持与其他mod兼容性更好。

                // Adjust the existing rule: Change the Normal mode drop rate from 50% to 33.3%
                // 调整现有规则：将普通模式下的掉落率从50％更改为33.3％
                foreach (var rule in npcLoot.Get())
                {
                    // You must study the vanilla code to know what to objects to cast to.
                    // 您必须研究香草代码以知道要转换成什么对象。
                    if (rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff)
                        normalDropRule.chanceDenominator = 3;
                }

                // Remove the rule, then add another rule: Change the Normal mode drop rate from 50% to 16.6%
                // 删除该规则，然后添加另一个新的：将普通模式下的掉落率从50％更改为16.6％
                /*
				npcLoot.RemoveWhere(
					rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff
				);
				npcLoot.Add(ItemDropRule.NormalvsExpert(4269, 6, 1));
				*/
            }
            // Editing an existing drop rule, but for a boss
            // In addition to this code, we also do similar code in Common/GlobalItems/BossBagLoot.cs to edit the boss bag loot. Remember to do both if your edits should affect boss bags as well.

            // 编辑现有BOSS战斗中已存在的物品掉落
            // 除了这段代码之外，在Common / GlobalItems / BossBagLoot.cs中还会进行类似操作以编辑BOSS袋子里面物品掉落情况，请记得同时处理。
            if (npc.type == NPCID.QueenBee)
            {
                foreach (var rule in npcLoot.Get())
                {
                    if (rule is DropBasedOnExpertMode dropBasedOnExpertMode && dropBasedOnExpertMode.ruleForNormalMode is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<AnalysisContent.Items.Accessories.WaspNest>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }
                }
            }

            if (npc.type == NPCID.Crimera || npc.type == NPCID.Corruptor)
            {
                // Here we make use of our own special rule we created: drop during daytime
                // Drop an item from the other evil with 33% chance

                // 在这里，我们使用了自己创建的特殊规则：白天掉落
                // 从另一个邪恶中以33％的几率掉落物品
                int itemType = npc.type == NPCID.Crimera ? ItemID.RottenChunk : ItemID.Vertebrae;
                npcLoot.Add(ItemDropRule.ByCondition(new AnalysisDropCondition(), itemType, chanceDenominator: 3));
            }

            // A simple Analysis of using a 'standard' condition
            // 使用“标准”条件的简单分析
            if (npc.aiStyle == NPCAIStyleID.Slime)
            {
                npcLoot.Add(ItemDropRule.ByCondition(Condition.TimeDay.ToDropCondition(ShowItemDropInUI.Always), ModContent.ItemType<AnalysisContent.Items.Weapons.AnalysisSword>()));
            }

            //TODO: Add the rest of the vanilla drop rules!!
            //TODO：添加其余香草掉落规则！！
        }

        // ModifyGlobalLoot allows you to modify loot that every NPC should be able to drop, preferably with a condition.
        // Vanilla uses this for the biome keys, souls of night/light, as well as the holiday drops.
        // Any drop rules in ModifyGlobalLoot should only run once. Everything else should go in ModifyNPCLoot.

        // ModifyGlobalLoot允许您修改每个NPC都应该能够掉落的战利品，最好带有条件。
        // Vanilla将此用于生物群系钥匙、黑暗之魂/光明之魂以及节日掉落。
        // 在ModifyGlobalLoot中的任何放置规则都只运行一次。其他所有内容都应放在ModifyNPCLoot中。
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            // If the AnalysisSoulCondition is true, drop AnalysisSoul 20% of the time. See Common/ItemDropRules/DropConditions/AnalysisSoulCondition.cs for how it's determined
            // 如果AnalysisSoulCondition为true，则20％时间会丢弃AnalysisSoul。请参见Common / ItemDropRules / DropConditions / AnalysisSoulCondition.cs以了解如何确定
            globalLoot.Add(ItemDropRule.ByCondition(new AnalysisSoulCondition(), ModContent.ItemType<AnalysisSoul>(), 5, 1, 1));
        }
    }
}
