using UnityEngine; // needed for ScriptableObject, Sprite

[CreateAssetMenu(fileName = "NewCase", menuName = "Cases/Case Data")]
public class CaseData : ScriptableObject
{
    [Header("Identity")]
    public string caseID; // unique id, e.g. "eScooter"

    [Header("Folder Display")]
    public string caseTitle; // shown once unlocked
    public Sprite folderThumbnail; // placeholder sprite for now, real art later

    [Header("Scene Link")]
    public string sceneToLoad; // must match scene name in Build Profiles

    [Header("Gameplay Requirement")]
    public int itemsRequiredToReturn; // how many clues needed before return button appears

    [Header("Cause Of Incident")]
    public string correctCauseID; // matches one CauseOption's causeID

    [Header("Unlocked Content")]
    [TextArea(3, 6)] public string overviewText; // placeholder text is fine for now
    public Sprite[] newsArticleImages; // leave empty array for now, fill later
}