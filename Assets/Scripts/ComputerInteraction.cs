using UnityEngine; // needed for MonoBehaviour, SerializeField, Cursor
using UnityEngine.InputSystem; // needed for Keyboard, New Input System
using Unity.Cinemachine; // matches your installed package version

public class ComputerInteraction : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera computerVCam; // drag ComputerViewCamera here
    [SerializeField] private GameObject computerCanvas; // drag your Canvas here
    [SerializeField] private Behaviour playerMovementScript; // drag FirstPersonController component here

    private bool playerInRange;
    private bool isUsingComputer;

    void Update()
    {
        if (playerInRange && !isUsingComputer && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenComputer();
        }
        else if (isUsingComputer && Keyboard.current.escapeKey.wasPressedThisFrame) // temporary fallback
        {
            CloseComputer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    private void OpenComputer()
    {
        isUsingComputer = true;
        if (computerVCam != null) computerVCam.Priority = 20;
        if (computerCanvas != null) computerCanvas.SetActive(true);
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        ComputerScreenManager.instance.ShowMainMenu();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseComputer() // hook to X button OnClick, also called by OnDeucePressed()
    {
        isUsingComputer = false;
        if (computerVCam != null) computerVCam.Priority = 5;
        if (computerCanvas != null) computerCanvas.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}