using UnityEngine;

/// <summary>Rejects the padded rectangle above a point-up terrain tile during UI raycasts.</summary>
public sealed class HexSlotHitArea : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        var rect = (RectTransform)transform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, eventCamera, out var local)) return false;
        float size = rect.rect.width;
        float y = Mathf.Abs(local.y);
        if (y > size * 0.5f) return false;
        return Mathf.Abs(local.x) <= Mathf.Min(size * 0.5f, (size * 0.5f - y) * 2);
    }
}
