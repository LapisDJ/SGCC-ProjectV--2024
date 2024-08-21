using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class MonsterController : MonoBehaviour
{
    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;

        public int gCost = 0;
        public int hCost = 0;
        public Node parent;

        public int fCost { get { return gCost + hCost; } }

        public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPosition = _worldPosition;
            gridX = _gridX;
            gridY = _gridY;
            gCost = int.MaxValue; // 초기값 설정
            hCost = int.MaxValue; // 초기값 설정
        }

    }

    public Transform player;  // 플레이어의 위치
    public float speed = 5f;  // 몬스터 이동 속도
    public int gridWidth, gridHeight;
    public float nodeSize;
    public Tilemap[] unwalkableTilemaps;  // 이동 불가능한 타일맵들

    private Node[,] grid;
    private List<Node> path;

    void Start()
    {
        CreateGrid();
        StartCoroutine(UpdatePath());
    }

    void CreateGrid()
    {
        grid = new Node[gridWidth, gridHeight];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWidth / 2 - Vector3.up * gridHeight / 2;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + nodeSize / 2) + Vector3.up * (y * nodeSize + nodeSize / 2);
                bool walkable = IsWalkable(worldPoint);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    bool IsWalkable(Vector3 worldPoint)
    {
        foreach (Tilemap tilemap in unwalkableTilemaps)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
            TileBase tile = tilemap.GetTile(cellPosition);

            if (tile != null)
            {
                return false;  // 타일맵에 타일이 존재하면 이동 불가능한 영역으로 설정
            }
        }

        return true;  // 어느 타일맵에도 해당하지 않는 경우 이동 가능
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            path = FindPath(transform.position, player.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

    List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return 10 * (dstX + dstY);
    }


    Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x - transform.position.x + gridWidth * nodeSize / 2) / (gridWidth * nodeSize);
        float percentY = (worldPosition.y - transform.position.y + gridHeight * nodeSize / 2) / (gridHeight * nodeSize);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridWidth - 1) * percentX);
        int y = Mathf.RoundToInt((gridHeight - 1) * percentY);

        return grid[x, y];
    }


    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    void Update()
    {
        if (path != null && path.Count > 0)
        {
            Vector3 targetPosition = path[0].worldPosition;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                path.RemoveAt(0);
            }
        }
    }
}


