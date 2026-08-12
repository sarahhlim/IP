using UnityEngine; // needed for MonoBehaviour, Collider
using UnityEngine.SceneManagement; // needed for LoadScene

public class CaseEndZone : MonoBehaviour
{
    [SerializeField] private CaseData caseData; // drag THIS scene's matching CaseData asset
    [SerializeField] private string officeSceneName = "Office"; // exposed, not hardcoded

    void OnTriggerEnter(Collider other) // tag-based, matches your existing pattern
    {
        if (other.CompareTag("Player"))
        {
            print("Case scene ended, returning to office: " + caseData.caseID); // preferred logging
            SceneManager.LoadScene(officeSceneName); // teleport back
        }
    }
}