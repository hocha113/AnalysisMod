using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisResourcePlayer : ModPlayer
    {
        // Here we create a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our Analysis resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
        public int AnalysisResourceCurrent; // Current value of our Analysis resource
        public const int DefaultAnalysisResourceMax = 100; // Default maximum value of Analysis resource
        public int AnalysisResourceMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().
        public int AnalysisResourceMax2; // Maximum amount of our Analysis resource. We will change that variable to increase maximum amount of our resource
        public float AnalysisResourceRegenRate; // By changing that variable we can increase/decrease regeneration rate of our resource
        internal int AnalysisResourceRegenTimer = 0; // A variable that is required for our timer
        public static readonly Color HealAnalysisResource = new(187, 91, 201); // We can use this for CombatText, if you create an item that replenishes AnalysisResourceCurrent.

        // In order to make the Analysis Resource Analysis straightforward, several things have been left out that would be needed for a fully functional resource similar to mana and health. 
        // Here are additional things you might need to implement if you intend to make a custom resource:
        // - Multiplayer Syncing: The current Analysis doesn't require MP code, but pretty much any additional functionality will require this. ModPlayer.SendClientChanges and CopyClientState will be necessary, as well as SyncPlayer if you allow the user to increase AnalysisResourceMax.
        // - Save/Load permanent changes to max resource: You'll need to implement Save/Load to remember increases to your AnalysisResourceMax cap.
        // - Resouce replenishment item: Use GlobalNPC.NPCLoot to drop the item. ModItem.OnPickup and ModItem.ItemSpace will allow it to behave like Mana Star or Heart. Use code similar to Player.HealEffect to spawn (and sync) a colored number suitable to your resource.

        public override void Initialize()
        {
            AnalysisResourceMax = DefaultAnalysisResourceMax;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        // We need this to ensure that regeneration rate and maximum amount are reset to default values after increasing when conditions are no longer satisfied (e.g. we unequip an accessory that increaces our recource)
        private void ResetVariables()
        {
            AnalysisResourceRegenRate = 1f;
            AnalysisResourceMax2 = AnalysisResourceMax;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        // Lets do all our logic for the custom resource here, such as limiting it, increasing it and so on.
        private void UpdateResource()
        {
            // For our resource lets make it regen slowly over time to keep it simple, let's use AnalysisResourceRegenTimer to count up to whatever value we want, then increase currentResource.
            AnalysisResourceRegenTimer++; // Increase it by 60 per second, or 1 per tick.

            // A simple timer that goes up to 3 seconds, increases the AnalysisResourceCurrent by 1 and then resets back to 0.
            if (AnalysisResourceRegenTimer > 180 / AnalysisResourceRegenRate)
            {
                AnalysisResourceCurrent += 1;
                AnalysisResourceRegenTimer = 0;
            }

            // Limit AnalysisResourceCurrent from going over the limit imposed by AnalysisResourceMax.
            AnalysisResourceCurrent = Utils.Clamp(AnalysisResourceCurrent, 0, AnalysisResourceMax2);
        }
    }
}
