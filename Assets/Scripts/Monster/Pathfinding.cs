using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimplePathfinding : MonoBehaviour
{
    private Monster monster_S;
    public Tilemap backgroundTilemap;
    public Tilemap[] obstacleTilemaps; // Top, Bottom, Left, Right, Wreck, Building 등을 포함
    public Transform player;
    public Transform monster;
    private Vector3Int playerCellPosition;
    private Vector3Int monsterCellPosition;
    private float speed;
    private Dictionary<Vector3Int, List<Vector3Int>> graph = new Dictionary<Vector3Int, List<Vector3Int>>();
    private List<Vector3Int> path;
    private List<Vector3Int> moveDirectPool;
    private List<Vector3> moveDirect;
    public Vector3 nextPosition;
    void Start()
    {
        monster_S = GetComponent<Monster>();
        if (monster_S == null)
        {
            Debug.LogError("Monster 컴포넌트를 찾을 수 없습니다!");
        }
        GenerateGraphFromTilemap();
        UpdateGraphNodes(new Vector3Int(1, 1, 0));
    }

    void Update()
    {
        if (monster != null)
        {
            // 이동 속도를 가져옴
            speed = monster_S.GetCurrentSpeed();
            Debug.Log(speed);
        }

        if (Vector3.Distance(monster.position, nextPosition) <= 0.8f)
        {
            monster.position = nextPosition;
        }

        // 경로 갱신 전에 이전 경로를 null로 초기화
        path = null;
        moveDirect = null;

        playerCellPosition = backgroundTilemap.WorldToCell(player.position);
        monsterCellPosition = backgroundTilemap.WorldToCell(monster.position);

        if (graph.ContainsKey(monsterCellPosition) && graph.ContainsKey(playerCellPosition))
        {
            path = FindPath(monsterCellPosition, playerCellPosition);

            if (path != null && path.Count > 0)
            {
                moveDirect = new List<Vector3>();
                // path에서 moveDirectPool에 있는 좌표들을 moveDirect에 추가
                foreach (var position in path)
                {
                    if (moveDirectPool.Contains(position))
                    {
                        moveDirect.Add(position);
                    }
                }
                moveDirect.Add(player.position);

                nextPosition = moveDirect[0];

                monster.position = Vector3.MoveTowards(monster.position, nextPosition, Time.deltaTime * speed);

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

        // Gizmos를 갱신하도록 요청
        UpdateGizmos();

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
        float radius = 0.1f;
        Gizmos.color = Color.blue;

        // 경로를 파란색 구와 선으로 표시
        if (path != null)
        {
            for (int j = 0; j < path.Count; j++)
            {
                Gizmos.DrawSphere(path[j], radius);

                if (j < path.Count - 1)
                {
                    Gizmos.DrawLine(path[j], path[j + 1]);
                }
            }
        }

        // 대각선 방향을 위한 배열
        Vector3Int[] diagonalDirections = {
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, -1, 0)
    };

        // 상하좌우 방향을 위한 배열
        Vector3Int[] cardinalDirections = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

        // moveDirectPool 초기화
        moveDirectPool = new List<Vector3Int>();

        // 조건을 만족하는 경우 초록색 구를 그리고, 좌표를 moveDirectPool에 추가
        Gizmos.color = Color.green;
        foreach (var node in graph)
        {
            // 상하좌우 방향이 모두 그래프에 포함되는지 확인
            bool isCardinalValid = true;
            foreach (var cardinalDirection in cardinalDirections)
            {
                Vector3Int adjacentPosition = node.Key + cardinalDirection;
                if (!graph.ContainsKey(adjacentPosition))
                {
                    isCardinalValid = false;
                    break;
                }
            }

            if (isCardinalValid)
            {
                // 대각선 방향에 하나라도 그래프에 포함되지 않은 경우
                bool hasDiagonalNonGraph = false;

                foreach (var direction in diagonalDirections)
                {
                    Vector3Int diagonalPosition = node.Key + direction;

                    if (!graph.ContainsKey(diagonalPosition))
                    {
                        hasDiagonalNonGraph = true;
                        break;
                    }
                }

                if (hasDiagonalNonGraph)
                {
                    // 해당 위치에 초록색 원을 표시하고 moveDirectPool에 추가
                    Vector3 worldPosition = backgroundTilemap.CellToWorld(node.Key);
                    Gizmos.DrawSphere(worldPosition, 3f * radius);

                    // moveDirectPool에 노드의 위치 추가
                    moveDirectPool.Add(node.Key);
                }
            }
        }
        // moveDirect를 빨간 점으로 표시하고 선으로 연결
        if (moveDirect != null && moveDirect.Count > 0)
        {
            Gizmos.color = Color.red;

            // 첫 번째 점은 플레이어의 위치로 설정
            Vector3 previousWorldPos = monster.position;
            Gizmos.DrawSphere(previousWorldPos, 2f * radius);

            // moveDirect의 각 좌표들을 순서대로 연결
            for (int i = 0; i < moveDirect.Count; i++)
            {
                Vector3 worldPos = moveDirect[i];//
                Gizmos.DrawSphere(worldPos, 2f * radius);

                // 이전 위치와 현재 위치를 선으로 연결
                Gizmos.DrawLine(previousWorldPos, worldPos);

                // 현재 위치를 다음 선의 시작 위치로 설정
                previousWorldPos = worldPos;
            }
        }
    }

    void UpdateGizmos()
    {
        // 이 메서드는 OnDrawGizmos를 호출하도록 Unity에게 알려줍니다.
        UnityEditor.EditorUtility.SetDirty(this);
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
