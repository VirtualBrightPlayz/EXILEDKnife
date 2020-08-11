using EXILED;
using EXILED.Extensions;
using ItemManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knife
{
    public class PluginMain : Plugin
    {
        public override string getName => "Knife";
        public CustomItemHandler<KnifeItem> knife;
        public bool gunLoaded;

        public static float damage;

        public override void OnDisable()
        {

        }

        public void RefreshConfig()
        {
            damage = Config.GetFloat("knife_dmg", 25f);
        }

        public override void OnEnable()
        {
            if (!Config.GetBool("knife_enabled", true))
                return;
            gunLoaded = false;
            RefreshConfig();
            EXILED.Events.WaitingForPlayersEvent += Events_WaitingForPlayersEvent;
            Events.UseMedicalItemEvent += Events_UseMedicalItemEvent;
            Log.Info("Knife plugin loaded.");
        }

        private void Events_UseMedicalItemEvent(MedicalItemEvent ev)
        {
            Log.Info("Medkit!!");
        }

        private void Events_WaitingForPlayersEvent()
        {
            if (gunLoaded)
                return;
            gunLoaded = true;
            knife = new CustomItemHandler<KnifeItem>(207)
            {
                DefaultType = ItemType.Adrenaline
            };
            knife.Register();
            /*foreach (var item in PlayerManager.localPlayer.GetComponent<ConsumableAndWearableItems>().usableItems)
            {
                Log.Info(item.inventoryID.ToString());
            }*/
            Log.Info("New Item Knife registered.");
        }
        public override void OnReload()
        {
            if (!Config.GetBool("knife_enabled", true))
                return;
            EXILED.Events.WaitingForPlayersEvent -= Events_WaitingForPlayersEvent;
            Events.UseMedicalItemEvent -= Events_UseMedicalItemEvent;
            knife.Unregister();
            knife = null;
        }
    }
}
