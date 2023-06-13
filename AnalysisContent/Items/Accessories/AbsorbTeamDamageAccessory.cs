using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using AnalysisMod.AnalysisContent.Buffs;
using Terraria.Localization;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    /// <summary>
    /// AbsorbTeamDamageAccessory mimics the unique effect of the Paladin's Shield item.
    /// This Analysis showcases some advanced interplay between accessories, buffs, and ModPlayer hooks.
    /// Of particular note is how this accessory gives other players a buff and how a player might act on another player being hit.<br/>
    /// 吸收团队伤害饰品模仿了圣骑士盾牌物品的独特效果。
    /// 这个分析展示了配件、增益和ModPlayer钩子之间的一些高级相互作用。
    /// 特别值得注意的是，这个配件给其他玩家提供了一个增益，以及一个玩家可能会在另一个玩家被击中时采取的行动。
    /// </summary>
    [AutoloadEquip(EquipType.Shield)]
    public class AbsorbTeamDamageAccessory : ModItem
    {
        public static readonly int DamageAbsorptionAbilityLifeThresholdPercent = 50;
        public static float DamageAbsorptionAbilityLifeThreshold => DamageAbsorptionAbilityLifeThresholdPercent / 100f;

        public static readonly int DamageAbsorptionPercent = 30;
        public static float DamageAbsorptionMultiplier => DamageAbsorptionPercent / 100f;

        // 50 tiles is 800 world units. (50 * 16 == 800)
        // 50格为800世界单位。(50 * 16 == 800)
        public static readonly int DamageAbsorbtionRange = 800;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAbsorptionPercent, DamageAbsorptionAbilityLifeThresholdPercent);

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 6;
            Item.value = Item.buyPrice(0, 30, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // player.noKnockback = true; could be used here if this accessory prevented knockback.
            // 如果此配件防止击退，则可以在此处使用player.noKnockback = true;

            player.GetModPlayer<AnalysisDamageModificationPlayer>().hasAbsorbTeamDamageEffect = true;

            // Remember that UpdateAccessory runs for all players on all clients. Only check every 10 ticks
            // 记住UpdateAccessory运行于所有客户端上的所有玩家。只检查每10个tick
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                Player localPlayer = Main.player[Main.myPlayer];
                if (localPlayer.team == player.team && player.team != 0 && player.statLife > player.statLifeMax2 * DamageAbsorptionAbilityLifeThreshold && player.Distance(localPlayer.Center) <= DamageAbsorbtionRange)
                {
                    // The buff is used to visually indicate to the player that they are defended, and is also synchronized automatically to other players, letting them know that we were defended at the time we took the hit
                    // 增益用于向玩家视觉地指示他们已经受到保护，并自动与其他玩家同步，让他们知道我们在受到攻击时得到了保护
                    localPlayer.AddBuff(ModContent.BuffType<AbsorbTeamDamageBuff>(), 20);
                }
            }
        }
    }
}
