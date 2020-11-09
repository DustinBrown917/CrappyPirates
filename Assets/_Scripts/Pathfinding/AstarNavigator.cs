using System.Collections.Generic;
using UnityEngine;

public static class AstarNavigator
{

    public static List<NavigationNode> GetPath(NavigationNode start, NavigationNode end)
    {
        Heap<NavigationNode> frontier = new Heap<NavigationNode>(100);
        Dictionary<NavigationNode, NavigationNode> cameFrom = new Dictionary<NavigationNode, NavigationNode>();
        cameFrom.Add(start, null);
        Dictionary<NavigationNode, float> costSoFar = new Dictionary<NavigationNode, float>();
        costSoFar.Add(start, 0);
        start.cost = 0;
        frontier.Add(start);

        while(frontier.Count > 0) {
            NavigationNode current = frontier.Pop();
            if(current == end) { break; }

            foreach(NavigationNode next in current.connectedNodes) {
                float newCost = current.cost + Vector3.Distance(next.transform.position, current.transform.position);

                float oldCost = 0;
                bool keyExists = costSoFar.TryGetValue(next, out oldCost);

                if(!keyExists || newCost < oldCost) {
                    if (!keyExists) { costSoFar.Add(next, newCost); }
                    else { costSoFar[next] = newCost; }

                    next.cost = newCost + Heuristic(end, next); //Cost to be used with ICompareable

                    frontier.Add(next);
                    if (!cameFrom.ContainsKey(next)) { cameFrom.Add(next, null); }
                    cameFrom[next] = current;
                }
            }
        }

        Debug.Log(cameFrom.Count);

        List<NavigationNode> finalPath = new List<NavigationNode>();
        NavigationNode n = end;
        finalPath.Add(n);

        while (n != start) {
            n = cameFrom[n];
            finalPath.Add(n);
        }

        return finalPath;
    }

    private static float Heuristic(NavigationNode to, NavigationNode from)
    {
        return Vector3.Distance(to.transform.position, from.transform.position);
    }
}
