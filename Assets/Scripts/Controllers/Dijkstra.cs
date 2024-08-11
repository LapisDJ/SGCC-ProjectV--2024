using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public GameObject player;   // 몬스터는 player 객체를 목표로 이동
    public LayerMask obstacleLayer; // 장애물 레이어  ( 레이어에 속하는 객체는 경로 탐색 시 피해야 함 )
    public LayerMask wallLayer; // 벽 레이어 ( 레이어에 속하는 객체는 경로 탐색 시 피해야 함 )
    public float updateInterval = 0.5f; // 경로 업데이트 간격 ( 0.5초마다 경로를 갱신 )

    private List<Vector2> path = new List<Vector2>();   // 계산된 경로를 저장하는 리스트 ( 몬스터가 따라가야 할 각 위치 포함 )
    private Vector2 targetPosition; // 몬스터가 현재 이동 중인 목표 위치
    private Rigidbody2D rb; // 몬스터를 관리하는 Rigidbody2D 컴포넌트를 참조
    private MonsterController monsterController;    // 몬스터를 관리하는 MonsterController 스크립트를 참조
    private float timeSinceLastUpdate = 0.0f;   // 마지막 경로 업데이트 이후 경과된 시간을 추적하는 변수

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   // 컴포넌트를 현재 객체에서 가져와 rb 변수에 할당 ( 물리적 움직임을 제어 )
        monsterController = GetComponent<MonsterController>();  // MonsterController 컴포넌트를 현재 객체에서 가져와 monsterController 변수에 할당 ( 이 스크립트에서 사용 )
        targetPosition = player.transform.position; // 플레이어의 현재 위치를 targetPosition으로 설정 ( 몬스터가 처음에 이동할 목표 )
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;  // 마지막 경로 업데이트 이후 경과된 시간에 현재 프레임의 시간을 더함 ( Time.deltaTime은 마지막 프레임과 현재 프레임 사이의 시간 간격을 의미 )

        if (timeSinceLastUpdate >= updateInterval || Vector2.Distance(rb.position, targetPosition) < 0.1f)  // 일정 시간이 경과했거나 몬스터가 목표 위치에 거의 도달했을 때 경로를 다시 계산
        {
            path = FindPath(rb.position, player.transform.position);    // 현재 위치 (rb.position)에서 플레이어의 위치 (player.transform.position)까지의 최단 경로를 계산하여 path 변수에 저장
            timeSinceLastUpdate = 0.0f; // 경로를 새로 계산했으므로 경과 시간을 0으로 리셋

            if (path.Count > 0) // 경로가 존재할 때만 실행 ( path 리스트에 하나 이상의 노드가 있을 경우에만 이동 )
            {
                targetPosition = path[0];   // path의 첫 번째 위치를 targetPosition으로 설정 ( 몬스터가 다음으로 이동할 목표 지점 )
                path.RemoveAt(0);   // 리스트에서 첫 번째 위치를 제거하여 다음 노드로 이동할 준비
            }   
        }

        MoveTowardsTarget();    // 목표 위치(targetPosition)를 향해 몬스터를 이동시키는 메서드를 호출
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - rb.position).normalized;  // 현재 위치에서 목표 위치로 가는 벡터를 normalized
        rb.velocity = direction * monsterController.speed;  // 방향 벡터에 MonsterController에서 설정한 속도를 곱하여, Rigidbody2D의 속도를 설정
                // speed가 MonsterController.cs에서 [SerializeField] float speed = 0.0f;로 선언되서 public으로 변환하면 해결됨
    }

    List<Vector2> FindPath(Vector2 start, Vector2 goal) // 시작 위치(start)에서 목표 위치(goal)까지의 최단 경로를 계산하는 메서드 ( A* 알고리즘을 사용해 경로를 계산 )
    {
        var openList = new SortedSet<Node>(new NodeComparer()); // openList는 경로를 탐색하는 동안 확인할 노드들을 저장하는 리스트 ( SortedSet을 사용하여 FScore 값에 따라 자동으로 정렬 0
        var closedList = new HashSet<Vector2>();    // closedList는 이미 탐색을 마친 노드들을 저장하는 리스트 ( 중복된 노드 탐색을 방지하기 위해 사용 )
        var startNode = new Node(start, null, 0, Vector2.Distance(start, goal));    // 시작 위치에서의 노드를 생성 ( Node는 현재 위치, 부모 노드, GScore, HScore를 포함하는 구조체 0

        openList.Add(startNode);    // 시작 노드를 openList에 추가 ( 경로 탐색의 출발점 )
            
        while (openList.Count > 0)  // openList에 노드가 남아있는 동안 반복 ( 탐색할 노드가 남아있을 때까지 경로를 찾음 )
        {
            Node currentNode = openList.Min;    // openList에서 가장 작은 FScore 값을 가진 노드를 가져옴 ( 현재 탐색 중인 최적의 노드 )
            openList.Remove(currentNode);   // currentNode를 openList에서 제거

            if (Vector2.Distance(currentNode.position, goal) < 0.1f)    // 현재 노드가 목표 위치에 도달했는지 확인 ( 목표 위치와의 거리가 0.1 유닛보다 작으면 목표에 도달한 것으로 간주 )
            {
                return ReconstructPath(currentNode);    // 목표 위치에 도달한 경우 현재 노드부터 시작하여 경로를 역추적하고 그 경로를 반환
            }

            closedList.Add(currentNode.position);   // 현재 노드의 위치를 closedList에 추가하여 이 위치가 다시 탐색되지 않도록 함

            foreach (var neighbor in GetNeighbors(currentNode.position))    // 현재 노드의 이웃 노드들을 반복하여 탐색 ( 이웃 노드들은 현재 위치에서 이동 가능한 인접한 위치 )
            {
                if (closedList.Contains(neighbor))  // 만약 이웃 노드의 위치가 이미 closedList에 포함되어 있으면 이 위치는 이미 탐색이 완료된 것으로 간주하고 더 이상 탐색하지 않음
                    continue;   // 다음 이웃 노드로 넘어감

                float tentativeGScore = currentNode.gScore + Vector2.Distance(currentNode.position, neighbor);  // 현재 노드에서 이웃 노드까지의 예상 비용을 계산
                // currentNode.gScore는 시작 지점에서 현재 노드까지의 실제 비용 , Vector2.Distance(currentNode.position, neighbor)는 현재 노드에서 이웃 노드까지의 거리이며 두 값을 더한 것이 이웃 노드까지의 잠정적인 비용 (tentativeGScore)

                Node neighborNode = new Node(neighbor, currentNode, tentativeGScore, Vector2.Distance(neighbor, goal)); // 새로운 이웃 노드를 생성
                // neighbor: 이웃 노드의 위치 , currentNode: 현재 노드를 부모 노드로 설정 , tentativeGScore: 앞서 계산한 이웃 노드까지의 잠정적인 실제 비용 , 이웃 노드에서 목표 지점까지의 예상 비용(휴리스틱)
                if (openList.TryGetValue(neighborNode, out Node existingNeighbor) && tentativeGScore >= existingNeighbor.gScore)    // 이웃 노드가 이미 openList에 있는지 확인하고, 만약 있다면 그 노드의 gScore(실제 비용)를 비교
                // openList.TryGetValue(neighborNode, out Node existingNeighbor): openList에 이웃 노드가 이미 존재하는지 확인하고 있으면 existingNeighbor에 저장
                // entativeGScore >= existingNeighbor.gScore: 새로 계산한 tentativeGScore가 기존의 gScore보다 크거나 같으면 이미 있는 경로가 더 최적이므로 현재 경로를 무시하고 다음 이웃 노드를 탐색
                    continue;

                openList.Remove(existingNeighbor);  // 기존의 이웃 노드를 openList에서 제거 ( 이웃 노드를 탐색할 때 더 나은 경로가 발견되었으므로 기존의 경로는 삭제 )
                openList.Add(neighborNode); // 새로 계산된 경로의 이웃 노드를 openList에 추가 ( 이 노드가 다음 탐색 대상으로 설정 )
            }
        }

        return new List<Vector2>(); // 목표 지점에 도달하지 못했을 때 빈 경로 리스트를 반환 ( 몬스터가 플레이어에게 도달할 수 있는 경로를 찾지 못한 것 )
    }

    List<Vector2> GetNeighbors(Vector2 position)    // 현재 위치에서 네 방향(위, 아래, 왼쪽, 오른쪽)에 있는 인접한 타일들의 좌표를 반환 ( 경로 탐색 시 고려해야 할 이웃 노드들을 결정하는 데 사용 )
    {
        List<Vector2> neighbors = new List<Vector2>();  // 이웃 노드들의 좌표를 저장할 리스트를 초기화

        Vector2[] directions = {    // 4개의 방향 벡터를 정의
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (var direction in directions)   // 정의된 네 방향에 대해 반복문을 실행 ( 각 방향으로 이웃 노드의 위치를 계산 )
        {
            Vector2 neighborPos = position + direction; // 현재 위치에서 각 방향으로 이동한 새로운 위치(neighborPos)를 계산 ( 이 위치는 이웃 노드의 위치 )
            if (!Physics2D.OverlapCircle(neighborPos, 0.1f, obstacleLayer) && !Physics2D.OverlapCircle(neighborPos, 0.1f, wallLayer))   // 이웃 노드의 위치가 장애물(obstacleLayer)이나 벽(wallLayer)에 속하지 않는지 확인
            // Physics2D.OverlapCircle은 특정 위치에 지정된 레이어(obstacleLayer 또는 wallLayer)에 속한 오브젝트가 있는지 확인
            // neighborPos가 장애물이나 벽에 해당하지 않으면 이 위치는 유효한 이웃 노드로 간주
            {
                neighbors.Add(neighborPos); // 유효한 이웃 노드의 위치를 neighbors 리스트에 추가
            }
        }

        return neighbors;   // 유효한 이웃 노드들의 위치를 반환 ( 경로 탐색 알고리즘에서 사용 )
    }

    List<Vector2> ReconstructPath(Node currentNode) // 목표 지점에 도달했을 때, 경로를 역추적하여 완성된 경로를 반환
    {
        List<Vector2> path = new List<Vector2>();   // 경로를 저장할 리스트를 초기화
        while (currentNode != null) // 현재 노드가 null이 아닐 동안 시작점(root node)까지 역추적할 때까지 반복
        {
            path.Add(currentNode.position); // 현재 노드의 위치를 path 리스트에 추가 ( 현재 노드에서 시작점까지의 경로가 저장 )
            currentNode = currentNode.parent;   // 현재 노드의 부모 노드로 이동 ( 경로를 역추적하는 과정 )
        }
        path.Reverse();
        // 경로 리스트를 뒤집어 경로를 순차적으로 정렬 ( 역추적 과정에서 저장된 경로는 목표 지점부터 시작점까지의 순서로 저장되므로 reverse하여 시작점에서 목표 지점까지의 순서로 만듬 )
        return path;    // 완성된 경로를 반환 ( 경로는 적이 따라가야 할 최단 경로 )
    }

    private class Node  // 경로 탐색에서 사용되는 노드
    {
        public Vector2 position;    // 이 노드의 위치
        public Node parent; // 이 노드의 부모 노드 ( 경로를 추적할 때 사용 )
        public float gScore; // 시작 지점부터 현재 노드까지의 실제 비용 ( 거리 )
        public float hScore; // 현재 노드부터 목표 지점까지의 예상 비용 ( 거리 )

        public Node(Vector2 position, Node parent, float gScore, float hScore)  // 노드의 생성자
        {
            this.position = position;   // 노드의 위치를 초기화
            this.parent = parent;   // 노드의 부모를 초기화
            this.gScore = gScore;   // 노드의 실제 비용을 초기화
            this.hScore = hScore;   // 노드의 예상 비용을 초기화
        }   

        public float FScore => gScore + hScore; // FScore가 낮을수록 더 유리한 경로
    }

    private class NodeComparer : IComparer<Node>    // Node 객체들을 비교하기 위한 커스텀 비교기를 정의 ( NodeComparer는 노드들을 비용 순으로 정렬하는 데 사용 )
    {
        public int Compare(Node x, Node y)  // 두 Node 객체를 비교하여, FScore가 작은 순서대로 정렬되도록 함
        {
            if (x == null || y == null) 
                return 0;   // 두 노드 중 하나가 null일 경우 비교하지 않고 0을 반환

            return x.FScore.CompareTo(y.FScore);    // 두 노드의 FScore를 비교하여 결과를 반환 ( FScore가 작은 노드가 우선시 )
        }
    }
}