using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEditor.Animations;
#if Has_Compatible_VRCFury
using com.vrcfury.api;
using UwU.Resources;
#endif

namespace UwU
{
    internal class UwUVRCFullControllerBuilder
    {
        #if Has_Compatible_VRCFury
        internal static void BuildUwUToggles(GameObject UwUObject, UwUMenu UwUData, VRCExpressionParameters UwUExpressionParameters, AnimatorController UwUController, VRCExpressionsMenu UwUExpressionsMenu)
        {
            var UwUVRCFController = FuryComponents.CreateFullController(UwUObject);
            string namePrefix = UwUData.namePrefix;
            string menuPath = UwUHelperMethods.GetMenuFolderPath(UwUData.menuPath, UwUData.createSubMenu);
            UwUVRCFController.AddController(UwUController);
            UwUVRCFController.AddParams(UwUExpressionParameters);
            UwUVRCFController.AddGlobalParam($"{namePrefix}/Load");
            //UwUVRCFController.AddGlobalParam("*");
            foreach (var globalParameter in UwUData.globalParams)
            {
                UwUVRCFController.AddGlobalParam(globalParameter);
            }
            if (UwUData.ingameMenu)
                UwUVRCFController.AddMenu(UwUExpressionsMenu, menuPath);
        }
        #endif
    }
}