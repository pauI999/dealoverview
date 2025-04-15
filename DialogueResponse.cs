namespace DealOverviewMod
{
    public class DialogueResponse
    {
        public string Id;

        public string Text;

        public Action Action;

        public DialogueResponse[] NextResponses;

        public bool DisableDefaultBehavior;

        public DialogueResponse(string id, string text, Action action, bool disableDefaultBehavior = true, DialogueResponse[] next = null)
        {
            Id = id;
            Text = text;
            Action = action;
            DisableDefaultBehavior = disableDefaultBehavior;
            NextResponses = next;
        }
    }
}