using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UwU.Resources;
using System;
using DG.DOTweenEditor.UI;

namespace UwU
{
    [CustomEditor(typeof(UwUMenu))]
    internal class UwUComponentEditor : Editor
    {
        private SerializedProperty namePrefix;
        private SerializedProperty thresholdState;
        private SerializedProperty menuPath;
        private SerializedProperty ingameMenu;
        private SerializedProperty allowOffLocal;
        private SerializedProperty defaultVisibility;
        private SerializedProperty localSaved;
        private SerializedProperty friendsSaved;
        private SerializedProperty globalSaved;
        private ReorderableList outerList;
        private Dictionary<string, ReorderableList> innerLists = new Dictionary<string, ReorderableList>();

        private void OnEnable()
        {
            namePrefix = serializedObject.FindProperty("namePrefix");
            thresholdState = serializedObject.FindProperty("thresholdState");
            menuPath = serializedObject.FindProperty("menuPath");
            ingameMenu = serializedObject.FindProperty("ingameMenu");
            allowOffLocal = serializedObject.FindProperty("allowOffLocal");
            defaultVisibility = serializedObject.FindProperty("defaultVisibility");
            localSaved = serializedObject.FindProperty("localSaved");
            friendsSaved  = serializedObject.FindProperty("friendsSaved");
            globalSaved = serializedObject.FindProperty("globalSaved");
            SerializedProperty conditions = serializedObject.FindProperty("conditions");

            outerList = new ReorderableList(serializedObject, conditions, true, true, true, true);

            outerList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Additional Output Parameter & Conditions");
            };

            outerList.elementHeightCallback = (int index) => {
                SerializedProperty element = conditions.GetArrayElementAtIndex(index);
                string key = element.propertyPath;

                ReorderableList innerList = GetInnerList(element, key);

                return (EditorGUIUtility.singleLineHeight * 2) + innerList.GetHeight() + 14;
            };

            outerList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = conditions.GetArrayElementAtIndex(index);
                SerializedProperty outputParameter = element.FindPropertyRelative("outputParameter");
                SerializedProperty conditionState = element.FindPropertyRelative("conditionState");

                rect.y += 4;
                Rect titleRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(titleRect, outputParameter, new GUIContent($"Custom  Output {(index) + 1} Name:"));
                
                rect.y += EditorGUIUtility.singleLineHeight + 4;
                Rect conditionStateRectLabel = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                Rect conditionStateRect = new Rect(conditionStateRectLabel.xMax + 1, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                //EditorGUI.PropertyField(conditionStateRect, conditionState,new GUIContent("Condition State:"));
                EditorGUI.LabelField(conditionStateRectLabel, "Condition State:");
                string[] conditionStateText = {"True", "False"};
                conditionState.intValue = EditorGUI.Popup(conditionStateRect, conditionState.intValue, conditionStateText);

                rect.y += EditorGUIUtility.singleLineHeight + 4;
            
                string key = element.propertyPath;
                ReorderableList innerList = GetInnerList(element, key);

                Rect innerListRect = new Rect(rect.x, rect.y, rect.width, innerList.GetHeight());
                innerList.DoList(innerListRect);
            };
            

            outerList.onAddCallback = (ReorderableList l) =>
            {
                // 1. Expand the array size by 1
                conditions.arraySize++;
                l.index = conditions.arraySize - 1;

                // 2. Grab the newly created element
                SerializedProperty newElement = conditions.GetArrayElementAtIndex(l.index);

                // 3. Reset/clear its fields to default values instead of keeping the duplicate
                SerializedProperty outputName = newElement.FindPropertyRelative("outputParameter");
                if (outputName != null)
                    outputName.stringValue = "";
                
                SerializedProperty conditionState = newElement.FindPropertyRelative("conditionState");
                if (conditionState != null)
                    conditionState.intValue = 0;

                SerializedProperty valueProp = newElement.FindPropertyRelative("outputConditions");
                if (valueProp != null)
                    valueProp.arraySize = 0;

                // 4. Apply changes to the serialized object
                serializedObject.ApplyModifiedProperties();
            };
        }

        private ReorderableList GetInnerList(SerializedProperty elementProp, string key)
        {
            SerializedProperty childItemsProp = elementProp.FindPropertyRelative("outputConditions");
            SerializedProperty outputParameter = elementProp.FindPropertyRelative("outputParameter");

            if (!innerLists.TryGetValue(key, out ReorderableList innerList))
            {
                innerList = new ReorderableList(childItemsProp.serializedObject, childItemsProp, true, true, true, true);
            
                innerList.drawHeaderCallback = (Rect innerRect) => {
                    string headerText = string.IsNullOrEmpty(outputParameter.stringValue) ? "Conditions" : outputParameter.stringValue + " Conditions";
                    EditorGUI.LabelField(innerRect, headerText);
                };
            
                innerList.drawElementCallback = (Rect innerRect, int innerIndex, bool innerActive, bool innerFocused) => {
                    SerializedProperty itemProp = childItemsProp.GetArrayElementAtIndex(innerIndex);
                    SerializedProperty nameProp = itemProp.FindPropertyRelative("conditionParameterName");
                    SerializedProperty typeProp = itemProp.FindPropertyRelative("conditionParameterType");
                    SerializedProperty boolProp = itemProp.FindPropertyRelative("boolValue");
                    SerializedProperty intProp = itemProp.FindPropertyRelative("intValue");
                    SerializedProperty floatProp = itemProp.FindPropertyRelative("floatValue");
                    SerializedProperty conditionLogic = itemProp.FindPropertyRelative("conditionLogic");
                    string[] boolLogicText = {"True", "False"};
                    string[] intLogicText = {"Greater", "Less", "Equals", "NotEquals"};
                    string[] floatLogicText = {"Greater", "Less"};

                    innerRect.y += 2;
                    float width = innerRect.width;
                    
                    float nameWidth = width * 0.4f;
                    float typeWidth = width * 0.20f;
                    float logWidth = width * 0.25f;
                    float valWidth = width - nameWidth - typeWidth - logWidth - 15;
                    
                    float logWidthOnly = width - nameWidth - typeWidth - 10;

                    Rect nameRect = new Rect(innerRect.x, innerRect.y, nameWidth, EditorGUIUtility.singleLineHeight);
                    Rect typeRect = new Rect(nameRect.xMax + 5, innerRect.y, typeWidth, EditorGUIUtility.singleLineHeight);
                    Rect logRect = new Rect(typeRect.xMax + 5, innerRect.y, logWidth, EditorGUIUtility.singleLineHeight);
                    Rect valRect = new Rect(logRect.xMax + 5, innerRect.y, valWidth, EditorGUIUtility.singleLineHeight);
                    
                    Rect logRectOnly = new Rect(typeRect.xMax + 5, innerRect.y, logWidthOnly, EditorGUIUtility.singleLineHeight);
                    
                    EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
                    EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);

                    ConditionType entryType = (ConditionType)typeProp.enumValueIndex;
                    switch (entryType)
                    {
                        case ConditionType.Bool:
                            conditionLogic.intValue = Mathf.Clamp(conditionLogic.intValue, 0, boolLogicText.Length - 1);
                            conditionLogic.intValue = EditorGUI.Popup(logRectOnly, conditionLogic.intValue, boolLogicText);
                            boolProp.boolValue = (conditionLogic.intValue == 0);
                            break;
                        case ConditionType.Int:
                            conditionLogic.intValue = Mathf.Clamp(conditionLogic.intValue, 0, intLogicText.Length - 1);
                            conditionLogic.intValue = EditorGUI.Popup(logRect, conditionLogic.intValue, intLogicText);
                            EditorGUI.PropertyField(valRect, intProp, GUIContent.none);
                            break;
                        case ConditionType.Float:
                            conditionLogic.intValue = Mathf.Clamp(conditionLogic.intValue, 0, floatLogicText.Length - 1);
                            conditionLogic.intValue = EditorGUI.Popup(logRect, conditionLogic.intValue, floatLogicText);
                            EditorGUI.PropertyField(valRect, floatProp, GUIContent.none);
                            break;
                    }
                };

                innerList.elementHeightCallback = (int innerIndex) => {
                    return EditorGUIUtility.singleLineHeight + 6;
                };

                innerLists[key] = innerList;
            }
            else
            {
                innerList.serializedProperty = childItemsProp;
            }

            return innerList;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GUIStyle redLabelStyle = new GUIStyle(EditorStyles.label);
            redLabelStyle.normal.textColor = Color.red;
            redLabelStyle.alignment = TextAnchor.MiddleCenter;
            
            #if !Has_Compatible_VRCFury
                EditorGUILayout.LabelField("VRCFury not detected or too out of date.", redLabelStyle);
                EditorGUILayout.LabelField("Make sure you have the most recent version of VRCFury.", redLabelStyle);
                EditorGUILayout.Space();
            #endif
            
            string namePrefix = serializedObject.FindProperty("namePrefix").stringValue.Trim();
            
            if (!namePrefix.Equals(""))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("namePrefix"),new GUIContent("Output Prefix (Required):"));
                EditorGUILayout.LabelField("Default Output Variable: " + namePrefix + "/Load");
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("namePrefix"),new GUIContent("Output Prefix (Required):"));
                EditorGUILayout.LabelField("Output Prefix is Missing. This component will do nothing.", redLabelStyle);
            }

            string[] thresholdStateText = new string[] { "True", "False"};
            thresholdState.intValue = EditorGUILayout.Popup("Threshold State:", thresholdState.intValue, thresholdStateText);
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ingameMenu"),new GUIContent("Add In-Game Menu:"));
            EditorGUILayout.Space();

            bool previousAllowOffLocal = allowOffLocal.boolValue;
            if (ingameMenu.boolValue)
            {
                string allowLocalText;
                if (thresholdState.intValue == 0)
                {
                    allowLocalText = "Allow False Locally:";
                }
                else
                {
                    allowLocalText = "Allow True Locally:";
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowOffLocal"),new GUIContent(allowLocalText));
                bool currentAllowOfLocal = allowOffLocal.boolValue;
                
                DropdownStateFix(previousAllowOffLocal, currentAllowOfLocal);
                
                string[] defaultVisibilityText;
                if (allowOffLocal.boolValue)
                {
                    // 4 entries when NOT checked (Off is included at index 0)
                    defaultVisibilityText = new string[] { "None", "Local Only", "Friends and Local Only", "Everyone" };
                }
                else
                {
                    // 3 entries when checked (Off is omitted)
                    defaultVisibilityText = new string[] { "Local Only", "Friends and Local Only", "Everyone" };
                }

                // Ensure index stays within bounds if toggled
                if (defaultVisibility.intValue >= defaultVisibilityText.Length)
                {
                    defaultVisibility.intValue = defaultVisibilityText.Length - 1;
                }

                // Ensure index stays within bounds safely
                defaultVisibility.intValue = Mathf.Clamp(defaultVisibility.intValue, 0, defaultVisibilityText.Length - 1);

                // Render the popup dropdown
                defaultVisibility.intValue = EditorGUILayout.Popup("Visibility Threshold:", defaultVisibility.intValue, defaultVisibilityText);
                
                EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("createSubMenu").boolValue, "'Create Folder' should only be off if a menu at the path below already exists"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("menuPath"),new GUIContent("Menu Path:"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("createSubMenu"),new GUIContent("Create Folder:"));
                //EditorGUILayout.LabelField("Menu Path: " + UwUHelperMethods.GetMenuFolderPath(serializedObject.FindProperty("menuPath").stringValue, serializedObject.FindProperty("createSubMenu").boolValue));
                //EditorGUILayout.LabelField("Folder Name: " + UwUHelperMethods.GetMenuFolderName(serializedObject.FindProperty("menuPath").stringValue, serializedObject.FindProperty("createSubMenu").boolValue));
                EditorGUILayout.Space();

                if (!currentAllowOfLocal)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("localSaved"),new GUIContent("Local Toggle Saved:"));
                    EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("localSaved").boolValue, "Will Revert to 'Local Only'"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("friendsSaved"),new GUIContent("Friends Toggle Saved:"));
                    EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("friendsSaved").boolValue, "Will Revert to 'Local Only'"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("globalSaved"),new GUIContent("Global Toggle Saved:"));
                    EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("globalSaved").boolValue, "Will Revert to 'Local Only'"));
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("localSaved"),new GUIContent("Local Toggle Saved:"));
                    EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("localSaved").boolValue, "Will Revert to 'None'"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("friendsSaved"),new GUIContent("Friends Toggle Saved:"));
                    EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("friendsSaved").boolValue, "Will Revert to 'None'"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("globalSaved"),new GUIContent("Global Toggle Saved:"));
                    EditorGUILayout.LabelField(ToggleOffLabel(serializedObject.FindProperty("globalSaved").boolValue, "Will Revert to 'None'"));
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                allowOffLocal.boolValue = false;
                bool currentAlwaysOnLocal = allowOffLocal.boolValue;

                DropdownStateFix(previousAllowOffLocal, currentAlwaysOnLocal);
                
                // 3 entries (Off is omitted)
                string[] defaultVisibilityText = { "Local Only", "Friends and Local Only", "Everyone"};
                
                // Ensure index stays within bounds if toggled
                if (defaultVisibility.intValue >= defaultVisibilityText.Length)
                {
                    defaultVisibility.intValue = defaultVisibilityText.Length - 1;
                }
                
                defaultVisibility.intValue = EditorGUILayout.Popup("Visibility:", defaultVisibility.intValue, defaultVisibilityText);
                
            }
            EditorGUILayout.Space();
            
            outerList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DropdownStateFix(bool previousAllowOffLocal, bool currentAllowOffLocal)
        {
            if (previousAllowOffLocal != currentAllowOffLocal)
            {
                if (currentAllowOffLocal)
                {
                    defaultVisibility.intValue += 1;
                }
                else
                {
                    defaultVisibility.intValue -= 1;
                }
            }
        }

        private string ToggleOffLabel(bool toggleSaved, string offMessage)
        {
            string toggleLabel = "";

            if (!toggleSaved)
            {
                toggleLabel = offMessage;
            }

            return toggleLabel;
        }
    }
}