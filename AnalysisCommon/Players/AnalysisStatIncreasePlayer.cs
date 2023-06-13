using AnalysisMod.AnalysisContent.Items.Consumables;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisStatIncreasePlayer : ModPlayer
    {
        public int AnalysisLifeFruits;
        public int AnalysisManaCrystals;

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            health.Base = AnalysisLifeFruits * AnalysisLifeFruit.LifePerFruit;
            // Alternatively:  health = StatModifier.Default with { Base = AnalysisLifeFruits * AnalysisLifeFruit.LifePerFruit };
            mana = StatModifier.Default;
            mana.Base = AnalysisManaCrystals * AnalysisManaCrystal.ManaPerCrystal;
            // Alternatively:  mana = StatModifier.Default with { Base = AnalysisManaCrystals * AnalysisManaCrystal.ManaPerCrystal };
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)AnalysisMod.MessageType.AnalysisStatIncreasePlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)AnalysisLifeFruits);
            packet.Write((byte)AnalysisManaCrystals);
            packet.Send(toWho, fromWho);
        }

        // Called in AnalysisMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            AnalysisLifeFruits = reader.ReadByte();
            AnalysisManaCrystals = reader.ReadByte();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            AnalysisStatIncreasePlayer clone = (AnalysisStatIncreasePlayer)targetCopy;
            clone.AnalysisLifeFruits = AnalysisLifeFruits;
            clone.AnalysisManaCrystals = AnalysisManaCrystals;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            AnalysisStatIncreasePlayer clone = (AnalysisStatIncreasePlayer)clientPlayer;

            if (AnalysisLifeFruits != clone.AnalysisLifeFruits || AnalysisManaCrystals != clone.AnalysisManaCrystals)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }

        // NOTE: The tag instance provided here is always empty by default.
        // Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.
        public override void SaveData(TagCompound tag)
        {
            tag["AnalysisLifeFruits"] = AnalysisLifeFruits;
            tag["AnalysisManaCrystals"] = AnalysisManaCrystals;
        }

        public override void LoadData(TagCompound tag)
        {
            AnalysisLifeFruits = tag.GetInt("AnalysisLifeFruits");
            AnalysisManaCrystals = tag.GetInt("AnalysisManaCrystals");
        }
    }
}
