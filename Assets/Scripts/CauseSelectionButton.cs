using UnityEngine; // needed for MonoBehaviour, SerializeField
using UnityEngine.UI; // needed for Image

public class CauseSelectionButton : MonoBehaviour
{
    [SerializeField] private CauseOption causeOption; // drag one of your 6 CauseOption assets
    [SerializeField] private Image buttonImageDisplay; // drag this button's own Image component (already has your PNG set manually)

    private static CauseSelectionButton currentlySelected;

    public void OnCauseButtonClicked() // hook this to the Button's OnClick
    {
        ComputerScreenManager.instance.SelectCause(causeOption.causeID);

        if (currentlySelected != null) currentlySelected.SetHighlighted(false);
        currentlySelected = this;
        SetHighlighted(true);
    }

    public void SetHighlighted(bool on)
    {
        buttonImageDisplay.color = on ? new Color(1f, 0.85f, 0.3f) : Color.white; // tints the already-assigned image
    }

    public static void ClearHighlight()
    {
        if (currentlySelected != null)
        {
            currentlySelected.SetHighlighted(false);
            currentlySelected = null;
        }
    }
}