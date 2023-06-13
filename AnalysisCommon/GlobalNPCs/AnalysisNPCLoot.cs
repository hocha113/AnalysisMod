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

    // ����ļ�չʾ��ʹ�ù㷺��NPCս��Ʒϵͳ������ʲô���ķ�����
    // ��������ά�����ҵ�������Ϣ��https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
    // ��������ļ���GlobalNPC�����������������Ҳ������ModNPC����μ�Content/NPCs�ļ����еķ�����
    public class AnalysisNPCLoot : GlobalNPC
    {
        // ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
        // Here we go through all of them, and how they can be used.
        // There are tons of other Analysiss in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.

        // ModifyNPCLootʹ��һ����ΪItemDropDatabase�Ķ���ϵͳ����ϵͳ������಻ͬ������������಻ͬ���������
        // ���������ǽ���������ȫ���Լ����ʹ�����ǡ�
        // �д������������ɹ�ѡ���ڷ�����vanilla�����У�GameContent/ItemDropRules/ItemDropDatabase����ÿ��vanilla NPC�����Ʒ���䣬�������һ���ܺõ���Դ��
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!NPCID.Sets.CountsAsCritter[npc.type])
            { // If npc is not a critter
              // Make it drop AnalysisItem.

                // ���npc���Ƕ����ʹ�����AnalysisItem��
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnalysisItem>(), 1));

                // Drop an AnalysisResearchPresent in journey mode with 2/7ths base chance, but only in journey mode
                // ������ģʽ����2/7�Ļ������ʵ���AnalysisResearchPresent������������ģʽ��.
                npcLoot.Add(ItemDropRule.ByCondition(new AnalysisJourneyModeDropCondition(), ModContent.ItemType<AnalysisResearchPresent>(), chanceDenominator: 7, chanceNumerator: 2));
            }

            // We will now use the Guide to explain many of the other types of drop rules.
            // �������ڽ�ʹ��ָ������������������͵ĵ������
            if (npc.type == NPCID.Guide)
            {
                // RemoveWhere will remove any drop rule that matches the provided expression.
                // To make your own expressions to remove vanilla drop rules, you'll usually have to study the original source code that adds those rules.

                // RemoveWhere��ɾ�����ṩ�ı��ʽƥ����κε������
                // Ҫ�����Լ��ı��ʽ��ɾ����ݵ������ͨ�������о������Щ�����ԭʼԴ���롣
                npcLoot.RemoveWhere(
                    // The following expression returns true if the following conditions are met:
                    // ���±��ʽ����true�����������������
                    rule => rule is ItemDropWithConditionRule drop // If the rule is an ItemDropWithConditionRule instance
                                                                   // ����ù�����ItemDropWithConditionRuleʵ��
                        && drop.itemId == ItemID.GreenCap // And that instance drops a green cap
                                                          // ���Ҹ�ʵ�������һ����ɫñ��
                        && drop.condition is Conditions.NamedNPC npcNameCondition // ..And if its condition is that an npc name must match some string
                                                                                  // ..��������������npc���Ʊ���ƥ��ĳ���ַ���
                        && npcNameCondition.neededName == "Andrew" // And the condition's string is "Andrew".
                                                                   // �����ַ���Ϊ��Andrew����
                );

                npcLoot.Add(ItemDropRule.Common(ItemID.GreenCap, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
                                                                      // ��������Ƴ���������ʹ���κ�����ΪGuide�Ľ�ɫ�����Ի��Green Cap��
            }

            // Editing an existing drop rule
            // �༭���е������
            if (npc.type == NPCID.BloodNautilus)
            {
                // Dreadnautilus, known as BloodNautilus in the code, drops SanguineStaff. The drop rate is 100% in Expert mode and 50% in Normal mode. This Analysis will change that rate.
                // The vanilla code responsible for this drop is: ItemDropRule.NormalvsExpert(4269, 2, 1)
                // The NormalvsExpert method creates a DropBasedOnExpertMode rule, and that rule is made up of 2 CommonDrop rules. We'll need to use this information in our casting to properly identify the recipe to edit.

                // Dreadnautilus��Ҳ��ΪBloodNautilus������SanguineStaff����ר��ģʽ�µ���Ϊ100������ͨģʽ��Ϊ50�����˷��������ĸñ��ʡ�
                // ����˵���Ļ��������ǣ�ItemDropRule.NormalvsExpert��4269, 2, 1��
                // NormalvsExpert����������һ������ר��ģʽ�Ĺ��򣬲��Ҹù�����2��CommonDrop������ɡ�������Ҫʹ����Щ��Ϣ����ȷʶ��Ҫ�༭���䷽��

                // There are 2 options. One option is remove the original rule and then add back a similar rule. The other option is to modify the existing rule.
                // It is preferred to modify the existing rule to preserve compatibility with other mods.

                // ������ѡ��һ��ѡ����ɾ��ԭʼ����Ȼ��������ƵĹ�����һ��ѡ�����޸����й���
                // �޸����й����Ա���������mod�����Ը��á�

                // Adjust the existing rule: Change the Normal mode drop rate from 50% to 33.3%
                // �������й��򣺽���ͨģʽ�µĵ����ʴ�50������Ϊ33.3��
                foreach (var rule in npcLoot.Get())
                {
                    // You must study the vanilla code to know what to objects to cast to.
                    // �������о���ݴ�����֪��Ҫת����ʲô����
                    if (rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff)
                        normalDropRule.chanceDenominator = 3;
                }

                // Remove the rule, then add another rule: Change the Normal mode drop rate from 50% to 16.6%
                // ɾ���ù���Ȼ�������һ���µģ�����ͨģʽ�µĵ����ʴ�50������Ϊ16.6��
                /*
				npcLoot.RemoveWhere(
					rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff
				);
				npcLoot.Add(ItemDropRule.NormalvsExpert(4269, 6, 1));
				*/
            }
            // Editing an existing drop rule, but for a boss
            // In addition to this code, we also do similar code in Common/GlobalItems/BossBagLoot.cs to edit the boss bag loot. Remember to do both if your edits should affect boss bags as well.

            // �༭����BOSSս�����Ѵ��ڵ���Ʒ����
            // ������δ���֮�⣬��Common / GlobalItems / BossBagLoot.cs�л���������Ʋ����Ա༭BOSS����������Ʒ�����������ǵ�ͬʱ����
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

                // ���������ʹ�����Լ�������������򣺰������
                // ����һ��а������33���ļ��ʵ�����Ʒ
                int itemType = npc.type == NPCID.Crimera ? ItemID.RottenChunk : ItemID.Vertebrae;
                npcLoot.Add(ItemDropRule.ByCondition(new AnalysisDropCondition(), itemType, chanceDenominator: 3));
            }

            // A simple Analysis of using a 'standard' condition
            // ʹ�á���׼�������ļ򵥷���
            if (npc.aiStyle == NPCAIStyleID.Slime)
            {
                npcLoot.Add(ItemDropRule.ByCondition(Condition.TimeDay.ToDropCondition(ShowItemDropInUI.Always), ModContent.ItemType<AnalysisContent.Items.Weapons.AnalysisSword>()));
            }

            //TODO: Add the rest of the vanilla drop rules!!
            //TODO�����������ݵ�����򣡣�
        }

        // ModifyGlobalLoot allows you to modify loot that every NPC should be able to drop, preferably with a condition.
        // Vanilla uses this for the biome keys, souls of night/light, as well as the holiday drops.
        // Any drop rules in ModifyGlobalLoot should only run once. Everything else should go in ModifyNPCLoot.

        // ModifyGlobalLoot�������޸�ÿ��NPC��Ӧ���ܹ������ս��Ʒ����ô���������
        // Vanilla������������ȺϵԿ�ס��ڰ�֮��/����֮���Լ����յ��䡣
        // ��ModifyGlobalLoot�е��κη��ù���ֻ����һ�Ρ������������ݶ�Ӧ����ModifyNPCLoot�С�
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            // If the AnalysisSoulCondition is true, drop AnalysisSoul 20% of the time. See Common/ItemDropRules/DropConditions/AnalysisSoulCondition.cs for how it's determined
            // ���AnalysisSoulConditionΪtrue����20��ʱ��ᶪ��AnalysisSoul����μ�Common / ItemDropRules / DropConditions / AnalysisSoulCondition.cs���˽����ȷ��
            globalLoot.Add(ItemDropRule.ByCondition(new AnalysisSoulCondition(), ModContent.ItemType<AnalysisSoul>(), 5, 1, 1));
        }
    }
}
