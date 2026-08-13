using UnityEngine;

[System.Serializable]
public class CorrectItem
{
    public string itemID;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "NewCase", menuName = "Cases/Case Data")]
public class CaseData : ScriptableObject
{
    [Header("Identity")]
    public string caseID;

    [Header("Scene Link")]
    public string sceneToLoad;

    [Header("Cause Of Incident")]
    public string correctCauseID;

    [Header("Folder Unlocked View")]
    public Sprite overviewImage;

    [Header("News Article View")]
    public Sprite newsBodyImage;
    public string newsLinkText;

    [Header("Correct Clue Items")]
    public CorrectItem[] correctItems; // exactly 3, each tied to one collectible in this case's scene
}