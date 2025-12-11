using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [Tooltip("Tag that identifies the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Name of the Main Menu scene to load. If empty or unavailable, scene index 0 will be loaded.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Delay in seconds before returning to the main menu (allows win animation/sfx).")]
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

        StartCoroutine(TransitionToMainMenu());
    }

    IEnumerator TransitionToMainMenu()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delayBeforeReturn));

        if (!string.IsNullOrEmpty(mainMenuSceneName) && Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            yield break;
        }

        // Fallback: load scene index 0
        SceneManager.LoadScene(0);
    }
}
