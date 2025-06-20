using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public PlayerController playerController;
    public FoodPlate foodPlate;
    public Bed bed;

    public Slider sleepySlider;
    public Slider hungerSlider;

    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    private void Start()
    {
        if (
            pauseMenuPanel == null
            || playerController == null
            || foodPlate == null
            || bed == null
            || sleepySlider == null
            || hungerSlider == null
        )
        {
            Debug.LogError("UIManager missing references!");
            return;
        }

        pauseMenuPanel?.SetActive(false);
    }

    public void DanceBtnClick()
    {
        StartCoroutine(playerController.StartDancing());
    }

    public void EatBtnOnClick()
    {
        StartCoroutine(playerController.StartEating(foodPlate.eatPosition));
    }

    public void SleepBtnOnClick()
    {
        StartCoroutine(playerController?.StartSleeping(bed.sleepPosition));
    }

    public void PauseBtnOnClick()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeBtnOnClick()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuBtnOnClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        sleepySlider.value = playerController.GetSleepyStatValue();
        hungerSlider.value = playerController.GetHungerStatValue();

        if (playerController.GetIsDead() && !gameOverPanel.activeSelf) {
            StartCoroutine(HandleGameOver());
        }
    }

    private IEnumerator HandleGameOver()
    {
        yield return new WaitForSeconds(3f);
        gameOverPanel.SetActive(true);
    }

}