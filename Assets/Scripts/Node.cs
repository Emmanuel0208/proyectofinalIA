using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors;
    [HideInInspector] public Node prev; // Agregar la variable para almacenar el nodo anterior

    void OnDrawGizmos()
    {
        if (neighbors == null) return;

        Gizmos.color = Color.red;
        foreach (Node neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}
