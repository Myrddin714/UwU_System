using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace UwU
{
    internal class UwUBuildInit
    {
        internal static void UwUFolderSetup(string avatarName)
        {
            string outputFolder = "Assets/UwUTemp";
            // 1. Ensure the folder exists, then clear only its contents (leaving the folder itself)
            if (AssetDatabase.IsValidFolder(outputFolder))
            {
                string[] tempGuids = AssetDatabase.FindAssets("", new[] { outputFolder });
                List<string> matchingGuids = new List<string>();
                foreach (string guid in tempGuids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = Path.GetFileNameWithoutExtension(assetPath);
                    if (fileName.EndsWith(avatarName))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
                AssetDatabase.Refresh();
            }
            else
            {
                AssetDatabase.CreateFolder("Assets", "UwUTemp");
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}