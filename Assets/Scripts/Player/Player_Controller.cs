using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utilities;    // Priority Queue 

public class PlayerController : MonoBehaviour
{


    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Vector3 dir;
    public float speed;
    public static bool isInteractionStarted = false;

    private Tilemap backgroundTilemap;
    private Tilemap[] obstacleTilemaps;
    public Transform player_T;
    public static Vector3 questPosition = new Vector3(-20f, 50f, 0);
    private Vector3Int playerCellPosition;
    private Vector3Int previousPlayerCellPosition;
    private Vector3Int questCellPosition;
    public Dictionary<Vector3Int, List<Vector3Int>> graph = new Dictionary<Vector3Int, List<Vector3Int>>();
    private List<Vector3Int> pathInt;
    private List<Vector3> path;
    public Vector3 nextPosition;
    public Animator animator;
    public LineRenderer lineRenderer;
    private Vector3 prevPlayerPosition;

    private void Awake()
    {
        if (gameObject.CompareTag("Player"))
        {
            DontDestroyOnLoad(gameObject); // Player 오브젝트가 파괴되지 않도록 설정
        }
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때마다 호출
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 메모리 누수 방지
    }

    private void Start()
    {
        // player_T가 null이면 다시 Player 오브젝트를 찾음
        if (player_T == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player_T = playerObject.transform; // Player의 Transform 할당
            }
        }
        // LineRenderer 설정
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 기본 스프라이트 셰이더 사용
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // graph를 초기화
        graph.Clear();
        InitializeTilemaps();
    }
    private void InitializeTilemaps()
    {

        SetTilemaps(); // 씬이 로드된 후 타일맵을 설정
        InitializePlayerAndGraph(); // 씬이 로드된 후 플레이어 위치 및 그래프 초기화
    }

    private void SetTilemaps()
    {
        Debug.Log("Map을 불러오는 중입니다...");

        // Background 타일맵을 Tag를 사용해서 불러옴
        GameObject backgroundTilemapObject = GameObject.FindWithTag("map");
        if (backgroundTilemapObject != null)
        {
            backgroundTilemap = backgroundTilemapObject.GetComponent<Tilemap>();
            Debug.Log("Background 타일맵을 불러왔습니다...");
        }
        else
        {
            Debug.LogWarning("Background 타일맵을 찾을 수 없습니다...");
        }

        // 새로운 장애물 타일맵을 이름으로 찾아서 배열에 저장
        string[] obstacleTilemapNames = { "Top", "Right", "Bottom", "Left", "Wreck", "Building", "Wall" };

        // obstacleTilemaps 배열이 충분한 크기를 가지고 있는지 확인
        if (obstacleTilemaps == null || obstacleTilemaps.Length < obstacleTilemapNames.Length)
        {
            // 배열 크기를 타일맵 이름 배열 크기와 일치하도록 재할당
            obstacleTilemaps = new Tilemap[obstacleTilemapNames.Length];
        }

        // 각 타일맵을 찾고 배열에 저장
        for (int i = 0; i < obstacleTilemapNames.Length; i++)
        {
            GameObject obstacleTilemapObject = GameObject.Find(obstacleTilemapNames[i]);
            if (obstacleTilemapObject != null)
            {
                obstacleTilemaps[i] = obstacleTilemapObject.GetComponent<Tilemap>();
                Debug.Log(obstacleTilemapNames[i] + " 타일맵을 불러왔습니다.");
            }
            else
            {
                // 타일맵을 찾을 수 없을 때, 배열에 null로 남겨두고 경고 메시지 출력
                obstacleTilemaps[i] = null;
                Debug.LogWarning(obstacleTilemapNames[i] + " 타일맵을 찾을 수 없습니다.");
            }
        }


    }

    private void InitializePlayerAndGraph() // 새로운 씬 로드 후 플레이어와 그래프 초기화
    {
        // QuestManager에서 현재 퀘스트에 맞는 플레이어 시작 위치를 가져옴
        Debug.Log("Map " + QuestManager.instance.GetCurrentQuest() + "의 플레이어 위치를 초기화 했습니다...");
        player_T.position = QuestManager.instance.GetPlayerStartPosition(QuestManager.instance.GetCurrentQuest());

        // 그래프 초기화
        GenerateGraphFromTilemap();
        UpdateGraphNodes(new Vector3Int(1, 1, 0));
    }





    void Update()
    {
        // 경로 갱신 전에 이전 경로들을 null로 초기화
        path = null;
        pathInt = null;
        playerCellPosition = backgroundTilemap.WorldToCell(player_T.position);  // 플레이어 위치를 정수좌표로 저장
        // 만약 이전 프레임의 플레이어 위치가 그래프에 포함되지 않았다면, 이전 위치로 되돌림
        if (!graph.ContainsKey(playerCellPosition))
        {
            Debug.Log("그래프에 포함되지 않은 위치로 이동하려고 합니다. 이전 위치로 되돌립니다.");
            playerCellPosition = previousPlayerCellPosition;
            player_T.position = prevPlayerPosition;
            prevPlayerPosition = player_T.position;
        }
        else
        {
            // 현재 위치가 그래프에 포함된 경우, 이전 위치를 갱신
            previousPlayerCellPosition = playerCellPosition;
            prevPlayerPosition = player_T.position;
        }



        questCellPosition = backgroundTilemap.WorldToCell(questPosition);   // 퀘스트에서 이동해야할 위치를 정수좌표로 저장

        if (graph.ContainsKey(questCellPosition) && graph.ContainsKey(playerCellPosition))  // 퀘스트 이동좌표와 플레이어 위치가 그래프 안에 있는 경우
        {
            pathInt = FindPath(questCellPosition, playerCellPosition);    // path를 통해 플레이어 위치에서 퀘스트 이동좌표까지 최단 경로를 계산
            if (pathInt != null)
            {
                path = ConvertVector3IntToVector3(pathInt);
            }
        }

        if (!isInteractionStarted)
        {
            if (Player_Stat.instance == null)
            {
                Debug.LogError("PlayerStat component is missing!");
                return;
            }
            speed = Player_Stat.instance.speedAdd * Player_Stat.instance.speedMulti;
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            if (dir.x != 0 || dir.y != 0)
            {
                animator.SetBool("isWalk", true);
                if (dir.x >= 0)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }
            }
            else
            {
                animator.SetBool("isWalk", false);
            }
            rb.velocity = dir * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        // Path를 LineRenderer로 그리기
        DrawPath();
    }

    void GenerateGraphFromTilemap() // 이동가능한 타일을 기반으로 그래프 생성하는 함수
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

    void UpdateGraphNodes(Vector3Int offset)    // 그래프의 좌표를 조정하여 그래프가 타일에 맞게 제대로 적용되게끔 보정하는 함수
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

    bool IsWalkable(Vector3Int position)    // 이동가능한 타일인지 확인하는 함수
    {
        // backgroundTilemap이 null인지 확인
        if (backgroundTilemap == null)
        {
            Debug.LogWarning("Background Tilemap이 할당되지 않았습니다.");
            return false; // 걷기 불가능 처리
        }

        // 타일이 존재하지 않으면 걷기 불가능
        if (!backgroundTilemap.HasTile(position))
            return false;

        // 장애물 타일을 확인하는 방향
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        // 각 방향에 대해 인접한 타일이 장애물 타일인지 확인
        foreach (Tilemap obstacleTilemap in obstacleTilemaps)
        {
            if (obstacleTilemap != null && obstacleTilemap.HasTile(position))
            {
                return false; // 현재 위치가 장애물 타일이면 걷기 불가능
            }
        }

        // 어떤 장애물 타일과도 닿아 있지 않으면 걷기 가능
        return true;
    }


    void DrawPath()
    {
        int diagonalTurnIndex = FindDiagonalTurnIndex(path);
        if (diagonalTurnIndex != -1)
        {
            if (path != null && path.Count > 0)
            {
                // 시작 포지션
                Vector3 prevWorldPos = questPosition;

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
                Vector3 prevWorldPos = questPosition;
                lineRenderer.positionCount = path.Count + 2;
                lineRenderer.SetPosition(0, prevWorldPos);
                for (int i = 1; i < path.Count; i++)    // sdaf
                {
                    // 월드 좌표에서의 위치를 얻고, 약간의 오프셋을 추가하여 타일의 중심에 맞춤
                    Vector3 worldPosition = path[i];
                    lineRenderer.SetPosition(i + 1, worldPosition);
                }
                lineRenderer.SetPosition(path.Count + 1, player_T.position);
            }
            else
            {
                lineRenderer.positionCount = 0; // moveDirect가 없으면 LineRenderer를 비움
            }
        }

        // 경로가 유효할 때만 그림을 그림
        if (path == null || path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // LineRenderer로 선을 그리기 위한 경로 설정
        lineRenderer.positionCount = path.Count;

        // 첫 번째 점(path[0])은 건너뜀
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 pathPosition = path[i];
            lineRenderer.SetPosition(i, pathPosition);

            // 각 경로 점에 원을 그리기
            DrawCircle(pathPosition, 0.1f, Color.red);
        }
    }

    void DrawCircle(Vector3 position, float radius, Color color)
    {
        int segments = 50; // 원을 그릴 때 사용할 세그먼트 수 (세그먼트가 많을수록 원이 더 부드러워짐)
        LineRenderer circleRenderer = new GameObject("Circle").AddComponent<LineRenderer>();

        circleRenderer.positionCount = segments + 1;
        circleRenderer.startWidth = 0.05f; // 원의 선 두께
        circleRenderer.endWidth = 0.05f;
        circleRenderer.useWorldSpace = false;
        circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        circleRenderer.startColor = color;
        circleRenderer.endColor = color;

        // 원을 그릴 위치 계산
        float angle = 0f;
        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            circleRenderer.SetPosition(i, new Vector3(x, y, 0) + position);
            angle += (360f / segments);
        }

        // Circle LineRenderer 삭제 (필요하지 않다면 일정 시간 후 삭제 가능)
        Destroy(circleRenderer.gameObject, 0.01f);  // 원이 너무 오래 유지되는 것이 싫다면 즉시 삭제
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

    // 맨허튼 거리
    float Heuristic(Vector3Int a, Vector3Int b) //
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    /*

    //유클리드 거리(Euclidean Distance)
    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    // 체비쇼프 거리(Chebyshev Distance)
    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    */
    List<Vector3> ConvertVector3IntToVector3(List<Vector3Int> pathVector3Int)
    {
        List<Vector3> pathVecctor3 = new List<Vector3>();  // List<Vector3> 타입으로 변경
        for (int i = 0; i < pathVector3Int.Count; i++)
        {
            pathVecctor3.Add(backgroundTilemap.CellToWorld(pathVector3Int[i]));
        }
        return pathVecctor3;
    }



    int FindDiagonalTurnIndex(List<Vector3> path)
    {
        if (path == null || path.Count < 2)
            return -1;  // 경로가 없거나 경로 길이가 2 미만이면 대각선 전환이 발생할 수 없음
        int DiagonalTurnIndex = 0;
        for (int idx = path.Count - 1; idx > 0; idx--)
        {
            float distancePath = Vector3.Distance(path[idx], path[idx - 1]);
            if (distancePath > 1.01)
            {
                if (DiagonalTurnIndex == 0)
                {
                    return -1;
                }
                return DiagonalTurnIndex;  // 대각선으로 처음 꺾이는 지점의 index 반환
            }
            else
            {
                DiagonalTurnIndex++;
            }
        }
        return -1;  // 대각선으로 꺾이는 부분이 없는 경우
    }

}
