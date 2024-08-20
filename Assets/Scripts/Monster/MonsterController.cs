using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterController : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 0.0f;
    private List<Vector2> path = new List<Vector2>();  // A* 알고리즘을 통해 계산된 몬스터의 이동 경로를 저장하는 Vector2 리스트
    private Vector2 targetPosition;    // 몬스터가 현재 이동 중인 목표 위치
    private List<TilemapCollider2D> obstacleColliders = new List<TilemapCollider2D>();  // 타일맵의 장애물들을 저장하는 TilemapCollider2D 컴포넌트의 리스트
    [SerializeField] private Monster monster;
    private Vector2 lastPlayerPosition;  // 마지막으로 경로를 계산한 플레이어의 위치

    void Start()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody2D>();
        speed = monster.GetCurrentSpeed();  // Monster 스크립트에서 속도 가져오기

        // 태그로 오브젝트를 찾아 TilemapCollider2D 컴포넌트를 obstacleColliders 리스트에 추가
        foreach (var collider in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            TilemapCollider2D tilemapCollider = collider.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                obstacleColliders.Add(tilemapCollider);
            }
        }

        lastPlayerPosition = player.transform.position;
        UpdatePath();  // 몬스터가 처음 이동할 경로를 설정
    }

    void Update()
    {
        // 몬스터가 목표 위치에 거의 도달했을 때, 리스트에서 다음 목표 위치를 가져옴
        if (path.Count > 0 && Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            targetPosition = path[0];
            path.RemoveAt(0);
        }

        MoveTowardsTarget();    // 현재 목표 위치로 이동을 시작

        // 플레이어의 위치가 마지막 위치에서 일정 거리 이상 움직였을 때 경로 업데이트
        if (Vector2.Distance(player.transform.position, lastPlayerPosition) > 1.0f)
        {
            UpdatePath();
            lastPlayerPosition = player.transform.position;
        }
    }

    // 목표 위치로 향하는 방향 벡터를 계산하고 그 방향으로 이동 속도를 설정
    void MoveTowardsTarget()
    {
        if (path.Count > 0)
        {
            Vector2 direction = (targetPosition - rb.position).normalized;  // 몬스터가 움직여야 할 방향
            rb.velocity = direction * speed;
        }
    }

    // A* 알고리즘을 사용하여 몬스터와 플레이어 사이의 경로를 계산하고 그 경로를 path 리스트에 저장
    void UpdatePath()
    {
        path = FindPath(rb.position, player.transform.position);    // 현재 몬스터의 위치와 플레이어의 위치 사이의 최적 경로를 계산

        if (path.Count > 0) // 경로가 성공적으로 계산되면 첫 번째 목표 위치를 targetPosition으로 설정 후 path에서 제거
        {
            targetPosition = path[0];
            path.RemoveAt(0);
        }
    }

    // A* 알고리즘의 구현
    // 시작 위치와 목표 위치 사이의 최적 경로 찾음
    List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        var openList = new SortedSet<Node>(new NodeComparer()); // 탐색할 노드들 [ FScore(gScore + hScore) 오름차순으로 정렬 ]
        var closedList = new HashSet<Vector2>();    // 이미 탐색이 완료된 노드들 [ 중복된 노드 탐색 방지 ]
        var startNode = new Node(start, null, 0, Vector2.Distance(start, goal));    // 시작 노드 [ gScore는 0 ,  hScore는 Vector2.Distance(start, goal)로 설정 ]

        openList.Add(startNode);    // 시작 노드를 openList에 추가하여 탐색

        // 현재 노드의 인접 노드를 검사하고 경로 점수(gScore)가 더 낮은 경우 경로를 업데이트
        while (openList.Count > 0)
        {
            // openList에서 FScore가 가장 낮은 노드를 currentNode로 선택하고 이 노드를 openList에서 제거
            Node currentNode = openList.Min;
            openList.Remove(currentNode);

            if (Vector2.Distance(currentNode.position, goal) < 0.1f)    // 현재 노드의 위치가 목표 위치에 거의 도달했다면 경로 탐색이 완료된 것
            {
                return ReconstructPath(currentNode);    // 경로를 재구성하고 반환
            }

            closedList.Add(currentNode.position);   // 이 위치가 이미 탐색되었음을 표시

            // 이웃 노드들을 가져와 반복문을 통해 각 이웃 노드를 검사
            foreach (var neighbor in GetNeighbors(currentNode.position))
            {
                // 이미 탐색된 위치라면 건너뜀
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeGScore = currentNode.gScore + Vector2.Distance(currentNode.position, neighbor);  // 이웃 노드의 gScore를 임시로 계산

                Node neighborNode = new Node(neighbor, currentNode, tentativeGScore, Vector2.Distance(neighbor, goal)); // 이웃 노드를 생성하고 그 노드의 부모를 현재 노드로 설정

                // openList에 이미 동일한 위치의 노드가 있고(TryGetValue()) 계산된 tentativeGScore가 기존 노드의 gScore보다 크거나 같으면 더 나은 경로가 아니므로 이 노드를 건너뜀
                if (openList.TryGetValue(neighborNode, out Node existingNeighbor) && tentativeGScore >= existingNeighbor.gScore)
                    continue;

                // 기존 노드가 존재하면 이를 제거하고 새로운 이웃 노드를 openList에 추가 [ 더 나은 경로가 발견된 경우 기존 경로를 대체 ]
                openList.Remove(existingNeighbor);
                openList.Add(neighborNode);
            }
        }

        return new List<Vector2>();  // 경로를 찾지 못한 경우 빈 리스트 반환
    }

    // 현재 노드의 위치에서 상하좌우 방향으로 인접한 노드들을 반환
    List<Vector2> GetNeighbors(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>();

        Vector2[] directions = {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        // 각 방향에서 장애물에 막혀있지 않으면 neighbors 리스트에 추가
        foreach (var direction in directions)
        {
            Vector2 neighborPos = position + direction;
            if (!IsTileBlocked(neighborPos))
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    // 특정 위치에 장애물이 있는지 확인
    // obstacleColliders 리스트에 있는 콜라이더가 해당 위치에 있으면 true를 반환
    bool IsTileBlocked(Vector2 position)
    {
        // 각 콜라이더를 검사하여 해당 위치가 막혀있는지 확인
        foreach (var collider in obstacleColliders)
        {
            if (collider.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    // 경로를 역추적하여 path 리스트에 저장
    List<Vector2> ReconstructPath(Node currentNode)
    {
        List<Vector2> path = new List<Vector2>();
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // 리스트를 뒤집어 올바른 순서로 반환
        return path;
    }

    // 현재 위치(position), 부모 노드(parent), 현재까지의 이동 비용(gScore), 목표 지점까지의 예상 비용(hScore) ,  FScore는 총 비용 =( gScore + hScore )
    private class Node
    {
        public Vector2 position;
        public Node parent;
        public float gScore;
        public float hScore;

        public Node(Vector2 position, Node parent, float gScore, float hScore)
        {
            this.position = position;
            this.parent = parent;
            this.gScore = gScore;
            this.hScore = hScore;
        }

        public float FScore => gScore + hScore;

        public override bool Equals(object obj)
        {
            if (obj is Node other)
                return position.Equals(other.position);
            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }

    // Node 객체들을 비교하여 SortedSet에서 사용될 수 있도록 함
    // FScore를 기준으로 노드를 비교하여 A* 알고리즘에서 최적의 경로를 찾음
    private class NodeComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if (x == null || y == null)
                return 0;

            int result = x.FScore.CompareTo(y.FScore);
            if (result == 0)
                result = x.position.x.CompareTo(y.position.x); // Prevent equality
            if (result == 0)
                result = x.position.y.CompareTo(y.position.y);

            return result;
        }
    }
}
