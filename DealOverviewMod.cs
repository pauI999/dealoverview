using MelonLoader;
using HarmonyLib;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.UI.Phone.ContactsApp;
using Il2CppScheduleOne.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Il2CppScheduleOne.GameTime;
using Il2CppFishNet.Object;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppFishNet.Managing.Timing;
using Il2CppLiteNetLib;
using Il2CppScheduleOne.UI.Phone.Messages;
using Il2CppScheduleOne.UI.Phone.ProductManagerApp;
using Il2CppScheduleOne.Economy;
using static MelonLoader.MelonLogger;
using Il2CppAmplifyColor;
using System.Collections;
using static Il2CppFishNet.Managing.Transporting.LatencySimulator;

[assembly: MelonInfo(typeof(DealOverviewMod.DealOverviewMod), DealOverviewMod.BuildInfo.Name, DealOverviewMod.BuildInfo.Version, DealOverviewMod.BuildInfo.Author, DealOverviewMod.BuildInfo.DownloadLink)]
[assembly: MelonColor()]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace DealOverviewMod
{
    public static class BuildInfo
    {
        public const string Name = "DealOverviewMod";
        public const string Description = "";
        public const string Author = "pauI999";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class DealOverviewMod : MelonMod
    {
        private DealOverview _dealOverview;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            ((MelonBase)this).LoggerInstance.Msg("Scene loaded: " + sceneName);
            if (sceneName == "Main")
            {
                ((MelonBase)this).LoggerInstance.Msg("Starting 5s delay!");
                MelonCoroutines.Start(InitAfterDelay());
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (!(sceneName == "Main"))
            {
                return;
            }
            _dealOverview?.Cleanup();
            _dealOverview = null;
        }

        private IEnumerator InitAfterDelay()
        {
            yield return new WaitForSeconds(5f); // 5 Sekunden Delay, bis alles im Spiel initialisiert ist
            _dealOverview = DealOverview.Create();
        }

    }

    public class DealOverview
    {
        private TestDialogue _dialogue;

        private GameObject _npcObject;

        private NPC _npc;

        public static DealOverview Create()
        {
            DealOverview dealOverview = new DealOverview();
            dealOverview.Init();
            return dealOverview;
        }

        private void Init()
        {
            NPC val = null;
            Il2CppSystem.Collections.Generic.List<NPC> nPCRegistry = NPCManager.NPCRegistry;
            for (int i = 0; i < nPCRegistry.Count; i++)
            {
                NPC val2 = nPCRegistry[i];
                if ((Object)(object)val2 != (Object)null && val2.FirstName == "Beth")
                {
                    val = val2;
                    break;
                }
            }
            if (!((Object)(object)val == (Object)null))
            {
                _npcObject = Object.Instantiate<GameObject>(val.gameObject);
                _npc = _npcObject.GetComponent<NPC>();
                _npc.FirstName = "Deal";
                _npc.LastName = "Overview";
                _npc.BakedGUID = Guid.NewGuid().ToString();
                _npc.IsImportant = true;
                _npc.ConversationCanBeHidden = false;
                _npc.ShowRelationshipInfo = false;
                _npc.MSGConversation = null;
                _npc.ConversationCategories = new Il2CppSystem.Collections.Generic.List<EConversationCategory>();
                _npc.dialogueHandler = null;
                _npc.MugshotSprite = ((App<ProductManagerApp>)(object)PlayerSingleton<ProductManagerApp>.Instance).AppIcon;
                _npcObject.SetActive(true);
                ((Component)_npc).gameObject.SetActive(true);
                _npc.Awake();
                ((NetworkBehaviour)_npc).NetworkInitializeIfDisabled();
                NPCManager.NPCRegistry.Add(_npc);
                _npc.InitializeSaveable();
                _dialogue = new TestDialogue(_npc);
                Run();
            }
        }

        private void Run()
        {
            _dialogue.SendNPCMessage("Click button to list deals!");
            _dialogue.ShowResponses(new DialogueResponse[1]
            {
                new DialogueResponse("List deals", "List deals", SumUpDeals)
            }, 0.5f);
        }

        private void SumUpDeals()
        {
            List<DialogueResponse> list = new List<DialogueResponse>();
            var productTotals = new Dictionary<string, (uint bricks, uint jars, uint baggies)>();
            Il2CppSystem.Collections.Generic.List<Customer> customers = Customer.UnlockedCustomers;
            foreach (Customer c in customers)
            {
                if (c.CurrentContract?.ProductList?.entries != null &&
                c.CurrentContract.ProductList.entries.Count != 0 && c.CurrentContract.Dealer == null)
                {
                    ProductList.Entry targetProduct = c.CurrentContract.ProductList.entries[0];
                    string productId = targetProduct.ProductID;
                    uint quantity = (uint)targetProduct.Quantity;

                    uint bricks = quantity / 20;
                    uint remainder = quantity % 20;
                    uint jars = remainder / 5;
                    uint baggies = remainder % 5;
                    if (!productTotals.ContainsKey(productId))
                        productTotals[productId] = (0, 0, 0);
                    var current = productTotals[productId];
                    productTotals[productId] = (
                        current.bricks + bricks,
                        current.jars + jars,
                        current.baggies + baggies
                    );
                }
            }
            foreach (var kvp in productTotals)
            {
                string productId = kvp.Key;
                var (bricks, jars, baggies) = kvp.Value;

                string message = $"{productId}:\n" +
                                 (bricks > 0 ? $"{bricks} bricks\n" : "") +
                                 (jars > 0 ? $"{jars} jars\n" : "") +
                                 (baggies > 0 ? $"{baggies} baggies" : "");

                _dialogue.SendNPCMessage(message);
            }
            if (productTotals.Count == 0)
            {
                _dialogue.SendNPCMessage("No active deals at the moment!");
                return;
            }
            else
            {
                _dialogue.SendNPCMessage("Click button to list deals!");
            }
        }

        public static explicit operator MelonBase(DealOverview v)
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            if ((Object)(object)_npcObject != (Object)null)
            {
                Object.Destroy((Object)(object)_npcObject);
            }
            if ((Object)(object)_npc != (Object)null && NPCManager.NPCRegistry.Contains(_npc))
            {
                NPCManager.NPCRegistry.Remove(_npc);
            }
        }
    }
}
