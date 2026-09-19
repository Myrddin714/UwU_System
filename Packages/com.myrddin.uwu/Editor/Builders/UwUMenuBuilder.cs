using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UwU.Resources;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace UwU
{
    internal class UwUMenuBuilder
    {
        internal static VRCExpressionsMenu BuildUwUMenu(UwUMenu UwUData, VRCExpressionParameters UwUExpressionParams)
        {
            string UwUControllerName = UwUData.namePrefix;
            string texturePath = "Packages/com.myrddin.uwu/Editor/Resources/";
            
            Texture2D menuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath + "UwU System.png");
            Texture2D localIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath + "Local.png");
            Texture2D friendsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath + "Friends Only.png");
            Texture2D globalIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath + "Global.png");

            VRCExpressionsMenu.Control.Parameter localMenuParameter = new VRCExpressionsMenu.Control.Parameter
            {
                name = $"{UwUControllerName}/Local",
            };
            VRCExpressionsMenu.Control.Parameter friendsMenuParameter = new VRCExpressionsMenu.Control.Parameter
            {
                name = $"{UwUControllerName}/Friends",
            };
            VRCExpressionsMenu.Control.Parameter globalMenuParameter = new VRCExpressionsMenu.Control.Parameter
            {
                name = $"{UwUControllerName}/Global",
            };

            List<VRCExpressionsMenu.Control> menuItems = new List<VRCExpressionsMenu.Control>();
            VRCExpressionsMenu.Control localVRCMenu = new VRCExpressionsMenu.Control
            {
                name = "Toggle <color=green>Local",
                type =  VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = localMenuParameter,
                icon = localIcon
            };
            VRCExpressionsMenu.Control friendsVRCMenu = new VRCExpressionsMenu.Control
            {
                name = "Toggle <color=yellow>Friends-Only",
                type =  VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = friendsMenuParameter,
                icon = friendsIcon
            };
            VRCExpressionsMenu.Control globalVRCMenu = new VRCExpressionsMenu.Control
            {
                name = "Toggle <color=red>Everyone",
                type =  VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = globalMenuParameter,
                icon = globalIcon
            };
            menuItems.Add(localVRCMenu);
            menuItems.Add(friendsVRCMenu);
            menuItems.Add(globalVRCMenu);

            VRCExpressionsMenu UwUSubExpressionsMenu = ScriptableObject.CreateInstance(typeof(VRCExpressionsMenu)) as VRCExpressionsMenu;
            UwUSubExpressionsMenu.Parameters = UwUExpressionParams;
            UwUSubExpressionsMenu.controls = menuItems;

            if (UwUData.createSubMenu)
            {
                List<VRCExpressionsMenu.Control> rootMenuItems = new List<VRCExpressionsMenu.Control>();
                VRCExpressionsMenu.Control subVRCMenu = new VRCExpressionsMenu.Control
                {
                    name = UwUHelperMethods.GetMenuFolderName(UwUData.menuPath, UwUData.createSubMenu),
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = UwUSubExpressionsMenu,
                    icon = menuIcon
                };
                rootMenuItems.Add(subVRCMenu);

                VRCExpressionsMenu UwURootExpressionsMenu = ScriptableObject.CreateInstance(typeof(VRCExpressionsMenu)) as VRCExpressionsMenu;
                UwURootExpressionsMenu.Parameters = UwUExpressionParams;
                UwURootExpressionsMenu.controls = rootMenuItems;
                return UwURootExpressionsMenu;
            }
            else
            {
                return UwUSubExpressionsMenu;
            }
            
        }
    }
}