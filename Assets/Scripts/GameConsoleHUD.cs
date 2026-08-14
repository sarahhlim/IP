using System.Collections; // for IEnumerator
using System.Collections.Generic; // for Queue
using System.Text; // for StringBuilder
using TMPro; // for TextMeshProUGUI
using UnityEngine; // for MonoBehaviour, WaitForSeconds
using UnityEngine.UI; // for ScrollRect

// The single in-gameplay UI element: a console-style panel docked at the bottom of the screen.
// Two things are shown: a pinned live status line (e.g. the "[x/3]" clue counter, once it exists) and a
// single "now showing" dialogue/interaction line below it. The dialogue line clears the instant a new one
// starts typing - old text disappears as new text appears, there is no scrolling history.
// Any script can call PrintLine / PrintDialogue / PrintInteractDesc to show a new dialogue line, and
// SetObjectiveStatus to create/update the pinned status line.
public class GameConsoleHUD : MonoBehaviour
{
    public static GameConsoleHUD Instance; // scene-local singleton so any script can print without a direct reference

    [Header("Console Panel")]
    [SerializeField] private TextMeshProUGUI logText; // the text block the status line + current dialogue line are drawn into
    [SerializeField] private ScrollRect scrollRect; // optional, kept for setups that still use one; harmless if left unassigned
    [SerializeField] private float secondsPerCharacter = 0.02f; // typewriter speed for each printed line

    private readonly Queue<string> pendingLines = new Queue<string>(); // dialogue lines waiting to be typed out, in order
    private Coroutine typingRoutine; // reference to the queue-processing coroutine, null when idle
    private string objectiveStatusLine = ""; // pinned line shown above the current dialogue line, e.g. "Find clues around the map [1/3]"
    private string currentDialogueLine = ""; // the single dialogue/interaction line currently on screen, replaced (not appended to) by each new PrintLine

    private void Awake()
    {
        Instance = this; // assign singleton reference for this scene

        if (logText != null) // guard in case it's unassigned
        {
            logText.text = ""; // start with an empty console
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) // only clear if we're still the active instance
        {
            Instance = null; // avoid a stale reference when the scene unloads
        }
    }

    public void PrintLine(string text) // generic entry point, queues a dialogue line to replace whatever's currently shown
    {
        if (string.IsNullOrEmpty(text)) // ignore empty/null lines rather than clearing the panel for nothing
        {
            return; // exit early, nothing to print
        }

        pendingLines.Enqueue(text); // add to the print queue

        if (typingRoutine == null) // start processing if nothing is currently typing
        {
            typingRoutine = StartCoroutine(ProcessQueue()); // begin working through the queue
        }
    }

    public void PrintDialogue(string speaker, string line) // call this when a script wants to show spoken/narrated dialogue
    {
        string formatted = string.IsNullOrEmpty(speaker) ? line : speaker + ": " + line; // "Speaker: line" format, or just the line if no speaker given
        PrintLine(formatted); // route into the same queue as everything else
    }

    public void PrintInteractDesc(string desc) // call this when a script wants to show an interaction description (e.g. picking something up, examining an object)
    {
        PrintLine("> " + desc); // console-style prefix to visually distinguish interaction text from dialogue
    }

    public void SetObjectiveStatus(string text) // creates (first call) or updates (every call after) the pinned status line, e.g. a clue counter
    {
        objectiveStatusLine = text; // overwrite in place, instantly, no typewriter, so counting up doesn't feel laggy
        RefreshDisplay(); // redraw immediately; does not touch or interrupt the current dialogue line
    }

    private IEnumerator ProcessQueue() // works through pendingLines one at a time so lines never overlap
    {
        while (pendingLines.Count > 0) // keep going until the queue is drained
        {
            string nextLine = pendingLines.Dequeue(); // pull the next line to type
            yield return StartCoroutine(TypeLine(nextLine)); // wait for it to finish typing before moving on
        }

        typingRoutine = null; // clear the reference now that the queue is empty
    }

    private IEnumerator TypeLine(string line) // clears the current dialogue line, then reveals the new one's characters one at a time
    {
        currentDialogueLine = ""; // wipe the previous line immediately - old text disappears the instant the new one starts
        RefreshDisplay(); // show the now-empty dialogue line right away

        StringBuilder builder = new StringBuilder(); // accumulates the in-progress line's revealed characters

        foreach (char nextChar in line) // step through each character in order
        {
            builder.Append(nextChar); // reveal the next character
            currentDialogueLine = builder.ToString(); // update what's currently shown
            RefreshDisplay(); // redraw with the line still in progress
            yield return new WaitForSeconds(secondsPerCharacter); // wait before revealing the next one
        }

        currentDialogueLine = line; // ensure the fully revealed line is exactly what was requested
        RefreshDisplay(); // final redraw, line stays on screen until the next PrintLine call replaces it
    }

    private void RefreshDisplay() // rebuilds the visible text from the pinned status line plus the current dialogue line
    {
        if (logText == null) // guard in case it's unassigned
        {
            return; // nothing to draw to
        }

        StringBuilder display = new StringBuilder(); // holds the full text to show this frame

        if (!string.IsNullOrEmpty(objectiveStatusLine)) // only show the status row once it's been set at least once
        {
            display.AppendLine(objectiveStatusLine); // pinned status line first
            display.AppendLine(); // blank separator row between the status and the current dialogue line
        }

        display.Append(currentDialogueLine); // the single "now showing" dialogue/interaction line

        logText.text = display.ToString(); // push the combined text to the label
        ScrollToBottom(); // no-op unless a ScrollRect is assigned
    }

    private void ScrollToBottom() // snaps the scroll view to show the newest content, if a ScrollRect is assigned
    {
        if (scrollRect == null) // optional, most setups won't need this since there's no history to scroll through
        {
            return; // nothing to scroll
        }

        Canvas.ForceUpdateCanvases(); // force layout to rebuild before reading/setting scroll position
        scrollRect.verticalNormalizedPosition = 0f; // 0 = bottom, since TMP content typically grows downward with a bottom-anchored pivot
    }
}
