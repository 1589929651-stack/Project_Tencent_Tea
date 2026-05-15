
using UnityEngine;

public class Drag2DAs3D : MonoBehaviour
{
    [Header("物体ID，用于挂点匹配")]
    public string itemID;

    [Header("拖拽参数")]
    public float snapDistance = 2f;        // 吸附挂点的最大距离
    public float zScrollSpeed = 0.2f;      // 鼠标滚轮Z轴移动速度

    private Vector3 startPosition;
    private bool dragging = false;
    private WallSlot currentSlot = null;
    private Collider col;

    void Awake()
    {
        startPosition = transform.position;
        col = GetComponent<Collider>();

        if (col == null)
            Debug.LogError($"[{gameObject.name}] 缺少3D Collider组件！");
    }

    void Update()
    {
        HandleZScroll();
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
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // 保持与摄像机的深度
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.position = worldPos; // XYZ 都跟随鼠标
    }

    void OnMouseUp()
    {
        dragging = false;

        WallSlot slot = FindNearestSlot();

        bool canPlace = slot != null && !slot.occupied && !IsOverlapping(slot.transform.position);

        if (canPlace)
        {
            SnapToSlot(slot);
            currentSlot = slot;
            slot.occupied = true;

            if (UIManager.Instance != null)
                UIManager.Instance.ShowText("✔ 放置成功");
        }
        else
        {
            ReturnToStart();
            if (UIManager.Instance != null)
                UIManager.Instance.ShowText("❌ 不能放这里");
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

    // 检测物体是否与其他物体重叠
    private bool IsOverlapping(Vector3 targetPos)
    {
        if (col == null) return false;

        Vector3 halfSize = col.bounds.extents;

        Collider[] hits = Physics.OverlapBox(targetPos, halfSize, transform.rotation);

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.GetComponent<Drag2DAs3D>() != null)
                return true;
        }

        return false;
    }

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

        if (minDist > snapDistance)
            return null;

        return nearest;
    }

    // 鼠标滚轮控制 Z 轴移动
    private void HandleZScroll()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            Vector3 pos = transform.position;
            pos.z += scroll * zScrollSpeed;
            transform.position = pos;
        }
    }

    // 可选：可视化OverlapBox检测范围
    private void OnDrawGizmosSelected()
    {
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}