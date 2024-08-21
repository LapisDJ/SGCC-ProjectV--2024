using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public class Node
    {
        public Vector3Int Position { get; private set; }
        public List<Node> Neighbors { get; private set; }

        public Node(Vector3Int position)
        {
            Position = position;
            Neighbors = new List<Node>();
        }

        public void AddNeighbor(Node neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
            }
        }
    }

    public class PriorityQueue<T>
    {
        private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add(new KeyValuePair<T, float>(item, priority));
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            int bestIndex = 0;
            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].Value < elements[bestIndex].Value)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Key;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

    public Tilemap tilemap;
    public Transform player;
    public Transform monster;
    private Vector3Int playerCellPosition;
    private Vector3Int monsterCellPosition;

    private Dictionary<Vector3Int, Node> graph = new Dictionary<Vector3Int, Node>();

    void Start()
    {
        GenerateGraphFromTilemap();
    }

    void Update()
    {
        playerCellPosition = tilemap.WorldToCell(player.transform.position);
        monsterCellPosition = tilemap.WorldToCell(monster.transform.position);

        if (graph.ContainsKey(monsterCellPosition) && graph.ContainsKey(playerCellPosition))
        {
            List<Vector3Int> path = FindPath(monsterCellPosition, playerCellPosition);

            if (path.Count > 0)
            {
                Vector3 nextPosition = tilemap.CellToWorld(path[0]);
                monster.position = Vector3.MoveTowards(monster.position, nextPosition, Time.deltaTime * 2f);
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
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (!tilemap.HasTile(tilePosition))
                {
                    Node node = new Node(tilePosition);
                    graph[tilePosition] = node;

                    Vector3Int[] neighbors = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
                    foreach (Vector3Int direction in neighbors)
                    {
                        Vector3Int neighborPos = tilePosition + direction;
                        if (!tilemap.HasTile(neighborPos) && graph.ContainsKey(neighborPos))
                        {
                            node.AddNeighbor(graph[neighborPos]);
                            graph[neighborPos].AddNeighbor(node);
                        }
                    }
                }
            }
        }
    }

    List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        if (!graph.ContainsKey(start) || !graph.ContainsKey(goal))
        {
            Debug.LogError("Start or goal position is not in the graph.");
            return new List<Vector3Int>();
        }

        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(graph[start], 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();

        cameFrom[graph[start]] = graph[start];
        costSoFar[graph[start]] = 0;

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current.Position == goal)
                break;

            foreach (Node next in current.Neighbors)
            {
                float newCost = costSoFar[current] + 1;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next.Position, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = graph[goal];

        while (currentNode.Position != start)
        {
            path.Add(currentNode.Position);
            if (!cameFrom.ContainsKey(currentNode))
            {
                Debug.LogWarning("Incomplete path found.");
                return new List<Vector3Int>();
            }
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
