using UnityEngine; // needed for MonoBehaviour, SerializeField
using TMPro; // needed for TMP_Text

public class MainMenuFolderButton : MonoBehaviour
{
    [SerializeField] private CaseData caseData; // drag the matching CaseData asset
    [SerializeField] private TMP_Text titleLabel; // drag this button's own TMP_Text child

    void Start()
    {
        titleLabel.text = caseData.caseTitle; // show the case name on the button itself
    }

    public void OnButtonClicked() // hook this to the Button's OnClick
    {
        ComputerScreenManager.instance.ShowFolderDetail(caseData); // same call the '2' key was making
    }
}