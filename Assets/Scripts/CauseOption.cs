using UnityEngine; // needed for ScriptableObject, Sprite

[CreateAssetMenu(fileName = "NewCause", menuName = "Cases/Cause Option")]
public class CauseOption : ScriptableObject
{
    public string causeID; // unique id, used for comparison logic
    public Sprite revealIcon; // shown on Result panels to represent this cause
}