using AnalysisMod.AnalysisContent.Buffs;
using AnalysisMod.AnalysisContent.Items.Accessories;
using AnalysisMod.AnalysisContent.Items.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    internal class AnalysisDamageModificationPlayer : ModPlayer
    {
        public float AdditiveCritDamageBonus;

        // These 3 fields relate to the Analysis Dodge. Analysis Dodge is modeled after the dodge ability of the Hallowed armor set bonus.
        // AnalysisDodge indicates if the player actively has the ability to dodge the next attack. This is set by AnalysisDodgeBuff, which in this Analysis is applied by the HitModifiersShowcase weapon. The buff is only applied if AnalysisDodgeCooldown is 0 and will be cleared automatically if an attack is dodged or if the player is no longer holding HitModifiersShowcase.

        // 这三个字段与分析闪避有关。分析闪避是模拟神圣盔甲套装加成的闪避能力。
        // AnalysisDodge表示玩家是否主动具备下一次攻击的躲避能力。这由AnalysisDodgeBuff设置，在此分析中，该效果由HitModifiersShowcase武器应用。如果AnalysisDodgeCooldown为0，则仅应用该buff，并且如果成功躲避攻击或玩家不再持有HitModifiersShowcase，则会自动清除该buff。
        public bool AnalysisDodge; // TODO: Analysis of custom player render
                                   // TODO：自定义玩家渲染的分析

        // Used to add a delay between Analysis Dodge being consumed and the next time the dodge buff can be aquired.
        // 用于在消耗Analysis Dodge和下一次获取dodge buff之间添加延迟。

        public int AnalysisDodgeCooldown;
        // Controls the intensity of the visual effect of the dodge.
        // 控制闪避视觉效果的强度。
        public int AnalysisDodgeVisualCounter;

        // If this player has an accessory which gives this effect
        // 如果此玩家配备了提供此效果的饰品
        public bool hasAbsorbTeamDamageEffect;

        // If the player is currently in range of a player with hasAbsorbTeamDamageEffect
        // 如果玩家当前处于hasAbsorbTeamDamageEffect范围内
        public bool defendedByAbsorbTeamDamageEffect;

        public bool AnalysisDefenseDebuff;

        public override void PreUpdate()
        {
            // Timers and cooldowns should be adjusted in PreUpdate
            // 计时器和冷却时间应在PreUpdate中进行调整
            if (AnalysisDodgeCooldown > 0)
            {
                AnalysisDodgeCooldown--;
            }
        }

        public override void ResetEffects()
        {
            AdditiveCritDamageBonus = 0f;

            AnalysisDodge = false;

            hasAbsorbTeamDamageEffect = false;
            defendedByAbsorbTeamDamageEffect = false;

            AnalysisDefenseDebuff = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (AdditiveCritDamageBonus > 0)
            {
                modifiers.CritDamage += AdditiveCritDamageBonus;
            }
        }

        public override void PostUpdateEquips()
        {
            // If the conditions for the player having the buff are no longer true, remove the buff.
            // This could could techinically go in AnalysisDodgeBuff.Update, but typically these effects are given by armor or accessories, so showing this Analysis here is more useful.

            // 如果玩家拥有buff条件不再满足，请删除buff。
            // 这可以放在AnalysisDodgeBuff.Update中，但通常这些效果是由盔甲或饰品赋予的，因此在此显示更有用。
            if (AnalysisDodge && Player.HeldItem.type != ModContent.ItemType<HitModifiersShowcase>())
            {
                Player.ClearBuff(ModContent.BuffType<AnalysisDodgeBuff>());
            }

            // AnalysisDodgeVisualCounter should be updated here, not in DrawEffects, to work properly
            // AnalysisDodgeVisualCounter应在此更新，而不是DrawEffects中，以正常工作
            AnalysisDodgeVisualCounter = Math.Clamp(AnalysisDodgeVisualCounter + (AnalysisDodge ? 1 : -1), 0, 30);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            // AnalysisDodgeVisualCounter helps fade the color effect in and out.
            // AnalysisDodgeVisualCounter帮助淡入淡出颜色效果。
            if (AnalysisDodgeVisualCounter > 0)
            {
                g = Math.Max(0, g - AnalysisDodgeVisualCounter * 0.03f);
            }

            if (AnalysisDefenseDebuff)
            {
                // These color adjustments match the withered armor debuff visuals.
                // 这些颜色调整与枯萎盔甲debuff视觉效果相匹配。
                g *= 0.5f;
                r *= 0.75f;
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (AnalysisDodge)
            {
                AnalysisDodgeEffects();
                return true;
            }

            return false;
        }

        // AnalysisDodgeEffects() will be called from ConsumableDodge and HandleAnalysisDodgeMessage to sync the effect.
        // AnalysisDodgeEffects()将从ConsumableDodge和HandleAnalysisDodgeMessage中调用以同步效果。
        public void AnalysisDodgeEffects()
        {
            Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);

            // Some sound and visual effects
            // 一些声音和视觉效果
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.BlueCrystalShard, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f });

            // The visual and sound effects happen on all clients, but the code below only runs for the dodging player 
            // 视觉和声音效果在所有客户端上发生，但下面的代码仅运行于闪避玩家
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Clearing the buff and assigning the cooldown time
            // 清除buff并分配冷却时间
            Player.ClearBuff(ModContent.BuffType<AnalysisDodgeBuff>());
            AnalysisDodgeCooldown = 180; // 3 second cooldown before the buff can be given again.
                                         // 3秒冷却时间，然后可以再次获得该buff。

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SendAnalysisDodgeMessage(Player.whoAmI);
            }
        }

        public static void HandleAnalysisDodgeMessage(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                player = whoAmI;
            }

            Main.player[player].GetModPlayer<AnalysisDamageModificationPlayer>().AnalysisDodgeEffects();

            if (Main.netMode == NetmodeID.Server)
            {
                // If the server receives this message, it sends it to all other clients to sync the effects.
                // 如果服务器收到此消息，则将其发送给所有其他客户端以同步效果。
                SendAnalysisDodgeMessage(player);
            }
        }

        public static void SendAnalysisDodgeMessage(int whoAmI)
        {
            // This code is called by both the initial 
            // 这段代码被初始调用
            ModPacket packet = ModContent.GetInstance<AnalysisMod>().GetPacket();
            packet.Write((byte)AnalysisMod.MessageType.AnalysisDodge);
            packet.Write((byte)whoAmI);
            packet.Send(ignoreClient: whoAmI);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (defendedByAbsorbTeamDamageEffect && Player == Main.LocalPlayer && TeammateCanAbsorbDamage())
            {
                modifiers.FinalDamage *= 1f - AbsorbTeamDamageAccessory.DamageAbsorptionMultiplier;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            // On Hurt is used in this Analysis to act upon another player being hurt.
            // If the player who was hurt was defended, check if the local player should take the remaining damage for them

            // On Hurt 用于在另一个玩家受伤时进行操作。
            // 如果被攻击的玩家得到了保护，检查本地玩家是否应该为他们承担剩余的伤害。
            Player localPlayer = Main.LocalPlayer;
            if (defendedByAbsorbTeamDamageEffect && Player != localPlayer && IsClosestShieldWearerInRange(localPlayer, Player.Center, Player.team))
            {
                // The intention of AbsorbTeamDamageAccessory is to transfer 30% of damage taken by teammates to the wearer.
                // In ModifiedHurt, we reduce the damage by 30%. The resulting reduced damage is passed to OnHurt, where the player wearing AbsorbTeamDamageAccessory hurts themselves.
                // Since OnHurt is provided with the damage already reduced by 30%, we need to reverse the math to determine how much the damage was originally reduced by
                // Working throught the math, the amount of damage that was reduced is equal to: damage * (percent / (1 - percent))

                // AbsorbTeamDamageAccessory 的目的是将队友所受伤害的30%转移给佩戴者。
                // 在 ModifiedHurt 中，我们减少了30%的伤害。结果减少后的伤害传递给 OnHurt，在那里佩戴 AbsorbTeamDamageAccessory 的玩家会自己受到伤害。
                // 由于 OnHurt 已经提供了已经减少30％的损坏量，因此我们需要反向计算以确定原始减少多少损坏量
                // 经过数学计算，被减少掉的损坏量等于：damage *（percent /（1- percent））
                float percent = AbsorbTeamDamageAccessory.DamageAbsorptionMultiplier;
                int damage = (int)(info.Damage * (percent / (1 - percent)));

                // Don't bother pinging the defending player and upsetting their immunity frames if the portion of damage we're taking rounds down to 0
                // 如果我们要承担部分损失并且四舍五入为0，则不必ping防御玩家并打乱其免疫帧。
                if (damage > 0)
                {
                    localPlayer.Hurt(PlayerDeathReason.LegacyEmpty(), damage, 0);
                }
            }
        }

        private bool TeammateCanAbsorbDamage()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player otherPlayer = Main.player[i];
                if (i != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, Player.team))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAbleToAbsorbDamageForTeammate(Player player, int team)
        {
            return player.active
                && !player.dead
                && !player.immune // This check can be removed, allowing players to take hits for team-mates in quick succession. Removing it can also help with de-syncs where the player getting hurt thinks there is no-one to tank the damage, but by the time the hit arrives on the player with the shield, they take extra damage
                                  // 可以删除此检查，使玩家能够快速连续地为队友挨打。删除也有助于解决出现假死情况时正在接收攻击但认为没有人来吸收攻击造成额外伤害的玩家。

                && player.GetModPlayer<AnalysisDamageModificationPlayer>().hasAbsorbTeamDamageEffect
                && player.team == team
                && player.statLife > player.statLifeMax2 * AbsorbTeamDamageAccessory.DamageAbsorptionAbilityLifeThreshold;
        }

        // This code finds the closest player wearing AbsorbTeamDamageAccessory. 
        // 此代码查找佩戴 AbsorbTeamDamageAccessory 的最近玩家。
        private static bool IsClosestShieldWearerInRange(Player player, Vector2 target, int team)
        {
            if (!IsAbleToAbsorbDamageForTeammate(player, team))
            {
                return false;
            }

            float distance = player.Distance(target);
            if (distance > AbsorbTeamDamageAccessory.DamageAbsorbtionRange)
            {
                return false; // player we're out of range, so can't take the hit
                              // 我们的范围之外，因此无法承受打击的玩家
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player otherPlayer = Main.player[i];
                if (i != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, team))
                {
                    float otherPlayerDistance = otherPlayer.Distance(target);
                    if (distance > otherPlayerDistance || distance == otherPlayerDistance && i < Main.myPlayer)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
