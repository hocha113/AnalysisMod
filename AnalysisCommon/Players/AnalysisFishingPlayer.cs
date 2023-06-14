using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Items.Tools;
using AnalysisMod.AnalysisContent.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    // This class showcases things you can do with fishing
    // 这个类展示了你可以用钓鱼做的事情
    public class AnalysisFishingPlayer : ModPlayer
    {
        public bool hasAnalysisCrateBuff;

        public override void ResetEffects()
        {
            hasAnalysisCrateBuff = false;
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            // If the player has the Analysis Crate buff (given by Analysis Crate Potion), 10% additional chance that the catch will be a crate
            // The "tier" of the crate depends on the rarity, which we don't modify here, see the comments in CatchFish for details

            // 如果玩家拥有分析箱 buff（通过分析箱药水获得），则额外增加10%的几率捕获到一个箱子
            // 箱子的“等级”取决于稀有度，我们在这里不修改它，请参见CatchFish中的注释以获取详细信息
            if (hasAnalysisCrateBuff && !attempt.crate)
            {
                if (Main.rand.Next(100) < 10)
                {
                    attempt.crate = true;
                }
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;
            bool inAnalysisSurfaceBiome = Player.InModBiome<AnalysisSurfaceBiome>();
            if (attempt.playerFishingConditions.PoleItemType == ModContent.ItemType<AnalysisFishingRod>() && inWater && inAnalysisSurfaceBiome)
            {
                // In this Analysis, we will fish up an Analysis Person from the water in Analysis Surface Biome,
                // as long as there isn't one in the world yet
                // NOTE: if a fishing rod has multiple bobbers, then each one can spawn the NPC

                // 在这个分析中，我们将从Analysis Surface Biome 的水中钓起一位 Analysis Person，
                // 只要世界上还没有他
                // 注意：如果一个鱼竿有多个浮标，则每个都可以生成NPC
                int npc = ModContent.NPCType<AnalysisPerson>();
                if (!NPC.AnyNPCs(npc))
                {
                    // Make sure itemDrop = -1 when summoning an NPC, as otherwise terraria will only spawn the item
                    // 召唤NPC时确保itemDrop = -1，否则terraria只会生成物品而不是NPC。
                    npcSpawn = npc;
                    itemDrop = -1;

                    // Also, to make it cooler, we will make a special sonar message for when it shows up
                    // 为了让它更酷炫，我们将为其制作特殊声纳消息，
                    sonar.Text = "Something's wrong...";
                    sonar.Color = Color.LimeGreen;
                    sonar.Velocity = Vector2.Zero;
                    sonar.DurationInFrames = 300;

                    // And that text shows up on the player's head, not on the bobber location.
                    // 并且该文本显示在玩家头顶上而非浮标位置。
                    sonarPosition = new Vector2(Player.position.X, Player.position.Y - 64);

                    return; // This is important so your code after this that rolls items will not run
                            // 这很重要，因此您之后滚动物品的代码将不会运行。
                }
            }

            if (inWater && inAnalysisSurfaceBiome && attempt.crate)
            {
                // If the game rolls a crate, we want to give ours to the player if he is in Analysis Surface Biome
                // 如果游戏掉落了一个箱子，则如果玩家处于Analysis Surface Biome中，则想把自己的给予玩家

                // We don't want to replace golden/titanium crates (the highest tier crates), as they take highest priority in crate catches
                // Their drop conditions are "veryrare" or "legendary"
                // (After that come biome crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none of the previous))
                // Let's replace biome crates 50% of the time (player could be in multiple (modded) biomes, we should respect that)

                // 我们不想替换黄金/钛合金箱（最高级别的箱子），因为它们在捕获时具有最高优先级。
                // 它们掉落条件是“veryrare”或“legendary”
                // （之后是生物群系箱子（“rare”），然后是铁/秘银（“uncommon”），最后是木头/珍珠（前面都没有））
                // 让我们50%的时间替换生物群系箱子（玩家可能在多个模组化的生物群系中，我们应该尊重这一点）
                if (!attempt.veryrare && !attempt.legendary && attempt.rare && Main.rand.NextBool())
                {
                    itemDrop = ModContent.ItemType<AnalysisContent.Items.AnalysisQuestFish>();
                    return; // This is important so your code after this that rolls items will not run
                            // 这很重要，因此您之后滚动物品的代码将不会运行。
                }
            }

            // Here we will set the catch conditions for our AnalysisQuestFish
            // 在这里，我们将设置AnalysisQuestFish的捕获条件
            int AnalysisQuestFish = ModContent.ItemType<AnalysisContent.Items.AnalysisQuestFish>(); // We'll store the type as a variable, since we'll be referencing it several times
                                                                                                    // 由于我们将多次引用它，因此我们将其类型存储为变量

            // First we check if today's quest matches our quest fish
            // 首先检查今天的任务是否与我们的任务鱼匹配

            if (attempt.questFish == AnalysisQuestFish)
            {
                // Our AnalysisQuestFish states that it can only be caught whilst upside-down, so we'll have to check the gravity
                // Normal gravity is positive, whilst reversed gravity is negative
                // Finally, most vanilla quest fish only appear on an uncommon roll, so we'll do the same

                // 我们的AnalysisQuestFish声明只能在倒挂时捕获，因此必须检查重力
                // 正常重力为正数，而反向重力为负数
                // 最后，请注意大多数基础任务鱼仅出现在不寻常掉落上，因此我们也会这样做。
                if (Player.gravDir < 0f && attempt.uncommon)
                {
                    itemDrop = AnalysisQuestFish;
                    return; // While there is no more code that could roll a fish after this, we might add some in the future so it's best to return here
                            // 虽然在此之后没有更多可以卷起鱼类的代码了，但未来可能会添加一些内容。所以最好返回到这里。
                }
            }
        }

        public override bool? CanConsumeBait(Item bait)
        {
            // Player.GetFishingConditions() returns you the best fishing pole Item, type and power, the best bait Item, type and Power, and the total fishing level, including modded values
            // These are the same Pole and Bait the game considers when calculating the obtained fish.
            // during CanConsumeBait, Player.GetFishingConditions() == attempt.playerFishingConditions from CatchFish.

            // Player.GetFishingConditions() 返回最佳的钓鱼竿物品、类型和能力，最佳的饵料物品、类型和能力，以及总钓鱼等级（包括修改后的值）
            // 这些是游戏在计算获得的鱼时考虑到的相同竿和饵。
            // 在 CanConsumeBait 中，Player.GetFishingConditions() == CatchFish 的 attempt.playerFishingConditions。
            PlayerFishingConditions conditions = Player.GetFishingConditions();

            // The golden fishing rod will never consume a ladybug
            // 黄金钓竿永远不会消耗瓢虫。
            if ((bait.type == ItemID.LadyBug || bait.type == ItemID.GoldLadyBug) && conditions.Pole.type == ItemID.GoldenFishingRod)
            {
                return false;
            }

            return null; // Let the default logic run
                         // 让默认逻辑运行
        }

        // If fishing with ladybug, we will receive multiple "fish" per bobber. Does not apply to quest fish
        // 如果使用瓢虫进行钓鱼，则每个浮标将收到多个“鱼”。不适用于任务中需要捕捉特定种类鱼类。
        public override void ModifyCaughtFish(Item fish)
        {
            // In this Analysis, we make sure that we got a Ladybug as bait, and later on use that to determine what we catch
            // 在此分析中，我们确保我们有一只瓢虫作为诱 bait，并在稍后使用它来确定我们捕获了什么
            if (Player.GetFishingConditions().BaitItemType == ItemID.LadyBug && fish.rare != ItemRarityID.Quest)
            {
                fish.stack += Main.rand.Next(1, 4);
            }
        }
    }
}
