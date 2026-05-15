
using UnityEngine;
using TMPro;


public class Drag3D : MonoBehaviour
{
    [Header("物体ID，用于匹配挂点")]
    public string itemID;

    private Vector3 startPosition;
    private bool dragging = false;
    private WallSlot currentSlot = null;

    private Collider col;

    void Awake()
    {
        startPosition = transform.position;
        col = GetComponent<Collider>();

        if (col == null)
        {
            Debug.LogError($"[{gameObject.name}] 缺少Collider组件！");
        }
    }

    void OnMouseDown()
    {
        dragging = true;

        // 如果已经占用挂点，释放挂点
        if (currentSlot != null)
        {
            currentSlot.occupied = false;
            currentSlot = null;
        }
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = worldPos;
    }

    void OnMouseUp()
    {
        dragging = false;

        // 找最近的挂点
        WallSlot slot = FindNearestSlot();

        // 判断能否放置
        bool canPlace = slot != null && !slot.occupied && !IsOverlapping(slot.transform.position);

        if (canPlace)
        {
            SnapToSlot(slot);
            currentSlot = slot;
            slot.occupied = true;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowText("✔ 放置成功");
            }
        }
        else
        {
            ReturnToStart();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowText("❌ 不能放这里");
            }
        }
    }

    private void SnapToSlot(WallSlot slot)
    {
        transform.position = slot.transform.position;
    }

    private void ReturnToStart()
    {
        transform.position = startPosition;
    }

    // 检测是否与其他物体重叠
    private bool IsOverlapping(Vector3 targetPos)
    {
        if (col == null) return false;

        Vector3 halfSize = col.bounds.extents;

        // 检测重叠物体
        Collider[] hits = Physics.OverlapBox(
            targetPos,
            halfSize,
            transform.rotation
        );

        foreach (var hit in hits)
        {
            // 排除自己
            if (hit.gameObject != gameObject && hit.GetComponent<Drag3D>() != null)
            {
                return true;
            }
        }

        return false;
    }

    // 找最近的挂点
    private WallSlot FindNearestSlot()
    {
        WallSlot[] slots = FindObjectsOfType<WallSlot>();
        WallSlot nearest = null;
        float minDist = float.MaxValue;

        foreach (var slot in slots)
        {
            float dist = Vector3.Distance(transform.position, slot.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = slot;
            }
        }

        // 可以设置阈值，超出就返回null
        if (minDist > 2f) // 这里2f可以调节挂点吸附距离
            return null;

        return nearest;
    }
}