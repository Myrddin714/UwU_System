using System.Collections.Generic;
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
            var descriptor = avatarGameObject.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null)
            {
                Debug.LogError("[UwU] No VRCAvatarDescriptor found on avatar!");
                return false;
            }
            
            var UwUMenus = avatarGameObject.GetComponentsInChildren<UwUMenu>(true);
            
            List<string> UwUPrefixes = new List<string>();
            
            foreach (var menuData in UwUMenus)
            {
                // Execute your custom component build-time logic
                Debug.Log($"[UwU] UwUComponent: {menuData.name} found");
                UwUMenu UwUData = UwUHelperMethods.Trim(menuData);
                UwUData.avatarName = avatarGameObject.name;
                AnimatorController UwUController;
                VRCExpressionParameters UwUExpressionParams;
                VRCExpressionsMenu UwUExpressionMenu;
                if (UwUData.namePrefix != "" && !UwUPrefixes.Contains(UwUData.namePrefix))
                {
                    UwUPrefixes.Add(UwUData.namePrefix);
                    UwUController = UwUControllerBuilder.BuildUwUController(UwUData);
                    if (UwUController != null)
                    {
                        UwUExpressionParams = UwUParameterBuilder.BuildUwUParameters(UwUData);
                        UwUExpressionMenu = UwUMenuBuilder.BuildUwUMenu(UwUData, UwUExpressionParams);
                        //UwUVRCToggleBuilder.BuildUwUToggles(menuData.gameObject, menuData);
                        UwUVRCFullControllerBuilder.BuildUwUToggles(menuData.gameObject, UwUData, UwUExpressionParams, UwUController, UwUExpressionMenu);
                    }
                    else
                        Debug.Log($"[UwU] Controller build failed");
                }
                else if (UwUPrefixes.Contains(UwUData.namePrefix))
                    Debug.LogWarning($"[UwU] Component with the prefix {UwUData.namePrefix} already exists on the avatar {UwUData.avatarName}. Skipping duplicate occurance on {menuData.name}");
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