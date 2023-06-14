using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisLuckPlayer : ModPlayer
    {
        public override void ModifyLuck(ref float luck)
        {   // ModifyLuck is what you'll normally use for any modded content that wants to modify luck.
            // Luck in total has a vanilla soft cap of 1. You can technically go above that value, but theres no benefit to be gained with vanilla luck calculations.
            // However, modders can use the luck value however they want, so going above 1 may be beneficial. Decimal values are still recommended, though.

            // ModifyLuck通常用于任何想要修改运气的模组内容。
            // 总体上，幸运值有一个1的基础软上限。你可以在这个值之上，但是使用原版幸运计算不会带来任何好处。
            // 然而，modders可以随意使用幸运值，因此超过1可能是有益的。建议仍然使用小数。
            if (Main.hardMode)
            { // If it is currently hardmode...
                // 如果当前为困难模式...

                luck += 0.5f; // ...add 0.5 luck to the total luck count!
                              // ...将0.5点幸运加入总幸运！
            }
            // Of course you can make luck negative as well, in which case a soft cap of -1 applies.
            // 当然你也可以让幸运变成负数，在这种情况下应用-1的软上限。

            // As the above code runs every time luck is calculated, and `hardMode` is accessible on both client and server, we don't need to worry about multiplayer syncing.
            // If you have some code which relies on client side calculations, you will need to sync the variables to calculate luck correctly on the server.

            // 由于以上代码每次计算luck时都会执行，并且`hardMode`可在客户端和服务器端访问,我们无需担心多人游戏同步问题。
            // 如果您编写了一些依赖客户端计算的代码，则需要同步变量以正确地在服务器上计算luck。
        }

        public override bool PreModifyLuck(ref float luck)
        { // PreModifyLuck is useful if you want to modify any vanilla luck values or want to prevent vanilla luck calculations from happening.
            // PreModifyLuck非常有用，如果您想修改任何原始版本中的luck值或者防止进行原始版本中默认的luck计算。

            Terraria.GameContent.Events.LanternNight.GenuineLanterns = true; // The game now thinks its a Lantern Night all the time, giving you the luck bonus.
                                                                             // 现在游戏认为一直处于灯笼节期间，并给予玩家额外奖励。

            Player.HasGardenGnomeNearby = true; // The game now thinks there's a garden gnome nearby all the time, giving you the luck bonus.
                                                // 现在游戏认为附近一直存在花园小矮人，并给予玩家额外奖励。

            if (Player.ladyBugLuckTimeLeft < 0)
            {   // If you have bad ladybug luck...
                // 如果你遇到了坏瓢虫的运气...
                Player.ladyBugLuckTimeLeft = 0; // ...completely cancel it out.
                                                // ...完全取消它。
            }

            return true; // PreModifyLuck returns true by default, but you can also return false if you want to prevent vanilla luck calculations from happening at all.
                         // PreModifyLuck默认返回true，但如果您想完全防止进行原始版本中默认的luck计算，则也可以返回false。
        }
    }
}
