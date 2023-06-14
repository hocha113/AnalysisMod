using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)] // Load the spritesheet you create as a shield for the player when it is equipped.
                                      // 加载你创建的精灵图表作为玩家装备盾牌时使用。
    public class AnalysisShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;

            Item.defense = 1000;
            Item.lifeRegen = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 1f; // Increase ALL player damage by 100%
                                                         // 增加所有玩家伤害100％
            player.endurance = 1f - 0.1f * (1f - player.endurance);  // The percentage of damage reduction
                                                                     // 减伤百分比
            player.GetModPlayer<AnalysisDashPlayer>().DashAccessoryEquipped = true;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // 请参阅AnalysisContent / AnalysisRecipes.cs，了解有关配方创建的详细说明。
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }

    public class AnalysisDashPlayer : ModPlayer
    {
        // These indicate what direction is what in the timer arrays used
        // 这些指示计时器数组中的哪个方向是什么
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
                                            // 开始破折号之间的时间（帧）。 如果这比DashDuration短，则可以在旧的结束之前启动新的破折号

        public const int DashDuration = 35; // Duration of the dash afterimage effect in frames
                                            // 虚影效果持续时间（以帧为单位）

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        // 初始速度。 10速度约为37.5个平铺/秒或50英里/小时
        public const float DashVelocity = 10f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        // 玩家双击过去的方向。 默认值为-1表示没有dash双击
        public int DashDir = -1;

        // The fields related to the dash accessory
        // 与dash附件相关联的字段
        public bool DashAccessoryEquipped;
        public int DashDelay = 0; // frames remaining till we can dash again
                                  // 剩余框架间隔，直到我们可以再次冲刺

        public int DashTimer = 0; // frames remaining in the dash
                                  // 剩余框架间隔，在冲刺中保持不变

        public override void ResetEffects()
        {
            // Reset our equipped flag. If the accessory is equipped somewhere, AnalysisShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
            // 重置我们已装备标志。 如果配件在某处安装，则将调用AnalysisShield.UpdateAccessory并在PreUpdateMovement之前设置标志
            DashAccessoryEquipped = false;

            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap

            // ResetEffects不久后会调用player.doubleTapCardinalTimer值已设置
            //当按下并释放定向键时，香草会启动一个15 tick（1/4秒）计时器，在此期间第二次按下激活冲刺
            //如果计时器设置为15，则这是香草逻辑刚处理的第一次按下。 否则，这是双击
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
            {
                DashDir = DashDown;
            }
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
            {
                DashDir = DashUp;
            }
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
            {
                DashDir = DashRight;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }
        }

        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame

        // 这是应用冲刺运动的完美位置，它在香草运动代码之后，在根据速度修改玩家位置之前。
        //如果他们在本帧中双击，则他们将快速移动
        public override void PreUpdateMovement()
        {
            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            // 如果玩家可以使用我们的dash，并且已经向某个方向进行了双击，并且我们的dash当前没有处于冷却状态
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    // 仅当所需方向上的当前速度小于DashVelocity时才应用破折号速度
                    case DashUp when Player.velocity.Y > -DashVelocity:
                    case DashDown when Player.velocity.Y < DashVelocity:
                        {
                            // Y-velocity is set here
                            // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                            // This adjustment is roughly 1.3x the intended dash velocity

                            // 这里设置Y轴速度
                            // 如果请求的方向是DashUp，则我们调整速度以使冲刺看起来更“快”，因为重力立即生效
                            // 此调整大约是预期冲刺速度的1.3倍
                            float dashDirection = DashDir == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * DashVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            // X-velocity is set here
                            // 在这里设置X轴速度
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                                // 移动不够快，所以不开始冲刺
                }

                // start our dash
                // 开始我们的冲刺
                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                // Here you'd be able to set an effect that happens when the dash first activates
                // Some Analysiss include:  the larger smoke effect from the Master Ninja Gear and Tabi

                // 在此处您可以设置当第一次激活dash时发生的效果
                // 一些分析包括：Master Ninja Gear和Tabi产生较大的烟雾效果等。
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            { // dash is active
              // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
              // Some Analysiss include:  spawning dust where the player is, adding buffs, making the player immune, etc.
              // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect

              // dash已激活
              // 这是我们设置残影效果的地方。 您可以用任何您想要在dash期间发生的事情替换这两行代码。
              // 一些分析包括：在玩家位置生成灰尘、添加增益、使玩家免疫等。
              // 在这里，我们利用“player.eocDash”和“player.armorEffectDrawShadowEOCShield”来获得Cthulhu之盾残影效果。
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;

                // count down frames remaining
                // 倒计时剩余帧数
                DashTimer--;
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && Player.dashType == 0 // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                                        // 玩家没有装备Tabi或EoCShield（优先考虑那些闪避）

                && !Player.setSolar // player isn't wearing solar armor
                                    // 玩家没有穿太阳神套装

                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
                                         // 玩家没有骑乘坐骑，因为在坐骑上进行闪避看起来很奇怪。
        }
    }
}
