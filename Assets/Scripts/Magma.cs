using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Magma : MonoBehaviour
{
    [Tooltip("Tag that identifies the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Name of the Game Over scene to load. If empty, current scene will be reloaded.")]
    public string gameOverSceneName = "GameOver";

    [Tooltip("Delay in seconds before loading the Game Over scene (gives time for death animation).")]
    public float delayBeforeGameOver = 0.5f;

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

        // Try to notify player script(s) about death (optional receiver)
        other.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
        other.SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        // Try to disable common movement components so player stops moving immediately
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

        // Optionally disable player's colliders to avoid further physics interactions
        foreach (var c in other.GetComponentsInChildren<Collider>())
            c.enabled = false;
        foreach (var c2 in other.GetComponentsInChildren<Collider2D>())
            c2.enabled = false;

        StartCoroutine(TransitionToGameOver());
    }

    IEnumerator TransitionToGameOver()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delayBeforeGameOver));

        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            // If the named scene is not added to build settings, this will throw in editor/runtime.
            // Fallback to reloading current scene.
            if (Application.CanStreamedLevelBeLoaded(gameOverSceneName))
            {
                SceneManager.LoadScene(gameOverSceneName);
                yield break;
            }
        }

        // fallback: reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
