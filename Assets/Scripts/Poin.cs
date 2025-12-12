using System.Collections.Generic;
using UnityEngine;

public class Poin : MonoBehaviour
{
    [Header("Settings")]
    public int points = 1;
    public Material newMaterial;
    public bool changeOnlyOnce = true;

    private static readonly HashSet<GameObject> activatedParents = new HashSet<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // trigger jika player sedang MELOMPAT NAIK (Velocity Y > 0)
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.y <= 0)
            return; // Jika sedang jatuh/diam, abaikan.

        // Blok Induk 
        GameObject parentBlock = transform.parent.gameObject;

        // apakah sudah pernah aktif
        if (changeOnlyOnce && activatedParents.Contains(parentBlock))
            return;

        Debug.Log("Sundulan Maut! Poin Bertambah.");

        // Tambah Poin 
        PointsManager.Instance?.AddPoints(points);

        ApplyMaterialToParent(parentBlock);

        // Tandai sudah aktif
        if (changeOnlyOnce)
            activatedParents.Add(parentBlock);
    }

    void ApplyMaterialToParent(GameObject parent)
    {
        if (newMaterial == null) return;

        // Cari Renderer di objek Induk 
        Renderer rend = parent.GetComponent<Renderer>();

        if (rend != null)
        {
            rend.material = newMaterial;
        }
    }

    // Reset (jika restart game)
    public static void ResetActivatedParents()
    {
        activatedParents.Clear();
    }
}