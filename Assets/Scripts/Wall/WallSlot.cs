using UnityEngine;

public class WallSlot : MonoBehaviour
{
    public bool occupied = false;
    public string allowedItemID; // 可选，用于关卡规则

    [HideInInspector] public Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    public void Highlight(Color color)
    {
        if (rend != null)
            rend.material.color = color;
    }

    public void ResetHighlight()
    {
        if (rend != null)
            rend.material.color = originalColor;
    }
}