using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SideAdjust : MonoBehaviour
{
    public List<Sprite> sprites;
    public float width = 0.04f;
    [Range(0, 3)]
    public int orientation;

    private GameObject sprite;
    private GameObject position;
    float baseSize;

    public void AdjustAndSlicePosition()
    {
        AdjustPosition();
        // Subtract box collider from the parent's polygon collider
        SubtractBoxColliderFromPolygon();

    }
    public void AdjustPosition()
    {
        foreach (Transform obj in transform)
        {
            DestroyImmediate(obj.gameObject);
        }
        DestroyImmediate(sprite);
        DestroyImmediate(position);

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(0.16f, 0.16f); // Default size
        boxCollider.offset = Vector2.zero; // Default offset

        sprite = new GameObject();
        position = new GameObject();
        sprite.name = "DoorSprite";
        position.name = "DoorPosition";
        sprite.transform.SetParent(transform, false);
        position.transform.SetParent(transform, false);
        sprite.AddComponent<SpriteRenderer>();
        position.AddComponent<SpriteRenderer>();

        SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        baseSize = boxCollider.size.y;

        switch (orientation)
        {
            case 1:
                spriteRenderer.sprite = sprites[0];
                spriteRenderer.flipX = true;
                AdjustSize(sprite.transform, position.transform, new Vector3(boxCollider.size.x / 2 + 0.05f, 0, 0));
                boxCollider.size = new Vector2(width, baseSize);
                boxCollider.offset = new Vector2((baseSize / 2) + width / 2, 0);
                break;
            case 3:
                spriteRenderer.sprite = sprites[0];
                AdjustSize(sprite.transform, position.transform, new Vector3(-boxCollider.size.x / 2 - 0.05f, 0, 0));
                boxCollider.size = new Vector2(width, baseSize);
                boxCollider.offset = new Vector2(-((baseSize / 2) + width / 2), 0);
                break;
            case 0:
                spriteRenderer.sprite = sprites[1];
                AdjustSize(sprite.transform, position.transform, new Vector3(0, boxCollider.size.y / 2, 0));
                boxCollider.size = new Vector2(baseSize, width);
                boxCollider.offset = new Vector2(0, (baseSize / 2) + width / 2);
                break;
            case 2:
                spriteRenderer.sprite = sprites[1];
                AdjustSize(sprite.transform, position.transform, new Vector3(0, -boxCollider.size.y / 2, 0));
                boxCollider.size = new Vector2(baseSize, width);
                boxCollider.offset = new Vector2(0, -((baseSize / 2) + width / 2));

                Color color = spriteRenderer.color;
                color.a = 0.25f;
                spriteRenderer.color = color;
                break;
        }
        GetComponent<SpriteRenderer>().enabled = false;
    }
    private void SubtractBoxColliderFromPolygon()
    {
        PolygonCollider2D polygonCollider = GetComponentInParent<RoomController>().border;
        Vector3 lastPosition = transform.parent.transform.position;
        Vector3 parent = transform.parent.transform.position;

        Vector2[] originalPoints = polygonCollider.points;
        List<Vector2> newPoints = new List<Vector2>();

        Bounds boxBounds = GetComponent<BoxCollider2D>().bounds;

        // Convert original points to world coordinates
        Vector2[] worldPoints = new Vector2[originalPoints.Length];
        for (int i = 0; i < originalPoints.Length; i++)
        {
            worldPoints[i] = polygonCollider.transform.TransformPoint(originalPoints[i]);
        }

        // Calculate points around the box collider
        int xCon = parent.x > 0 ? -1 : 1;
        int yCon = parent.y > 0 ? -1 : 1;
        Vector2 topLeft = new Vector2(boxBounds.min.x + xCon * parent.x, boxBounds.max.y + yCon * parent.y);
        Vector2 topRight = new Vector2(boxBounds.max.x + xCon * parent.x, boxBounds.max.y + yCon * parent.y);
        Vector2 bottomLeft = new Vector2(boxBounds.min.x + xCon * parent.x, boxBounds.min.y + yCon * parent.y);
        Vector2 bottomRight = new Vector2(boxBounds.max.x + xCon * parent.x, boxBounds.min.y + yCon * parent.y);

        bool added = false;

        // Add points to newPoints if they are outside the box collider
        for (int i = 0; i < polygonCollider.points.Length; i++)
        {
            if (!boxBounds.Contains(polygonCollider.points[i]))
            {
                newPoints.Add(polygonCollider.points[i]);
                if (!added && i< polygonCollider.points.Length-1)
                {
                    switch (orientation)
                    {
                        case 1:
                            if (Mathf.Round(topLeft.x * 1000f) / 1000f == Mathf.Round(polygonCollider.points[i].x * 1000f) / 1000f && topLeft.y < polygonCollider.points[i].y && bottomLeft.y > polygonCollider.points[i + 1].y)
                            {
                                newPoints.Add(topLeft);
                                newPoints.Add(topRight);
                                newPoints.Add(bottomRight);
                                newPoints.Add(bottomLeft);
                                added = true;
                            }
                            break;
                        case 3:
                            if (Mathf.Round(bottomRight.x * 1000f) / 1000f == Mathf.Round(polygonCollider.points[i].x * 1000f) / 1000f && bottomRight.y > polygonCollider.points[i].y && topRight.y < polygonCollider.points[i + 1].y)
                            {
                                newPoints.Add(bottomRight);
                                newPoints.Add(bottomLeft);
                                newPoints.Add(topLeft);
                                newPoints.Add(topRight);
                                added = true;
                            }
                            break;
                        case 0:
                            if (Mathf.Round(bottomLeft.y * 1000f) / 1000f == Mathf.Round(polygonCollider.points[i].y * 1000f) / 1000f && bottomLeft.x > polygonCollider.points[i].x && bottomRight.x < polygonCollider.points[i + 1].x)
                            {
                                newPoints.Add(bottomLeft);
                                newPoints.Add(topLeft);
                                newPoints.Add(topRight);
                                newPoints.Add(bottomRight);
                                added = true;
                            }
                            break;
                        case 2:
                            if (Mathf.Round(topRight.y * 1000f) / 1000f == Mathf.Round(polygonCollider.points[i].y * 1000f) / 1000f && topRight.x < polygonCollider.points[i].x && topLeft.x > polygonCollider.points[i + 1].x)
                            {
                                newPoints.Add(topRight);
                                newPoints.Add(bottomRight);
                                newPoints.Add(bottomLeft);
                                newPoints.Add(topLeft);
                                added = true;
                            }
                            break;
                    }
                }
            }
        }

        // Set the new points to the polygon collider
        polygonCollider.points = newPoints.ToArray();
    }


    public void AdjustSize(Transform sprite, Transform anchor, Vector3 position)
    {
        sprite.transform.localPosition = position;
        anchor.transform.localPosition = -position;
    }
}
