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
    //public Vector3 questPosition =  new Vector3(-20, 50, 0);
    private Vector3Int playerCellPosition;
    private Vector3Int questPositionCellPosition;
    public Dictionary<Vector3Int, List<Vector3Int>> graph = new Dictionary<Vector3Int, List<Vector3Int>>();
    private List<Vector3Int> path;
    private List<Vector3Int> moveDirectPool;
    private List<Vector3> moveDirect;
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
        moveDirect = null;
        playerCellPosition = backgroundTilemap.WorldToCell(player_T.position);  // 플레이어 위치를 정수좌표로 저장
        questPositionCellPosition = backgroundTilemap.WorldToCell(_questPosition);   // 퀘스트에서 이동해야할 위치를 정수좌표로 저장

        if (graph.ContainsKey(questPositionCellPosition) && graph.ContainsKey(playerCellPosition))  // 퀘스트 이동좌표와 플레이어 위치가 그래프 안에 있는 경우
        {
            path = FindPath(questPositionCellPosition, playerCellPosition);    // path를 통해 플레이어 위치에서 퀘스트 이동좌표까지 최단 경로를 계산

            if (path != null && path.Count > 0)
            {
                moveDirect = new List<Vector3>();

                if (moveDirectPool != null)
                {
                    foreach (var position in path)  // path에서 moveDirectPool에 있는 좌표들을 moveDirect에 추가
                    {
                        if (moveDirectPool.Contains(position))
                        {
                            moveDirect.Add(position);
                        }
                    }

                    moveDirect.Add(player_T.position);
                }
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
        if (moveDirect != null && moveDirect.Count > 0)
        {
            Vector3 prevWorldPos = _questPosition;
            lineRenderer.positionCount = moveDirect.Count + 1;
            lineRenderer.SetPosition(0, prevWorldPos);
            for (int i = 0; i < moveDirect.Count; i++)
            {
                // 월드 좌표에서의 위치를 얻고, 약간의 오프셋을 추가하여 타일의 중심에 맞춤
                Vector3 worldPosition = moveDirect[i];
                lineRenderer.SetPosition(i + 1, worldPosition);
                //prevWorldPos = worldPosition;
            }
        }
        else
        {
            lineRenderer.positionCount = 0; // moveDirect가 없으면 LineRenderer를 비움
        }
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
                    //Gizmos.DrawSphere(worldPosition, 3f * radius);

                    // moveDirectPool에 노드의 위치 추가
                    moveDirectPool.Add(node.Key);
                }
            }
        }

        // moveDirect를 빨간 점으로 표시하고 선으로 연결
        if (moveDirect != null && moveDirect.Count > 0)
        {
            Gizmos.color = Color.green;

            // 첫 번째 점은 퀘스트 이동의 위치로 설정
            Vector3 previousWorldPos = _questPosition;
            Gizmos.DrawSphere(previousWorldPos, 1f * radius);

            // moveDirect의 각 좌표들을 순서대로 연결
            for (int i = 0; i < moveDirect.Count; i++)
            {
                Vector3 worldPos = moveDirect[i];
                //Gizmos.DrawSphere(worldPos, 2f * radius);

                // 이전 위치와 현재 위치를 선으로 연결
                //Gizmos.DrawLine(previousWorldPos, worldPos);

                // 현재 위치를 다음 선의 시작 위치로 설정
                previousWorldPos = worldPos;
            }

            // 마지막에 questPosition 추가
            Vector3 questWorldPos = _questPosition;
            Gizmos.DrawSphere(questWorldPos, 2f * radius);
            Gizmos.DrawLine(previousWorldPos, questWorldPos);
        }
    }


    void UpdateGizmos()
    {
        // 이 메서드는 OnDrawGizmos를 호출하도록 Unity에게 알려줍니다.
        UnityEditor.EditorUtility.SetDirty(this);
    }



}