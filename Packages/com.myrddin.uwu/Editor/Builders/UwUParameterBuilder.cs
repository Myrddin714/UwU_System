using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace UwU
{
    internal class UwUParameterBuilder
    {
        internal static VRCExpressionParameters BuildUwUParameters(UwUMenu UwUData)
        {
            string UwUControllerName = UwUData.namePrefix;
            
            // 1. Create a new instance of VRCExpressionParameters ScriptableObject
            VRCExpressionParameters UwUExpressionParams = ScriptableObject.CreateInstance<VRCExpressionParameters>();

            // 2. Define your parameters list
            List<VRCExpressionParameters.Parameter> UwUExpressionParameterList = new List<VRCExpressionParameters.Parameter>
            {
                new VRCExpressionParameters.Parameter
                {
                    name = $"{UwUControllerName}/Load",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0f,
                    saved = false,
                    networkSynced = false
                }
            };

            if (UwUData.ingameMenu)
            {
                UwUExpressionParameterList.Add(new VRCExpressionParameters.Parameter
                {
                    name = $"{UwUControllerName}/Local",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0f,
                    saved = UwUData.localSaved,
                    networkSynced = true
                });
                UwUExpressionParameterList.Add(new VRCExpressionParameters.Parameter
                {
                    name = $"{UwUControllerName}/Friends",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0f,
                    saved = UwUData.friendsSaved,
                    networkSynced = true
                });
                UwUExpressionParameterList.Add(new VRCExpressionParameters.Parameter
                {
                    name = $"{UwUControllerName}/Global",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0f,
                    saved = UwUData.globalSaved,
                    networkSynced = true
                });

                if (UwUData.allowOffLocal)
                {
                    switch (UwUData.defaultVisibility)
                    {
                        case 1:
                            UwUExpressionParameterList[1].defaultValue = 1f;
                            break;
                        case 2:
                            UwUExpressionParameterList[2].defaultValue = 1f;
                            break;
                        case 3:
                            UwUExpressionParameterList[3].defaultValue = 1f;
                            break;
                    }
                }
                else
                {
                    switch (UwUData.defaultVisibility)
                    {
                        case 0:
                            UwUExpressionParameterList[1].defaultValue = 1f;
                            break;
                        case 1:
                            UwUExpressionParameterList[2].defaultValue = 1f;
                            break;
                        case 2:
                            UwUExpressionParameterList[3].defaultValue = 1f;
                            break;
                    }
                }
            }

            foreach (var output in UwUData.conditions)
            {
                VRCExpressionParameters.Parameter newParam = new VRCExpressionParameters.Parameter
                {
                    name = output.outputParameter,
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0f,
                    saved = UwUData.globalSaved,
                    networkSynced = false
                };
                UwUExpressionParameterList.Add(newParam);
            }

            // 3. Assign the parameters array to the asset
            UwUExpressionParams.parameters = UwUExpressionParameterList.ToArray();

            /*// 4. Save the asset into your Unity project
            string outputFolder = "Assets/UwUTemp";
            string path = $"{outputFolder}/{UwUControllerName}_Parameters.asset";
            
            // Ensure unique path so we don't accidentally overwrite an existing file
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(UwUExpressionParams, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[UwU] Successfully created VRC Expression Parameters asset at: {path}");*/
            
            return UwUExpressionParams;
        }                       
    }
}