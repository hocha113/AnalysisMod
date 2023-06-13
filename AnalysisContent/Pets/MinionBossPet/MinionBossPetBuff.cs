using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.MinionBossPet
{
    // You can find a simple pet Analysis in AnalysisMod\Content\Pets\AnalysisPet
    // 你可以在 AnalysisMod\Content\Pets\AnalysisPet 中找到一个简单的宠物分析。
    public class MinionBossPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {   // This method gets called every frame your buff is active on your player.
            // 每当你的玩家身上有此增益效果时，该方法会被调用。
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<MinionBossPetProjectile>());
        }
    }
}
