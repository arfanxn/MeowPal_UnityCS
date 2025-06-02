using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public PlayerController playerController;

    public void GoToMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void menu()
    {
        Time.timeScale = 0;
    }

    public void resume()
    {
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Keluar dari aplikasi"); // Ini hanya muncul di editor, tidak di build
    }

    public void OnSleepButtonClicked()
    {
        playerController.Sleep();
    }

    public void OnEatButtonClicked()
    {
        playerController.Eat();
    }

    public void OnDanceButtonClicked()
    {
        playerController.Dance();
    }

    public void TestButton() { Debug.Log("Button works!"); }

}
