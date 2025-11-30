using System.Collections.Generic;
using UnityEngine;

public class Poin : MonoBehaviour
{
    [Tooltip("Points to add to the HUD when the Player hits this block from below.")]
    public int points = 1;

    [Tooltip("Material to apply to the parent when collected.")]
    public Material newMaterial;

    [Tooltip("If true, the block (parent) will only be activated once.")]
    public bool changeOnlyOnce = true;

    [Tooltip("If set, the script will look for a child with this name and change its renderer only.")]
    public string bottomChildName = "";

    [Tooltip("If >= 0, will replace only this material index on the parent's Renderer (useful for cubes with submeshes).")]
    public int bottomMaterialIndex = -1;

    // Static set so multiple child blocks don't give points repeatedly for the same parent
    private static readonly HashSet<GameObject> activatedParents = new HashSet<GameObject>();

    private Collider ownCollider;

    void Start()
    {
        ownCollider = GetComponent<Collider>();
    }

    // Uses 3D physics. Ensure Player is tagged "Player" and has a Rigidbody and Collider.
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        // Determine whether the player contacted this block from below.
        bool hitFromBelow = false;

        // 1) Check contact normals — robust for typical upward hits.
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                hitFromBelow = true;
                break;
            }
        }

        // 2) Fallback: check player's position relative to the block.
        if (!hitFromBelow)
        {
            var playerY = collision.transform.position.y;
            var blockY = transform.position.y;
            if (playerY < blockY - 0.1f)
                hitFromBelow = true;
        }

        if (!hitFromBelow)
            return;

        var parent = transform.parent != null ? transform.parent.gameObject : gameObject;

        if (changeOnlyOnce && activatedParents.Contains(parent))
            return;

        ApplyNewMaterialToParentBottomOnly(parent);
        PointsManager.Instance?.AddPoints(points);

        if (changeOnlyOnce)
            activatedParents.Add(parent);

        // Keep collider enabled so object remains solid.
    }

    private void ApplyNewMaterialToParentBottomOnly(GameObject parent)
    {
        if (newMaterial == null || parent == null)
            return;

        // 1) If bottomChildName provided, find that child (exact match)
        if (!string.IsNullOrEmpty(bottomChildName))
        {
            var bottomTransform = parent.transform.Find(bottomChildName);
            if (bottomTransform != null)
            {
                var rend = bottomTransform.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = newMaterial;
                    return;
                }
            }
            // try case-insensitive search among children
            foreach (Transform t in parent.transform)
            {
                if (t.name.ToLowerInvariant().Contains(bottomChildName.ToLowerInvariant()))
                {
                    var rend = t.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        rend.material = newMaterial;
                        return;
                    }
                }
            }
        }

        // 2) If bottomMaterialIndex set, replace only that index on the parent's Renderer
        if (bottomMaterialIndex >= 0)
        {
            var parentRenderer = parent.GetComponent<Renderer>();
            if (parentRenderer != null)
            {
                var mats = parentRenderer.materials;
                if (bottomMaterialIndex < mats.Length)
                {
                    mats[bottomMaterialIndex] = newMaterial;
                    parentRenderer.materials = mats;
                    return;
                }
            }

            // If not found on parent directly, try children
            foreach (var r in parent.GetComponentsInChildren<Renderer>())
            {
                var mats = r.materials;
                if (bottomMaterialIndex < mats.Length)
                {
                    mats[bottomMaterialIndex] = newMaterial;
                    r.materials = mats;
                    return;
                }
            }
        }

        // 3) Best-effort: find child renderers that are positioned at the bottom (lowest local Y)
        Renderer bottomRenderer = null;
        float lowestY = float.MaxValue;
        foreach (var r in parent.GetComponentsInChildren<Renderer>())
        {
            var localY = r.transform.localPosition.y;
            if (localY < lowestY)
            {
                lowestY = localY;
                bottomRenderer = r;
            }
        }
        if (bottomRenderer != null)
        {
            bottomRenderer.material = newMaterial;
            return;
        }

        // 4) Fallback: apply to all renderers (will change whole cube)
        Debug.LogWarning($"Poin: could not identify a single bottom face for '{parent.name}'. Falling back to changing all renderers. " +
                         "For precise per-face changes, either add a child GameObject for the bottom face and set `bottomChildName`, " +
                         "or use a mesh with separate submesh/material for the bottom face and set `bottomMaterialIndex`.");
        var renderers = parent.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
            r.material = newMaterial;
    }

    // Public helper to reset activation (useful for testing or level reset)
    public static void ResetActivatedParents()
    {
        activatedParents.Clear();
    }
}