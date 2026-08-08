using UnityEngine; // needed for ScriptableObject

[CreateAssetMenu(fileName = "NewCause", menuName = "Cases/Cause Option")]
public class CauseOption : ScriptableObject
{
    public string causeID; // unique id, e.g. "Speeding"
    public string displayLabel; // text shown on the button, e.g. "Speeding"
}