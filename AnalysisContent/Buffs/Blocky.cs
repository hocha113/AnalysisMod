using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.Items.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class Blocky : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            AnalysisCostumePlayer p = player.GetModPlayer<AnalysisCostumePlayer>();

            // We use blockyAccessoryPrevious here instead of blockyAccessory because UpdateBuffs happens before UpdateEquips but after ResetEffects.
            // 我们在这里使用 blockyAccessoryPrevious 而不是 blockyAccessory，因为 UpdateBuffs 在 UpdateEquips 之前但在 ResetEffects 之后发生。
            if (player.townNPCs >= 1 && p.BlockyAccessoryPrevious)
            {
                p.BlockyPower = true;

                if (Main.myPlayer == player.whoAmI && Main.time % 1000 == 0)
                {
                    player.QuickSpawnItem(player.GetSource_Buff(buffIndex), ModContent.ItemType<AnalysisBlock>());
                }

                player.jumpSpeedBoost += 4.8f;
                player.extraFall += 45;

                // Some other effects:
                // 其他一些效果：

                //player.lifeRegen++;
                //player.meleeCrit += 2;
                //player.meleeDamage += 0.051f;
                //player.meleeSpeed += 0.051f;
                //player.statDefense += 3;
                //player.moveSpeed += 0.05f;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
