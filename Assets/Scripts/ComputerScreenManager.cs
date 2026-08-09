using UnityEngine; // needed for MonoBehaviour, GameObject

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

    [Header("Top-Level Panels")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelFolderDetail;
    [SerializeField] private GameObject panelCauseSelection;
    [SerializeField] private GameObject panelResultSuccess;
    [SerializeField] private GameObject panelResultFail;
    [SerializeField] private GameObject panelNewsArticle;
    [SerializeField] private GameObject panelEnding; // NEW - drag Panel_Ending here

    [Header("Folder Detail Sub-Views")]
    [SerializeField] private GameObject folderLockedView; // drag FolderLockedView here
    [SerializeField] private GameObject folderUnlockedView; // drag FolderUnlockedView here

    [Header("Ending Sub-Views")]
    [SerializeField] private GameObject endingOutstandingView; // NEW - drag Panel_EndingOutstanding here
    [SerializeField] private GameObject endingSatisfactoryView; // NEW - drag Panel_EndingSatisfactory here
    [SerializeField] private GameObject endingUnsatisfactoryView; // NEW - drag Panel_EndingUnsatisfactory here
    
    [Header("Cause Selection State")]
    private string pendingCauseID; // holds the player's current pick until Confirm is pressed
    
    private ComputerScreen currentScreen;
    private CaseData activeCase;

    void Awake()
    {
        instance = this; // singleton, UI lives in Office only
    }

    private void SetActivePanel(GameObject target) // turns on the target panel, turns off every other one
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
        activeCase = data; // remember which case is being viewed
        currentScreen = ComputerScreen.FolderDetail;
        SetActivePanel(panelFolderDetail);

        bool visited = GameManager.instance.IsCaseVisited(data.caseID); // check whether player has played this case before
        folderLockedView.SetActive(!visited); // "?" view only if never visited
        folderUnlockedView.SetActive(visited); // overview + news view once visited - Deduct still lives here for replay
    }

    public void ShowCauseSelection()
    {
        currentScreen = ComputerScreen.CauseSelection;
        SetActivePanel(panelCauseSelection);
    }

    public void SubmitCause(string chosenCauseID)
    {
        bool correct = chosenCauseID == activeCase.correctCauseID; // determines which result screen shows

        GameManager.instance.MarkCaseVisited(activeCase.caseID); // ALWAYS mark visited - progression never blocked by a wrong answer

        if (correct)
        {
            GameManager.instance.MarkCaseSolvedCorrectly(activeCase.caseID); // feeds the ending tally only
            currentScreen = ComputerScreen.ResultSuccess;
            SetActivePanel(panelResultSuccess);
        }
        else
        {
            currentScreen = ComputerScreen.ResultFail;
            SetActivePanel(panelResultFail);
        }

        if (GameManager.instance.IsGameComplete()) // auto-trigger check - runs after every submission
        {
            TriggerEnding(); // overrides whatever screen would normally show once the 6th case is visited
        }
    }

    private void TriggerEnding()
    {
        EndingType result = GameManager.instance.EvaluateEnding(); // ask GameManager for the tally-based outcome
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

    public void OnDeducePressed() // hook to the Deduce button's OnClick in BOTH folder sub-views
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(activeCase.sceneToLoad); // loads whichever case is currently open
    }
    
    public void SelectCause(string causeID) // called when a cause button is clicked - just records the pick
    {
        pendingCauseID = causeID; // store it, don't submit yet
        print("Cause selected (pending): " + causeID);
    }

    public void ConfirmCauseSelection() // called when Confirm button is clicked
    {
        if (string.IsNullOrEmpty(pendingCauseID)) // guard against confirming with nothing picked
        {
            print("No cause selected yet - Confirm ignored");
            return;
        }
        SubmitCause(pendingCauseID); // reuses your existing evaluation logic unchanged
        pendingCauseID = null; // clear for next time
    }
    
}