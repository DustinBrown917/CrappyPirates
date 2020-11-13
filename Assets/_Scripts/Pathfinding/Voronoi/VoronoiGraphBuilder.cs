using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

public class VoronoiGraphBuilder : MonoBehaviour
{
    private static VoronoiGraphBuilder instance_ = null;
    public static VoronoiGraphBuilder Instance { get => instance_; }

    [SerializeField] private bool zBased = false;

    [SerializeField] private Transform boundaryMax = null;
    [SerializeField] private Transform boundaryMin = null;

    private ForuneSiteMono[] fortuneSites = new ForuneSiteMono[0];

    Dictionary<Vector3, NavigationNode> navNodes = new Dictionary<Vector3, NavigationNode>();

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ForuneSiteMono[] foundSites = FindObjectsOfType<ForuneSiteMono>();

        List<ForuneSiteMono> sitesInBoundary = new List<ForuneSiteMono>();

        foreach(ForuneSiteMono fsm in foundSites) {
            if (IsInBoundaries(fsm.transform.position)) {
                sitesInBoundary.Add(fsm);
            }
        }

        fortuneSites = sitesInBoundary.ToArray();

        BuildGraph();
    }

    private void OnDestroy()
    {
        if(instance_== this) {
            instance_ = null;
        }
    }

    private void BuildGraph()
    {

        List<FortuneSite> sites = new List<FortuneSite>();
        foreach (ForuneSiteMono fsm in fortuneSites) {
            sites.Add(fsm.GetSite(zBased));
        }

        LinkedList<VEdge> edges = new LinkedList<VEdge>();

        if (zBased) {
            edges = FortunesAlgorithm.Run(sites, boundaryMin.position.x, boundaryMin.position.z, boundaryMax.position.x, boundaryMax.position.z);
        } else {
            edges = FortunesAlgorithm.Run(sites, boundaryMin.position.x, boundaryMin.position.y, boundaryMax.position.x, boundaryMax.position.z);
        }

        int i = 1;

        foreach (VEdge edge in edges) {
            Vector3 start;
            Vector3 end;

            if (zBased) {
                start = edge.Start.Y2Z;
                end = edge.End.Y2Z;
            } else {
                start = (Vector2)edge.Start;
                end = (Vector2)edge.End;
            }

            if (!navNodes.ContainsKey(start)) {
                navNodes.Add(start, new GameObject("VoronoiNavNode" + i).AddComponent<NavigationNode>());
                navNodes[start].transform.position = start;
                i++;
            }

            if (!navNodes.ContainsKey(end)) {
                navNodes.Add(end, new GameObject("VoronoiNavNode" + i).AddComponent<NavigationNode>());
                navNodes[end].transform.position = end;
                i++;
            }

            navNodes[start].AddConnectedNode(navNodes[end]);
            navNodes[end].AddConnectedNode(navNodes[start]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        if(navNodes.Count > 0) {
            foreach(NavigationNode n in navNodes.Values) {
                foreach(NavigationNode neighbour in n.connectedNodes) {
                    Gizmos.DrawLine(n.transform.position, neighbour.transform.position);
                }
            }
        }

        if(boundaryMax != null && boundaryMin != null) {
            Gizmos.DrawLine(boundaryMax.transform.position, new Vector3(boundaryMin.transform.position.x, boundaryMax.transform.position.y, boundaryMax.transform.position.z));
            Gizmos.DrawLine(boundaryMax.transform.position, new Vector3(boundaryMax.transform.position.x, boundaryMax.transform.position.y, boundaryMin.transform.position.z));
            Gizmos.DrawLine(boundaryMin.transform.position, new Vector3(boundaryMax.transform.position.x, boundaryMax.transform.position.y, boundaryMin.transform.position.z));
            Gizmos.DrawLine(boundaryMin.transform.position, new Vector3(boundaryMin.transform.position.x, boundaryMax.transform.position.y, boundaryMax.transform.position.z));
        }
    }

    private bool IsInBoundaries(Vector3 v)
    {
        if (zBased) {
            return (v.x <= boundaryMax.position.x && v.z <= boundaryMax.position.z && v.x >= boundaryMin.position.x && v.z >= boundaryMin.position.z);
        } else {
            return (v.x <= boundaryMax.position.x && v.y <= boundaryMax.position.y && v.x >= boundaryMin.position.x && v.y >= boundaryMin.position.y);
        }
    }

    public NavigationNode GetNearestNavNode(Vector3 point)
    {
        NavigationNode closestNode = null;
        float shortestDistance = float.MaxValue;
        foreach(Vector3 key in navNodes.Keys) {
            float currentDistance = Vector3.Distance(key, point);
            if (currentDistance < shortestDistance) {
                shortestDistance = currentDistance;
                closestNode = navNodes[key];
            }
        }

        return closestNode;
    }
}
