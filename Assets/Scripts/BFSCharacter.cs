using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSCharacter : MonoBehaviour
{
    public Transform target; // Este es el objetivo o meta del personaje
    public float speed = 2.0f;

    private Queue<Node> path;
    private HashSet<Node> visited;
    private List<Node> allNodes;

    void Start()
    {
        path = new Queue<Node>();
        visited = new HashSet<Node>();

        // Obtenemos todos los nodos en el laberinto
        allNodes = new List<Node>(FindObjectsOfType<Node>());

        Node startNode = GetClosestNode(transform.position);
        Node targetNode = GetClosestNode(target.position);

        BFS(startNode, targetNode);

        if (path.Count == 0)
        {
            Debug.Log("No path found to the target.");
        }
    }

    void Update()
    {
        if (path.Count > 0)
        {
            Node nextNode = path.Peek(); // Obtener el siguiente nodo sin eliminarlo
            if (Vector3.Distance(transform.position, nextNode.transform.position) < 0.1f)
            {
                path.Dequeue(); // Eliminar el nodo una vez alcanzado
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

    void BFS(Node start, Node target)
    {
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();
            Debug.Log("Visiting node: " + current.name);

            if (current == target)
            {
                Debug.Log("Target found!");
                ReconstructPath(current);
                break;
            }

            foreach (Node neighbor in current.neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    neighbor.prev = current; // Asigna el nodo anterior
                }
            }
        }
    }

    void ReconstructPath(Node endNode)
    {
        Node current = endNode;
        Stack<Node> stack = new Stack<Node>();

        while (current != null)
        {
            stack.Push(current);
            current = current.prev; // Obtener el nodo anterior
        }

        while (stack.Count > 0)
        {
            path.Enqueue(stack.Pop());
        }
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
        if (other.CompareTag("CoinBFS"))
        {
            Destroy(other.gameObject);
        }
    }
}
