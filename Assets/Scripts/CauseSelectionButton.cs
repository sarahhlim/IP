using UnityEngine; // needed for MonoBehaviour, SerializeField
using UnityEngine.UI; // needed for Image
using TMPro; // needed for TMP_Text

public class CauseSelectionButton : MonoBehaviour
{
    [SerializeField] private CauseOption causeOption; // drag one of your 6 CauseOption assets
    [SerializeField] private TMP_Text label; // drag this button's own TMP_Text child
    [SerializeField] private Image background; // drag this button's own Image component

    private static CauseSelectionButton currentlySelected; // tracks which button is highlighted across all 6

    void Start()
    {
        label.text = causeOption.displayLabel; // show the readable cause name
    }

    public void OnCauseButtonClicked() // hook this to the Button's OnClick - the ONLY definition of this method
    {
        ComputerScreenManager.instance.SelectCause(causeOption.causeID); // records pick, does NOT submit

        if (currentlySelected != null) currentlySelected.SetHighlighted(false); // un-highlight previous pick
        currentlySelected = this; // this button is now the pick
        SetHighlighted(true); // highlight it
    }

    public void SetHighlighted(bool on)
    {
        background.color = on ? Color.yellow : Color.white; // placeholder colors, swap for real styling later
    }

    public static void ClearHighlight() // called by ComputerScreenManager.ShowCauseSelection() on screen open
    {
        if (currentlySelected != null)
        {
            currentlySelected.SetHighlighted(false);
            currentlySelected = null;
        }
    }
}