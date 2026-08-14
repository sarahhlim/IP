using UnityEngine; // for MonoBehaviour, Sprite

// Bridges case progress (spirit dialogue, clues, timer expiry) into the bottom console panel (GameConsoleHUD).
// Flow: scene load prints the objective title only -> SpiritInteraction calls TriggerSpiritDialogue() once the
// player talks to the spirit, which prints the reaction line, pins the live "[x/3]" clue counter, and starts the
// timer -> each clue collected updates the counter and logs its own line -> all clues collected or the timer
// running out (whichever happens first) prints "Return to Office".
public class CaseHUDController : MonoBehaviour
{
    public static CaseHUDController Instance; // scene-local singleton so SpiritInteraction can call into this without a manual reference

    public static bool CluesUnlocked { get; private set; } // true only after the player has talked to the spirit; ClueItem checks this before allowing a pickup

    [Header("Case Data")] // single source of truth for this case's objective text and timing
    [SerializeField] private CaseData caseData; // same asset assigned to ClueTracker and CaseTimer for this scene

    private bool caseResolved; // true once "Return to Office" has been shown, guards against showing it twice

    private void Awake()
    {
        Instance = this; // assign singleton reference for this scene, no DontDestroyOnLoad since each case scene has its own controller
        CluesUnlocked = false; // reset every time this scene (re)loads, clues are always locked until the spirit is talked to again
    }

    private void OnEnable() // subscribe to tracker/timer events while this controller is active
    {
        ClueTracker.OnClueCountChanged += HandleClueCountChanged; // update the pinned "[x/3]" counter
        ClueTracker.OnItemCollected += HandleItemCollected; // print a line for each collected clue
        ClueTracker.OnAllCluesCollected += HandleAllCluesCollected; // print the return-to-office prompt
        CaseTimer.OnTimerExpired += HandleTimerExpired; // print the return-to-office prompt if time runs out first
    }

    private void OnDisable() // unsubscribe to avoid stale references when the scene unloads
    {
        ClueTracker.OnClueCountChanged -= HandleClueCountChanged; // remove listener
        ClueTracker.OnItemCollected -= HandleItemCollected; // remove listener
        ClueTracker.OnAllCluesCollected -= HandleAllCluesCollected; // remove listener
        CaseTimer.OnTimerExpired -= HandleTimerExpired; // remove listener
    }

    private void Start() // scene load: just point the player at the spirit, nothing else yet
    {
        GameConsoleHUD.Instance?.PrintLine(caseData.objectiveTitle); // e.g. "Talk to the Spirit"
    }

    public void TriggerSpiritDialogue() // called by SpiritInteraction once the player presses E near the spirit
    {
        CluesUnlocked = true; // clues are only collectible from this point on
        GameConsoleHUD.Instance?.PrintLine(caseData.objectiveFlavorText); // e.g. "WHAT? THAT'S IT? HOW DID THIS HAPPEN?"
        GameConsoleHUD.Instance?.SetObjectiveStatus("Find clues around the map [0/" + caseData.correctItems.Length + "]"); // pin the live clue counter
        CaseTimer.Instance?.StartTimer(); // the clue-finding phase starts counting down now, not at scene load
    }

    private void HandleClueCountChanged(int current, int total) // keeps the pinned "[x/3]" counter live
    {
        GameConsoleHUD.Instance?.SetObjectiveStatus("Find clues around the map [" + current + "/" + total + "]"); // overwrites in place, does not scroll
    }

    private void HandleItemCollected(string itemID, Sprite icon) // icon kept for event signature compatibility, no longer shown - the console is text-only
    {
        GameConsoleHUD.Instance?.PrintLine(itemID + " collected."); // e.g. "Phone collected."
    }

    private void HandleAllCluesCollected() // fires once the player has found every clue
    {
        ShowReturnToOffice(); // whichever finishes first - all clues found or the timer running out - shows the same prompt
    }

    private void HandleTimerExpired() // fires once the 120-second window runs out
    {
        ShowReturnToOffice(); // whichever finishes first - all clues found or the timer running out - shows the same prompt
    }

    private void ShowReturnToOffice() // shared by both end conditions, guarded so it only ever prints once
    {
        if (caseResolved) // already shown by the other end condition
        {
            return; // exit early, avoid a duplicate print
        }

        caseResolved = true; // lock further calls out
        CaseTimer.Instance?.StopTimer(); // stop counting down if the clues finished first
        GameConsoleHUD.Instance?.PrintLine("Return to Office"); // final prompt
    }
}
