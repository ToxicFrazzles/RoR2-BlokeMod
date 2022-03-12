using BepInEx;
using RoR2;
using RoR2.Stats;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin
{
	//This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

	
	//This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	
	
	//This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class BlokeMod : BaseUnityPlugin
	{
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ToxicFrazzles";
        public const string PluginName = "BlokeMod";
        public const string PluginVersion = "1.0.0";

		//The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {   


            // Make all lunar coin purchases affordable
            On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += (orig, self, activator) =>
            {
                return self.costType == CostTypeIndex.LunarCoin || orig(self, activator);
            };

            On.RoR2.PurchaseInteraction.OnInteractionBegin += (orig, self, activator) =>
            {
                // Don't take lunar coins for purchases
                if(self.costType == CostTypeIndex.LunarCoin)
                {
                    self.onPurchase.Invoke(activator);
                    self.lastActivator = activator;
                }
                else
                {
                    orig(self, activator);
                }
            };

            // Delete barrels after they have been opened
            On.RoR2.BarrelInteraction.OnInteractionBegin += (orig, self, activator) =>
            {
                orig(self, activator);
                Destroy(self.gameObject);
            };

            // Delete chests after they have been opened
            On.RoR2.ChestBehavior.ItemDrop += (orig, self) =>
            {
                orig(self);
                Destroy(self.gameObject);
            };

            // Always spawn the blue portal.
            On.RoR2.TeleporterInteraction.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    self.shouldAttemptToSpawnShopPortal = true;
                }
            };


        }

        //The Update() method is run on every frame of the game.
        //private void Update()
        //{
        //    //This if statement checks if the player has currently pressed F2.
        //    if (Input.GetKeyDown(KeyCode.F2))
        //    {
        //        //Get the player body to use a position:	
        //        var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
        //
        //        //And then drop our defined item in front of the player.
        //        //PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(myItemDef.itemIndex), transform.position, transform.forward * 20f);
        //    }   
        //}
    }
}
