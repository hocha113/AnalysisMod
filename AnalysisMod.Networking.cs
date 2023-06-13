using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.NPCs;
using System.IO;
using Terraria;
using Terraria.ID;

namespace AnalysisMod
{
	// This is a partial class, meaning some of its parts were split into other files. See AnalysisMod.*.cs for other portions.
	partial class AnalysisMod
	{
		internal enum MessageType : byte
		{
			AnalysisStatIncreasePlayerSync,
			AnalysisTeleportToStatue,
			AnalysisDodge
		}

		// Override this method to handle network packets sent for this mod.
		//TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType) {
				// This message syncs AnalysisStatIncreasePlayer.AnalysisLifeFruits and AnalysisStatIncreasePlayer.AnalysisManaCrystals
				case MessageType.AnalysisStatIncreasePlayerSync:
					byte playernumber = reader.ReadByte();
					AnalysisStatIncreasePlayer AnalysisPlayer = Main.player[playernumber].GetModPlayer<AnalysisStatIncreasePlayer>();
					AnalysisPlayer.ReceivePlayerSync(reader);

					if (Main.netMode == NetmodeID.Server) {
						// Forward the changes to the other clients
						AnalysisPlayer.SyncPlayer(-1, whoAmI, false);
					}
					break;
				case MessageType.AnalysisTeleportToStatue:
					if (Main.npc[reader.ReadByte()].ModNPC is AnalysisPerson person && person.NPC.active) {
						person.StatueTeleport();
					}

					break;
				case MessageType.AnalysisDodge:
					AnalysisDamageModificationPlayer.HandleAnalysisDodgeMessage(reader, whoAmI);
					break;
				default:
					Logger.WarnFormat("AnalysisMod: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}