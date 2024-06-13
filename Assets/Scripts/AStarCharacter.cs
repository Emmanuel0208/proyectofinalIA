using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarCharacter : MonoBehaviour
{
    public Transform target; // Este es el objetivo o meta del personaje
    public float speed = 2.0f;

    private List<Node> path;
    private List<Node> allNodes;

    void Start()
    {
        path = new List<Node>();

        // Obtenemos todos los nodos en el laberinto
        allNodes = new List<Node>(FindObjectsOfType<Node>());

        Node startNode = GetClosestNode(transform.position);
        Node targetNode = GetClosestNode(target.position);

        AStar(startNode, targetNode);

        if (path.Count == 0)
        {
            Debug.Log("No path found to the target.");
        }
    }

    void Update()
    {
        if (path.Count > 0)
        {
            Node nextNode = path[0];
            if (Vector3.Distance(transform.position, nextNode.transform.position) < 0.1f)
            {
                path.RemoveAt(0);
                Debug.Log("Reached node: " + nextNode.name);
            }
            else
            {
                StartCoroutine(MoveToNextNode(nextNode));
            }
        }
    }

    IEnumerator MoveToNextNode(Node node)
    {
        while (Vector3.Distance(transform.position, node.transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, node.transform.position, speed * Time.deltaTime);
            yield return null;
        }
    }

    void AStar(Node start, Node target)
    {
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        foreach (Node node in allNodes)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
            cameFrom[node] = null;
        }

        gScore[start] = 0;
        fScore[start] = Heuristic(start, target);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            foreach (Node node in openSet)
            {
                if (fScore[node] < fScore[current])
                {
                    current = node;
                }
            }

            if (current == target)
            {
                ReconstructPath(cameFrom, current);
                return;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentative_gScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentative_gScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);
            }
        }

        Debug.Log("No path found to the target.");
    }

    void ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        Stack<Node> stack = new Stack<Node>();
        while (cameFrom[current] != null)
        {
            stack.Push(current);
            current = cameFrom[current];
        }

        while (stack.Count > 0)
        {
            path.Add(stack.Pop());
        }
    }

    float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    Node GetClosestNode(Vector3 position)
    {
        Node closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (Node node in allNodes)
        {
            float distance = Vector3.Distance(position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
        }
    }
}
