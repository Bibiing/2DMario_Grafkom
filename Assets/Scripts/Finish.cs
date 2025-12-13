using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [Tooltip("Tag that identifies the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Optional UI panel (Canvas) to show when player wins. Assign a panel with Continue/ Main Menu buttons.")]
    public GameObject winUIPanel;

    [Tooltip("Name of the Main Menu scene to load. If empty or unavailable, scene index 0 will be loaded.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Delay in seconds before showing the Win UI (allows win animation/sfx).")]
    public float delayBeforeReturn = 1f;

    [Tooltip("If true, finish will only trigger once.")]
    public bool triggerOnce = true;

    bool triggered;

    // 3D trigger/collision
    void OnTriggerEnter(Collider other) => HandleFinish(other.gameObject);
    void OnCollisionEnter(Collision collision) => HandleFinish(collision.gameObject);

    // 2D trigger/collision
    void OnTriggerEnter2D(Collider2D other) => HandleFinish(other.gameObject);
    void OnCollisionEnter2D(Collision2D collision) => HandleFinish(collision.gameObject);

    void HandleFinish(GameObject other)
    {
        if (triggered && triggerOnce)
            return;

        if (!other.CompareTag(playerTag))
            return;

        triggered = true;

        // Optional: notify player scripts about win
        other.SendMessage("Win", SendMessageOptions.DontRequireReceiver);
        other.SendMessage("OnWin", SendMessageOptions.DontRequireReceiver);

        // Stop player movement and interactions if possible
        var rb3 = other.GetComponent<Rigidbody>();
        if (rb3 != null)
        {
            rb3.linearVelocity = Vector3.zero;
            rb3.isKinematic = true;
        }
        var rb2 = other.GetComponent<Rigidbody2D>();
        if (rb2 != null)
        {
            rb2.linearVelocity = Vector2.zero;
            rb2.simulated = false;
        }
        var animator = other.GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;

        foreach (var c in other.GetComponentsInChildren<Collider>())
            c.enabled = false;
        foreach (var c2 in other.GetComponentsInChildren<Collider2D>())
            c2.enabled = false;

        StartCoroutine(ShowWinUIOrReturn());
    }

    IEnumerator ShowWinUIOrReturn()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delayBeforeReturn));

        // If a UI panel is provided, show it and pause the game.
        if (winUIPanel != null)
        {
            winUIPanel.SetActive(true);

            // Make cursor visible/unlocked for UI navigation
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pause gameplay so player can choose (buttons should use unscaled time if needed)
            Time.timeScale = 0f;
            yield break; // do not auto-load main menu
        }

        // Fallback: load main menu (preserve previous behaviour)
        if (!string.IsNullOrEmpty(mainMenuSceneName) && Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
            yield break;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // Public UI callbacks - hook these to your Continue/ Main Menu buttons.
    public void OnContinue()
    {
        Time.timeScale = 1f;
        // Default behavior: load main menu (or you can implement next level)
        if (!string.IsNullOrEmpty(mainMenuSceneName) && Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        SceneManager.LoadScene(0);
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName) && Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        SceneManager.LoadScene(0);
    }

    // Optional: allow other scripts to reset finish trigger (useful for testing)
    public void ResetTrigger()
    {
        triggered = false;
    }
}
