using MelonLoader;
using HarmonyLib;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.PlayerScripts;

[assembly: MelonInfo(typeof(ExampleMod.ExampleMod), ExampleMod.BuildInfo.Name, ExampleMod.BuildInfo.Version, ExampleMod.BuildInfo.Author, ExampleMod.BuildInfo.DownloadLink)]
[assembly: MelonColor()]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ExampleMod
{
    public static class BuildInfo
    {
        public const string Name = "Example";
        public const string Description = "";
        public const string Author = "XOWithSauce";
        public const string Company = null;
        public const string Version = "1.0";
        public const string DownloadLink = null;
    }

    public class ExampleMod : MelonMod
    {
        [HarmonyPatch(typeof(Player), "ConsumeProduct")]
        public static class Player_ConsumeProduct_Patch
        {
            public static bool Prefix(Player __instance, ProductItemInstance product)
            {
                MelonLogger.Msg("Product is being consumed");
                return true;
            }
        }
    }
}
