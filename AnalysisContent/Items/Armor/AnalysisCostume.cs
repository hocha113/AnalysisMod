using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Armor
{
    // This and several other classes show off using EquipTextures to do a Merfolk or Werewolf effect.
    // Typically Armor items are automatically paired with an EquipTexture, but we can manually use EquipTextures to achieve more unique effects.
    // There is code for this effect in many places, look in the following files for the full implementation:
    // NPCs.AnalysisPerson drops this item when killed
    // Content.Items.Armor.AnalysisCostume (below) is the accessory item that sets AnalysisCostumePlayer values. Note that this item does not have EquipTypes set. This is a vital difference and key to our approach.
    // Content.Items.Armor.BlockyHead (below) is an EquipTexture class. It spawns dust when active.
    // AnalysisCostume.Load() shows calling AddEquipTexture 3 times with appropriate parameters. This is how we register EquipTexture manually instead of the automatic pairing of ModItem and EquipTexture that other equipment uses.
    // Buffs.Blocky is the Buff that is shown while in Blocky mode. The buff is responsible for the actual stat effects of the costume. It also needs to remove itself when not near town npcs.
    // AnalysisCostumePlayer has 5 bools. They manage the visibility and other things related to this effect.
    // AnalysisCostumePlayer.ResetEffects resets those bool, except blockyAccessoryPrevious which is special because of the order of hooks.
    // AnalysisCostumePlayer.UpdateVanityAccessories is responsible for forcing the visual effect of our costume if the item is in a vanity slot. Note that ModItem.UpdateVanitySet can't be used for this because it is called too late.
    // AnalysisCostumePlayer.UpdateEquips is responsible for applying the Blocky buff to the player if the conditions are met and the accessory is equipped.
    // AnalysisCostumePlayer.FrameEffects is most important. It overrides the drawn equipment slots and sets them to our Blocky EquipTextures.
    // AnalysisCostumePlayer.ModifyDrawInfo is for some fun effects for our costume.
    // Remember that the visuals and the effects of Costumes must be kept separate. Follow this Analysis for best results.

    // 这个类和其他几个类展示了如何使用EquipTextures来实现人鱼或狼人效果。
    // 通常，装备物品会自动与EquipTexture配对，但我们可以手动使用EquipTextures来实现更独特的效果。
    // 许多地方都有这种效果的代码，请查看以下文件以获取完整的实现：
    // NPCs.AnalysisPerson在被杀时掉落此物品
    // Content.Items.Armor.AnalysisCostume（下面）是设置AnalysisCostumePlayer值的附件物品。请注意，该项未设置EquipTypes。这是我们方法的重要区别和关键所在。
    // Content.Items.Armor.BlockyHead（下面）是一个EquipTexture类。激活时会产生粉尘。
    // AnalysisCostume.Load()显示调用AddEquipTexture 3次，并带有适当的参数。这就是我们手动注册EquipTexture而不是ModItem和EquipTexture之间自动配对所使用的方法。
    // Buffs.Blocky 是处于Blocky模式下显示出来的Buff。该buff负责服装实际状态影响。它还需要在离城镇npc较远时将其删除。
    // AnalysisCostumePlayer有5个bool变量，它们管理与此效果相关联的可见性等其他事项。
    // AnalysisCostumePlayer.ResetEffects重置那些bool值，除了blockyAccessoryPrevious因为钩子顺序而特殊。
    // AnalysisCostumePlayer.UpdateVanityAccessories负责强制使用我们的服装的视觉效果，如果该物品在虚荣插槽中。请注意，ModItem.UpdateVanitySet不能用于此，因为它调用太晚了。
    // AnalysisCostumePlayer.UpdateEquips负责将Blocky buff应用于玩家（如果满足条件并且配件已经装备）。
    // AnalysisCostumePlayer.FrameEffects最重要。它覆盖了绘制的装备插槽，并将其设置为我们的Blocky EquipTextures。
    // AnalysisCostumePlayer.ModifyDrawInfo是一些有趣效果的实现方式。
    // 请记住，服装的外观和效果必须保持分离状态。遵循这个Analysis可以获得最佳结果。
    public class AnalysisCostume : ModItem
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            // 以下代码仅在非服务器加载时运行
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            // 添加装备纹理
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, equipTexture: new BlockyHead());
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);

            //Add a separate set of equip textures by providing a custom name reference instead of an item reference
            //通过提供自定义名称引用而不是物品引用添加单独的装备纹理集
            EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Head}", EquipType.Head, name: "BlockyAlt", equipTexture: new BlockyHead());
            EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Body}", EquipType.Body, name: "BlockyAlt");
            EquipLoader.AddEquipTexture(Mod, $"{Texture}Alt_{EquipType.Legs}", EquipType.Legs, name: "BlockyAlt");
        }

        // Called in SetStaticDefaults
        // 在SetStaticDefaults中调用
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            //由于装备纹理未在服务器上加载，因此我们无法在服务器端运行此代码。
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            int equipSlotHeadAlt = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Head);
            int equipSlotBodyAlt = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Body);
            int equipSlotLegsAlt = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Head.Sets.DrawHead[equipSlotHeadAlt] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBodyAlt] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBodyAlt] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegsAlt] = true;
        }

        public override void SetStaticDefaults()
        {
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 15);
            Item.rare = ItemRarityID.Pink;
            Item.hasVanityEffects = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var p = player.GetModPlayer<AnalysisCostumePlayer>();
            p.BlockyAccessory = true;
            p.BlockyHideVanity = hideVisual;
        }

        // TODO: Fix this once new hook prior to FrameEffects added.
        // Required so UpdateVanitySet gets called in EquipTextures

        // TODO：一旦添加了新的钩子以优先于FrameEffects，则修复此问题。
        // 必须这样做才能使UpdateVanitySet在EquipTextures中被调用
        public override bool IsVanitySet(int head, int body, int legs) => true;
    }

    public class BlockyHead : EquipTexture
    {
        public override void UpdateVanitySet(Player player)
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Sparkle>());
            }
        }
    }
}