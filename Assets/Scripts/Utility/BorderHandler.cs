using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class BorderHandler : MonoBehaviour
{
    public float width = 0.1f;
    private GameObject ground;
    public float offset = 0.5f;

    // Method to recreate the border
    public void RecreateBorder()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        if (GetComponentInParent<RoomController>().border != null)
        {
            DestroyImmediate(GetComponentInParent<RoomController>().border.gameObject);
        }
        CreateOrUpdateBorder();
        AdjustWall();
        AdjustGround();
        AddPolygonCollider();
        GetComponent<SpriteRenderer>().color =new Color(1f,1f,1f,0);
    }

    // Method to create or update the border colliders
    private void CreateOrUpdateBorder()
    {
        if (ground == null)
        {
            ground = new GameObject();
            ground.name = "RoomBorder";
            ground.transform.position = Vector3.zero;
            ground.tag = "Wall";
            ground.layer = 3;
            ground.transform.SetParent(transform, false);
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is missing on this GameObject.");
            return;
        }
        Vector2 spriteSize = spriteRenderer.bounds.size;

        PolygonCollider2D polygonCollider = ground.AddComponent<PolygonCollider2D>();

        Vector2[] points = new Vector2[10];
        points[0] = new Vector2((spriteSize.x / 2 + width), -(spriteSize.y / 2 + width));
        points[1] = new Vector2((spriteSize.x / 2 + width), (spriteSize.y / 2 + width));
        points[2] = new Vector2(-(spriteSize.x / 2 + width), (spriteSize.y / 2 + width));
        points[3] = new Vector2(-(spriteSize.x / 2 + width), -(spriteSize.y / 2 + width));
        points[4] = new Vector2((spriteSize.x / 2 + width), -(spriteSize.y / 2 + width));
        points[5] = new Vector2((spriteSize.x / 2 + width), -(spriteSize.y / 2));
        points[6] = new Vector2(-(spriteSize.x / 2), -(spriteSize.y / 2));
        points[7] = new Vector2(-(spriteSize.x / 2), (spriteSize.y / 2));
        points[8] = new Vector2((spriteSize.x / 2), (spriteSize.y / 2));
        points[9] = new Vector2((spriteSize.x / 2), -(spriteSize.y / 2 + width));

        polygonCollider.points = points;

        GetComponentInParent<RoomController>().border = polygonCollider;
    }

    // Method to adjust the "Wall" child object
    public void AdjustWall()
    {
        Transform wallTransform = transform.Find("Wall");
        if (wallTransform == null)
        {
            Debug.LogWarning("No child object named 'Wall' found.");
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is missing on this GameObject.");
            return;
        }

        Vector2 spriteSize = spriteRenderer.bounds.size;

        // Set size and position of the "Wall" child object
        wallTransform.gameObject.GetComponent<SpriteRenderer>().size = new Vector3(spriteSize.x + 0.04f, offset);
        wallTransform.position = new Vector3(transform.position.x, transform.position.y + spriteSize.y / 2 + offset / 2, transform.position.z);
    }
    public void AdjustGround()
    {
        Transform groundTransform = transform.Find("Ground");
        if (groundTransform == null)
        {
            Debug.LogWarning("No child object named 'Ground' found.");
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is missing on this GameObject.");
            return;
        }

        Vector2 spriteSize = spriteRenderer.bounds.size;

        // Set size and position of the "Wall" child object
        groundTransform.gameObject.GetComponent<SpriteRenderer>().size = new Vector3(spriteSize.x + 0.04f, spriteSize.y + 0.04f);
        groundTransform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Method to add and configure the PolygonCollider2D
    public void AddPolygonCollider()
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        if (polygonCollider == null)
        {
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        }
        polygonCollider.isTrigger = true;

        // Get the size of the SpriteRenderer and calculate half sizes
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is missing on this GameObject.");
            return;
        }

        Vector2 spriteSize = spriteRenderer.bounds.size;
        float halfWidth = spriteSize.x / 2;
        float halfHeight = spriteSize.y / 2;

        // Get the height of the "Wall" child object
        Transform wallTransform = transform.Find("Wall");
        float wallHeight = offset;  // Default wall height if wall object is not found
        if (wallTransform != null)
        {
            wallHeight = wallTransform.gameObject.GetComponent<SpriteRenderer>().size.y;
        }

        // Define the 4 points for the polygon collider, adjusting the top points for wall height
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(-halfWidth, halfHeight + wallHeight);
        points[1] = new Vector2(-halfWidth, -halfHeight);
        points[2] = new Vector2(halfWidth, -halfHeight);
        points[3] = new Vector2(halfWidth, halfHeight + wallHeight);

        // Set the points to the collider
        polygonCollider.points = points;
    }
}
