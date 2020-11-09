using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NavigationNode : MonoBehaviour, IHeapItem<NavigationNode>
{
    public HashSet<NavigationNode> connectedNodes = new HashSet<NavigationNode>();
    public float cost = 0.0f;

    private int heapIndex_;
    public int HeapIndex { get => heapIndex_; set => heapIndex_ = value; }

    private void OnDrawGizmosSelected()
    {
        foreach(NavigationNode n in connectedNodes) {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }

        Gizmos.DrawWireSphere(transform.position, PathFindingTester.NODE_CONNECTION_RANGE);
    }

    public void AddConnectedNode(NavigationNode n)
    {
        if(n == null) { return; }
        
        connectedNodes.Add(n);
    }

    public int CompareTo(NavigationNode other)
    {
        return -cost.CompareTo(other.cost);
    }

    public void RemoveConnectedNode(NavigationNode n)
    {
        connectedNodes.Remove(n);
    }
}
