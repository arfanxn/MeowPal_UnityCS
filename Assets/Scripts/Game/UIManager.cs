using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public PlayerController playerController;
    public FoodPlate foodPlate;
    public Bed bed;

    public GameObject pauseMenuPanel;

    private void Start()
    {
        if (playerController == null || foodPlate == null || bed == null)
        {
            Debug.LogError("UIManager missing references!");
            return;
        }
        playerController.SetDependencies(foodPlate, bed);

        pauseMenuPanel?.SetActive(false);
    }

    public void EatBtnOnClick()
    {
        if (playerController == null || foodPlate == null) return;
        StartCoroutine(playerController.StartEating());
    }

    public void SleepBtnOnClick()
    {
        if (playerController == null || bed == null) return;
        playerController?.StartSleeping();
    }

    public void PauseBtnOnClick()
    {
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ResumeBtnOnClick()
    {
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void MenuBtnOnClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}