﻿using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Buffs;
using AnalysisMod.AnalysisContent.Items.Armor;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisCostumePlayer : ModPlayer
    {
        // These 5 relate to AnalysisCostume.
        public bool BlockyAccessoryPrevious;
        public bool BlockyAccessory;
        public bool BlockyHideVanity;
        public bool BlockyForceVanity;
        public bool BlockyPower;

        public override void ResetEffects()
        {
            BlockyAccessoryPrevious = BlockyAccessory;
            BlockyAccessory = BlockyHideVanity = BlockyForceVanity = BlockyPower = false;
        }

        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.GetAmountOfExtraAccessorySlotsToShow(); n++)
            {
                Item item = Player.armor[n];
                if (item.type == ModContent.ItemType<AnalysisCostume>())
                {
                    BlockyHideVanity = false;
                    BlockyForceVanity = true;
                }
            }
        }

        public override void UpdateEquips()
        {
            // Make sure this condition is the same as the condition in the Buff to remove itself. We do this here instead of in ModItem.UpdateAccessory in case we want future upgraded items to set blockyAccessory
            if (Player.townNPCs >= 1 && BlockyAccessory)
            {
                Player.AddBuff(ModContent.BuffType<Blocky>(), 60);
            }
        }

        public override void FrameEffects()
        {
            // TODO: Need new hook, FrameEffects doesn't run while paused.
            if ((BlockyPower || BlockyForceVanity) && !BlockyHideVanity)
            {
                var AnalysisCostume = ModContent.GetInstance<AnalysisCostume>();
                Player.head = EquipLoader.GetEquipSlot(Mod, AnalysisCostume.Name, EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, AnalysisCostume.Name, EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, AnalysisCostume.Name, EquipType.Legs);

                // Use the alternative equipment textures by calling them through their internal name.
                if (Player.wet)
                {
                    Player.head = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Head);
                    Player.body = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Body);
                    Player.legs = EquipLoader.GetEquipSlot(Mod, "BlockyAlt", EquipType.Legs);
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if ((BlockyPower || BlockyForceVanity) && !BlockyHideVanity)
            {
                Player.headRotation = Player.velocity.Y * Player.direction * 0.1f;
                Player.headRotation = Utils.Clamp(Player.headRotation, -0.3f, 0.3f);
                if (Player.InModBiome<AnalysisSurfaceBiome>())
                {
                    Player.headRotation = (float)Main.time * 0.1f * Player.direction;
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (BlockyAccessory)
            {
                modifiers.DisableSound();
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (BlockyAccessory)
            {
                SoundEngine.PlaySound(SoundID.Frog, Player.position);
            }
        }
    }
}