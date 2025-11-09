using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    [Header("Settings")]
    public Camera playerCamera;                 // Reference to the player camera
    public GameObject floatingTextPrefab;       // 3D TextMeshPro prefab
    public Vector3 textOffset = new Vector3(0, 0.5f, 0); // Offset above object
    public float textLifetime = 0f;             // 0 = infinite until look away

    private GameObject currentText;
    private Transform currentTarget;

    /// <summary>
    /// Call this to show floating text above an object
    /// </summary>
    public void ShowText(Transform target, string label)
    {
        if (target == null || string.IsNullOrEmpty(label) || floatingTextPrefab == null)
            return;

        // Only create new text if target changed
        if (currentTarget != target)
        {
            ClearText();

            currentText = Instantiate(floatingTextPrefab);
            if (currentText.TryGetComponent<TextMeshPro>(out TextMeshPro tmp))
            {
                tmp.text = label;
            }

            currentText.transform.position = GetTextPosition(target);
            currentText.transform.SetParent(target); // follow target
            currentTarget = target;

            if (textLifetime > 0)
                Destroy(currentText, textLifetime);
        }
    }

    /// <summary>
    /// Call this to hide text when player looks away
    /// </summary>
    public void ClearText()
    {
        if (currentText != null)
        {
            Destroy(currentText);
            currentText = null;
            currentTarget = null;
        }
    }

    /// <summary>
    /// Updates the text every frame to face the player
    /// </summary>
    void LateUpdate()
    {
        if (currentText != null && playerCamera != null)
        {
            // Make the text face the camera
            Vector3 direction = currentText.transform.position - playerCamera.transform.position;
            currentText.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// Returns a position above the target using Renderer bounds if available
    /// </summary>
    private Vector3 GetTextPosition(Transform target)
    {
        // Get all renderers on this object and its children
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return target.position + textOffset;

        // Calculate combined bounds
        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        // Return top-center plus offset
        return combinedBounds.center + new Vector3(0, combinedBounds.extents.y, 0) + textOffset;
    }
}
