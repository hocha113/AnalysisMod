using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisContent.NPCs
{
    public class AnalysisGlobalNPC : GlobalNPC
    {
        // TODO, npc.netUpdate when this changes, and GlobalNPC gets a SendExtraAI hook
        // TODO�����������ʱ�������npc.netUpdate����ʹGlobalNPC���SendExtraAI����
        public bool HasBeenHitByPlayer;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            // after ModNPC has run (lateInstantiation), check if the entity is a townNPC
            //��ModNPC���к�lateInstantiation�������ʵ���Ƿ�Ϊ����NPC
            return lateInstantiation && entity.townNPC;
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner != 255)
            {
                HasBeenHitByPlayer = true;
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            HasBeenHitByPlayer = true;
        }

        //If the merchant has been hit by a player, they will double their sell price
        //������˱���ҹ������������ۼ۸񽫷���
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (!npc.GetGlobalNPC<AnalysisGlobalNPC>().HasBeenHitByPlayer)
            {
                return;
            }

            foreach (Item item in items)
            {
                int value = item.shopCustomPrice ?? item.value;
                item.shopCustomPrice = value * 2;
            }
        }

        public override void SaveData(NPC npc, TagCompound tag)
        {
            if (HasBeenHitByPlayer)
            {
                tag.Add("HasBeenHitByPlayer", true);
            }
        }

        public override void LoadData(NPC npc, TagCompound tag)
        {
            HasBeenHitByPlayer = tag.ContainsKey("HasBeenHitByPlayer");
        }
    }
}