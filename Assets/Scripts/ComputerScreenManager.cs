using UnityEngine; // needed for MonoBehaviour, GameObject
using UnityEngine.UI; // needed for Image
using TMPro; // needed for TMP_Text
using UnityEngine.SceneManagement; // needed for LoadScene

public enum ComputerScreen
{
    MainMenu,
    FolderDetail,
    CauseSelection,
    ResultSuccess,
    ResultFail,
    NewsArticle,
    Ending
}

public class ComputerScreenManager : MonoBehaviour
{
    public static ComputerScreenManager instance;

    [Header("Interaction Reference")]
    [SerializeField] private ComputerInteraction computerInteraction; // drag ComputerInteractZone here - handles cursor/camera/movement cleanup

    [Header("Top-Level Panels")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelFolderDetail;
    [SerializeField] private GameObject panelCauseSelection;
    [SerializeField] private GameObject panelResultSuccess;
    [SerializeField] private GameObject panelResultFail;
    [SerializeField] private GameObject panelNewsArticle;
    [SerializeField] private GameObject panelEnding;

    [Header("Folder Detail Sub-Views")]
    [SerializeField] private GameObject folderLockedView;
    [SerializeField] private GameObject folderUnlockedView;
    [SerializeField] private Image overviewImageDisplay; // drag the Image inside folderUnlockedView

    [Header("News Article Display")]
    [SerializeField] private Image newsBodyImageDisplay; // drag the Image inside panelNewsArticle
    [SerializeField] private TMP_Text newsLinkTextDisplay; // drag the TMP_Text inside panelNewsArticle

    [Header("Result Item Display")]
    [SerializeField] private Image[] resultSuccessItemSlots; // 3 slots on Panel_ResultSuccess
    [SerializeField] private Image[] resultFailItemSlots; // 3 slots on Panel_ResultFail

    [Header("Ending Sub-Views")]
    [SerializeField] private GameObject endingOutstandingView;
    [SerializeField] private GameObject endingSatisfactoryView;
    [SerializeField] private GameObject endingUnsatisfactoryView;

    private ComputerScreen currentScreen;
    private CaseData activeCase;
    private string pendingCauseID;

    void Awake()
    {
        instance = this; // singleton, UI lives in Office only
    }

    private void SetActivePanel(GameObject target)
    {
        panelMainMenu.SetActive(target == panelMainMenu);
        panelFolderDetail.SetActive(target == panelFolderDetail);
        panelCauseSelection.SetActive(target == panelCauseSelection);
        panelResultSuccess.SetActive(target == panelResultSuccess);
        panelResultFail.SetActive(target == panelResultFail);
        panelNewsArticle.SetActive(target == panelNewsArticle);
        panelEnding.SetActive(target == panelEnding);
    }

    public void OpenDefaultScreen() // called by ComputerInteraction.OpenComputer() instead of ShowMainMenu() directly
    {
        if (GameManager.instance.InProgressCase != null) // a case is mid-playthrough, awaiting cause submission
        {
            activeCase = GameManager.instance.InProgressCase; // restore which case this is
            ShowCauseSelection(); // skip straight past Main Menu
        }
        else
        {
            ShowMainMenu(); // normal default, no case in progress
        }
    }

    public void ShowMainMenu()
    {
        currentScreen = ComputerScreen.MainMenu;
        SetActivePanel(panelMainMenu);
    }

    public void ShowFolderDetail(CaseData data)
    {
        activeCase = data;
        currentScreen = ComputerScreen.FolderDetail;
        SetActivePanel(panelFolderDetail);

        bool visited = GameManager.instance.IsCaseVisited(data.caseID);
        folderLockedView.SetActive(!visited);
        folderUnlockedView.SetActive(visited);

        if (visited)
        {
            overviewImageDisplay.sprite = data.overviewImage;
        }
    }

    public void ShowCauseSelection()
    {
        currentScreen = ComputerScreen.CauseSelection;
        SetActivePanel(panelCauseSelection);
        CauseSelectionButton.ClearHighlight(); // reset visual state each time this screen opens
    }

    public void SelectCause(string causeID) // called when a cause button is clicked - just records the pick
    {
        pendingCauseID = causeID;
        print("Cause selected (pending): " + causeID);
    }

    public void ConfirmCauseSelection() // called when Confirm button is clicked
    {
        if (string.IsNullOrEmpty(pendingCauseID))
        {
            print("No cause selected yet - Confirm ignored");
            return;
        }
        SubmitCause(pendingCauseID);
        pendingCauseID = null;
    }

    public void SubmitCause(string chosenCauseID)
    {
        bool correct = chosenCauseID == activeCase.correctCauseID;

        GameManager.instance.MarkCaseVisited(activeCase.caseID); // ALWAYS mark visited - progression never blocked
        GameManager.instance.ClearInProgressCase(); // case is now resolved, no longer "in progress"

        if (correct)
        {
            GameManager.instance.MarkCaseSolvedCorrectly(activeCase.caseID);
            currentScreen = ComputerScreen.ResultSuccess;
            SetActivePanel(panelResultSuccess);
            UpdateResultItemDisplay(resultSuccessItemSlots);
        }
        else
        {
            currentScreen = ComputerScreen.ResultFail;
            SetActivePanel(panelResultFail);
            UpdateResultItemDisplay(resultFailItemSlots);
        }

        if (GameManager.instance.IsGameComplete())
        {
            TriggerEnding();
        }
    }

    private void UpdateResultItemDisplay(Image[] slots) // fills the 3 item slots based on what was actually collected
    {
        for (int i = 0; i < activeCase.correctItems.Length && i < slots.Length; i++)
        {
            bool collected = GameManager.instance.HasCollectedItem(activeCase.correctItems[i].itemID);
            slots[i].sprite = activeCase.correctItems[i].icon;
            slots[i].color = collected ? Color.white : new Color(1f, 1f, 1f, 0.15f); // dim if not collected
        }
    }

    private void TriggerEnding()
    {
        EndingType result = GameManager.instance.EvaluateEnding();
        currentScreen = ComputerScreen.Ending;
        SetActivePanel(panelEnding);

        endingOutstandingView.SetActive(result == EndingType.Outstanding);
        endingSatisfactoryView.SetActive(result == EndingType.Satisfactory);
        endingUnsatisfactoryView.SetActive(result == EndingType.Unsatisfactory);

        print("Game complete | correct: " + GameManager.instance.GetCorrectCount() + " | ending: " + result);
    }

    public void ShowNewsArticle()
    {
        currentScreen = ComputerScreen.NewsArticle;
        SetActivePanel(panelNewsArticle);

        newsBodyImageDisplay.sprite = activeCase.newsBodyImage;
        newsLinkTextDisplay.text = activeCase.newsLinkText;
    }

    public void ReturnToMenu()
    {
        activeCase = null;
        ShowMainMenu();
    }

    public void OnDeucePressed() // hook to the Deduce button's OnClick in BOTH folder sub-views
    {
        GameManager.instance.SetInProgressCase(activeCase); // remember this case is now in progress before leaving
        GameManager.instance.ResetCollectedItems(); // clear leftover items from any previous attempt

        if (computerInteraction != null)
        {
            computerInteraction.CloseComputer(); // restores movement, cursor, camera - same as pressing X
        }
        else
        {
            Debug.LogError("ComputerScreenManager: Computer Interaction reference is not assigned - player will stay frozen after scene load");
        }

        SceneManager.LoadScene(activeCase.sceneToLoad);
    }

    public void OnEndingContinuePressed() // hook to Continue button on the ending panel
    {
        if (computerInteraction != null)
        {
            computerInteraction.CloseComputer();
        }
    }

    public void OnExitGamePressed() // hook to Exit Game button on the ending panel
    {
        print("Exit Game pressed");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops Play mode when testing inside the Editor
#else
        Application.Quit(); // closes the actual built application
#endif
    }
}