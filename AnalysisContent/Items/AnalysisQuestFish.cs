using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisQuestFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true; // All vanilla fish can be placed in a weapon rack.
                                                               // 所有的香草鱼(原版鱼)都可以放在武器架上。
        }

        public override void SetDefaults()
        {
            // DefaultToQuestFish sets quest fish properties.
            // Of note, it sets rare to ItemRarityID.Quest, which is the special rarity for quest items.
            // It also sets uniqueStack to true, which prevents players from picking up a 2nd copy of the item into their inventory.

            // DefaultToQuestFish设置任务鱼属性。
            //值得注意的是，它将稀有度设置为ItemRarityID.Quest，这是任务物品的特殊稀有度。
            //它还将uniqueStack设置为true，防止玩家将第二份副本拾起放入他们的库存中。
            Item.DefaultToQuestFish();
        }

        public override bool IsQuestFish() => true; // Makes the item a quest fish
                                                    // 使物品成为任务鱼

        public override bool IsAnglerQuestAvailable() => Main.hardMode; // Makes the quest only appear in hard mode. Adding a '!' before Main.hardMode makes it ONLY available in pre-hardmode.
                                                                        // 使任务只出现在困难模式下。 在Main.hardMode前面添加'!'会使其仅在未到达困难模式时可用。

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            // How the angler describes the fish to the player.
            // 渔夫向玩家描述该鱼类。
            description = "I've heard stories of a fish that swims upside-down. Supposedly you have the stand upside-down yourself to even find one. One of those would go great on my ceiling. Go fetch!";
            // What it says on the bottom of the angler's text box of how to catch the fish.
            // 出现在渔夫文本框底部关于如何捕捉该鱼类的说明文字。
            catchLocation = "Caught anywhere while standing upside-down.";
        }
    }
}
