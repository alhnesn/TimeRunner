using UnityEngine;

public class PlayerBoxBreaker : MonoBehaviour
{
    public float breakDistance = 1f;
    public Vector2 breakBoxSize = new Vector2(1f, 1.5f); // width, height of the box area
    public KeyCode breakKey = KeyCode.E;
    public LayerMask breakableLayer;

    void Update()
    {
        if (Input.GetKeyDown(breakKey))
        {
            TryBreakBoxes();
        }
    }

    void TryBreakBoxes()
    {
        // Determine direction (right if facing right, left if facing left)
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Center the box in front of the player
        Vector2 boxCenter = (Vector2)transform.position + direction * (breakDistance * 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, breakBoxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            BreakableBox box = hit.GetComponent<BreakableBox>();
            if (box != null)
            {
                box.Break();
            }
        }

        // Optional debug
        DebugDrawBox(boxCenter, breakBoxSize, Color.red, 0.2f);
    }

    void DebugDrawBox(Vector2 center, Vector2 size, Color color, float duration)
    {
        Vector2 half = size * 0.5f;
        Vector2 topLeft = center + new Vector2(-half.x, half.y);
        Vector2 topRight = center + new Vector2(half.x, half.y);
        Vector2 bottomLeft = center + new Vector2(-half.x, -half.y);
        Vector2 bottomRight = center + new Vector2(half.x, -half.y);

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }

}

