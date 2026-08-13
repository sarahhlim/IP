using UnityEngine;

public class CrashManager : MonoBehaviour
{
    [Header("Car Objects")]
    public GameObject movingCar;
    public GameObject crashedCar;

    [Header("Aftermath Objects")]
    public GameObject deadVictim;
    public GameObject ghostVictim;
    public GameObject fireParticleSystem;

    [Header("Audio")]
    public AudioClip crashSoundEffect; // Using AudioClip prevents the sound from cutting off!

    private bool hasCrashed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasCrashed) return;

        if (other.CompareTag("Car"))
        {
            hasCrashed = true;
            TriggerCrashEvent();
        }
    }

    private void TriggerCrashEvent()
    {
        // 1. Play audio at the crash location (won't stop when object disappears)
        if (crashSoundEffect != null)
        {
            AudioSource.PlayClipAtPoint(crashSoundEffect, transform.position);
        }
        else
        {
            Debug.LogWarning("⚠️ Missing: Crash Sound Effect is not assigned in Inspector!");
        }

        // 2. Handle Car swapping
        if (movingCar != null) movingCar.SetActive(false);

        if (crashedCar != null) crashedCar.SetActive(true);

        // 3. Enable Aftermath Objects
        if (fireParticleSystem != null) fireParticleSystem.SetActive(true);
        else Debug.LogWarning("⚠️ Missing: Fire Particle System is not assigned in Inspector!");

        if (deadVictim != null) deadVictim.SetActive(true);
        else Debug.LogWarning("⚠️ Missing: Dead Victim is not assigned in Inspector!");

        if (ghostVictim != null) ghostVictim.SetActive(true);
        else Debug.LogWarning("⚠️ Missing: Ghost Victim is not assigned in Inspector!");

        // 4. Disable standing victim
        gameObject.SetActive(false);
    }
}