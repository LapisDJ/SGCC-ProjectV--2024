using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;  // PriorityQueue가 포함된 네임스페이스 참조
public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 dir;
    public float speed;
    public bool isInteractionStarted = false; // 상호작용이 시작되었는지 여부

    public Tilemap backgroundTilemap;
    public Tilemap[] obstacleTilemaps; // Top, Bottom, Left, Right, Wreck, Building 등을 포함
    public Transform player_T;
    private Vector3 _questPosition = new Vector3(-20, 50, 0);
    public Vector3 questPosition
    {
        get => _questPosition;
        set
        {
            _questPosition = value;
        }
    }
    private Vector3Int playerCellPosition;
    private Vector3Int questPositionCellPosition;
    public Dictionary<Vector3Int, List<Vector3Int>> graph = new Dictionary<Vector3Int, List<Vector3Int>>();
    private List<Vector3Int> pathInt;
    private List<Vector3> path;
    public Vector3 nextPosition;
    // 기존 변수들
    public LineRenderer lineRenderer;
    public void Start()
    {
        player_T = GameObject.FindGameObjectWithTag("Player").transform;

        GenerateGraphFromTilemap();
        UpdateGraphNodes(new Vector3Int(1, 1, 0));
        // Map을 그래프로 저장함

        if (Player_Stat.instance == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("rb is missing on the Player.");
        }

        player_T.position = QuestManager.instance.GetCurrentQuest() switch
        {
            1 => new Vector3(29.5f, -3.5f, 0),
            2 => new Vector3(1.5f, -2f, 0),
            3 => new Vector3(2f, 24f, 0),
            _ => player_T.position
        };


        // LineRenderer 설정
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 기본 스프라이트 셰이더 사용
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        // 경로 갱신 전에 이전 경로를 null로 초기화
        path = null;
        pathInt = null;
        //moveDirect = null;
        playerCellPosition = backgroundTilemap.WorldToCell(player_T.position);  // 플레이어 위치를 정수좌표로 저장
        questPositionCellPosition = backgroundTilemap.WorldToCell(_questPosition);   // 퀘스트에서 이동해야할 위치를 정수좌표로 저장

        if (graph.ContainsKey(questPositionCellPosition) && graph.ContainsKey(playerCellPosition))  // 퀘스트 이동좌표와 플레이어 위치가 그래프 안에 있는 경우
        {
            pathInt = FindPath(questPositionCellPosition, playerCellPosition);    // path를 통해 플레이어 위치에서 퀘스트 이동좌표까지 최단 경로를 계산
            if (pathInt != null)
            {
                path = ConvertPathToWorldCoordinates(pathInt);
            }
        }

        rb = GetComponent<Rigidbody2D>();
        if (!isInteractionStarted)
        {
            if (Player_Stat.instance == null)
            {
                Debug.LogError("PlayerStat component is missing!");
                return;
            }
            speed = Player_Stat.instance.speedAdd * Player_Stat.instance.speedMulti;
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            rb.velocity = dir * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        // Gizmos를 갱신하도록 요청
        UpdateGizmos();
        // moveDirect를 LineRenderer로 그리기
        DrawMoveDirect();
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

    void DrawMoveDirect()
    {
        int diagonalTurnIndex = FindDiagonalTurnIndex(path);
        if (diagonalTurnIndex != -1)
        {
            if (path != null && path.Count > 0)
            {
                // 시작 포지션
                Vector3 prevWorldPos = _questPosition;

                // LineRenderer의 포지션 개수 설정
                int positionsCount = path.Count - diagonalTurnIndex + 2; // 시작 포인트 + 경로 포인트들 + 마지막 포인트
                lineRenderer.positionCount = positionsCount;

                // 시작 포지션 설정
                lineRenderer.SetPosition(0, prevWorldPos);

                // 경로 포인트 설정 (diagonalTurnIndex만큼 제외)
                for (int i = 0; i < path.Count - diagonalTurnIndex; i++)
                {
                    Vector3 worldPosition = path[i];
                    lineRenderer.SetPosition(i + 1, worldPosition);
                }

                // 마지막 포인트 설정 (플레이어의 위치)
                lineRenderer.SetPosition(path.Count - diagonalTurnIndex + 1, player_T.position); 

            }
            else
            {
                lineRenderer.positionCount = 0; // path가 없으면 LineRenderer를 비움
            }
        }
        else
        {
            if (path != null && path.Count > 0)
            {
                Vector3 prevWorldPos = _questPosition;
                lineRenderer.positionCount = path.Count + 2;
                lineRenderer.SetPosition(0, prevWorldPos);
                for (int i = 0; i < path.Count; i++)
                {
                    // 월드 좌표에서의 위치를 얻고, 약간의 오프셋을 추가하여 타일의 중심에 맞춤
                    Vector3 worldPosition = path[i];
                    lineRenderer.SetPosition(i + 1, worldPosition);
                    //prevWorldPos = worldPosition;
                }
                lineRenderer.SetPosition(path.Count + 1, player_T.position);
            }
            else
            {
                lineRenderer.positionCount = 0; // moveDirect가 없으면 LineRenderer를 비움
            }
        }
        
    }

    /*
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

                // moveDirectPool에 포함된 노드일 경우, 코스트에 보너스를 줌
                if (moveDirect != null)
                {
                    if (moveDirectPool.Contains(next))
                    {
                        newCost -= -5f; // 보너스 값은 조정 가능
                    }
                }
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
    */

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

        // 모든 방향(대각선 포함) 설정
        Vector3Int[] directions = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right,
        new Vector3Int(1, 1, 0),   // 오른쪽 위 대각선
        new Vector3Int(-1, 1, 0),  // 왼쪽 위 대각선
        new Vector3Int(1, -1, 0),  // 오른쪽 아래 대각선
        new Vector3Int(-1, -1, 0)  // 왼쪽 아래 대각선
    };

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, start, goal);
            }

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighborPos = current + direction;
                if (!graph.ContainsKey(neighborPos))  // 그래프에 노드가 없으면 건너뜀
                    continue;

                // 대각선 이동 시, 인접한 가로 및 세로 방향에 장애물이 있는지 확인
                if (Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1)
                {
                    Vector3Int adjacent1 = current + new Vector3Int(direction.x, 0, 0);
                    Vector3Int adjacent2 = current + new Vector3Int(0, direction.y, 0);

                    if (!graph.ContainsKey(adjacent1) || !graph.ContainsKey(adjacent2))
                        continue;
                }

                // 코스트 계산 (대각선 이동 시 가중치 1.4)
                float newCost = costSoFar[current] + ((Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1) ? 1.4f : 1f);

                if (!costSoFar.ContainsKey(neighborPos) || newCost < costSoFar[neighborPos])
                {
                    costSoFar[neighborPos] = newCost;
                    float priority = newCost + Heuristic(neighborPos, goal);
                    frontier.Enqueue(neighborPos, priority);
                    cameFrom[neighborPos] = current;
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
                //Gizmos.DrawSphere(path[j], radius);

                if (j < path.Count - 1)
                {
                    //Gizmos.DrawLine(path[j], path[j + 1]);
                }
            }
        }
    }


    void UpdateGizmos()
    {
        // 이 메서드는 OnDrawGizmos를 호출하도록 Unity에게 알려줍니다.
        UnityEditor.EditorUtility.SetDirty(this);
    }
    

    List<Vector3> ConvertPathToWorldCoordinates(List<Vector3Int> pathInCells)
    {
        List<Vector3> pathInWorld = new List<Vector3>();  // List<Vector3> 타입으로 변경
        for (int i = 0; i < pathInCells.Count; i++)
        {
            pathInWorld.Add(backgroundTilemap.CellToWorld(pathInCells[i]));
        }
        return pathInWorld;
    }

    int FindDiagonalTurnIndex(List<Vector3> path)
    {
        if (path == null || path.Count < 2)
            return -1;  // 경로가 없거나 경로 길이가 2 미만이면 대각선 전환이 발생할 수 없음
        int j = 0;
        for (int i = path.Count - 1; i > 0; i--)
        {
            float distancePath = Vector3.Distance(path[i] , path[i - 1]);
            if(distancePath > 1.01)
            { 
                if(j == 0)
                {
                    return -1;
                }
                return j;  // 대각선으로 처음 꺾이는 지점의 index 반환
            }
            else 
            {
                j++;
            }
        }
        return -1;  // 대각선으로 꺾이는 부분이 없는 경우
    }

}