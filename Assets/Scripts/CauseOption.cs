using UnityEngine; // needed for ScriptableObject

[CreateAssetMenu(fileName = "NewCause", menuName = "Cases/Cause Option")]
public class CauseOption : ScriptableObject
{
    public string causeID; // unique id, e.g. "Speeding" - used for comparison logic only
}