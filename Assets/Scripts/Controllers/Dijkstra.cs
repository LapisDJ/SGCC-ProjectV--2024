using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public GameObject player;   // ���ʹ� player ��ü�� ��ǥ�� �̵�
    public LayerMask obstacleLayer; // ��ֹ� ���̾�  ( ���̾ ���ϴ� ��ü�� ��� Ž�� �� ���ؾ� �� )
    public LayerMask wallLayer; // �� ���̾� ( ���̾ ���ϴ� ��ü�� ��� Ž�� �� ���ؾ� �� )
    public float updateInterval = 0.5f; // ��� ������Ʈ ���� ( 0.5�ʸ��� ��θ� ���� )

    private List<Vector2> path = new List<Vector2>();   // ���� ��θ� �����ϴ� ����Ʈ ( ���Ͱ� ���󰡾� �� �� ��ġ ���� )
    private Vector2 targetPosition; // ���Ͱ� ���� �̵� ���� ��ǥ ��ġ
    private Rigidbody2D rb; // ���͸� �����ϴ� Rigidbody2D ������Ʈ�� ����
    private MonsterController monsterController;    // ���͸� �����ϴ� MonsterController ��ũ��Ʈ�� ����
    private float timeSinceLastUpdate = 0.0f;   // ������ ��� ������Ʈ ���� ����� �ð��� �����ϴ� ����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   // ������Ʈ�� ���� ��ü���� ������ rb ������ �Ҵ� ( ������ �������� ���� )
        monsterController = GetComponent<MonsterController>();  // MonsterController ������Ʈ�� ���� ��ü���� ������ monsterController ������ �Ҵ� ( �� ��ũ��Ʈ���� ��� )
        targetPosition = player.transform.position; // �÷��̾��� ���� ��ġ�� targetPosition���� ���� ( ���Ͱ� ó���� �̵��� ��ǥ )
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;  // ������ ��� ������Ʈ ���� ����� �ð��� ���� �������� �ð��� ���� ( Time.deltaTime�� ������ �����Ӱ� ���� ������ ������ �ð� ������ �ǹ� )

        if (timeSinceLastUpdate >= updateInterval || Vector2.Distance(rb.position, targetPosition) < 0.1f)  // ���� �ð��� ����߰ų� ���Ͱ� ��ǥ ��ġ�� ���� �������� �� ��θ� �ٽ� ���
        {
            path = FindPath(rb.position, player.transform.position);    // ���� ��ġ (rb.position)���� �÷��̾��� ��ġ (player.transform.position)������ �ִ� ��θ� ����Ͽ� path ������ ����
            timeSinceLastUpdate = 0.0f; // ��θ� ���� ��������Ƿ� ��� �ð��� 0���� ����

            if (path.Count > 0) // ��ΰ� ������ ���� ���� ( path ����Ʈ�� �ϳ� �̻��� ��尡 ���� ��쿡�� �̵� )
            {
                targetPosition = path[0];   // path�� ù ��° ��ġ�� targetPosition���� ���� ( ���Ͱ� �������� �̵��� ��ǥ ���� )
                path.RemoveAt(0);   // ����Ʈ���� ù ��° ��ġ�� �����Ͽ� ���� ���� �̵��� �غ�
            }   
        }

        MoveTowardsTarget();    // ��ǥ ��ġ(targetPosition)�� ���� ���͸� �̵���Ű�� �޼��带 ȣ��
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - rb.position).normalized;  // ���� ��ġ���� ��ǥ ��ġ�� ���� ���͸� normalized
        rb.velocity = direction * monsterController.speed;  // ���� ���Ϳ� MonsterController���� ������ �ӵ��� ���Ͽ�, Rigidbody2D�� �ӵ��� ����
                // speed�� MonsterController.cs���� [SerializeField] float speed = 0.0f;�� ����Ǽ� public���� ��ȯ�ϸ� �ذ��
    }

    List<Vector2> FindPath(Vector2 start, Vector2 goal) // ���� ��ġ(start)���� ��ǥ ��ġ(goal)������ �ִ� ��θ� ����ϴ� �޼��� ( A* �˰����� ����� ��θ� ��� )
    {
        var openList = new SortedSet<Node>(new NodeComparer()); // openList�� ��θ� Ž���ϴ� ���� Ȯ���� ������ �����ϴ� ����Ʈ ( SortedSet�� ����Ͽ� FScore ���� ���� �ڵ����� ���� 0
        var closedList = new HashSet<Vector2>();    // closedList�� �̹� Ž���� ��ģ ������ �����ϴ� ����Ʈ ( �ߺ��� ��� Ž���� �����ϱ� ���� ��� )
        var startNode = new Node(start, null, 0, Vector2.Distance(start, goal));    // ���� ��ġ������ ��带 ���� ( Node�� ���� ��ġ, �θ� ���, GScore, HScore�� �����ϴ� ����ü 0

        openList.Add(startNode);    // ���� ��带 openList�� �߰� ( ��� Ž���� ����� )
            
        while (openList.Count > 0)  // openList�� ��尡 �����ִ� ���� �ݺ� ( Ž���� ��尡 �������� ������ ��θ� ã�� )
        {
            Node currentNode = openList.Min;    // openList���� ���� ���� FScore ���� ���� ��带 ������ ( ���� Ž�� ���� ������ ��� )
            openList.Remove(currentNode);   // currentNode�� openList���� ����

            if (Vector2.Distance(currentNode.position, goal) < 0.1f)    // ���� ��尡 ��ǥ ��ġ�� �����ߴ��� Ȯ�� ( ��ǥ ��ġ���� �Ÿ��� 0.1 ���ֺ��� ������ ��ǥ�� ������ ������ ���� )
            {
                return ReconstructPath(currentNode);    // ��ǥ ��ġ�� ������ ��� ���� ������ �����Ͽ� ��θ� �������ϰ� �� ��θ� ��ȯ
            }

            closedList.Add(currentNode.position);   // ���� ����� ��ġ�� closedList�� �߰��Ͽ� �� ��ġ�� �ٽ� Ž������ �ʵ��� ��

            foreach (var neighbor in GetNeighbors(currentNode.position))    // ���� ����� �̿� ������ �ݺ��Ͽ� Ž�� ( �̿� ������ ���� ��ġ���� �̵� ������ ������ ��ġ )
            {
                if (closedList.Contains(neighbor))  // ���� �̿� ����� ��ġ�� �̹� closedList�� ���ԵǾ� ������ �� ��ġ�� �̹� Ž���� �Ϸ�� ������ �����ϰ� �� �̻� Ž������ ����
                    continue;   // ���� �̿� ���� �Ѿ

                float tentativeGScore = currentNode.gScore + Vector2.Distance(currentNode.position, neighbor);  // ���� ��忡�� �̿� �������� ���� ����� ���
                // currentNode.gScore�� ���� �������� ���� �������� ���� ��� , Vector2.Distance(currentNode.position, neighbor)�� ���� ��忡�� �̿� �������� �Ÿ��̸� �� ���� ���� ���� �̿� �������� �������� ��� (tentativeGScore)

                Node neighborNode = new Node(neighbor, currentNode, tentativeGScore, Vector2.Distance(neighbor, goal)); // ���ο� �̿� ��带 ����
                // neighbor: �̿� ����� ��ġ , currentNode: ���� ��带 �θ� ���� ���� , tentativeGScore: �ռ� ����� �̿� �������� �������� ���� ��� , �̿� ��忡�� ��ǥ ���������� ���� ���(�޸���ƽ)
                if (openList.TryGetValue(neighborNode, out Node existingNeighbor) && tentativeGScore >= existingNeighbor.gScore)    // �̿� ��尡 �̹� openList�� �ִ��� Ȯ���ϰ�, ���� �ִٸ� �� ����� gScore(���� ���)�� ��
                // openList.TryGetValue(neighborNode, out Node existingNeighbor): openList�� �̿� ��尡 �̹� �����ϴ��� Ȯ���ϰ� ������ existingNeighbor�� ����
                // entativeGScore >= existingNeighbor.gScore: ���� ����� tentativeGScore�� ������ gScore���� ũ�ų� ������ �̹� �ִ� ��ΰ� �� �����̹Ƿ� ���� ��θ� �����ϰ� ���� �̿� ��带 Ž��
                    continue;

                openList.Remove(existingNeighbor);  // ������ �̿� ��带 openList���� ���� ( �̿� ��带 Ž���� �� �� ���� ��ΰ� �߰ߵǾ����Ƿ� ������ ��δ� ���� )
                openList.Add(neighborNode); // ���� ���� ����� �̿� ��带 openList�� �߰� ( �� ��尡 ���� Ž�� ������� ���� )
            }
        }

        return new List<Vector2>(); // ��ǥ ������ �������� ������ �� �� ��� ����Ʈ�� ��ȯ ( ���Ͱ� �÷��̾�� ������ �� �ִ� ��θ� ã�� ���� �� )
    }

    List<Vector2> GetNeighbors(Vector2 position)    // ���� ��ġ���� �� ����(��, �Ʒ�, ����, ������)�� �ִ� ������ Ÿ�ϵ��� ��ǥ�� ��ȯ ( ��� Ž�� �� ����ؾ� �� �̿� ������ �����ϴ� �� ��� )
    {
        List<Vector2> neighbors = new List<Vector2>();  // �̿� ������ ��ǥ�� ������ ����Ʈ�� �ʱ�ȭ

        Vector2[] directions = {    // 4���� ���� ���͸� ����
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (var direction in directions)   // ���ǵ� �� ���⿡ ���� �ݺ����� ���� ( �� �������� �̿� ����� ��ġ�� ��� )
        {
            Vector2 neighborPos = position + direction; // ���� ��ġ���� �� �������� �̵��� ���ο� ��ġ(neighborPos)�� ��� ( �� ��ġ�� �̿� ����� ��ġ )
            if (!Physics2D.OverlapCircle(neighborPos, 0.1f, obstacleLayer) && !Physics2D.OverlapCircle(neighborPos, 0.1f, wallLayer))   // �̿� ����� ��ġ�� ��ֹ�(obstacleLayer)�̳� ��(wallLayer)�� ������ �ʴ��� Ȯ��
            // Physics2D.OverlapCircle�� Ư�� ��ġ�� ������ ���̾�(obstacleLayer �Ǵ� wallLayer)�� ���� ������Ʈ�� �ִ��� Ȯ��
            // neighborPos�� ��ֹ��̳� ���� �ش����� ������ �� ��ġ�� ��ȿ�� �̿� ���� ����
            {
                neighbors.Add(neighborPos); // ��ȿ�� �̿� ����� ��ġ�� neighbors ����Ʈ�� �߰�
            }
        }

        return neighbors;   // ��ȿ�� �̿� ������ ��ġ�� ��ȯ ( ��� Ž�� �˰��򿡼� ��� )
    }

    List<Vector2> ReconstructPath(Node currentNode) // ��ǥ ������ �������� ��, ��θ� �������Ͽ� �ϼ��� ��θ� ��ȯ
    {
        List<Vector2> path = new List<Vector2>();   // ��θ� ������ ����Ʈ�� �ʱ�ȭ
        while (currentNode != null) // ���� ��尡 null�� �ƴ� ���� ������(root node)���� �������� ������ �ݺ�
        {
            path.Add(currentNode.position); // ���� ����� ��ġ�� path ����Ʈ�� �߰� ( ���� ��忡�� ������������ ��ΰ� ���� )
            currentNode = currentNode.parent;   // ���� ����� �θ� ���� �̵� ( ��θ� �������ϴ� ���� )
        }
        path.Reverse();
        // ��� ����Ʈ�� ������ ��θ� ���������� ���� ( ������ �������� ����� ��δ� ��ǥ �������� ������������ ������ ����ǹǷ� reverse�Ͽ� ���������� ��ǥ ���������� ������ ���� )
        return path;    // �ϼ��� ��θ� ��ȯ ( ��δ� ���� ���󰡾� �� �ִ� ��� )
    }

    private class Node  // ��� Ž������ ���Ǵ� ���
    {
        public Vector2 position;    // �� ����� ��ġ
        public Node parent; // �� ����� �θ� ��� ( ��θ� ������ �� ��� )
        public float gScore; // ���� �������� ���� �������� ���� ��� ( �Ÿ� )
        public float hScore; // ���� ������ ��ǥ ���������� ���� ��� ( �Ÿ� )

        public Node(Vector2 position, Node parent, float gScore, float hScore)  // ����� ������
        {
            this.position = position;   // ����� ��ġ�� �ʱ�ȭ
            this.parent = parent;   // ����� �θ� �ʱ�ȭ
            this.gScore = gScore;   // ����� ���� ����� �ʱ�ȭ
            this.hScore = hScore;   // ����� ���� ����� �ʱ�ȭ
        }   

        public float FScore => gScore + hScore; // FScore�� �������� �� ������ ���
    }

    private class NodeComparer : IComparer<Node>    // Node ��ü���� ���ϱ� ���� Ŀ���� �񱳱⸦ ���� ( NodeComparer�� ������ ��� ������ �����ϴ� �� ��� )
    {
        public int Compare(Node x, Node y)  // �� Node ��ü�� ���Ͽ�, FScore�� ���� ������� ���ĵǵ��� ��
        {
            if (x == null || y == null) 
                return 0;   // �� ��� �� �ϳ��� null�� ��� ������ �ʰ� 0�� ��ȯ

            return x.FScore.CompareTo(y.FScore);    // �� ����� FScore�� ���Ͽ� ����� ��ȯ ( FScore�� ���� ��尡 �켱�� )
        }
    }
}