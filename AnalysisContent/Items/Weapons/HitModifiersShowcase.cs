using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.Buffs;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    /// <summary>
    /// This item can help conceptualize various damage modification concepts. <br/>
    /// The Item.damage of this weapon is 100 so the math is easy to follow. Damage variation is disabled for all modes except the 1st mode for the same reason. <br/>
    /// When testing this weapon the first time, it is recommended to disable other mods and to remove all damage boosting accessories, as they will complicate the math being taught. <br/>
    /// Testing against <see cref="NPCID.BlueArmoredBonesNoPants"/> is recommended as it has high defense (50), good knockback resistance, and enough health for a few hits. Having 50 defense makes the math for defense and armor penetration easy to follow.
    /// <br/>
    /// The math taught in this Analysis also assumes the player is in a normal world. <br/> 
    /// Use right click to switch modes.<br/>
    /// This Analysis is purely for demonstation purposes only, it will not work in multiplayer. This should also not be considered correct code for a working dual-use weapon. <br/>
    /// 这个物品可以帮助概念化各种伤害修改的概念。<br/>
    /// 该武器的Item.damage为100，因此数学计算很容易跟进。除了第一模式外，所有模式都禁用了伤害变化，原因是相同的。<br/>
    /// 在第一次测试这个武器时，建议禁用其他mod并删除所有增加伤害的配件，因为它们会使所教授的数学变得复杂。<br/>
    /// 推荐对抗<see cref="NPCID.BlueArmoredBonesNoPants"/>进行测试，因为其具有高防御力（50），良好的击退抗性和足够多次攻击所需健康值。拥有50点防御力使得防御和穿甲方面的数学计算更容易理解。
    /// <br/>
    /// 本分析中所教授的数学公式也假定玩家处于普通世界中。<br/>
    /// 使用右键切换模式。<br/>
    /// 本分析仅供演示目的，在多人游戏中不起作用。同时也不应将其视为可工作双重使用武器正确代码。
    /// </summary>
    public class HitModifiersShowcase : ModItem
    {
        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisSword";

        private const int numberOfModes = 8;
        private int mode = 0;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 100;
            Item.knockBack = 5;
            Item.crit = 10;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)mode);
        }

        public override void NetReceive(BinaryReader reader)
        {
            mode = reader.ReadByte();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                mode++;
                if (mode >= numberOfModes)
                {
                    mode = 0;
                }
                Main.NewText($"Switching to mode #{mode}: {GetMessageForMode()}");
                // This line will trigger NetSend to be called at the end of this game update, allowing the changes to useStyle to be in sync. 
                //这行代码将在游戏更新结束时调用NetSend，使useStyle的更改保持同步。
                Item.NetStateChanged();
            }
            else
            {
                Main.NewText($"Mode #{mode}: {GetMessageForMode()}");
            }
            return true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (!Main.rand.NextBool(3))
                return;
            Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Firework_Red + mode);
        }

        private string GetMessageForMode()
        {
            switch (mode)
            {
                case 0:
                    return "Normal damage behavior";
                case 1:
                    return "Damage variation disabled";
                case 2:
                    return "50% extra knockback";
                case 3:
                    return "200% extra critical hit damage";
                case 4:
                    return "10 extra armor penetration. Test against high defense enemy";
                case 5:
                    // This is similar to the Lightning Aura and Flymeal weapon effects                  
                    return "50% extra armor penetration. Ignores 50% of enemy defense";
                case 6:
                    return "Will apply AnalysisDefenseDebuff, reducing defense by 25%";
                case 7:
                    return "On hit, gives player AnalysisDodgeBuff to dodge the next hit";

            }
            return "Unknown mode";
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            // These effects modify the hit itself, so they need to be in this method.
            //这些效果修改了打击本身，因此它们需要在此方法中。
            if (mode != 0)
            {
                modifiers.DamageVariationScale *= 0f;
            }
            if (mode == 2)
            {
                modifiers.Knockback += .5f;
            }
            else if (mode == 3)
            {
                modifiers.CritDamage += 2f; // Default crit is 100% more than a normal hit, so with this in effect, crits should deal 4x damage
                                            // 默认暴击比普通攻击多100％，因此启用后，暴击应该造成4倍伤害
            }
            else if (mode == 4)
            {
                modifiers.ArmorPenetration += 10f;
            }
            else if (mode == 5)
            {
                modifiers.ScalingArmorPenetration += 0.5f;
            }

            // Below is an Analysis of using ModifyHitInfo to alter the final value of damage, between Modify and OnHit hooks.
            // This 'backdoor' is a replacmenet for the old style of modifiers which allowed modifying the damage via `ref`
            // Please only use this if absolutely necessary, as multiple mods freely altering the damage results will create incompatible or unintutive player experiences.

            // 以下是使用ModifyHitInfo来修改最终伤害值的分析，在Modify和OnHit钩子之间。
            //如果绝对必要，请使用此“后门”替换旧式修饰符允许通过`ref`修改伤害
            //请仅在绝对必要时使用此功能，因为多个自由更改伤害结果的修饰符会创建不兼容或不直观的玩家体验。
            //
            // For Analysis, the effect below could be better implemented by checking `player.GetWeaponDamage(Item)` and adding to FinalDamage.Base, SourceDamage.Base, SourceDamage.Flat or FlatBonusDamage
            //对于分析而言，下面的效果可以通过检查'player.GetWeaponDamage（Item）'并添加到FinalDamage.Base、SourceDamage.Base、SourceDamage.Flat或FlatBonusDamage中来更好地实现
            /*
			modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => {
				if (hitInfo.Damage > 10) {
					hitInfo.Damage += 5;
				}
			};
			*/
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // These effects act on a hit happening, so they should go here.
            // Buffs added locally are automatically synced to the server and other players in multiplayer

            //这些效果发生在命中时，所以应该放在这里。
            //本地添加的增益会自动同步到服务器和其他玩家
            if (mode == 6)
            {
                target.AddBuff(ModContent.BuffType<AnalysisDefenseDebuff>(), 600);
            }
            else if (mode == 7)
            {
                var damageModificationPlayer = player.GetModPlayer<AnalysisDamageModificationPlayer>();
                if (damageModificationPlayer.AnalysisDodgeCooldown == 0)
                {
                    player.AddBuff(ModContent.BuffType<AnalysisDodgeBuff>(), 1800);
                }
            }
        }

        // Due to the differences in pvp damage calculations, only some of the effects of this weapon work in pvp.
        // 由于PvP伤害计算方式不同，因此武器某些效果只适用于PvE。
        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            // Unlike the effects in OnHitPvp, these specific effects need to run on all clients to keep things in sync, so there is no check for local player.
            // 与OnHitPvp中的效果不同，这些特定效果需要运行所有客户端才能保持同步。所以没有检查本地玩家。
            if (mode == 2)
            {
                modifiers.Knockback += .5f;
            }
            else if (mode == 4)
            {
                modifiers.ArmorPenetration += 10f;
            }
            else if (mode == 5)
            {
                modifiers.ScalingArmorPenetration += 0.5f;
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            // These effects of this weapon should only run on the player damaging another, this check does that.
            // 这些武器的效果应该仅在攻击其他人的玩家时运行，此检查可以实现这一点。
            if (player != Main.LocalPlayer)
                return;

            if (mode == 6)
            {
                // This AddBuff is not quiet because it is affecting another player. This allows it to broadcast to all players that the target has a buff.
                // (Main.pvpBuff must be set to true for other players to be able to give buffs to a player)
                // Note that in PvP, it is possible to attack a player and see them take damage, but by the time the hit message arrives on the target client,
                // they may have recharged a dodge. In this case, the target will not actually take damage, and their health will appear to restore.
                // Because the attacking player applies the debuff, the target will receive the debuff regardless

                //AddBuff不是安静的，因为它会影响另一个玩家。这使其能够向所有玩家广播目标具有增益。
                //（Main.pvpBuff必须设置为true才能让其他玩家给予某个玩家增益）
                //请注意，在PvP中，您可能会攻击一个玩家并看到他们受到伤害，但当命中消息到达目标客户端时，
                //他们可能已经重新充电了闪避。在这种情况下，目标实际上不会受到伤害，并且他们的健康状况将显示恢复。
                //由于攻击者应用debuff，则无论如何都会对目标造成debuff
                target.AddBuff(ModContent.BuffType<AnalysisDefenseDebuff>(), 600, quiet: false);
            }
            else if (mode == 7)
            {
                var damageModificationPlayer = player.GetModPlayer<AnalysisDamageModificationPlayer>();
                if (damageModificationPlayer.AnalysisDodgeCooldown == 0)
                {
                    player.AddBuff(ModContent.BuffType<AnalysisDodgeBuff>(), 1800);
                }
            }
        }
    }
}
