using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Magma : MonoBehaviour
{
    [Tooltip("Tag that identifies the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Optional UI panel (Canvas) to show when player dies. Assign a panel with Retry and Main Menu buttons.")]
    public GameObject gameOverUIPanel;

    [Tooltip("Name of the Main Menu scene to load. If empty, scene index 0 will be loaded.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Delay in seconds before showing the Game Over UI (gives time for death animation).")]
    public float delayBeforeGameOverUI = 0.5f;

    [Tooltip("If true, the magma will only trigger once.")]
    public bool triggerOnce = true;

    bool triggered;

    // 3D trigger/collision
    void OnTriggerEnter(Collider other) => HandleHit(other.gameObject);
    void OnCollisionEnter(Collision collision) => HandleHit(collision.gameObject);

    // 2D trigger/collision
    void OnTriggerEnter2D(Collider2D other) => HandleHit(other.gameObject);
    void OnCollisionEnter2D(Collision2D collision) => HandleHit(collision.gameObject);

    void HandleHit(GameObject other)
    {
        if (triggered && triggerOnce)
            return;

        if (!other.CompareTag(playerTag))
            return;

        triggered = true;

        // Notify player script(s) about death (optional)
        other.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
        other.SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        // Stop player movement and interactions
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

        StartCoroutine(ShowGameOverUI());
    }

    IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delayBeforeGameOverUI));

        // If a UI panel is provided, show it and pause the game.
        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(true);

            // Make sure cursor is visible/unlocked for UI navigation
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Pause gameplay (UI buttons should operate on unscaled time or call scene loads which reset timeScale)
            Time.timeScale = 0f;

            yield break; // do not auto-reload scene
        }

        // Fallback: if no UI, reload current scene (preserves previous behaviour)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Public UI callbacks - hook these to your Retry and Main Menu buttons.
    public void OnRetry()
    {
        // Restore time scale in case it was paused
        Time.timeScale = 1f;
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName) && Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        // Fallback to scene index 0
        SceneManager.LoadScene(0);
    }

    // Optional: allow other scripts to cancel the pending trigger (useful for level editors/tests)
    public void ResetTrigger()
    {
        triggered = false;
    }
}
