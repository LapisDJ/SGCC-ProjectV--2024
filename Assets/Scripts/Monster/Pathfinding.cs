using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class SimplePathfinding : MonoBehaviour
{
    public Tilemap backgroundTilemap;
    public Tilemap[] obstacleTilemaps; // Top, Bottom, Left, Right, Wreck, Building 등을 포함
    public Transform player;
    public Transform monster;
    private Vector3Int playerCellPosition;
    private Vector3Int monsterCellPosition;
    public float Mon_speed = 20f;

    private Dictionary<Vector3Int, List<Vector3Int>> graph = new Dictionary<Vector3Int, List<Vector3Int>>();
    Vector3 nextPosition;
    Vector3 nextPosition_temp;
    private int path_x = 0;
    private int path_y = 0;
    private float obstacle_distance = 1f;
    public float obstacleCheckRadius = 0.5f; // 장애물 체크 반경
    public Vector3Int previous_nextcellPosition = new Vector3Int();
    int ifStart = 0;
    void Start()
    {
        GenerateGraphFromTilemap();
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
                int i = 1;
                Vector3Int nextCellPosition = path[0]; // 기본적으로는 첫 번째 셀을 목표로 설정 
                /*
                for (i = 1; i < path.Count; i++)    // 이동하는 그래프 노드에서 일직선 방향의 마지막 노드를 nextCellPosition에 저장
                {
                    if (path[i].x != path[0].x && path[i].y != path[0].y)
                    {
                        nextCellPosition = path[i - 1];
                        break;
                    }
                }
                */
                /*
                int j = 1;
                for (j = i; j < path.Count; j++)
                {
                    if (path[j].x != path[i - 1].x && path[j].y != path[i - 1].y)
                    {
 
                            nextCellPosition = path[j - 1];
                            break;
                    }
                }
                */
                if (ifStart == 0)
                {
                    nextCellPosition = path[0];
                    ifStart++;
                }
                previous_nextcellPosition = nextCellPosition;
                // 가장 가까운 그래프에 포함되지 않은 타일 찾기
                Vector3Int closestNonGraphTile = FindClosestNonGraphTile(nextCellPosition);

                // 두 점 사이의 거리 계산
                float distance = Vector3.Distance(backgroundTilemap.CellToWorld(nextCellPosition), backgroundTilemap.CellToWorld(closestNonGraphTile));
                //Debug.Log(distance);
                // 거리가 1.5f보다 작을 경우, 거리를 1.5f로 보정
                if (distance < 1.42f)
                {
                    Vector3 direction = (backgroundTilemap.CellToWorld(nextCellPosition) - backgroundTilemap.CellToWorld(closestNonGraphTile)).normalized;
                    //Debug.Log(direction);
                    Vector3 adjustedPosition = backgroundTilemap.CellToWorld(closestNonGraphTile) + direction * 0.48f;
                    
                    nextPosition = adjustedPosition; // 보정된 위치를 nextPosition에 저장
                    //Debug.Log(monsterCellPosition + "               " + backgroundTilemap.CellToWorld(nextCellPosition) + "               " + backgroundTilemap.CellToWorld(closestNonGraphTile) + "               " + monster.position + "               " + nextPosition + " aaaaaaaaaaaaa");
                }
                else
                {
                    nextPosition = backgroundTilemap.CellToWorld(nextCellPosition);
                    //Debug.Log(monster.position + "               " + nextPosition + " bbbbbbbbbbbbbbbbbb");
                }

                //monster.position += (nextPosition - monster.position).normalized * Time.deltaTime * Mon_speed;
                monster.position = Vector3.MoveTowards(monster.position, nextPosition, Mon_speed * Time.deltaTime);
                //Debug.Log(monster.position);
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


    Vector3Int FindClosestNonGraphTile(Vector3Int referenceTile)
    {
        Vector3Int closestTile = referenceTile;
        float closestDistance = float.MaxValue;

        for (int x = referenceTile.x - 100; x <= referenceTile.x + 100; x++)
        {
            for (int y = referenceTile.y - 100; y <= referenceTile.y + 100; y++)
            {
                Vector3Int currentTile = new Vector3Int(x, y, 0);

                if (!graph.ContainsKey(currentTile))
                {
                    float distance = Vector3.Distance(backgroundTilemap.CellToWorld(referenceTile), backgroundTilemap.CellToWorld(currentTile));

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTile = currentTile;
                    }
                }
            }
        }

        return closestTile;
    }


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