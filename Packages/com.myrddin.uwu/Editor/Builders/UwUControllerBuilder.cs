using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using System.Collections.Generic;
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;

namespace UwU
{
    internal class UwUControllerBuilder
    {
        //[MenuItem("Tools/UwU/Generate Controller")]
        //internal static void BuildUwUController()
        internal static AnimatorController BuildUwUController(UwUMenu UwUData)
        {
            string outputFolder = "Assets/UwUTemp";
            
            // Temp Variables
            //string UwUControllerName = "UwU";
            //string UwUOutputVariable = $"{UwUControllerName}/Load";
            
            string namePrefix = UwUData.namePrefix;
            string UwUOutputVariable = $"{namePrefix}/Load";
            bool thresholdState = UwUData.thresholdState == 0;
            
            // Paths for assets inside the clean folder
            string UwUControllerPath = $"{outputFolder}/{namePrefix}.controller";

            bool doesControllerExist = false;
            string[] tempGuids = AssetDatabase.FindAssets($"{namePrefix}.controller t:animatorcontroller", new[] { outputFolder });
            if (tempGuids.Length > 0)
            {
                List<string> exactMatchGuids = new List<string>();
                foreach (string guid in tempGuids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = Path.GetFileNameWithoutExtension(assetPath);
                    
                    if (fileName == namePrefix) 
                    {
                        exactMatchGuids.Add(guid);
                    }
                }

                if (exactMatchGuids.Count > 0)
                {
                    doesControllerExist = true;
                    Debug.LogWarning($"[UwU] Component with the name {namePrefix} already exists. Skipping duplicate occurance");
                }
            }

            if (!doesControllerExist)
            {
                // 2. Create the Animator Controller asset
                AnimatorController UwUController = AnimatorController.CreateAnimatorControllerAtPath(UwUControllerPath);
                
                // 3. Add Parameters
                var UwUAlwaysOnParameter = new AnimatorControllerParameter
                {
                    name = $"{namePrefix}/AlwaysOn",
                    type = AnimatorControllerParameterType.Float,
                    defaultFloat = 1f // Sets default value to 1
                };
                UwUController.AddParameter(UwUAlwaysOnParameter);
                UwUController.AddParameter("IsLocal", AnimatorControllerParameterType.Float);
                UwUController.AddParameter("IsOnFriendsList", AnimatorControllerParameterType.Float);
                if (UwUData.ingameMenu)
                {
                    UwUController.AddParameter($"{namePrefix}/Local", AnimatorControllerParameterType.Float);
                    UwUController.AddParameter($"{namePrefix}/Friends", AnimatorControllerParameterType.Float);
                    UwUController.AddParameter($"{namePrefix}/Global", AnimatorControllerParameterType.Float);
                }
                UwUController.AddParameter(UwUOutputVariable, AnimatorControllerParameterType.Float);
                
                // 4. Initialize Root State Machine & Root Direct Blend Tree
                //AnimatorStateMachine rootStateMachine = UwUController.layers[0].stateMachine;
                BlendTree UwURootDirectTree;
                UwUController.CreateBlendTreeInController($"{namePrefix}", out UwURootDirectTree, 0);
                
                UwURootDirectTree.blendType = BlendTreeType.Direct;
                UwURootDirectTree.name = $"{namePrefix}_Root";
                
                // 5. Programmatically generate an AAP Animation Clip (sets AAP value to 1)
                AnimationClip UwUOutputFalseClip;
                AnimationClip UwUOutputTrueClip;
                if (thresholdState)
                {
                    UwUOutputFalseClip = CreateAAPClip(UwUOutputVariable, 0.0f);
                    UwUOutputFalseClip.name = $"{UwUOutputVariable}_False";
                    AssetDatabase.AddObjectToAsset(UwUOutputFalseClip, UwUController);
                    UwUOutputTrueClip = CreateAAPClip(UwUOutputVariable, 1.0f);
                    UwUOutputTrueClip.name = $"{UwUOutputVariable}_True";
                    AssetDatabase.AddObjectToAsset(UwUOutputTrueClip, UwUController);
                }
                else
                {
                    UwUOutputFalseClip = CreateAAPClip(UwUOutputVariable, 1.0f);
                    UwUOutputFalseClip.name = $"{UwUOutputVariable}_False";
                    AssetDatabase.AddObjectToAsset(UwUOutputFalseClip, UwUController);
                    UwUOutputTrueClip = CreateAAPClip(UwUOutputVariable, 0.0f);
                    UwUOutputTrueClip.name = $"{UwUOutputVariable}_True";
                    AssetDatabase.AddObjectToAsset(UwUOutputTrueClip, UwUController);
                }
                

                if (UwUData.ingameMenu)
                {
                    BlendTree UwUIsLocalTree = CreateUwUBlendTree("IsLocal", "IsLocal");
                    AssetDatabase.AddObjectToAsset(UwUIsLocalTree, UwUController);
                    UwURootDirectTree.AddChild(UwUIsLocalTree);
                    BlendTree UwUIsOnFriendsListTree = CreateUwUBlendTree("IsOnFriendsList", "IsOnFriendsList");
                    AssetDatabase.AddObjectToAsset(UwUIsOnFriendsListTree, UwUController);
                    BlendTree UwULocalTree = CreateUwUBlendTree($"{namePrefix}/Local", $"{namePrefix}/Local");
                    AssetDatabase.AddObjectToAsset(UwULocalTree, UwUController);
                    BlendTree UwUFriendsTree = CreateUwUBlendTree($"{namePrefix}/Friends", $"{namePrefix}/Friends");
                    AssetDatabase.AddObjectToAsset(UwUFriendsTree, UwUController);
                    BlendTree UwUGlobalTree = CreateUwUBlendTree($"{namePrefix}/Global", $"{namePrefix}/Global");
                    AssetDatabase.AddObjectToAsset(UwUGlobalTree, UwUController);
                
                    UwUIsLocalTree.AddChild(UwUIsOnFriendsListTree, 0f);
                    UwUIsOnFriendsListTree.AddChild(UwUGlobalTree,0f);
                    UwUGlobalTree.AddChild(UwUOutputFalseClip, 0f);
                    UwUGlobalTree.AddChild(UwUOutputTrueClip, 1f);
                    UwUIsOnFriendsListTree.AddChild(UwUFriendsTree, 1f);
                    UwUFriendsTree.AddChild(UwUGlobalTree, 0f);
                    UwUFriendsTree.AddChild(UwUOutputTrueClip, 1f);
                    UwUIsLocalTree.AddChild(UwULocalTree, 1f);
                    UwULocalTree.AddChild(UwUFriendsTree, 0f);
                    UwULocalTree.AddChild(UwUOutputTrueClip, 1f);
                
                    CreateUwUToggleExclusiveLogic(UwUData, UwUController);
                
                    EditorUtility.SetDirty(UwUController);
                    AssetDatabase.SaveAssets();
                
                    if (!UwUData.allowOffLocal)
                        CreateUwUToggleAlwaysOnLocalLogic(UwUData, UwUController);
                
                    EditorUtility.SetDirty(UwUController);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    switch (UwUData.defaultVisibility)
                    {
                        case 0:
                            BlendTree UwUIsLocalTreeLocal = CreateUwUBlendTree("IsLocal", "IsLocal");
                            AssetDatabase.AddObjectToAsset(UwUIsLocalTreeLocal, UwUController);
                            UwURootDirectTree.AddChild(UwUIsLocalTreeLocal);
                            UwUIsLocalTreeLocal.AddChild(UwUOutputFalseClip, 0f);
                            UwUIsLocalTreeLocal.AddChild(UwUOutputTrueClip, 1f);
                            EditorUtility.SetDirty(UwUController);
                            AssetDatabase.SaveAssets();
                            break;
                        case 1:
                            BlendTree UwUIsLocalTreeFriend = CreateUwUBlendTree("IsLocal", "IsLocal");
                            AssetDatabase.AddObjectToAsset(UwUIsLocalTreeFriend, UwUController);
                            UwURootDirectTree.AddChild(UwUIsLocalTreeFriend);
                            BlendTree UwUIsOnFriendsListTree = CreateUwUBlendTree("IsOnFriendsList", "IsOnFriendsList");
                            AssetDatabase.AddObjectToAsset(UwUIsOnFriendsListTree, UwUController);
                            UwUIsLocalTreeFriend.AddChild(UwUIsOnFriendsListTree, 0f);
                            UwUIsLocalTreeFriend.AddChild(UwUOutputTrueClip, 1f);
                            UwUIsOnFriendsListTree.AddChild(UwUOutputFalseClip, 0f);
                            UwUIsOnFriendsListTree.AddChild(UwUOutputTrueClip, 1f);
                            EditorUtility.SetDirty(UwUController);
                            AssetDatabase.SaveAssets();
                            break;
                        case 2:
                            UwURootDirectTree.AddChild(UwUOutputTrueClip, 1f);
                            EditorUtility.SetDirty(UwUController);
                            AssetDatabase.SaveAssets();
                            break;
                    }
                }

                if (UwUData.conditions.Count > 0) 
                    CreateUwUOutputBranches(UwUData, UwUController, UwURootDirectTree, UwUOutputVariable);
                
                ChildMotion[] UwURootChildren = UwURootDirectTree.children;
                for (int i = 0; i < UwURootChildren.Length; i++)
                {
                    UwURootChildren[i].directBlendParameter = UwUAlwaysOnParameter.name;
                }
                UwURootDirectTree.children = UwURootChildren;
                
                EditorUtility.SetDirty(UwUController);
                AssetDatabase.SaveAssets();
                
                return UwUController;
            }
            else
            {
                return null;
            }
        }

        private static void CreateUwUOutputBranches(UwUMenu UwUData, AnimatorController UwUController, BlendTree UwURootDirectTree, string UwUOutputVariable)
        {
            List<UwUOutput> conditions = UwUData.conditions;
            
            string[] UwUConditionalOutputs = new string[conditions.Count];
            bool[] UwUOutputConditionState = new bool[conditions.Count];
            List<UwUOutputCondition>[] UwUConditionalConditions = new List<UwUOutputCondition>[conditions.Count];
            
            if (UwUConditionalOutputs.Length > 0)
            {
                for (int i = 0; i < UwUConditionalOutputs.Length; i++)
                {
                    UwUConditionalOutputs[i] = conditions[i].outputParameter;
                    UwUOutputConditionState[i] = conditions[i].conditionState == 0;
                    if (conditions[i].outputConditions.Count != 0)
                        UwUConditionalConditions[i] = conditions[i].outputConditions;
                    if (UwUConditionalOutputs[i] != "")
                    {
                        UwUData.globalParams = AddGlobalParameter(UwUData.globalParams, UwUConditionalOutputs[i]);
                        UwUController.AddParameter(UwUConditionalOutputs[i], AnimatorControllerParameterType.Float);
                        AnimationClip UwUConditionalOutputFalseClip;
                        AnimationClip UwUConditionalOutputTrueClip;
                        if (UwUOutputConditionState[i])
                        {
                            UwUConditionalOutputFalseClip = CreateAAPClip($"{UwUConditionalOutputs[i]}", 0.0f); 
                            UwUConditionalOutputFalseClip.name = $"{UwUConditionalOutputs[i]}_False";
                            AssetDatabase.AddObjectToAsset(UwUConditionalOutputFalseClip, UwUController);
                            UwUConditionalOutputTrueClip = CreateAAPClip($"{UwUConditionalOutputs[i]}", 1.0f);
                            UwUConditionalOutputTrueClip.name = $"{UwUConditionalOutputs[i]}_True";
                            AssetDatabase.AddObjectToAsset(UwUConditionalOutputTrueClip, UwUController);
                        }
                        else
                        {
                            UwUConditionalOutputFalseClip = CreateAAPClip($"{UwUConditionalOutputs[i]}", 1.0f); 
                            UwUConditionalOutputFalseClip.name = $"{UwUConditionalOutputs[i]}_False";
                            AssetDatabase.AddObjectToAsset(UwUConditionalOutputFalseClip, UwUController);
                            UwUConditionalOutputTrueClip = CreateAAPClip($"{UwUConditionalOutputs[i]}", 0.0f);
                            UwUConditionalOutputTrueClip.name = $"{UwUConditionalOutputs[i]}_True";
                            AssetDatabase.AddObjectToAsset(UwUConditionalOutputTrueClip, UwUController);
                        }
                        
                        
                        BlendTree UwUConditionalOutputVariableTree = CreateUwUBlendTree($"{UwUConditionalOutputs[i]}_{UwUOutputVariable}", $"{UwUOutputVariable}");
                        AssetDatabase.AddObjectToAsset(UwUConditionalOutputVariableTree, UwUController);
                        
                        UwURootDirectTree.AddChild(UwUConditionalOutputVariableTree);

                        if (UwUConditionalConditions[i] != null)
                        {
                            CreateOutputConditionTree(UwUData.globalParams, UwUController, UwUConditionalOutputVariableTree, UwUConditionalConditions[i], UwUConditionalOutputFalseClip, UwUConditionalOutputTrueClip, true);
                        }
                        else
                        {
                            UwUConditionalOutputVariableTree.AddChild(UwUConditionalOutputFalseClip, 0f);
                            UwUConditionalOutputVariableTree.AddChild(UwUConditionalOutputTrueClip, 1f);
                        }
                    }
                }
            }
            
            // Save assets
            EditorUtility.SetDirty(UwUController);
            AssetDatabase.SaveAssets();
        }

        private static void CreateOutputConditionTree(List<string> globalParameters, AnimatorController UwUController, BlendTree parentTree, List<UwUOutputCondition> outputConditions,AnimationClip UwUConditionalOutputFalseClip, AnimationClip UwUConditionalOutputTrueClip, bool parentOutputReversed)
        {
            int index = 0;
            string conditionParameterName = outputConditions[index].conditionParameterName;
            string conditionParameterLogicName = conditionParameterName + "_Logic";
            bool hasDuplicates = outputConditions.Count(x => x.conditionParameterName == conditionParameterName) > 1;
            if (outputConditions[index].conditionParameterName != "")
            {
                globalParameters = AddGlobalParameter(globalParameters, conditionParameterName);
                bool boolValue = outputConditions[index].boolValue;
                int intValue = outputConditions[index].intValue;
                float floatValue = outputConditions[index].floatValue;
                int conditionLogic = outputConditions[index].conditionLogic;
                bool parameterExists = false;
                foreach (var parameter in UwUController.parameters)
                {
                    if (parameter.name == conditionParameterName)
                        parameterExists = true;
                }
                bool logicParameterExists = false;

                float parentFalseThreshold = 0f;
                float parentTrueThreshold = 1f;
                if (parentOutputReversed)
                {
                    parentFalseThreshold = 1f;
                    parentTrueThreshold = 0f;
                }
                float falseThreshold = 0f;
                float trueThreshold = 1f;
                
                int layerIndex;
                switch (outputConditions[index].conditionParameterType)
                { 
                    case ConditionType.Bool:
                        conditionParameterLogicName = conditionParameterName;
                        boolValue = (conditionLogic == 0);
                        if (!parameterExists)
                            UwUController.AddParameter(conditionParameterName,
                                AnimatorControllerParameterType.Float);
                        if (!boolValue)
                        {
                            falseThreshold = 1f;
                            trueThreshold = 0f;
                        }
                        break;
                    case ConditionType.Int:
                        layerIndex = AddConditionLayer(UwUController, conditionParameterName);
                        conditionParameterLogicName += "_" + layerIndex;
                        foreach (var parameter in UwUController.parameters)
                        {
                            if (parameter.name == conditionParameterLogicName)
                                logicParameterExists = true;
                        }

                        if (!logicParameterExists)
                        {
                            UwUController.AddParameter(conditionParameterLogicName, AnimatorControllerParameterType.Float);
                        }
                            
                        if (!parameterExists)
                            UwUController.AddParameter(conditionParameterName,
                                AnimatorControllerParameterType.Int);
                        AddIntConditionLogic(UwUController, layerIndex, conditionParameterName, conditionLogic, intValue, !logicParameterExists);
                        break;
                    case ConditionType.Float:
                        layerIndex = AddConditionLayer(UwUController, conditionParameterName);
                        conditionParameterLogicName += "_" + layerIndex;
                        foreach (var parameter in UwUController.parameters)
                        {
                            if (parameter.name == conditionParameterLogicName)
                                logicParameterExists = true;
                        }
                        
                        if (!logicParameterExists)
                        {
                            UwUController.AddParameter(conditionParameterLogicName, AnimatorControllerParameterType.Float);
                        }
                        
                        if (!parameterExists)
                            UwUController.AddParameter(conditionParameterName,
                                AnimatorControllerParameterType.Float);
                        AddFloatConditionLogic(UwUController, layerIndex, conditionParameterName, conditionLogic, floatValue, !logicParameterExists);
                        break;
                }
                if (outputConditions.Count == 1)
                {
                    BlendTree recursiveTree = CreateUwUBlendTree( conditionParameterLogicName /*+ "_" + outputConditions.Count.ToString()*/, conditionParameterLogicName);
                    AssetDatabase.AddObjectToAsset(recursiveTree, UwUController);
                    recursiveTree.AddChild(UwUConditionalOutputTrueClip, trueThreshold);
                    recursiveTree.AddChild(UwUConditionalOutputFalseClip, falseThreshold);
                    ChildMotion[] recursiveChildren = recursiveTree.children;
                    recursiveChildren = recursiveChildren.OrderBy(c => c.threshold).ToArray();
                    recursiveTree.children = recursiveChildren;
                    parentTree.AddChild(recursiveTree, parentFalseThreshold);
                    EditorUtility.SetDirty(UwUController);
                    AssetDatabase.SaveAssets();
                }
                else if (outputConditions.Count > 0)
                {
                    BlendTree recursiveTree = CreateUwUBlendTree(conditionParameterLogicName /*+ "_" + outputConditions.Count.ToString()*/, conditionParameterLogicName);
                    AssetDatabase.AddObjectToAsset(recursiveTree, UwUController);
                    outputConditions.RemoveAt(index);
                    parentTree.AddChild(recursiveTree, parentFalseThreshold);
                    if (hasDuplicates)
                    {
                        Debug.LogWarning($"[UwU] Removing duplicate entries for {conditionParameterName}. Only one occurrence of a parameter is allowed per output.");
                        outputConditions.RemoveAll(x => x.conditionParameterName == conditionParameterName);
                    }

                    if (outputConditions.Count > 0)
                    {
                        CreateOutputConditionTree(globalParameters, UwUController, recursiveTree, outputConditions, UwUConditionalOutputFalseClip, UwUConditionalOutputTrueClip, boolValue);
                    }
                    else
                    {
                        recursiveTree.AddChild(UwUConditionalOutputTrueClip, trueThreshold);
                        recursiveTree.AddChild(UwUConditionalOutputFalseClip, falseThreshold);
                        ChildMotion[] recursiveChildren = recursiveTree.children;
                        recursiveChildren = recursiveChildren.OrderBy(c => c.threshold).ToArray();
                        recursiveChildren = recursiveChildren.OrderBy(c => c.threshold).ToArray();
                        recursiveTree.children = recursiveChildren;
                    }
                    EditorUtility.SetDirty(UwUController);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    parentTree.AddChild(UwUConditionalOutputFalseClip, parentTrueThreshold);
                }
                
                parentTree.AddChild(UwUConditionalOutputFalseClip, parentTrueThreshold);
                ChildMotion[] parentChildren = parentTree.children;
                parentChildren = parentChildren.OrderBy(c => c.threshold).ToArray();
                parentTree.children = parentChildren;
            }
            else if (outputConditions.Count > 1)
            {
                if (hasDuplicates)
                {
                    Debug.LogWarning($"[UwU] Removing duplicate entries for {conditionParameterName}. Only one occurrence of a parameter is allowed per output.");
                    outputConditions.RemoveAll(x => x.conditionParameterName == conditionParameterName);
                }
                outputConditions.RemoveAt(index);
                CreateOutputConditionTree(globalParameters, UwUController, parentTree, outputConditions, UwUConditionalOutputFalseClip, UwUConditionalOutputTrueClip, parentOutputReversed);
            }
            EditorUtility.SetDirty(UwUController);
            AssetDatabase.SaveAssets();
        }

        private static void AddIntConditionLogic(AnimatorController UwUController, int layerIndex, string parametername, int logicType, int conditionValue, bool createClips)
        {
            AnimatorControllerLayer[] layers = UwUController.layers;
            
            AnimatorStateMachine logicStateMachine = new  AnimatorStateMachine();
            logicStateMachine.name = UwUController.layers[layerIndex].name;
            
            UwUController.layers[layerIndex].stateMachine = logicStateMachine;
            
            AssetDatabase.AddObjectToAsset(logicStateMachine, UwUController);

            string logicParameterName = parametername + "_Logic_" + layerIndex;
            AnimationClip logicFalseClip;
            AnimationClip logicTrueClip;

            if (createClips)
            {
                logicFalseClip = CreateAAPClip(logicParameterName, 0.0f);
                logicFalseClip.name = $"{logicParameterName}_False";
                AssetDatabase.AddObjectToAsset(logicFalseClip, UwUController);
                logicTrueClip = CreateAAPClip(logicParameterName, 1.0f);
                logicTrueClip.name = $"{logicParameterName}_True";
                AssetDatabase.AddObjectToAsset(logicTrueClip, UwUController);
            }
            else
            {
                logicFalseClip = GetClipByName(UwUController, $"{logicParameterName}_False");
                logicTrueClip = GetClipByName(UwUController, $"{logicParameterName}_True");
            }
            
            
            AnimatorState logicFalseState = logicStateMachine.AddState($"{parametername}_Logic_False");
            logicFalseState.motion = logicFalseClip;
            AnimatorState logicTrueState = logicStateMachine.AddState($"{parametername}_Logic_True");
            logicTrueState.motion = logicTrueClip;

            AnimatorStateTransition logicFalseTransition = logicTrueState.AddTransition(logicFalseState);
            AnimatorStateTransition logicTrueTransition = logicFalseState.AddTransition(logicTrueState);
            
            logicFalseTransition.hasExitTime = false;
            logicFalseTransition.duration = 0f;
            
            logicTrueTransition.hasExitTime = false;
            logicTrueTransition.duration = 0f;

            switch (logicType)
            {
                case 0:
                    int maxValue = Math.Clamp(conditionValue + 1, 0, 255);
                    logicFalseTransition.AddCondition(AnimatorConditionMode.Less, maxValue, parametername);
                    logicTrueTransition.AddCondition(AnimatorConditionMode.Greater, conditionValue, parametername);
                    break;
                case 1:
                    int minValue = Math.Clamp(conditionValue - 1, 0, 255);
                    logicFalseTransition.AddCondition(AnimatorConditionMode.Greater, minValue, parametername);
                    logicTrueTransition.AddCondition(AnimatorConditionMode.Less, conditionValue, parametername);
                    break;
                case 2:
                    logicFalseTransition.AddCondition(AnimatorConditionMode.NotEqual, conditionValue, parametername);
                    logicTrueTransition.AddCondition(AnimatorConditionMode.Equals, conditionValue, parametername);
                    break;
                case 3:
                    logicFalseTransition.AddCondition(AnimatorConditionMode.Equals, conditionValue, parametername);
                    logicTrueTransition.AddCondition(AnimatorConditionMode.NotEqual, conditionValue, parametername);
                    break;
            }
            layers[layerIndex].stateMachine = logicStateMachine;
            UwUController.layers = layers;
            
            EditorUtility.SetDirty(UwUController);
            AssetDatabase.SaveAssets();
        }
        
        private static void AddFloatConditionLogic(AnimatorController UwUController, int layerIndex, string parametername, int logicType, float conditionValue, bool createClips)
        {
            AnimatorControllerLayer[] layers = UwUController.layers;
            
            AnimatorStateMachine logicStateMachine = new  AnimatorStateMachine();
            logicStateMachine.name = UwUController.layers[layerIndex].name;
            
            UwUController.layers[layerIndex].stateMachine = logicStateMachine;
            
            AssetDatabase.AddObjectToAsset(logicStateMachine, UwUController);

            string logicParameterName = parametername + "_Logic_" + layerIndex;
            AnimationClip logicFalseClip;
            AnimationClip logicTrueClip;

            if (createClips)
            {
                logicFalseClip = CreateAAPClip(logicParameterName, 0.0f);
                logicFalseClip.name = $"{logicParameterName}_False";
                AssetDatabase.AddObjectToAsset(logicFalseClip, UwUController);
                logicTrueClip = CreateAAPClip(logicParameterName, 1.0f);
                logicTrueClip.name = $"{logicParameterName}_True";
                AssetDatabase.AddObjectToAsset(logicTrueClip, UwUController);
            }
            else
            {
                logicFalseClip = GetClipByName(UwUController, $"{logicParameterName}_False");
                logicTrueClip = GetClipByName(UwUController, $"{logicParameterName}_True");
            }
            
            
            AnimatorState logicFalseState = logicStateMachine.AddState($"{parametername}_Logic_False");
            logicFalseState.motion = logicFalseClip;
            AnimatorState logicTrueState = logicStateMachine.AddState($"{parametername}_Logic_True");
            logicTrueState.motion = logicTrueClip;

            AnimatorStateTransition logicFalseTransition = logicTrueState.AddTransition(logicFalseState);
            AnimatorStateTransition logicTrueTransition = logicFalseState.AddTransition(logicTrueState);
            
            logicFalseTransition.hasExitTime = false;
            logicFalseTransition.duration = 0f;
            
            logicTrueTransition.hasExitTime = false;
            logicTrueTransition.duration = 0f;
            
            conditionValue = Mathf.Clamp(conditionValue,-1f,1f);

            switch (logicType)
            {
                case 0:
                    logicFalseTransition.AddCondition(AnimatorConditionMode.Less, conditionValue, parametername);
                    logicTrueTransition.AddCondition(AnimatorConditionMode.Greater, conditionValue, parametername);
                    break;
                case 1:
                    logicFalseTransition.AddCondition(AnimatorConditionMode.Greater, conditionValue, parametername);
                    logicTrueTransition.AddCondition(AnimatorConditionMode.Less, conditionValue, parametername);
                    break;
            }
            layers[layerIndex].stateMachine = logicStateMachine;
            UwUController.layers = layers;
            
            EditorUtility.SetDirty(UwUController);
            AssetDatabase.SaveAssets();
        }

        private static AnimatorControllerParameter GetParameterByName(AnimatorController UwUController, string parameterName)
        {
            for (int i = 1; i < UwUController.parameters.Length; i++)
            {
                AnimatorControllerParameter param = UwUController.parameters[i];
                if (UwUController.parameters[i].name == parameterName)
                    return param;
            }
            
            return null;
        }

        private static AnimationClip GetClipByName(AnimatorController UwUController, string clipName)
        {
            foreach (AnimationClip clip in UwUController.animationClips)
            {
                if (clip.name == clipName)
                {
                    Debug.Log($"[UwU] Found {clip.name}");
                    return clip;
                }
            }
            Debug.Log($"[UwU] Could not find {clipName}");
            return null;
        }

        private static int AddConditionLayer(AnimatorController UwUController, string parametername)
        {
            int layerIndex = 0;
            for (int i = 0; i < UwUController.layers.Length; i++)
            {
                if (UwUController.layers[i].name == parametername + "_Logic")
                {
                    layerIndex = i;
                }
            }

            if (layerIndex == 0)
            {
                layerIndex = UwUController.layers.Length;
                AnimatorControllerLayer newLayer = new AnimatorControllerLayer
                {
                    name = parametername + "_Logic_" + layerIndex,
                    defaultWeight = 1
                };
                UwUController.AddLayer(newLayer);
            }

            return layerIndex;
        }

        private static void CreateUwUToggleAlwaysOnLocalLogic(UwUMenu UwUData, AnimatorController UwUController)
        {
            string namePrefix = UwUData.namePrefix;
            AnimatorControllerParameter[] controllerParameters = UwUController.parameters;
            AnimatorControllerParameter alwaysOnParameter = controllerParameters[0];
            AnimatorControllerParameter localParameter = controllerParameters[3];
            AnimatorControllerParameter friendsParameter = controllerParameters[4];
            AnimatorControllerParameter globalParameter = controllerParameters[5];
            
            AnimatorControllerLayer exclusiveLayer = new AnimatorControllerLayer();
            exclusiveLayer.name = $"{namePrefix}_AlwaysOnLocal";

            AnimatorStateMachine exclusiveStateMachine = new  AnimatorStateMachine();
            exclusiveStateMachine.name = $"{namePrefix}_AlwaysOnLocal";
            
            exclusiveLayer.stateMachine = exclusiveStateMachine;
            
            AssetDatabase.AddObjectToAsset(exclusiveStateMachine, UwUController);
            
            UwUController.AddLayer(exclusiveLayer);
            
            AnimatorState idleState = exclusiveStateMachine.AddState($"Idle");
            
            AnimatorState alwaysOnLocalState = exclusiveStateMachine.AddState($"AlwaysOnLocal_{namePrefix}");

            VRC_AvatarParameterDriver.Parameter localVRCParam = new VRC_AvatarParameterDriver.Parameter()
            {
                name = localParameter.name,
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 1f
            };
            
            VRC_AvatarParameterDriver alwaysOnLocalBehaviour = alwaysOnLocalState.AddStateMachineBehaviour(typeof(VRCAvatarParameterDriver)) as VRCAvatarParameterDriver;
            alwaysOnLocalBehaviour.localOnly = true;
            alwaysOnLocalBehaviour.parameters.Add(localVRCParam);
            
            AnimatorStateTransition alwaysOnIdleTransition = exclusiveStateMachine.AddAnyStateTransition(idleState);
            AnimatorStateTransition alwaysOnLocalTransition = exclusiveStateMachine.AddAnyStateTransition(alwaysOnLocalState);
            
            alwaysOnIdleTransition.hasExitTime = false;
            alwaysOnIdleTransition.canTransitionToSelf = false;
            alwaysOnIdleTransition.duration = 0f;
            alwaysOnIdleTransition.AddCondition(AnimatorConditionMode.Greater, .9f, alwaysOnParameter.name);
            
            alwaysOnLocalTransition.hasExitTime = false;
            alwaysOnLocalTransition.canTransitionToSelf = false;
            alwaysOnLocalTransition.duration = 0f;
            alwaysOnLocalTransition.AddCondition(AnimatorConditionMode.Less, .1f, localParameter.name);
            alwaysOnLocalTransition.AddCondition(AnimatorConditionMode.Less, .1f, friendsParameter.name);
            alwaysOnLocalTransition.AddCondition(AnimatorConditionMode.Less, .1f, globalParameter.name);
        }
        
        private static void CreateUwUToggleExclusiveLogic(UwUMenu UwUData, AnimatorController UwUController)
        {
            string controllerName = UwUData.namePrefix;
            AnimatorControllerParameter[] controllerParameters = UwUController.parameters;
            AnimatorControllerParameter localParameter = controllerParameters[3];
            AnimatorControllerParameter friendsParameter = controllerParameters[4];
            AnimatorControllerParameter globalParameter = controllerParameters[5];
            
            AnimatorControllerLayer exclusiveLayer = new AnimatorControllerLayer();
            exclusiveLayer.name = $"{controllerName}_ExclusiveStates";

            AnimatorStateMachine exclusiveStateMachine = new  AnimatorStateMachine();
            exclusiveStateMachine.name = $"{controllerName}_ExclusiveStates";
            
            exclusiveLayer.stateMachine = exclusiveStateMachine;
            
            AssetDatabase.AddObjectToAsset(exclusiveStateMachine, UwUController);
            
            UwUController.AddLayer(exclusiveLayer);
            
            AnimatorState idleState = exclusiveStateMachine.AddState($"Idle");

            AnimatorState localExclusiveState = exclusiveStateMachine.AddState($"LocalExclusive_{controllerName}");
            AnimatorState friendsExclusiveState = exclusiveStateMachine.AddState($"FriendsExclusive_{controllerName}");
            AnimatorState globalExclusiveState = exclusiveStateMachine.AddState($"GlobalExclusive_{controllerName}");
            
            VRC_AvatarParameterDriver.Parameter localVRCParam = new VRC_AvatarParameterDriver.Parameter()
            {
                name = localParameter.name,
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0f
            };
            VRC_AvatarParameterDriver.Parameter friendsVRCParam = new VRC_AvatarParameterDriver.Parameter()
            {
                name = friendsParameter.name,
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0f
            };
            VRC_AvatarParameterDriver.Parameter globalVRCParam = new VRC_AvatarParameterDriver.Parameter()
            {
                name = globalParameter.name,
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0f
            };

            VRC_AvatarParameterDriver localExclusiveBehaviour = localExclusiveState.AddStateMachineBehaviour(typeof(VRCAvatarParameterDriver)) as VRCAvatarParameterDriver;
            localExclusiveBehaviour.localOnly = true;
            localExclusiveBehaviour.parameters.Add(friendsVRCParam);
            localExclusiveBehaviour.parameters.Add(globalVRCParam);
            
            VRC_AvatarParameterDriver friendsExclusiveBehaviour = friendsExclusiveState.AddStateMachineBehaviour(typeof(VRCAvatarParameterDriver)) as VRCAvatarParameterDriver;
            friendsExclusiveBehaviour.localOnly = true;
            friendsExclusiveBehaviour.parameters.Add(localVRCParam);
            friendsExclusiveBehaviour.parameters.Add(globalVRCParam);
            
            VRC_AvatarParameterDriver globalExclusiveBehaviour = globalExclusiveState.AddStateMachineBehaviour(typeof(VRCAvatarParameterDriver)) as VRCAvatarParameterDriver;
            globalExclusiveBehaviour.localOnly = true;
            globalExclusiveBehaviour.parameters.Add(localVRCParam);
            globalExclusiveBehaviour.parameters.Add(friendsVRCParam);
            
            AnimatorStateTransition exclusiveTransitionToLocal = exclusiveStateMachine.AddAnyStateTransition(localExclusiveState);
            AnimatorStateTransition exclusiveTransitionToFriends = exclusiveStateMachine.AddAnyStateTransition(friendsExclusiveState);
            AnimatorStateTransition exclusiveTransitionToGlobal = exclusiveStateMachine.AddAnyStateTransition(globalExclusiveState);

            exclusiveTransitionToLocal.hasExitTime = false;
            exclusiveTransitionToLocal.canTransitionToSelf = false;
            exclusiveTransitionToLocal.duration = 0f;
            exclusiveTransitionToLocal.AddCondition(AnimatorConditionMode.Greater, .9f, localParameter.name);

            exclusiveTransitionToFriends.hasExitTime = false;
            exclusiveTransitionToFriends.canTransitionToSelf = false;
            exclusiveTransitionToFriends.duration = 0f;
            exclusiveTransitionToFriends.AddCondition(AnimatorConditionMode.Greater, .9f, friendsParameter.name);

            exclusiveTransitionToGlobal.hasExitTime = false;
            exclusiveTransitionToGlobal.canTransitionToSelf = false;
            exclusiveTransitionToGlobal.duration = 0f;
            exclusiveTransitionToGlobal.AddCondition(AnimatorConditionMode.Greater, .9f, globalParameter.name);
        }

        private static BlendTree CreateUwUBlendTree(string name, string parameterName)
        {
            BlendTree newBlendTree = new BlendTree();
            newBlendTree.hideFlags = HideFlags.HideInHierarchy;
            newBlendTree.blendType = BlendTreeType.Simple1D;
            newBlendTree.blendParameter = parameterName;
            newBlendTree.name = name;
            newBlendTree.useAutomaticThresholds = false;

            return newBlendTree;
        }
        
        private static AnimationClip CreateAAPClip(string parameterName, float targetValue)
        {
            AnimationClip clip = new AnimationClip();
            clip.legacy = false;
            
            AnimationCurve curve = AnimationCurve.Constant(0f, 0f, targetValue);
            EditorCurveBinding binding = new EditorCurveBinding
            {
                path = "",
                propertyName = $"{parameterName}",
                type = typeof(Animator)
            };
            
            AnimationUtility.SetEditorCurve(clip, binding, curve);
            return clip;
        }

        private static List<string> AddGlobalParameter(List<string> list, string parameterName)
        {
            if (!string.IsNullOrEmpty(parameterName) && !list.Contains(parameterName))
            {
                list.Add(parameterName);
            }

            return list;
        }
    }
}