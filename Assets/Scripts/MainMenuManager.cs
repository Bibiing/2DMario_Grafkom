using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header ("Canvas Menu")]
    public Canvas mainMenuCanvas;
    public Canvas settingsCanvas;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI JumpText;
    //public GameObject player;
    public UnityEngine.UI.Slider speedSlider;
    public UnityEngine.UI.Slider jumpSlider;

    private PlayerMovement playerMovement;

    private void Start()
    {
        SceneManager.UnloadSceneAsync("Mario");
        mainMenuCanvas.gameObject.SetActive(true);
        settingsCanvas.gameObject.SetActive(false);
        //playerMovement = player.GetComponent<PlayerMovement>();
        speedSlider.onValueChanged.AddListener(ChangeSpeed);
        jumpSlider.onValueChanged.AddListener(ChangeJump);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Mario");
    }

    public void LoadSettings()
    {
        Debug.Log("Memuat Pengaturan...");
        mainMenuCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Keluar Game!");
    }

    public void ChangeSpeed(float speed)
    {
        Debug.Log("Mengubah Kecepatan: " + speed);
        SpeedText.text = "Speed: " + speed.ToString("0");
        //playerMovement.moveSpeed = speed;
    }

    public void ChangeJump(float jump)
    {
        Debug.Log("Mengubah Lompatan: " + jump);
        JumpText.text = "Jump: " + jump.ToString("0");
        //playerMovement.jumpForce = jump;
    }

    public void BackToMainMenu()
    {
        settingsCanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }
}