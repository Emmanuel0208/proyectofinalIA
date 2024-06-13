using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraCharacter : MonoBehaviour
{
    public Transform target; // Este es el objetivo o meta del personaje
    public float speed = 2.0f;

    public List<Node> path;
    private List<Node> allNodes;
    public List<GameObject> coins;
    private int coinsCollected = 0;
    private Node startNode;
    private Node targetNode;
    private bool isMoving = false;

    public int continuador = 0;

    void Start()
    {
        path = new List<Node>();

        // Obtenemos todos los nodos en el laberinto
        allNodes = new List<Node>(FindObjectsOfType<Node>());
        Debug.Log("Total nodes found: " + allNodes.Count);

        // Obtenemos todas las monedas en el laberinto
        coins = new List<GameObject>(GameObject.FindGameObjectsWithTag("Coin"));
        Debug.Log("Total coins found: " + coins.Count);

        startNode = GetClosestNode(transform.position);
        targetNode = GetClosestNode(target.position);
        Debug.Log("Start node: " + startNode.name);
        Debug.Log("Target node: " + targetNode.name);

        FindNextCoin();
    }

    void Update()
    {
        if (!isMoving && path.Count > 0)
        {
            StartCoroutine(MoveToNextNode(path[continuador]));
        }
    }

    IEnumerator MoveToNextNode(Node node)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, node.transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, node.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = node.transform.position; // Asegúrate de que el personaje esté exactamente en la posición del nodo

        if (path.Count > 1)
        {
            /* path.RemoveAt(0);*/ // Eliminar el nodo una vez alcanzado
            continuador++;
            Debug.Log("Reached node: " + node.name);
        }

        if (path.Count == 0 && coinsCollected < 10)
        {
            FindNextCoin();
        }
        else if (path.Count == 0 && coinsCollected >= 10)
        {
            Dijkstra(GetClosestNode(transform.position), targetNode);
        }

        isMoving = false;
    }

    void Dijkstra(Node start, Node target)
    {
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>(allNodes);

        foreach (Node node in allNodes)
        {
            dist[node] = float.MaxValue;
            prev[node] = null;
        }

        dist[start] = 0;

        while (unvisited.Count > 0)
        {
            Node current = null;
            foreach (Node node in unvisited)
            {
                if (current == null || dist[node] < dist[current])
                {
                    current = node;
                }
            }

            if (current == target)
            {
                break;
            }

            unvisited.Remove(current);

            foreach (Node neighbor in current.neighbors)
            {
                float alt = dist[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;
                }
            }
        }

        Stack<Node> stack = new Stack<Node>();
        Node currentPathNode = target;

        while (prev[currentPathNode] != null)
        {
            stack.Push(currentPathNode);
            currentPathNode = prev[currentPathNode];
        }

        path.Clear();
        while (stack.Count > 0)
        {
            path.Add(stack.Pop());
        }

        Debug.Log("Path found with " + path.Count + " nodes.");
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

        Debug.Log("Closest node found: " + closestNode.name);
        return closestNode;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coinsCollected++;
            Debug.Log("Coin collected. Total coins: " + coinsCollected);
            if (coinsCollected < 10)
            {
                FindNextCoin();
            }
            else
            {
                Dijkstra(GetClosestNode(transform.position), targetNode);
            }
        }
    }

    void FindNextCoin()
    {
        if (coins.Count > 0)
        {
            GameObject closestCoin = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject coin in coins)
            {
                if (coin != null)
                {
                    float distance = Vector3.Distance(transform.position, coin.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCoin = coin;
                    }
                }
            }

            if (closestCoin != null)
            {
                Node coinNode = GetClosestNode(closestCoin.transform.position);
                Dijkstra(GetClosestNode(transform.position), coinNode);
                Debug.Log("Next coin found at: " + closestCoin.transform.position);
            }
        }
    }
}
