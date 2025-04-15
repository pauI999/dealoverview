using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.UI.Phone.Messages;
using MelonLoader;
using Il2CppSystem.Collections.Generic;
using static Il2CppScheduleOne.Messaging.Message;
using Il2CppSystem.Collections;
using UnityEngine;

namespace DealOverviewMod
{
    internal class TestDialogue
    {
        private readonly NPC _npc;

        private readonly MSGConversation _conversation;

        public TestDialogue(NPC npc)
        {
            //IL_0015: Unknown result type (might be due to invalid IL or missing references)
            //IL_001f: Expected O, but got Unknown
            _npc = npc;
            _conversation = new MSGConversation(npc, npc.fullName);
            _npc.MSGConversation = _conversation;
            _conversation.messageHistory = new Il2CppSystem.Collections.Generic.List<Message>();
            _conversation.messageChainHistory = new Il2CppSystem.Collections.Generic.List<MessageChain>();
            _conversation.bubbles = new Il2CppSystem.Collections.Generic.List<MessageBubble>();
            _conversation.EntryVisible = true;
            if (_npc.ConversationCategories == null)
            {
                _npc.ConversationCategories = new Il2CppSystem.Collections.Generic.List<EConversationCategory>();
                _npc.ConversationCategories.Add((EConversationCategory)0);
            }
            _conversation.SetCategories(_npc.ConversationCategories);
        }

        public void SendNPCMessage(string text)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Expected O, but got Unknown
            Message val = new Message(text, (ESenderType)1, true, -1);
            _conversation.SendMessage(val, true, false);
        }

        public void SendPlayerMessage(string text)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Expected O, but got Unknown
            Message val = new Message(text, (ESenderType)0, true, -1);
            _conversation.SendMessage(val, true, false);
        }
        private IEnumerator DelayedShowResponses(float delay, DialogueResponse[] responses)
        {
            return (IEnumerator)CoroutineWrapper(delay, responses);
        }

        private System.Collections.IEnumerator CoroutineWrapper(float delay, DialogueResponse[] responses)
        {
            yield return new WaitForSeconds(delay);
            ShowResponses(responses, delay);
        }


        public void ShowResponses(DialogueResponse[] responses, float v)
        {
            //IL_006a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0074: Expected O, but got Unknown
            Il2CppSystem.Collections.Generic.List<Response> val = new Il2CppSystem.Collections.Generic.List<Response>();
            foreach (DialogueResponse response in responses)
            {
                Action action = response.Action;
                Il2CppSystem.Action action2 = new Action(() =>
                {
                    response.Action?.Invoke();
                    if (response.NextResponses != null && response.NextResponses.Length != 0)
                    {
                        MelonCoroutines.Start((System.Collections.IEnumerator)DelayedShowResponses(0.5f, response.NextResponses));
                    }
                });


                val.Add(new Response(response.Id, response.Text, action2, response.DisableDefaultBehavior));
            }
            _conversation.ClearResponses(false);
            _conversation.ShowResponses(val, 0f, false);
        }
    }
}