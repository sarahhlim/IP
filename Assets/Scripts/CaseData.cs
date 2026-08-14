using UnityEngine; // needed for ScriptableObject, Sprite

[System.Serializable]
public class CorrectItem // pairs an item ID with the icon shown when collected
{
    public string itemID;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "NewCase", menuName = "Cases/Case Data")]
public class CaseData : ScriptableObject
{
    [Header("Identity")]
    public string caseID; // unique id, e.g. "eScooter" - used for tracking logic only, never shown to player

    [Header("Scene Link")]
    public string sceneToLoad; // must match scene name in Build Profiles exactly

    [Header("Cause Of Incident")]
    public CauseOption correctCause; // direct reference, not a string ID

    [Header("Folder Unlocked View")]
    public Sprite overviewImage;

    [Header("News Article View")]
    public Sprite newsBodyImage;
    public string newsLinkText;

    [Header("Correct Clue Items")]
    public CorrectItem[] correctItems; // 3 correct items tied to collectibles in this case's scene

    [Header("HUD Objective Text")]
    public string objectiveTitle; // shown when the case scene loads, e.g. "Talk to the spirit"
    public string objectiveFlavorText; // reaction line shown under the objective title
    public string nextObjectiveTitle; // shown if the timer runs out before all clues are collected
    public string nextObjectiveFlavorText; // reaction line shown under the fallback objective

    [Header("HUD Timing")]
    public float timeLimitSeconds = 60f; // countdown length for this case, runs in the background, no on-screen display
}