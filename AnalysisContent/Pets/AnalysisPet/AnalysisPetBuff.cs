using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.AnalysisPet
{
    public class AnalysisPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {   // This method gets called every frame your buff is active on your player.
            // 这个方法会在你的玩家身上的buff每一帧都被调用。
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<AnalysisPetProjectile>());
        }
    }
}
