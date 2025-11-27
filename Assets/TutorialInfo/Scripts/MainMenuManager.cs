using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Mario");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Keluar Game!");
    }
}