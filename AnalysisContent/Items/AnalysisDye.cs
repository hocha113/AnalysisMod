using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Avoid loading assets on dedicated servers. They don't use graphics cards.
            // 避免在专用服务器上加载资源。它们不使用图形卡(显卡)。
            if (!Main.dedServ)
            {
                // The following code creates an effect (shader) reference and associates it with this item's type Id.
                // 以下代码创建一个效果（着色器）引用，并将其与此项的类型 ID 关联起来。
                GameShaders.Armor.BindShader(
                    Item.type,
                    new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/AnalysisEffect", AssetRequestMode.ImmediateLoad).Value), "AnalysisDyePass") // Be sure to update the effect path and pass name here.
                                                                                                                                                                               // 请确保在此处更新效果路径和传递名称。
                );
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            // Item.dye will already be assigned to this item prior to SetDefaults because of the above GameShaders.Armor.BindShader code in Load().
            // This code here remembers Item.dye so that information isn't lost during CloneDefaults.

            // Item.dye 将在 SetDefaults 之前分配给此项，因为 Load() 中的 GameShaders.Armor.BindShader 代码已经完成了这个工作。
            // 这里的代码记住了 Item.dye，以便在 CloneDefaults 中不会丢失信息。
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.GelDye); // Makes the item copy the attributes of the item "Gel Dye" Change "GelDye" to whatever dye type you want.
                                               // 使该物品复制“凝胶染料”的属性。将“GelDye”更改为您想要的任何染料类型。

            Item.dye = dye;
        }
    }
}
