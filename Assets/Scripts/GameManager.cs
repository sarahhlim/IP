using UnityEngine; // needed for MonoBehaviour, SerializeField
using System.Collections.Generic; // needed for HashSet

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Ending Thresholds")]
    [SerializeField] private int totalCaseCount = 6;
    [SerializeField] private int outstandingMinCorrect = 6;
    [SerializeField] private int satisfactoryMinCorrect = 4;

    private HashSet<string> visitedCases = new HashSet<string>();
    private HashSet<string> correctlySolvedCases = new HashSet<string>();

    public CaseData InProgressCase { get; private set; } // NEW - which case is currently awaiting a cause submission, if any

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkCaseVisited(string caseID)
    {
        visitedCases.Add(caseID);
        print("Case visited: " + caseID + " | total visited: " + visitedCases.Count);
    }

    public void MarkCaseSolvedCorrectly(string caseID)
    {
        correctlySolvedCases.Add(caseID);
        print("Solved correctly: " + caseID + " | total correct: " + correctlySolvedCases.Count);
    }

    public bool IsCaseVisited(string caseID)
    {
        return visitedCases.Contains(caseID);
    }

    public bool IsGameComplete()
    {
        return visitedCases.Count >= totalCaseCount;
    }

    public int GetCorrectCount()
    {
        return correctlySolvedCases.Count;
    }

    public EndingType EvaluateEnding()
    {
        int correct = correctlySolvedCases.Count;
        if (correct >= outstandingMinCorrect) return EndingType.Outstanding;
        if (correct >= satisfactoryMinCorrect) return EndingType.Satisfactory;
        return EndingType.Unsatisfactory;
    }

    public void SetInProgressCase(CaseData data) // NEW - called right before Deduce loads a case scene
    {
        InProgressCase = data;
        print("In-progress case set: " + (data != null ? data.caseID : "null"));
    }

    public void ClearInProgressCase() // NEW - called once a cause has been submitted for this case
    {
        InProgressCase = null;
    }
}

public enum EndingType
{
    Outstanding,
    Satisfactory,
    Unsatisfactory
}