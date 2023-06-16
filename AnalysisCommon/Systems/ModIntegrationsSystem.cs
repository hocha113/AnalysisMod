using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Systems
{
    // Showcases using Mod.Call of other mods to facilitate mod integration/compatibility/support
    // 展示使用Mod.Call调用其他模组的方法来实现模组整合/兼容/支持

    // Mod.Call is explained here：
    //// Mod.Call的解释在这里：
    // https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate

    // This only showcases one way to implement such integrations, you are free to explore your own options and other mods Analysiss
    // 这只是展示了一种实现此类整合的方式，您可以自由探索自己的选项和其他模组

    // You need to look for resources the mod developers provide regarding how they want you to add mod compatibility
    // This can be their homepage, workshop page, wiki, github, discord, other contacts etc.
    // If the mod is open source, you can visit its code distribution platform (usually GitHub) and look for "Call" in its Mod class

    // 您需要查找模组开发人员提供有关如何添加模组兼容性的资源
    // 这可以是他们的主页、创意工坊页面、维基、GitHub、Discord或其他联系方式等。
    // 如果该模组是开源的，则可以访问其代码分发平台（通常为GitHub）并在其Mod类中查找“Call”
    public class ModIntegrationsSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly
            // 大多数情况下，mod要求您使用PostSetupContent钩子来调用它们的方法。这保证了各种数据被正确初始化和设置

            // Census Mod allows us to add spawn information to the town NPCs UI:
            // Census Mod允许我们向城镇NPC UI添加生成信息：
            // https://forums.terraria.org/index.php?threads/.74786/
            DoCensusIntegration();

            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // Boss Checklist在其独立UI中显示有关Bosses详尽信息。我们可以进行自定义：
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();

            // We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
            // 我们可以通过遵循相同的设计与其他mod集成。某些modder可能更喜欢为每个他们集成到其中一个mod创建一个ModSystem，或者采用其他设计。
        }

        private void DoCensusIntegration()
        {
            // We figured out how to add support by looking at it's Call method:
            // 通过查看它的Call方法，我们弄清楚了如何添加支持：
            // https://github.com/JavidPack/Census/blob/1.4/Census.cs

            // Census also has a wiki, where the Call methods are better explained:
            // Census还有一个维基，其中更好地解释了Call方法：
            // https://github.com/JavidPack/Census/wiki/Support-using-Mod-Call

            if (!ModLoader.TryGetMod("Census", out Mod censusMod))
            {
                // TryGetMod returns false if the mod is not currently loaded, so if this is the case, we just return early
                // 如果mod当前未加载，则TryGetMod返回false，因此在这种情况下我们只需提前返回
                return;
            }

            // The "TownNPCCondition" method allows us to write out the spawn condition (which is coded via CanTownNPCSpawn), it requires an NPC type and a message
            // “TownNPCCondition”方法允许我们编写生成条件（通过CanTownNPCSpawn进行编码），它需要一个NPC类型和一条消息
            int npcType = ModContent.NPCType<AnalysisContent.NPCs.AnalysisPerson>();

            // The message makes use of chat tags to make the item appear directly, making it more fancy
            // 该消息利用聊天标签使物品直接出现，使其更加华丽
            string message = $"Have either an Analysis Item [i:{ModContent.ItemType<AnalysisContent.Items.AnalysisItem>()}] or an Analysis Block [i:{ModContent.ItemType<AnalysisContent.Items.Placeable.AnalysisBlock>()}] in your inventory";

            // Finally, call the desired method
            // 最后调用所需的方法
            censusMod.Call("TownNPCCondition", npcType, message);

            // Additional calls can be made here for other Town NPCs in our mod
            // 可以在此处进行其他城镇NPC的附加调用。
        }

        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained:
            // mod主页链接到自己的维基，在那里可以找到解释调用的内容：
            // https://github.com/JavidPack/BossChecklist/wiki/Support-using-Mod-Call

            // If we navigate the wiki, we can find the "AddBoss" method, which we want in this case
            // 如果我们浏览维基，就可以找到“AddBoss”方法，在这种情况下我们想要使用它。

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.3.1
            // Usually mods either provide that information themselves in some way, or it's found on the github through commit history/blame

            // 对于某些消息，模组可能在发布时没有它们，因此我们需要验证最后一次迭代该方法变体是何时添加到模组中的。在本例中为1.3.1
            // 通常模组会以某种方式提供这些信息或者可以通过提交历史记录/责任查看GitHub上找到。
            if (bossChecklistMod.Version < new Version(1, 3, 1))
            {
                return;
            }

            // The "AddBoss" method requires many parameters, defined separately below:
            // “AddBoss”方法需要许多参数，在下面单独定义：

            // The name used for the title of the page
            // 页面标题使用的名称
            string bossName = "Minion Boss";

            // The NPC type of the boss
            // Boss 的 NPC 类型
            int bossType = ModContent.NPCType<AnalysisContent.NPCs.MinionBoss.MinionBossBody>();

            // Value inferred from boss progression, see the wiki for details
            // 从 Boss 进度推断出的值，详见 Wiki
            float weight = 0.7f;

            // Used for tracking checklist progress
            // 用于跟踪清单进度
            Func<bool> downed = () => DownedBossSystem.downedMinionBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            // 如果 Boss 应该在清单中显示以及何时（这里是始终）
            Func<bool> available = () => true;

            // "collectibles" like relic, trophy, mask, pet
            // "收藏品" 如遗物、奖杯、面具、宠物等
            List<int> collection = new List<int>()
            {
                ModContent.ItemType<AnalysisContent.Items.Placeable.Furniture.MinionBossRelic>(),
                ModContent.ItemType<AnalysisContent.Pets.MinionBossPet.MinionBossPetItem>(),
                ModContent.ItemType<AnalysisContent.Items.Placeable.Furniture.MinionBossTrophy>(),
                ModContent.ItemType<AnalysisContent.Items.Armor.Vanity.MinionBossMask>()
            };

            // The item used to summon the boss with (if available)
            // 召唤 Boss 所需使用的物品（如果有）
            int summonItem = ModContent.ItemType<AnalysisContent.Items.Consumables.MinionBossSummonItem>();

            // Information for the player so he knows how to encounter the boss
            // 玩家需要知道如何遇到 Boss 的信息
            string spawnInfo = $"Use a [i:{summonItem}]";

            // The boss does not have a custom despawn message, so we omit it
            // Boss 没有自定义消失消息，因此我们省略了它。
            string despawnInfo = null;

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location

            // 默认情况下，它会绘制Boss的第一帧。如果您不需要自定义绘图，则可以省略。
            // 但是我们想要绘制怪兽图鉴纹理，因此我们创建了代码来在预期位置居中绘制。
            var customBossPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("AnalysisMod/Assets/Textures/Bestiary/MinionBoss_Preview").Value;
                Vector2 centered = new Vector2(rect.X + rect.Width / 2 - texture.Width / 2, rect.Y + rect.Height / 2 - texture.Height / 2);
                sb.Draw(texture, centered, color);
            };

            bossChecklistMod.Call(
                "AddBoss",
                Mod,
                bossName,
                bossType,
                weight,
                downed,
                available,
                collection,
                summonItem,
                spawnInfo,
                despawnInfo,
                customBossPortrait
            );

            // Other bosses or additional Mod.Call can be made here.
            // 其他Boss或其他Mod.Call可以在此处进行。
        }
    }
}
