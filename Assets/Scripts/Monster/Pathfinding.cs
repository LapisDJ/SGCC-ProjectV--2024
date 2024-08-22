using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                //Vector3 nextPosition = backgroundTilemap.CellToWorld(path[0]);
                Vector3Int nextCellPosition = path[0]; // 기본적으로는 첫 번째 셀을 목표로 설정
                Vector3Int up = new Vector3Int(monsterCellPosition.x, monsterCellPosition.y + 1, monsterCellPosition.z);
                Vector3Int down = new Vector3Int(monsterCellPosition.x, monsterCellPosition.y - 1, monsterCellPosition.z);
                Vector3Int left = new Vector3Int(monsterCellPosition.x - 1, monsterCellPosition.y, monsterCellPosition.z);
                Vector3Int right = new Vector3Int(monsterCellPosition.x + 1, monsterCellPosition.y, monsterCellPosition.z);

                Vector3Int up_left = new Vector3Int(monsterCellPosition.x - 1, monsterCellPosition.y + 1, monsterCellPosition.z);
                Vector3Int down_left = new Vector3Int(monsterCellPosition.x - 1, monsterCellPosition.y - 1, monsterCellPosition.z);
                Vector3Int down_right = new Vector3Int(monsterCellPosition.x + 1, monsterCellPosition.y - 1, monsterCellPosition.z);
                Vector3Int up_right = new Vector3Int(monsterCellPosition.x + 1, monsterCellPosition.y + 1, monsterCellPosition.z);
                // path[0]부터 탐색하여 x 또는 y 좌표가 모두 바뀌는 path[i]를 찾음
                for (i = 1; i < path.Count; i++)
                {
                    if (path[i].x != path[0].x && path[i].y != path[0].y)
                    {
                        nextCellPosition = path[i - 1];
                        break;
                    }
                }

                    for (int j = i; j < path.Count; j++)
                    {
                        if (path[j].x != path[i-1].x && path[j].y != path[i-1].y)
                        {
                            nextCellPosition = path[j - 1];
                            break;
                        }
                    }
                if (!graph.ContainsKey(up) || !graph.ContainsKey(down) || !graph.ContainsKey(left) || !graph.ContainsKey(right) || !graph.ContainsKey(up_left) || !graph.ContainsKey(up_right) || !graph.ContainsKey(down_left) || !graph.ContainsKey(down_right))
                {
                    Vector3 nextPosition = backgroundTilemap.CellToWorld(path[0]);
                    bool hasObstacleUp = !graph.ContainsKey(up);
                    bool hasObstacleDown = !graph.ContainsKey(down);
                    bool hasObstacleLeft = !graph.ContainsKey(left);
                    bool hasObstacleRight = !graph.ContainsKey(right);
                    bool hasObstacleUp_Left = !graph.ContainsKey(up_left);
                    bool hasObstacleup_Right = !graph.ContainsKey(up_right);
                    bool hasObstacledown_Left = !graph.ContainsKey(down_left);
                    bool hasObstacledown_Right = !graph.ContainsKey(down_right);

                    // 장애물이 있는 방향으로 이동하지 않도록 조정
                    if (hasObstacleUp && nextCellPosition == up)
                    {
                        nextPosition += new Vector3(0, -0.9f, 0); // 위쪽에 장애물이 있으면 약간 아래로 이동
                    }
                    if (hasObstacleDown && nextCellPosition == down)
                    {
                        nextPosition += new Vector3(0, 0.9f, 0); // 아래쪽에 장애물이 있으면 약간 위로 이동
                    }
                    if (hasObstacleLeft && nextCellPosition == left)
                    {
                        nextPosition += new Vector3(0.9f, 0, 0); // 왼쪽에 장애물이 있으면 약간 오른쪽으로 이동
                    }
                    if (hasObstacleRight && nextCellPosition == right)
                    {
                        nextPosition += new Vector3(-0.9f, 0, 0); // 오른쪽에 장애물이 있으면 약간 왼쪽으로 이동
                    }
                    if (hasObstacleUp_Left && nextCellPosition == up)
                    {
                        nextPosition += new Vector3(0.9f, -0.9f, 0); 
                    }
                    if (hasObstacleup_Right && nextCellPosition == down)
                    {
                        nextPosition += new Vector3(-0.9f, -0.9f, 0); 
                    }
                    if (hasObstacledown_Left && nextCellPosition == left)
                    {
                        nextPosition += new Vector3(0.9f, 0.9f, 0);
                    }
                    if (hasObstacledown_Right && nextCellPosition == right)
                    {
                        nextPosition += new Vector3(-0.9f, 0.9f, 0); 
                    }
                    monster.position = Vector3.MoveTowards(monster.position, nextPosition, Time.deltaTime * Mon_speed);
                }
                else
                {
                    Vector3 nextPosition = backgroundTilemap.CellToWorld(nextCellPosition);
                    monster.position = Vector3.MoveTowards(monster.position, nextPosition, Time.deltaTime * Mon_speed);
                }
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