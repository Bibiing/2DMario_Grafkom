using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance { get; private set; }

    [Tooltip("UI Text that displays the current points.")]
    public Text pointsText;

    public int Points { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        UpdateHUD();
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;

        Points += amount;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (pointsText != null)
            pointsText.text = Points.ToString();
    }
}