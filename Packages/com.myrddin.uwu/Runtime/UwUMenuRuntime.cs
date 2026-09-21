using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace UwU
{
    [AddComponentMenu("UwU System")]
    
    public class UwUMenu : MonoBehaviour, IEditorOnly
    {
        public string avatarName;
        public string namePrefix;
        public int thresholdState = 0;
        
        public bool ingameMenu = true;
        public string menuPath = "UwU System";
        public bool createSubMenu = true;
        
        public bool allowOffLocal;
        public int defaultVisibility;
        
        public bool localSaved = true;
        public bool friendsSaved = true;
        public bool globalSaved = true;
        
        public List<UwUOutput> conditions = new List<UwUOutput>();
        
        public List<string> globalParams = new List<string>();
    }
}