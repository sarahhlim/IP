using System.Collections; // for IEnumerator
using TMPro; // for TextMeshProUGUI
using UnityEngine; // for MonoBehaviour, Coroutine, WaitForSeconds

// Reveals a TextMeshProUGUI's text one character at a time, reusable across any HUD label
public class TypewriterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText; // the label this component animates
    [SerializeField] private float secondsPerCharacter = 0.03f; // delay between each revealed character, tweak per label in Inspector

    private Coroutine typingRoutine; // reference to the running coroutine so it can be stopped/restarted
    private string fullText; // the complete string being typed out, kept so Skip() can jump straight to it

    private void Awake()
    {
        if (targetText == null) // fallback lookup if no explicit reference was assigned in Inspector
        {
            targetText = GetComponentInChildren<TextMeshProUGUI>(); // find the TMP label on this object or its children
        }
    }

    public void PlayText(string newText) // call this instead of setting .text directly to get the typewriter effect
    {
        fullText = newText; // store the target string so Skip() can snap to it later

        if (typingRoutine != null) // stop any in-progress typing before starting a new line
        {
            StopCoroutine(typingRoutine); // cancel the previous coroutine
        }

        typingRoutine = StartCoroutine(TypeRoutine(newText)); // begin revealing the new text
    }

    private IEnumerator TypeRoutine(string textToType) // reveals characters one at a time
    {
        targetText.text = ""; // clear the label before typing starts
        print("Typewriter started: " + textToType); // log the start of this line

        foreach (char nextChar in textToType) // step through each character in order
        {
            targetText.text += nextChar; // append the next character
            yield return new WaitForSeconds(secondsPerCharacter); // wait before revealing the next one
        }

        typingRoutine = null; // clear the reference now that typing finished naturally
        print("Typewriter finished"); // log completion
    }

    public void Skip() // instantly completes the current line, e.g. bound to a player key to skip ahead
    {
        if (typingRoutine == null) // nothing to skip if not currently typing
        {
            return; // exit early
        }

        StopCoroutine(typingRoutine); // stop the in-progress reveal
        typingRoutine = null; // clear the reference
        targetText.text = fullText; // snap straight to the full text
        print("Typewriter skipped"); // log the skip
    }

    public bool IsTyping() // exposed so other scripts can check before allowing input to advance
    {
        return typingRoutine != null; // true while a coroutine is actively revealing characters
    }
}