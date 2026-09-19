using UnityEngine;
using UnityEditor.Animations;
using UwU.Resources;
using VRC.SDKBase.Editor.BuildPipeline;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace UwU
{
    internal class UWURuntimeHook : IVRCSDKPreprocessAvatarCallback
    {
        public int callbackOrder => -17140;

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            return RunUwUBuilders(avatarGameObject);
        }

        private static bool RunUwUBuilders(GameObject avatarGameObject)
        {
            #if Has_Compatible_VRCFury
            Debug.Log($"[UwU] Running preprocessing for avatar: {avatarGameObject.name}");

            var descriptor = avatarGameObject.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null)
            {
                Debug.LogError("[UwU] No VRCAvatarDescriptor found on avatar!");
                return false;
            }
            
            UwUBuildInit.UwUFolderSetup();
            
            var uwuMenus = avatarGameObject.GetComponentsInChildren<UwUMenu>(true);// ?? avatarGameObject.AddComponent<UwUMenu>();
            
            foreach (var menuData in uwuMenus)
            {
                // Execute your custom component build-time logic
                Debug.Log($"[UwU] UwUComponent: {menuData.name} found");
                UwUMenu UWUData = UwUHelperMethods.Trim(menuData);
                AnimatorController UwUController;
                VRCExpressionParameters UwUExpressionParams;
                VRCExpressionsMenu UwUExpressionMenu;
                if (UWUData.namePrefix != "")
                {
                    UwUController = UwUControllerBuilder.BuildUwUController(UWUData);
                    if (UwUController != null)
                    {
                        UwUExpressionParams = UwUParameterBuilder.BuildUwUParameters(UWUData);
                        UwUExpressionMenu = UwUMenuBuilder.BuildUwUMenu(UWUData, UwUExpressionParams);
                        //UwUVRCToggleBuilder.BuildUwUToggles(menuData.gameObject, menuData);
                        UwUVRCFullControllerBuilder.BuildUwUToggles(menuData.gameObject, UWUData, UwUExpressionParams, UwUController, UwUExpressionMenu);
                    }
                    else
                        Debug.Log($"[UwU] Controller build failed");
                }
                else
                    Debug.LogWarning($"[UwU] UwUComponent on {menuData.name} is missing a name");
            }
            #else
            Debug.LogWarning("[UwU] No Compatible VRCFury version found in project. Add the latest version of VRCFury for UwU components to work.");
            #endif
            
            return true;
        }
    }
}