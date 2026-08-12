using UnityEngine; // needed for MonoBehaviour

public class PersistentPlayer : MonoBehaviour
{
    public static PersistentPlayer instance; // singleton

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // persists the whole rig - capsule, camera, brain, vcam together
        }
        else
        {
            Destroy(gameObject); // prevent duplicates if one already exists
        }
    }
}