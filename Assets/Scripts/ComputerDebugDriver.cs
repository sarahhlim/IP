using UnityEngine; // needed for MonoBehaviour, SerializeField
using UnityEngine.InputSystem; // needed for Keyboard, New Input System

public class ComputerDebugDriver : MonoBehaviour // TEMPORARY - remove once real UI buttons drive everything
{
    [Header("Single-Screen Testing (existing)")]
    [SerializeField] private CaseData testCase; // drag any one CaseData asset
    [SerializeField] private CauseOption correctOption; // drag the CauseOption matching testCase.correctCauseID
    [SerializeField] private CauseOption wrongOption; // drag any other CauseOption

    [Header("Full Playthrough Testing (new)")]
    [SerializeField] private CaseData[] allSixCases; // drag all 6 CaseData assets in order
    [SerializeField] private CauseOption[] correctOptionsInOrder; // matching correct CauseOption for each of the 6 cases, same order

    void Update()
    {
        // --- existing single-screen tests ---
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ComputerScreenManager.instance.ShowMainMenu();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ComputerScreenManager.instance.ShowFolderDetail(testCase);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ComputerScreenManager.instance.ShowCauseSelection();
        if (Keyboard.current.digit4Key.wasPressedThisFrame) ComputerScreenManager.instance.SubmitCause(correctOption.causeID);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) ComputerScreenManager.instance.SubmitCause(wrongOption.causeID);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) ComputerScreenManager.instance.ShowNewsArticle();

        // --- new full playthrough tests, force all 6 visited with a target correct count ---
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SimulatePlaythrough(6); // 6/6 correct -> Outstanding
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SimulatePlaythrough(5); // 5/6 correct -> Satisfactory
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SimulatePlaythrough(2); // 2/6 correct -> Unsatisfactory
    }

    private void SimulatePlaythrough(int correctCount) // plays through all 6 cases, first 'correctCount' answered correctly
    {
        for (int i = 0; i < allSixCases.Length; i++) // loop handles however many cases are dragged in, not a fixed 6
        {
            ComputerScreenManager.instance.ShowFolderDetail(allSixCases[i]); // sets activeCase, required before SubmitCause
            string answer = (i < correctCount) ? correctOptionsInOrder[i].causeID : "WrongAnswer"; // first N correct, rest deliberately wrong
            ComputerScreenManager.instance.SubmitCause(answer);
        }
        print("Simulated playthrough complete | target correct: " + correctCount);
    }
}