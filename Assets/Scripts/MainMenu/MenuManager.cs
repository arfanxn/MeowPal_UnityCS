using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed!"); 
        Application.Quit();
    }
}