using UnityEngine; // needed for MonoBehaviour, GameObject
using UnityEngine.UI; // needed for Image
using TMPro; // needed for TMP_Text
using UnityEngine.SceneManagement; // needed for LoadScene

public class ComputerScreenManager : MonoBehaviour
{
    public static ComputerScreenManager instance;

    [Header("Interaction Reference")]
    [SerializeField] private ComputerInteraction computerInteraction; // drag ComputerInteractZone here

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
    [SerializeField] private Image overviewImageDisplay;

    [Header("News Article Display")]
    [SerializeField] private Image newsBodyImageDisplay;
    [SerializeField] private TMP_Text newsLinkTextDisplay;

    [Header("Result Item Display")]
    [SerializeField] private Image[] resultSuccessItemSlots; // 3 slots on Panel_ResultSuccess
    [SerializeField] private Image[] resultFailItemSlots; // 3 slots on Panel_ResultFail

    [Header("Result Success Cause Reveal")]
    [SerializeField] private Image successChosenCauseDisplay;
    [SerializeField] private Image successCorrectCauseDisplay;

    [Header("Result Fail Cause Reveal")]
    [SerializeField] private Image failChosenCauseDisplay;
    [SerializeField] private Image failCorrectCauseDisplay;

    [Header("Ending Sub-Views")]
    [SerializeField] private GameObject endingOutstandingView;
    [SerializeField] private GameObject endingSatisfactoryView;
    [SerializeField] private GameObject endingUnsatisfactoryView;

    private CaseData activeCase;
    private CauseOption pendingCause;

    void Awake()
    {
        instance = this;
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

    public void OpenDefaultScreen()
    {
        if (GameManager.instance.InProgressCase != null)
        {
            activeCase = GameManager.instance.InProgressCase;
            ShowCauseSelection();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        SetActivePanel(panelMainMenu);
    }

    public void ShowFolderDetail(CaseData data)
    {
        activeCase = data;
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
        SetActivePanel(panelCauseSelection);
        CauseSelectionButton.ClearHighlight();
    }

    public void SelectCause(CauseOption cause)
    {
        pendingCause = cause;
        print("Cause selected (pending): " + cause.causeID);
    }

    public void ConfirmCauseSelection()
    {
        if (pendingCause == null)
        {
            print("No cause selected yet - Confirm ignored");
            return;
        }
        SubmitCause(pendingCause);
        pendingCause = null;
    }

    public void SubmitCause(CauseOption chosenCause)
{
    print("Chosen revealIcon: " + (chosenCause.revealIcon != null)); // TEMPORARY
    print("Correct revealIcon: " + (activeCase.correctCause.revealIcon != null)); // TEMPORARY
    print("Success Chosen Display assigned: " + (successChosenCauseDisplay != null)); // TEMPORARY
    print("Success Correct Display assigned: " + (successCorrectCauseDisplay != null)); // TEMPORARY

    bool correct = chosenCause.causeID == activeCase.correctCause.causeID;

    GameManager.instance.MarkCaseVisited(activeCase.caseID);
    GameManager.instance.ClearInProgressCase();

    if (correct)
    {
        GameManager.instance.MarkCaseSolvedCorrectly(activeCase.caseID);
        SetActivePanel(panelResultSuccess);
        UpdateResultItemDisplay(resultSuccessItemSlots);
        successChosenCauseDisplay.sprite = chosenCause.revealIcon;
        successCorrectCauseDisplay.sprite = activeCase.correctCause.revealIcon;
    }
    else
    {
        SetActivePanel(panelResultFail);
        UpdateResultItemDisplay(resultFailItemSlots);
        failChosenCauseDisplay.sprite = chosenCause.revealIcon;
        failCorrectCauseDisplay.sprite = activeCase.correctCause.revealIcon;
    }

    if (GameManager.instance.IsGameComplete())
    {
        TriggerEnding();
    }
}

    private void UpdateResultItemDisplay(Image[] slots)
    {
        for (int i = 0; i < activeCase.correctItems.Length && i < slots.Length; i++)
        {
            bool collected = GameManager.instance.HasCollectedItem(activeCase.correctItems[i].itemID);
            slots[i].enabled = true; // always visible now, regardless of collected state
            slots[i].sprite = activeCase.correctItems[i].icon;
            slots[i].color = collected ? Color.white : new Color(1f, 1f, 1f, 0.15f); // full brightness if collected, faint if not
        }
    }

    private void TriggerEnding()
    {
        EndingType result = GameManager.instance.EvaluateEnding();
        SetActivePanel(panelEnding);

        endingOutstandingView.SetActive(result == EndingType.Outstanding);
        endingSatisfactoryView.SetActive(result == EndingType.Satisfactory);
        endingUnsatisfactoryView.SetActive(result == EndingType.Unsatisfactory);

        print("Game complete | correct: " + GameManager.instance.GetCorrectCount() + " | ending: " + result);
    }

    public void ShowNewsArticle()
    {
        SetActivePanel(panelNewsArticle);

        newsBodyImageDisplay.sprite = activeCase.newsBodyImage;
        newsLinkTextDisplay.text = activeCase.newsLinkText;
    }

    public void ReturnToMenu()
    {
        activeCase = null;
        ShowMainMenu();
    }

    public void OnDeucePressed()
    {
        GameManager.instance.SetInProgressCase(activeCase);
        GameManager.instance.ResetCollectedItems();

        if (computerInteraction != null)
        {
            computerInteraction.CloseComputer();
        }
        else
        {
            Debug.LogError("ComputerScreenManager: Computer Interaction reference is not assigned");
        }

        SceneManager.LoadScene(activeCase.sceneToLoad);
    }

    public void OnEndingContinuePressed()
    {
        if (computerInteraction != null)
        {
            computerInteraction.CloseComputer();
        }
    }

    public void OnExitGamePressed()
    {
        print("Exit Game pressed");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}