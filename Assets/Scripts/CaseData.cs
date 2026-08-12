using UnityEngine; // needed for ScriptableObject, Sprite

[CreateAssetMenu(fileName = "NewCase", menuName = "Cases/Case Data")]
public class CaseData : ScriptableObject
{
    [Header("Identity")]
    public string caseID; // unique id, e.g. "eScooter" - used for tracking logic only, never shown to player

    [Header("Scene Link")]
    public string sceneToLoad;

    [Header("Cause Of Incident")]
    public string correctCauseID;

    [Header("Folder Unlocked View")]
    public Sprite overviewImage;

    [Header("News Article View")]
    public Sprite newsTitleImage;
    public Sprite newsBodyImage;
    public Sprite newsLinkImage;
}