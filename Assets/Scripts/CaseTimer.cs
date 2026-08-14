using System; // for Action events
using UnityEngine; // for MonoBehaviour

// Counts down a time limit for the current case scene, length sourced from this case's CaseData asset.
// No longer starts automatically on scene load - StartTimer() is called once the clue-finding phase
// begins (e.g. after the player talks to the spirit), matching the new console-driven objective flow.
public class CaseTimer : MonoBehaviour
{
    public static CaseTimer Instance; // scene-local singleton so other scripts (e.g. CaseHUDController) can start/stop it directly

    [SerializeField] private CaseData caseData; // source of truth for this case's time limit

    private float timeRemaining; // current countdown value
    private bool isRunning; // guards against ticking after expiry, before start, or after a manual stop

    public static event Action<float> OnTimerTick; // fired every frame the timer runs, passes remaining seconds, kept in case a future feature needs it
    public static event Action OnTimerExpired; // fired once when time runs out, consequence left to listeners

    private void Awake()
    {
        Instance = this; // assign singleton reference for this scene, no DontDestroyOnLoad since each case scene has its own timer
    }

    public void StartTimer() // call this once the clue-finding phase should begin counting down
    {
        timeRemaining = caseData.timeLimitSeconds; // pull the starting time from CaseData instead of a duplicated local field
        isRunning = true; // allow Update to tick
        print("Case timer started: " + caseData.timeLimitSeconds + " seconds"); // log start
    }

    private void Update() // tick the countdown every frame
    {
        if (!isRunning) // stop processing once expired, stopped, or not yet started
        {
            return; // exit early
        }

        timeRemaining -= Time.deltaTime; // decrement remaining time by frame delta
        OnTimerTick?.Invoke(Mathf.Max(timeRemaining, 0f)); // notify any listeners of the current remaining time

        if (timeRemaining <= 0f) // check for expiry
        {
            isRunning = false; // stop ticking
            print("Case timer expired"); // log expiry
            OnTimerExpired?.Invoke(); // notify listeners, e.g. HUD prints "Return to Office"
        }
    }

    public void StopTimer() // exposed so other scripts (e.g. on case completion) can freeze the countdown
    {
        isRunning = false; // halt Update ticking
        print("Case timer stopped manually"); // log manual stop
    }
}
