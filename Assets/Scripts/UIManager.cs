using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Reference to PlayerController
    public PlayerController playerController;

    // Add more functions for other interactions, like sleeping, playing, etc.
    public void SleepPlayer()
    {
        if (playerController != null)
        {
            playerController.StartSleeping();  // Assuming the Sleep method exists in PlayerController
        }
    }
}
