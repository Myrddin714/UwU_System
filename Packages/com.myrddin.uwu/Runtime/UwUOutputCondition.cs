namespace UwU
{
    public enum ConditionType { Bool, Int, Float }
    
    [System.Serializable]
    public class UwUOutputCondition
    {
        public string conditionParameterName;
        public ConditionType conditionParameterType;
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public int conditionLogic;
    }
}