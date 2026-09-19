using UnityEditor;

namespace UwU
{
    internal class UwUBuildInit
    {
        internal static void UwUFolderSetup()
        {
            string outputFolder = "Assets/UwUTemp";
            // 1. Ensure the folder exists, then clear only its contents (leaving the folder itself)
            if (AssetDatabase.IsValidFolder(outputFolder))
            {
                string[] tempGuids = AssetDatabase.FindAssets("", new[] { outputFolder });
                foreach (string guid in tempGuids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    // Ensure we only delete assets directly inside or managed within this folder root search
                    if (!string.IsNullOrEmpty(assetPath))
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