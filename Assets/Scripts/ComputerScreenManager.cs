using UnityEngine; // needed for MonoBehaviour, GameObject
using UnityEngine.UI; // needed for Image
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
    [SerializeField] private ComputerInteraction computerInteraction;

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
    [SerializeField] private Image overviewImageDisplay; // NEW - drag the Image inside folderUnlockedView

    [Header("News Article Display")]
    [SerializeField] private Image newsTitleImageDisplay; // NEW - drag the Image inside panelNewsArticle
    [SerializeField] private Image newsBodyImageDisplay; // NEW
    [SerializeField] private Image newsLinkImageDisplay; // NEW

    [Header("Ending Sub-Views")]
    [SerializeField] private GameObject endingOutstandingView;
    [SerializeField] private GameObject endingSatisfactoryView;
    [SerializeField] private GameObject endingUnsatisfactoryView;

    private ComputerScreen currentScreen;
    private CaseData activeCase;
    private string pendingCauseID;

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

        if (visited) // NEW - only populate when actually shown
        {
            overviewImageDisplay.sprite = data.overviewImage;
        }
    }

    public void ShowCauseSelection()
    {
        currentScreen = ComputerScreen.CauseSelection;
        SetActivePanel(panelCauseSelection);
        CauseSelectionButton.ClearHighlight();
    }

    public void SelectCause(string causeID)
    {
        pendingCauseID = causeID;
        print("Cause selected (pending): " + causeID);
    }

    public void ConfirmCauseSelection()
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

        GameManager.instance.MarkCaseVisited(activeCase.caseID);
        GameManager.instance.ClearInProgressCase();

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

        newsTitleImageDisplay.sprite = activeCase.newsTitleImage; // NEW
        newsBodyImageDisplay.sprite = activeCase.newsBodyImage; // NEW
        newsLinkImageDisplay.sprite = activeCase.newsLinkImage; // NEW
    }

    public void ReturnToMenu()
    {
        activeCase = null;
        ShowMainMenu();
    }

    public void OnDeucePressed()
    {
        GameManager.instance.SetInProgressCase(activeCase);

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
}