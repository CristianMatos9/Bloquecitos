using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MoveBlack : MonoBehaviour
{
    private Vector3 offset;
    public bool isDragging = false;
    private Vector2 startPosition;
    private List<Vector2> blackBlocks = new List<Vector2>();
    private GameObject[] allCells;
    private Vector2 gridSize = new Vector2(0.64f, 0.64f);

    void Start()
    {
        startPosition = transform.position;

        foreach (Transform child in transform)
        {
            blackBlocks.Add(child.localPosition);
        }
        allCells = GameObject.FindGameObjectsWithTag("White");
    }

    void Update()
    {
        PhoneMovement();
    }

    private void OnMouseDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z) + offset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        TrySnap();
    }

    private void PhoneMovement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsTouchingObject(touchPosition))
                    {
                        offset = transform.position - new Vector3(touchPosition.x, touchPosition.y, transform.position.z);
                        isDragging = true;
                    }
                    break;
                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        transform.position = new Vector3(touchPosition.x, touchPosition.y, transform.position.z) + offset;
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    TrySnap();
                    break;
            }
        }
    }

    private bool IsTouchingObject(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    void TrySnap()
    {
        GameObject bestCell = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject cell in allCells)
        {
            float distance = Vector2.Distance(transform.position, cell.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                bestCell = cell;
            }
        }

        if (bestCell != null)
        {
            Vector2 targetPosition = bestCell.transform.position;

            targetPosition.x = Mathf.Round(targetPosition.x / gridSize.x) * gridSize.x;
            targetPosition.y = Mathf.Round(targetPosition.y / gridSize.y) * gridSize.y;

            if (CanFitInGrid(targetPosition))
            {
                transform.position = targetPosition;
            }
            else
            {
                Debug.Log("No cabe");
                transform.position = startPosition;
            }
        }
    }

    bool CanFitInGrid(Vector2 newPosition)
    {
        foreach (Vector2 localPos in blackBlocks)
        {
            Vector2 worldPos = newPosition + localPos;
            bool isOccupied = false;

            foreach (GameObject cell in allCells)
            {
                Vector2 cellPos = cell.transform.position;

                cellPos.x = Mathf.Round(cellPos.x / gridSize.x) * gridSize.x;
                cellPos.y = Mathf.Round(cellPos.y / gridSize.y) * gridSize.y;

                if (Vector2.Distance(cellPos, worldPos) < 0.1f)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied)
            {
                return false;
            }
        }

        return true;
    }
}