using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetRegionsManager : MonoBehaviour
{
    [System.Serializable]
    public class TargetRegion
    {
        public GameObject targetCell;
        public int requiredBlocks;
        public List<Vector2> occupiedPositions = new List<Vector2>();
        public int currentBlocks;
        public Color regionColor;
    }

    public List<TargetRegion> targetRegions = new List<TargetRegion>();
    public GameObject blocksParent;
    public float gridSize = 0.64f;
    public TextMeshProUGUI debugText;

    private Dictionary<int, HashSet<Vector2>> regionGroups = new Dictionary<int, HashSet<Vector2>>();
    private Dictionary<Vector2, int> blockToRegion = new Dictionary<Vector2, int>();

    void Start()
    {
        for (int i = 0; i < targetRegions.Count; i++)
        {
            regionGroups[i] = new HashSet<Vector2>();
        }
    }

    public bool TryAddBlocksToRegion(GameObject blockStack)
    {
        List<Vector2> blockPositions = GetBlockPositions(blockStack);

        int bestRegionIndex = GetBestRegionForBlocks(blockPositions);

        if (bestRegionIndex == -1)
        {
            return false;
        }

        if (WouldTouchOtherRegion(blockPositions, bestRegionIndex))
        {
            return false;
        }

        AddBlocksToRegion(blockPositions, bestRegionIndex);

        return true;
    }

    public bool RemoveBlocksFromRegion(GameObject blockStack)
    {
        List<Vector2> blockPositions = GetBlockPositions(blockStack);
        bool removedAny = false;

        foreach (Vector2 pos in blockPositions)
        {
            if (blockToRegion.TryGetValue(pos, out int regionIndex))
            {

                regionGroups[regionIndex].Remove(pos);
                blockToRegion.Remove(pos);

                targetRegions[regionIndex].currentBlocks--;

                removedAny = true;
            }
        }

        if (removedAny)
        {
            UpdateDebugText();
        }

        return removedAny;
    }

    private List<Vector2> GetBlockPositions(GameObject blockStack)
    {
        List<Vector2> positions = new List<Vector2>();

        foreach (Transform child in blockStack.transform)
        {
            Vector2 worldPos = child.position;

            worldPos.x = Mathf.Round(worldPos.x / gridSize) * gridSize;
            worldPos.y = Mathf.Round(worldPos.y / gridSize) * gridSize;

            positions.Add(worldPos);
        }

        return positions;
    }

    private int GetBestRegionForBlocks(List<Vector2> blockPositions)
    {
        for (int i = 0; i < targetRegions.Count; i++)
        {
            if (AreBlocksConnectedToRegion(blockPositions, i))
            {
                return i;
            }
        }

        int closestRegion = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < targetRegions.Count; i++)
        {
            if (targetRegions[i].currentBlocks >= targetRegions[i].requiredBlocks)
                continue;

            Vector2 targetPos = targetRegions[i].targetCell.transform.position;

            foreach (Vector2 blockPos in blockPositions)
            {
                float distance = Vector2.Distance(blockPos, targetPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestRegion = i;
                }
            }
        }

        return closestRegion;
    }

    private bool AreBlocksConnectedToRegion(List<Vector2> blockPositions, int regionIndex)
    {
        if (regionGroups[regionIndex].Count == 0)
            return false;

        foreach (Vector2 blockPos in blockPositions)
        {
            Vector2[] adjacentPositions = {
                new Vector2(blockPos.x + gridSize, blockPos.y),
                new Vector2(blockPos.x - gridSize, blockPos.y),
                new Vector2(blockPos.x, blockPos.y + gridSize),
                new Vector2(blockPos.x, blockPos.y - gridSize)
            };

            foreach (Vector2 adjPos in adjacentPositions)
            {
                if (regionGroups[regionIndex].Contains(adjPos))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool WouldTouchOtherRegion(List<Vector2> blockPositions, int regionIndex)
    {
        foreach (Vector2 blockPos in blockPositions)
        {
            Vector2[] adjacentPositions = {
                new Vector2(blockPos.x + gridSize, blockPos.y),
                new Vector2(blockPos.x - gridSize, blockPos.y),
                new Vector2(blockPos.x, blockPos.y + gridSize),
                new Vector2(blockPos.x, blockPos.y - gridSize)
            };

            foreach (Vector2 adjPos in adjacentPositions)
            {
                for (int i = 0; i < targetRegions.Count; i++)
                {
                    if (i != regionIndex && regionGroups[i].Contains(adjPos))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void AddBlocksToRegion(List<Vector2> blockPositions, int regionIndex)
    {
        foreach (Vector2 pos in blockPositions)
        {
            regionGroups[regionIndex].Add(pos);
            blockToRegion[pos] = regionIndex;
        }

        targetRegions[regionIndex].currentBlocks += blockPositions.Count;

        UpdateDebugText();

        CheckForVictory();
    }

    private void CheckForVictory()
    {
        bool allRegionsComplete = true;

        for (int i = 0; i < targetRegions.Count; i++)
        {
            if (targetRegions[i].currentBlocks < targetRegions[i].requiredBlocks)
            {
                allRegionsComplete = false;
                break;
            }
        }

        if (allRegionsComplete)
        {
            Debug.Log("Victoria");
        }
    }

    private void UpdateDebugText()
    {
        if (debugText == null) return;

        string text = "Estado de las regiones:\n";
        for (int i = 0; i < targetRegions.Count; i++)
        {
            text += "Región " + i + ": " + targetRegions[i].currentBlocks + "/" +
                targetRegions[i].requiredBlocks + " bloques\n";
        }

        debugText.text = text;
    }

    void OnDrawGizmos()
    {
        if (regionGroups == null) return;

        foreach (var pair in regionGroups)
        {
            if (pair.Key < targetRegions.Count)
            {
                Gizmos.color = targetRegions[pair.Key].regionColor;

                foreach (Vector2 pos in pair.Value)
                {
                    Gizmos.DrawWireCube(pos, new Vector3(gridSize * 0.9f, gridSize * 0.9f, 0.1f));
                }
            }
        }
    }
}
