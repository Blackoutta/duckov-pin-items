using Duckov.UI;
using HarmonyLib;
using PinItems.UI;
using UnityEngine.UI;

namespace PinItems.Patching
{
    [HarmonyPatch(typeof(ItemOperationMenu))]
    internal static class ItemOperationMenuPatches
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void InitializePostfix(ItemOperationMenu __instance, Button ___btn_Wishlist)
        {
            if (__instance == null || ___btn_Wishlist == null)
            {
                return;
            }

            PinMenuButtonController controller =
                __instance.gameObject.GetComponent<PinMenuButtonController>() ??
                __instance.gameObject.AddComponent<PinMenuButtonController>();
            controller.EnsureButton(__instance, ___btn_Wishlist);
        }

        [HarmonyPatch("Setup")]
        [HarmonyPostfix]
        private static void SetupPostfix(ItemOperationMenu __instance)
        {
            PinMenuButtonController controller = __instance.gameObject.GetComponent<PinMenuButtonController>();
            controller?.RefreshButton();
        }
    }
}
