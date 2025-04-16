using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.UI.Phone.ProductManagerApp;
using UnityEngine;

namespace DealOverviewMod
{
    public class DealOverview
    {
        private Dialogue _dialogue;

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
            NPC template = null;
            var allNpcs = NPCManager.NPCRegistry;

            for (int i = 0; i < allNpcs.Count; i++)
            {
                var npc = allNpcs[i];
                if (npc != null && npc.FirstName == "Beth")
                {
                    template = npc;
                    break;
                }
            }

            if (template == null)
            {
                return;
            }


            _npcObject = UnityEngine.Object.Instantiate(template.gameObject);
            _npc = _npcObject.GetComponent<NPC>();

            _npc.FirstName = "Deal";
            _npc.LastName = "Overview";
            _npc.BakedGUID = System.Guid.NewGuid().ToString();
            _npc.IsImportant = true;
            _npc.ConversationCanBeHidden = false;
            _npc.ShowRelationshipInfo = false;
            _npc.MSGConversation = null;
            _npc.ConversationCategories = new Il2CppSystem.Collections.Generic.List<EConversationCategory>();
            _npc.dialogueHandler = null;
            _npc.MugshotSprite = PlayerSingleton<ProductManagerApp>.Instance.AppIcon;


            _npcObject.SetActive(true);
            _npc.gameObject.SetActive(true);

            _npc.Awake();
            _npc.NetworkInitializeIfDisabled();
            NPCManager.NPCRegistry.Add(_npc);
            _npc.InitializeSaveable();


            _dialogue = new Dialogue(_npc);
            Run();
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

        public void Cleanup()
        {
            if (_npcObject != null)
                UnityEngine.Object.Destroy(_npcObject);
            if (_npc != null && NPCManager.NPCRegistry.Contains(_npc))
                NPCManager.NPCRegistry.Remove(_npc);
        }
    }
}
