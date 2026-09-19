using System.Collections.Generic;

namespace UwU
{
    [System.Serializable]
    public class UwUOutput
    {
        public string outputParameter;
        public int conditionState = 0;
        public List<UwUOutputCondition> outputConditions = new List<UwUOutputCondition>();
    }
}