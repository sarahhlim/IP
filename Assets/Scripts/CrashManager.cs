using UnityEngine;

public class CrashManager : MonoBehaviour
{
    [Header("Car Objects")]
    public GameObject movingCar;
    public GameObject crashedCar;

    [Header("Aftermath Objects")]
    public GameObject deadVictim;
    public GameObject ghostVictim;
    public ParticleSystem fireParticleSystem;

    [Header("Items To Reveal")]
    public GameObject[] itemsToSpawn; // Array for your 4 items (and any false items)

    [Header("Audio")]
    public AudioClip crashSoundEffect;

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
        // 1. Play Audio
        if (crashSoundEffect != null)
        {
            AudioSource.PlayClipAtPoint(crashSoundEffect, transform.position);
        }

        // 2. Hide moving car & show crashed car
        if (movingCar != null) movingCar.SetActive(false);
        if (crashedCar != null) crashedCar.SetActive(true);

        // 3. Play Fire Particles
        if (fireParticleSystem != null)
        {
            fireParticleSystem.gameObject.SetActive(true);
            fireParticleSystem.Play();
        }

        // 4. Enable Victims
        if (deadVictim != null) deadVictim.SetActive(true);
        if (ghostVictim != null) ghostVictim.SetActive(true);

        // 5. Reveal Items on the road
        foreach (GameObject item in itemsToSpawn)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }

        // 6. Hide standing victim
        gameObject.SetActive(false);
    }
}