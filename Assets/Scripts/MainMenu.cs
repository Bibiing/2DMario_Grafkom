using UnityEngine;
using UnityEngine.SceneManagement; // Wajib untuk urusan Scene

public class GameManager : MonoBehaviour
{
    [Header("PAUSE MENU SETTINGS")]
    [Tooltip("Panel Pause. kosong jika di Main Menu.")]
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
        // pastikan panel pause mati dan waktu berjalan normal.
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    void Update()
    {
        if (pausePanel != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    //  START (Main Menu)
    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //  RESTART (Pause Menu / Game Over)
    public void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //  MAIN MENU (Pause Menu)
    public void BackToMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0); 
    }

    //  QUIT (Main Menu)
    public void QuitGame()
    {
        Debug.Log("Keluar Game...");
        Application.Quit();
    }

    // to MainMenu from Pause Menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true); 
        Time.timeScale = 0f;       
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); 
        Time.timeScale = 1f;        
        isPaused = false;
    }
}