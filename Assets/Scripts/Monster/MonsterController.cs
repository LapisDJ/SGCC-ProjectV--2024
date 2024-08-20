using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterController : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 0.0f;
    private List<Vector2> path = new List<Vector2>();  // A* �˰����� ���� ���� ������ �̵� ��θ� �����ϴ� Vector2 ����Ʈ
    private Vector2 targetPosition;    // ���Ͱ� ���� �̵� ���� ��ǥ ��ġ
    private List<TilemapCollider2D> obstacleColliders = new List<TilemapCollider2D>();  // Ÿ�ϸ��� ��ֹ����� �����ϴ� TilemapCollider2D ������Ʈ�� ����Ʈ
    [SerializeField] private Monster monster;
    private Vector2 lastPlayerPosition;  // ���������� ��θ� ����� �÷��̾��� ��ġ

    void Start()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody2D>();
        speed = monster.GetCurrentSpeed();  // Monster ��ũ��Ʈ���� �ӵ� ��������

        // �±׷� ������Ʈ�� ã�� TilemapCollider2D ������Ʈ�� obstacleColliders ����Ʈ�� �߰�
        foreach (var collider in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            TilemapCollider2D tilemapCollider = collider.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                obstacleColliders.Add(tilemapCollider);
            }
        }

        lastPlayerPosition = player.transform.position;
        UpdatePath();  // ���Ͱ� ó�� �̵��� ��θ� ����
    }

    void Update()
    {
        // ���Ͱ� ��ǥ ��ġ�� ���� �������� ��, ����Ʈ���� ���� ��ǥ ��ġ�� ������
        if (path.Count > 0 && Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            targetPosition = path[0];
            path.RemoveAt(0);
        }

        MoveTowardsTarget();    // ���� ��ǥ ��ġ�� �̵��� ����

        // �÷��̾��� ��ġ�� ������ ��ġ���� ���� �Ÿ� �̻� �������� �� ��� ������Ʈ
        if (Vector2.Distance(player.transform.position, lastPlayerPosition) > 1.0f)
        {
            UpdatePath();
            lastPlayerPosition = player.transform.position;
        }
    }

    // ��ǥ ��ġ�� ���ϴ� ���� ���͸� ����ϰ� �� �������� �̵� �ӵ��� ����
    void MoveTowardsTarget()
    {
        if (path.Count > 0)
        {
            Vector2 direction = (targetPosition - rb.position).normalized;  // ���Ͱ� �������� �� ����
            rb.velocity = direction * speed;
        }
    }

    // A* �˰����� ����Ͽ� ���Ϳ� �÷��̾� ������ ��θ� ����ϰ� �� ��θ� path ����Ʈ�� ����
    void UpdatePath()
    {
        path = FindPath(rb.position, player.transform.position);    // ���� ������ ��ġ�� �÷��̾��� ��ġ ������ ���� ��θ� ���

        if (path.Count > 0) // ��ΰ� ���������� ���Ǹ� ù ��° ��ǥ ��ġ�� targetPosition���� ���� �� path���� ����
        {
            targetPosition = path[0];
            path.RemoveAt(0);
        }
    }

    // A* �˰����� ����
    // ���� ��ġ�� ��ǥ ��ġ ������ ���� ��� ã��
    List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        var openList = new SortedSet<Node>(new NodeComparer()); // Ž���� ���� [ FScore(gScore + hScore) ������������ ���� ]
        var closedList = new HashSet<Vector2>();    // �̹� Ž���� �Ϸ�� ���� [ �ߺ��� ��� Ž�� ���� ]
        var startNode = new Node(start, null, 0, Vector2.Distance(start, goal));    // ���� ��� [ gScore�� 0 ,  hScore�� Vector2.Distance(start, goal)�� ���� ]

        openList.Add(startNode);    // ���� ��带 openList�� �߰��Ͽ� Ž��

        // ���� ����� ���� ��带 �˻��ϰ� ��� ����(gScore)�� �� ���� ��� ��θ� ������Ʈ
        while (openList.Count > 0)
        {
            // openList���� FScore�� ���� ���� ��带 currentNode�� �����ϰ� �� ��带 openList���� ����
            Node currentNode = openList.Min;
            openList.Remove(currentNode);

            if (Vector2.Distance(currentNode.position, goal) < 0.1f)    // ���� ����� ��ġ�� ��ǥ ��ġ�� ���� �����ߴٸ� ��� Ž���� �Ϸ�� ��
            {
                return ReconstructPath(currentNode);    // ��θ� �籸���ϰ� ��ȯ
            }

            closedList.Add(currentNode.position);   // �� ��ġ�� �̹� Ž���Ǿ����� ǥ��

            // �̿� ������ ������ �ݺ����� ���� �� �̿� ��带 �˻�
            foreach (var neighbor in GetNeighbors(currentNode.position))
            {
                // �̹� Ž���� ��ġ��� �ǳʶ�
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeGScore = currentNode.gScore + Vector2.Distance(currentNode.position, neighbor);  // �̿� ����� gScore�� �ӽ÷� ���

                Node neighborNode = new Node(neighbor, currentNode, tentativeGScore, Vector2.Distance(neighbor, goal)); // �̿� ��带 �����ϰ� �� ����� �θ� ���� ���� ����

                // openList�� �̹� ������ ��ġ�� ��尡 �ְ�(TryGetValue()) ���� tentativeGScore�� ���� ����� gScore���� ũ�ų� ������ �� ���� ��ΰ� �ƴϹǷ� �� ��带 �ǳʶ�
                if (openList.TryGetValue(neighborNode, out Node existingNeighbor) && tentativeGScore >= existingNeighbor.gScore)
                    continue;

                // ���� ��尡 �����ϸ� �̸� �����ϰ� ���ο� �̿� ��带 openList�� �߰� [ �� ���� ��ΰ� �߰ߵ� ��� ���� ��θ� ��ü ]
                openList.Remove(existingNeighbor);
                openList.Add(neighborNode);
            }
        }

        return new List<Vector2>();  // ��θ� ã�� ���� ��� �� ����Ʈ ��ȯ
    }

    // ���� ����� ��ġ���� �����¿� �������� ������ ������ ��ȯ
    List<Vector2> GetNeighbors(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>();

        Vector2[] directions = {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        // �� ���⿡�� ��ֹ��� �������� ������ neighbors ����Ʈ�� �߰�
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

    // Ư�� ��ġ�� ��ֹ��� �ִ��� Ȯ��
    // obstacleColliders ����Ʈ�� �ִ� �ݶ��̴��� �ش� ��ġ�� ������ true�� ��ȯ
    bool IsTileBlocked(Vector2 position)
    {
        // �� �ݶ��̴��� �˻��Ͽ� �ش� ��ġ�� �����ִ��� Ȯ��
        foreach (var collider in obstacleColliders)
        {
            if (collider.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    // ��θ� �������Ͽ� path ����Ʈ�� ����
    List<Vector2> ReconstructPath(Node currentNode)
    {
        List<Vector2> path = new List<Vector2>();
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // ����Ʈ�� ������ �ùٸ� ������ ��ȯ
        return path;
    }

    // ���� ��ġ(position), �θ� ���(parent), ��������� �̵� ���(gScore), ��ǥ ���������� ���� ���(hScore) ,  FScore�� �� ��� =( gScore + hScore )
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

    // Node ��ü���� ���Ͽ� SortedSet���� ���� �� �ֵ��� ��
    // FScore�� �������� ��带 ���Ͽ� A* �˰��򿡼� ������ ��θ� ã��
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
