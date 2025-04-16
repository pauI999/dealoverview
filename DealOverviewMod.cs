using System.Collections;
using MelonLoader;
using UnityEngine;

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
            MelonLogger.Msg($"Scene loaded: {sceneName}");
            if (sceneName == "Main")
            {
                MelonLogger.Msg("Starting 3s delay!");
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
            yield return new WaitForSeconds(3f);
            _dealOverview = DealOverview.Create();
        }

    }


}
