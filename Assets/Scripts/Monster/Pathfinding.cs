using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimplePathfinding : MonoBehaviour
{
    public Tilemap backgroundTilemap;
    public Tilemap[] obstacleTilemaps; // Top, Bottom, Left, Right, Wreck, Building 등을 포함
    public Transform player;
    public Transform monster;
    private Vector3Int playerCellPosition;
    private Vector3Int monsterCellPosition;

    private Dictionary<Vector3Int, List<Vector3Int>> graph = new Dictionary<Vector3Int, List<Vector3Int>>();

    void Start()
    {
        GenerateGraphFromTilemap();
        UpdateGraphNodes(new Vector3Int(1, 1, 0));
        OnDrawGizmos();
    }

    void Update()
    {
        playerCellPosition = backgroundTilemap.WorldToCell(player.position);
        monsterCellPosition = backgroundTilemap.WorldToCell(monster.position);

        if (graph.ContainsKey(monsterCellPosition) && graph.ContainsKey(playerCellPosition))
        {
            List<Vector3Int> path = FindPath(monsterCellPosition, playerCellPosition);

            if (path != null && path.Count > 0)
            {
                Debug.Log("start : " + monsterCellPosition);
                for(int j = 0; j < path.Count; j++)
                {
                    Debug.Log(path[j]);
                }
                Debug.Log("End : " + playerCellPosition);

                Vector3 nextPosition = backgroundTilemap.CellToWorld(path[0]);
                monster.position = Vector3.MoveTowards(monster.position, nextPosition, Time.deltaTime * 2f);
            }
            else
            {
                Debug.LogWarning("No path found from monster to player.");
            }
        }
        else
        {
            Debug.LogError("One of the positions (monster or player) is not in the graph.");
        }
    }

    void GenerateGraphFromTilemap()
    {
        BoundsInt bounds = backgroundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (IsWalkable(tilePosition))
                {
                    List<Vector3Int> neighbors = new List<Vector3Int>();
                    Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                    foreach (Vector3Int direction in directions)
                    {
                        Vector3Int neighborPos = tilePosition + direction;
                        if (IsWalkable(neighborPos))
                        {
                            neighbors.Add(neighborPos);
                        }
                    }

                    if (neighbors.Count > 0)
                    {
                        graph[tilePosition] = neighbors;
                    }
                }
            }
        }
    }
    /*
    bool IsWalkable(Vector3Int position)
    {
        if (!backgroundTilemap.HasTile(position))
            return false;

        foreach (Tilemap obstacleTilemap in obstacleTilemaps)
        {
            if (obstacleTilemap.HasTile(position))
                return false;
        }

        return true;
    }
    */
    void UpdateGraphNodes(Vector3Int offset)
    {
        Dictionary<Vector3Int, List<Vector3Int>> newGraph = new Dictionary<Vector3Int, List<Vector3Int>>();

        foreach (var node in graph)
        {
            Vector3Int oldPosition = node.Key;
            List<Vector3Int> neighbors = node.Value;

            // 새 위치로 노드를 이동
            Vector3Int newPosition = oldPosition + offset;
            List<Vector3Int> newNeighbors = new List<Vector3Int>();

            // 각 이웃 노드도 새 위치로 이동
            foreach (var neighbor in neighbors)
            {
                newNeighbors.Add(neighbor + offset);
            }

            // 새로운 그래프에 추가
            newGraph[newPosition] = newNeighbors;
        }

        // 기존 그래프를 새로운 그래프로 교체
        graph = newGraph;
    }
    bool IsWalkable(Vector3Int position)
{
    // 타일이 존재하지 않으면 걷기 불가능
    if (!backgroundTilemap.HasTile(position))
        return false;

    // 장애물 타일을 확인하는 방향
    Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    // 각 방향에 대해 인접한 타일이 장애물 타일인지 확인
    foreach (Tilemap obstacleTilemap in obstacleTilemaps)
    {
        if (obstacleTilemap.HasTile(position))
        {
            return false; // 현재 위치가 장애물 타일이면 걷기 불가능
        }

        
    }

    // 어떤 장애물 타일과도 닿아 있지 않으면 걷기 가능
    return true;
}





    List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        PriorityQueue<Vector3Int> frontier = new PriorityQueue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> costSoFar = new Dictionary<Vector3Int, float>();

        frontier.Enqueue(start, 0);
        visited.Add(start);
        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, start, goal);
            }

            foreach (Vector3Int next in graph[current])
            {
                float newCost = costSoFar[current] + 1; // Assuming uniform cost between nodes

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return null;
    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentNode = goal;

        while (currentNode != start)
        {
            if (!cameFrom.ContainsKey(currentNode))
            {
                return null; // 경로가 불완전할 경우 null 반환
            }
            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }

        path.Reverse();
        return path;
    }

    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 노드를 표시할 색상
        float radius = 0.1f; // 노드를 표시할 구의 반지름

        if (graph != null)
        {
            foreach (var node in graph)
            {
                Vector3 nodePosition = backgroundTilemap.CellToWorld(node.Key);// + new Vector3(0.5f, 0.5f, 0); // 타일의 중앙을 구할 수 있도록 오프셋 추가
                Gizmos.DrawSphere(nodePosition, radius); // 노드 위치에 구를 그림

                foreach (var neighbor in node.Value)
                {
                    Vector3 neighborPosition = backgroundTilemap.CellToWorld(neighbor);// + new Vector3(0.5f, 0.5f, 0);
                    Gizmos.DrawLine(nodePosition, neighborPosition); // 노드와 인접 노드 사이에 선을 그림
                }
            }
        }
    }


}






// 우선순위 큐 클래스 (간단한 최소 힙 구현)
public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
        HeapifyUp(elements.Count - 1);
    }

    public T Dequeue()
    {
        if (elements.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        T bestItem = elements[0].Key;
        elements[0] = elements[elements.Count - 1];
        elements.RemoveAt(elements.Count - 1);
        HeapifyDown(0);

        return bestItem;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (elements[index].Value >= elements[parentIndex].Value) break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        while (index * 2 + 1 < elements.Count)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallestChildIndex = leftChildIndex;

            if (rightChildIndex < elements.Count && elements[rightChildIndex].Value < elements[leftChildIndex].Value)
            {
                smallestChildIndex = rightChildIndex;
            }

            if (elements[index].Value <= elements[smallestChildIndex].Value) break;

            Swap(index, smallestChildIndex);
            index = smallestChildIndex;
        }
    }

    private void Swap(int indexA, int indexB)
    {
        var temp = elements[indexA];
        elements[indexA] = elements[indexB];
        elements[indexB] = temp;
    }
}