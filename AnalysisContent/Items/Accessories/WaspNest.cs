using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class WaspNest : ModItem
    {
        // Only gets run once per type
        // 每种类型只运行一次
        public override void Load()
        {
            IL_Player.beeType += HookBeeType;
        }

        // This IL editing (Intermediate Language editing) Analysis is walked through in the guide: https://github.com/tModLoader/tModLoader/wiki/Expert-IL-Editing#Analysis---hive-pack-upgrade
        // 这个IL编辑（中间语言编辑）模组在指南中有详细介绍：https://github.com/tModLoader/tModLoader/wiki/Expert-IL-Editing#Analysis---hive-pack-upgrade
        private static void HookBeeType(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);

                // Try to find where 566 is placed onto the stack
                // 尝试找到将566放入堆栈的位置
                c.GotoNext(i => i.MatchLdcI4(566));

                // Move the cursor after 566 and onto the ret op.
                // 将光标移动到566之后并移到ret操作。
                c.Index++;
                // Push the Player instance onto the stack
                // 将Player实例推送到堆栈上
                c.Emit(OpCodes.Ldarg_0);
                // Call a delegate using the int and Player from the stack.
                // 使用从堆栈中获取的int和Player调用委托。
                c.EmitDelegate<Func<int, Player, int>>((returnValue, player) =>
                {
                    // Regular c# code
                    // 常规C#代码
                    if (player.GetModPlayer<WaspNestPlayer>().strongBeesUpgrade && Main.rand.NextBool(10) && Main.ProjectileUpdateLoopIndex == -1)
                    {
                        return ProjectileID.Beenade;
                    }

                    return returnValue;
                });
            }
            catch (Exception e)
            {
                // If there are any failures with the IL editing, this method will dump the IL to Logs/ILDumps/{Mod Name}/{Method Name}.txt
                // 如果IL编辑出现任何故障，此方法将转储IL到Logs / ILDumps / {Mod Name} / {Method Name} .txt中。
                MonoModHooks.DumpIL(ModContent.GetInstance<AnalysisMod>(), il);

                // If the mod cannot run without the IL hook, throw an exception instead. The exception will call DumpIL internally
                // 如果模块不能在没有IL钩子的情况下运行，则抛出异常。 异常将内部调用DumpIL

                // throw new ILPatchFailureException(ModContent.GetInstance<AnalysisMod>(), il, e);
            }
        }

        public override void SetDefaults()
        {
            int realBackSlot = Item.backSlot;
            Item.CloneDefaults(ItemID.HiveBackpack);
            Item.value = Item.sellPrice(0, 5);
            // CloneDefaults will clear out the autoloaded Back slot, so we need to preserve it this way.
            // CloneDefaults会清除自动加载的Back插槽，因此我们需要以这种方式保留它。

            Item.backSlot = realBackSlot;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // The original Hive Pack sets strongBees.
            // 原始Hive Pack设置了strongBees。

            player.strongBees = true;
            // Here we add an additional effect
            // 在这里我们添加了一个额外的效果

            player.GetModPlayer<WaspNestPlayer>().strongBeesUpgrade = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            // Don't allow Hive Pack and Wasp Nest to be equipped at the same time.
            // 不允许同时装备蜂巢包和黄蜂巢。
            return incomingItem.type != ItemID.HiveBackpack;
        }
    }

    public class WaspNestPlayer : ModPlayer
    {
        public bool strongBeesUpgrade;
    }
}