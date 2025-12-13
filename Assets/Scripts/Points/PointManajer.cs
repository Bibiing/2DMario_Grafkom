using UnityEngine;
using TMPro; // Wajib untuk TextMeshPro
using System.Collections;

public class PointManager : MonoBehaviour
{
    // SINGLETON: Agar bisa dipanggil dari mana saja dengan mudah
    public static PointManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Tarik Text Skor ke sini

    [Header("Settings")]
    public int currentScore = 0;
    public string prefix = "x "; // Tulisan sebelum angka (misal "x 100")

    void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    // Fungsi ini dipanggil oleh script Poin.cs Anda
    public void AddPoints(int amount)
    {
        currentScore += amount;
        UpdateUI();

        // Efek Animasi "Pop" (Membesar sebentar) agar menarik
        if (scoreText != null)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateTextBump());
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = prefix + currentScore.ToString("D1"); // D3 artinya format 3 digit (001, 002)
    }

    // Animasi Text Membesar (Juice Effect)
    IEnumerator AnimateTextBump()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 jumpScale = Vector3.one * 1.5f; // Membesar 1.5x

        float duration = 0.1f;
        float elapsed = 0f;

        // Membesar
        while (elapsed < duration)
        {
            scoreText.transform.localScale = Vector3.Lerp(originalScale, jumpScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Kembali Normal
        elapsed = 0f;
        while (elapsed < duration)
        {
            scoreText.transform.localScale = Vector3.Lerp(jumpScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.transform.localScale = originalScale;
    }
}