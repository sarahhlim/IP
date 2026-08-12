using UnityEngine; // needed for MonoBehaviour, GameObject
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

    [Header("Ending Sub-Views")]
    [SerializeField] private GameObject endingOutstandingView;
    [SerializeField] private GameObject endingSatisfactoryView;
    [SerializeField] private GameObject endingUnsatisfactoryView;

    private ComputerScreen currentScreen;
    private CaseData activeCase;
    private string pendingCauseID; // holds the player's current pick until Confirm is pressed

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

        if (correct)
        {
            GameManager.instance.MarkCaseSolvedCorrectly(activeCase.caseID);
            currentScreen = ComputerScreen.ResultSuccess;
            SetActivePanel(panelResultSuccess);
        }
        else
        {
            currentScreen = ComputerScreen.ResultFail;
            SetActivePanel(panelResultFail);
        }

        if (GameManager.instance.IsGameComplete())
        {
            TriggerEnding();
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
    }

    public void ReturnToMenu()
    {
        activeCase = null;
        ShowMainMenu();
    }

    public void OnDeucePressed() // hook to the Deduce button's OnClick in BOTH folder sub-views
    {
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
}