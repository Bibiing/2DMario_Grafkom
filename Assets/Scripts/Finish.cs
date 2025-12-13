using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Finish Line Tersentuh!");
            gameManager.PauseGame();
        }
    }
}