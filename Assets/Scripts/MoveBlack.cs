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

    public TargetRegionsManager regionsManager;
    private bool wasPlacedInRegion = false;
    [SerializeField] private AudioSource clip;

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

        if (regionsManager != null && wasPlacedInRegion)
        {
            wasPlacedInRegion = !regionsManager.RemoveBlocksFromRegion(gameObject);
        }

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
        if (isDragging)
        {
            isDragging = false;
            TrySnap();
        }
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

                        if (regionsManager != null && wasPlacedInRegion)
                        {
                            wasPlacedInRegion = !regionsManager.RemoveBlocksFromRegion(gameObject);
                        }

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
                    if (isDragging)
                    {
                        isDragging = false;
                        TrySnap();
                    }
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

            Vector3 originalPosition = transform.position;
            transform.position = targetPosition;

            if (CanFitInGrid(targetPosition))
            {
                if (regionsManager != null && regionsManager.TryAddBlocksToRegion(gameObject))
                {
                    wasPlacedInRegion = true;

                    transform.position = targetPosition;

                    if (clip != null)
                    {
                        clip.Play();
                    }

                    this.enabled = false;
                }
                else
                {
                    transform.position = startPosition;
                    wasPlacedInRegion = false;
                }
            }
            else
            {
                transform.position = startPosition;
                wasPlacedInRegion = false;
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

        if (!CheckOverlapWithOtherBlackBlocks(newPosition))
        {
            return false;
        }

        return true;
    }

    bool CheckOverlapWithOtherBlackBlocks(Vector2 newPosition)
    {
        GameObject[] allBlackBlocks = GameObject.FindGameObjectsWithTag("Black");

        foreach (Vector2 localPos in blackBlocks)
        {
            Vector2 worldPos = newPosition + localPos;

            worldPos.x = Mathf.Round(worldPos.x / gridSize.x) * gridSize.x;
            worldPos.y = Mathf.Round(worldPos.y / gridSize.y) * gridSize.y;

            foreach (GameObject blackBlock in allBlackBlocks)
            {
                if (blackBlock.transform.IsChildOf(transform))
                    continue;

                Vector2 otherBlockPos = blackBlock.transform.position;

                otherBlockPos.x = Mathf.Round(otherBlockPos.x / gridSize.x) * gridSize.x;
                otherBlockPos.y = Mathf.Round(otherBlockPos.y / gridSize.y) * gridSize.y;

                if (Vector2.Distance(worldPos, otherBlockPos) < 0.1f)
                {
                    Debug.Log("Colisión con otro bloque negro");
                    return false;
                }
            }
        }

        return true;
    }

    public void EnableMovement()
    {
        this.enabled = true;
    }
}