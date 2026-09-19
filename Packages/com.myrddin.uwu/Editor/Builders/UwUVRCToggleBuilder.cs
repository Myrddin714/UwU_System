using UnityEngine;
using UnityEditor;
#if Has_Compatible_VRCFury
using com.vrcfury.api;
#endif

namespace UwU
{
    internal class UwUVRCToggleBuilder
    {
        #if Has_Compatible_VRCFury
        //[MenuItem("Tools/UwU/Generate Toggles")]
        //internal static void BuildUwUToggles()
        internal static void BuildUwUToggles(GameObject UwUObject, UwUMenu UwUData)
        {
            string outputFolder = "Assets/UwUTemp";
            
            //Temp Variables
            /*GameObject UwUObject = Selection.activeGameObject;
            UwUMenu UwUData = new UwUMenu()
            {
                namePrefix = "UwU",
                menuPath = "UwU",
                alwaysOnLocal = true,
                defaultVisibility = 1,
                localSaved =  true,
                friendsSaved =  true,
                globalSaved = false
            };*/
            
            string UwUControllerName = UwUData.namePrefix;
            string UwUMenuPath = UwUData.menuPath;
            
            bool doesControllerExist = true;
            string[] tempGuids = AssetDatabase.FindAssets($"{UwUControllerName} t:animatorcontroller", new[] { outputFolder });
            if (tempGuids.Length == 0)
            {
                doesControllerExist = false;
                Debug.LogWarning($"[UwU] Controller with the name {UwUControllerName} doesn't exists. Skipping toggle creation.");
            }

            if (doesControllerExist)
            {
                Debug.Log($"[UwU] Creating {UwUControllerName} Toggles");
                var localToggle = FuryComponents.CreateToggle(UwUObject);
                var friendsToggle = FuryComponents.CreateToggle(UwUObject);
                var globalToggle = FuryComponents.CreateToggle(UwUObject);

                if (UwUMenuPath != "")
                {
                    localToggle.SetMenuPath($"{UwUMenuPath}/UwU System/Toggle <color=green>Local");
                    friendsToggle.SetMenuPath($"{UwUMenuPath}/UwU System/Toggle <color=yellow>Friends-Only");
                    globalToggle.SetMenuPath($"{UwUMenuPath}/UwU System/Toggle <color=red>Everyone");
                }
                else
                {
                    localToggle.SetMenuPath($"UwU System/Toggle <color=green>Local");
                    friendsToggle.SetMenuPath($"UwU System/Toggle <color=yellow>Friends-Only");
                    globalToggle.SetMenuPath($"UwU System/Toggle <color=red>Everyone");
                }
                
                localToggle.SetGlobalParameter($"{UwUControllerName}/Local");
                //localToggle.AddExclusiveTag(UwUControllerName);
                if (UwUData.localSaved)
                    localToggle.SetSaved();
                //if (UwUData.alwaysOnLocal)
                    //localToggle.SetExclusiveOffState();
                
                friendsToggle.SetGlobalParameter($"{UwUControllerName}/Friends");
                //friendsToggle.AddExclusiveTag(UwUControllerName);
                if (UwUData.friendsSaved)
                    friendsToggle.SetSaved();
                
                globalToggle.SetGlobalParameter($"{UwUControllerName}/Global");
                //globalToggle.AddExclusiveTag(UwUControllerName);
                if (UwUData.globalSaved)
                    globalToggle.SetSaved();

                if (UwUData.allowOffLocal)
                {
                    switch (UwUData.defaultVisibility)
                    {
                        case 1:
                            localToggle.SetDefaultOn();
                            break;
                        case 2:
                            friendsToggle.SetDefaultOn();
                            break;
                        case 3:
                            globalToggle.SetDefaultOn();
                            break;
                    }
                }
                else
                {
                    switch (UwUData.defaultVisibility)
                    {
                        case 0:
                            localToggle.SetDefaultOn();
                            break;
                        case 1:
                            friendsToggle.SetDefaultOn();
                            break;
                        case 2:
                            globalToggle.SetDefaultOn();
                            break;
                    }
                }
            }
        }
        #endif
    }
}