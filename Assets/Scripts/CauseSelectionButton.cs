using UnityEngine; // needed for MonoBehaviour, SerializeField
using UnityEngine.UI; // needed for Image

public class CauseSelectionButton : MonoBehaviour
{
    [SerializeField] private CauseOption causeOption; // drag one of your CauseOption assets
    [SerializeField] private Image buttonImageDisplay; // drag this button's own Image component

    private static CauseSelectionButton currentlySelected; // tracks which button is highlighted across all options

    public void OnCauseButtonClicked() // hook this to the Button's OnClick
    {
        ComputerScreenManager.instance.SelectCause(causeOption); // passes the whole CauseOption

        if (currentlySelected != null) currentlySelected.SetHighlighted(false); // un-highlight previous pick
        currentlySelected = this;
        SetHighlighted(true);
    }

    public void SetHighlighted(bool on)
    {
        buttonImageDisplay.color = on ? new Color(1f, 0.85f, 0.3f) : Color.white; // tints the assigned image
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