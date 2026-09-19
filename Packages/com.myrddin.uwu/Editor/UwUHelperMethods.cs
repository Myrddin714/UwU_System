namespace UwU.Resources
{
    internal class UwUHelperMethods
    {
        internal static string GetMenuFolderPath(string fullFolderPath, bool createSubMenu)
        {
            fullFolderPath = fullFolderPath.Trim();
            string folderPath = "";
            if (createSubMenu)
            {
                int index = FolderIndex(fullFolderPath);
                if (index != -1)
                {
                    folderPath = fullFolderPath.Substring(0, index);
                    folderPath.Trim();
                }
                /*else
                {
                    folderPath = fullFolderPath;
                }*/
            }
            else
            {
                folderPath = fullFolderPath;
            }

            return folderPath;
        }

        internal static string GetMenuFolderName(string fullFolderPath, bool createSubMenu)
        {
            fullFolderPath = fullFolderPath.Trim();
            string folderName = "";
            
            if (createSubMenu)
            {
                int index = FolderIndex(fullFolderPath);
                if (index != -1)
                {
                    folderName = fullFolderPath.Substring(index + 1);
                    folderName.Trim();
                }
                else
                {
                    folderName = fullFolderPath;
                }
            }

            return folderName;
        }

        private static int FolderIndex(string menuPath)
        {
            int index = -1;

            if (menuPath.Length > 0 && menuPath.LastIndexOf('/') > 0)
            {
                if (menuPath.Length > 1 && !(menuPath.Substring(menuPath.LastIndexOf('/') - 1, 1).Equals("\\") || menuPath.Substring(menuPath.LastIndexOf('/') - 1, 1).Equals("<")))
                {
                    index = menuPath.LastIndexOf('/');
                }
                else if (menuPath.Substring(menuPath.LastIndexOf('/') - 1, 1).Equals("\\"))
                {
                    if (FolderIndex(menuPath.Substring(0, menuPath.LastIndexOf('/'))) != -1)
                    {
                        index = FolderIndex(menuPath.Substring(0, menuPath.LastIndexOf('/')));
                    }
                }
            }
            
            return index;
        }

        internal static UwUMenu Trim(UwUMenu UWUData)
        {
            UwUMenu UWUDataTrimmed = UWUData;
            
            UWUDataTrimmed.namePrefix = UWUData.namePrefix.Trim();
            for (int i = 0; i < UWUData.conditions.Count; i++)
            {
                UWUDataTrimmed.conditions[i].outputParameter = UWUData.conditions[i].outputParameter.Trim();
                for (int j = 0; j < UWUData.conditions[i].outputConditions.Count; j++)
                {
                    UWUDataTrimmed.conditions[i].outputConditions[j].conditionParameterName = UWUData.conditions[i].outputConditions[j].conditionParameterName.Trim();
                }
            }
            
            return UWUDataTrimmed;
        }
    }
}