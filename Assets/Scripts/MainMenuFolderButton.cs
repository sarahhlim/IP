using UnityEngine; // needed for MonoBehaviour, SerializeField

public class MainMenuFolderButton : MonoBehaviour
{
    [SerializeField] private CaseData caseData; // drag the matching CaseData asset

    public void OnButtonClicked() // hook this to the Button's OnClick
    {
        ComputerScreenManager.instance.ShowFolderDetail(caseData); // same call the '2' key was making
    }
}