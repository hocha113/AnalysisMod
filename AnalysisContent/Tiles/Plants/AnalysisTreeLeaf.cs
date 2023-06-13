using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Tiles.Plants
{
    public class AnalysisTreeLeaf : ModGore
    {
        public override string Texture => "AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisTree_Leaf";

        public override void SetStaticDefaults()
        {

            GoreID.Sets.SpecialAI[Type] = 3;
        }
    }
}
