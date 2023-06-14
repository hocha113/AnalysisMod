using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisResourcePlayer : ModPlayer
    {
        // Here we create a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our Analysis resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.

        // 这里我们创建一个自定义资源，类似于法力或生命值。
        // 创建一些变量来定义当前分析资源的当前值以及当前最大值。我们还包括一个临时最大值，以及一些处理该资源自然再生的变量。
        public int AnalysisResourceCurrent; // Current value of our Analysis resource
                                            // 分析资源的当前价值

        public const int DefaultAnalysisResourceMax = 100; // Default maximum value of Analysis resource
                                                           // 默认分析资源的最大价值

        public int AnalysisResourceMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().
                                        // 用于在ResetDefaults()中重置最大资源为默认值的缓冲变量。

        public int AnalysisResourceMax2; // Maximum amount of our Analysis resource. We will change that variable to increase maximum amount of our resource
                                         // 我们分析资料来源的最大数量。我们将更改该变量以增加我们资源的最大数量

        public float AnalysisResourceRegenRate; // By changing that variable we can increase/decrease regeneration rate of our resource
                                                // 通过更改该变量，可以增加/减少我们资料来源再生速率

        internal int AnalysisResourceRegenTimer = 0; // A variable that is required for our timer
                                                     // 这是计时器所需的一个变量

        public static readonly Color HealAnalysisResource = new(187, 91, 201); // We can use this for CombatText, if you create an item that replenishes AnalysisResourceCurrent.
                                                                               // 如果您创建了补充AnalysisResourceCurrent项目，则可以将其用于CombatText。

        // In order to make the Analysis Resource Analysis straightforward, several things have been left out that would be needed for a fully functional resource similar to mana and health. 
        // Here are additional things you might need to implement if you intend to make a custom resource:
        // - Multiplayer Syncing: The current Analysis doesn't require MP code, but pretty much any additional functionality will require this. ModPlayer.SendClientChanges and CopyClientState will be necessary, as well as SyncPlayer if you allow the user to increase AnalysisResourceMax.
        // - Save/Load permanent changes to max resource: You'll need to implement Save/Load to remember increases to your AnalysisResourceMax cap.
        // - Resouce replenishment item: Use GlobalNPC.NPCLoot to drop the item. ModItem.OnPickup and ModItem.ItemSpace will allow it to behave like Mana Star or Heart. Use code similar to Player.HealEffect to spawn (and sync) a colored number suitable to your resource.

        // 为了使Analysis Resource Analysis简单明了，已省略了几个需要完全功能性类似于法力和生命之类的完整功能性所需内容。
        // 如果要制作自定义资源，则可能需要实现以下其他内容：
        // - 多人同步：目前不需要MP代码进行分析，但几乎任何其他功能都需要此项。ModPlayer.SendClientChanges和CopyClientState将是必要条件，并且如果允许用户增加AnalysisResourceMax，则还需要SyncPlayer。
        // - 永久更改保存/加载到max resource：您需要实现Save / Load来记住对AnalysisResourceMax上限进行的增加。
        // - 资源补给物品：使用GlobalNPC.NPCLoot放下物品。ModItem.OnPickup和ModItem.ItemSpace将使其像Mana Star或Heart一样运行。使用类似于Player.HealEffect的代码生成（并同步）适合您资源的彩色数字。

        public override void Initialize()
        {
            AnalysisResourceMax = DefaultAnalysisResourceMax;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        // We need this to ensure that regeneration rate and maximum amount are reset to default values after increasing when conditions are no longer satisfied (e.g. we unequip an accessory that increaces our recource)
        // 我们需要这个来确保在不再满足条件时（例如，我们取消装备增加我们资源的配件），再生速率和最大数量重置为默认值
        private void ResetVariables()
        {
            AnalysisResourceRegenRate = 1f;
            AnalysisResourceMax2 = AnalysisResourceMax;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        // Lets do all our logic for the custom resource here, such as limiting it, increasing it and so on.
        // 让我们在这里处理自定义资源的所有逻辑，例如限制它、增加它等等。
        private void UpdateResource()
        {
            // For our resource lets make it regen slowly over time to keep it simple, let's use AnalysisResourceRegenTimer to count up to whatever value we want, then increase currentResource.
            // 对于我们的资源，让它随着时间缓慢恢复以保持简单性。让我们使用AnalysisResourceRegenTimer计数到任何我们想要的值，然后增加currentResource。
            AnalysisResourceRegenTimer++; // Increase it by 60 per second, or 1 per tick.
                                          // 每秒增加60个单位或每个时刻增加1个单位。

            // A simple timer that goes up to 3 seconds, increases the AnalysisResourceCurrent by 1 and then resets back to 0.
            // 一个简单的定时器，在3秒钟内将AnalysisResourceCurrent增加1，然后重置为0。
            if (AnalysisResourceRegenTimer > 180 / AnalysisResourceRegenRate)
            {
                AnalysisResourceCurrent += 1;
                AnalysisResourceRegenTimer = 0;
            }

            // Limit AnalysisResourceCurrent from going over the limit imposed by AnalysisResourceMax.
            // 将AnalysisResourceCurrent限制在由AnalysisResourceMax强制实施的上限之下。
            AnalysisResourceCurrent = Utils.Clamp(AnalysisResourceCurrent, 0, AnalysisResourceMax2);
        }
    }
}
