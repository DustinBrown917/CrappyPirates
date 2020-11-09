using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

public class PathFindingTester : MonoBehaviour
{
    public const float NODE_CONNECTION_RANGE = 4.0f;

    private List<NavigationNode> path = new List<NavigationNode>();

    [SerializeField] private NavigationNode startNode = null;
    [SerializeField] private NavigationNode endNode = null;

    [SerializeField] private ForuneSiteMono[] fortuneSites = new ForuneSiteMono[0];
    [SerializeField] private float minX = -5.0f;
    [SerializeField] private float minY = -5.0f;
    [SerializeField] private float maxX = 5.0f;
    [SerializeField] private float maxY = 5.0f;

    private LinkedList<VEdge> edges = new LinkedList<VEdge>();

    private void Awake()
    {
        List<FortuneSite> sites = new List<FortuneSite>();
        foreach(ForuneSiteMono fsm in fortuneSites) {
            sites.Add(fsm.GetSite());
        }

        edges = FortunesAlgorithm.Run(sites, minX, minY, maxX, maxY);

        Debug.Log(edges.Count);

        Dictionary<Vector2, NavigationNode> navNodes = new Dictionary<Vector2, NavigationNode>();

        int i = 1;

        foreach(VEdge edge in edges) {
            if (!navNodes.ContainsKey(edge.Start)) {
                navNodes.Add(edge.Start, new GameObject("VoronoiNavNode" + i).AddComponent<NavigationNode>());
                navNodes[edge.Start].transform.position = (Vector2)edge.Start;
                i++;
            }

            if (!navNodes.ContainsKey(edge.End)) {
                navNodes.Add(edge.End, new GameObject("VoronoiNavNode" + i).AddComponent<NavigationNode>());
                navNodes[edge.End].transform.position = (Vector2)edge.End;
                i++;
            }

            navNodes[edge.Start].AddConnectedNode(navNodes[edge.End]);
            navNodes[edge.End].AddConnectedNode(navNodes[edge.Start]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if(edges.Count > 1) {
            foreach(VEdge edge in edges) {
                Gizmos.DrawLine(new Vector3((float)edge.Start.X, (float)edge.Start.Y, 0), new Vector3((float)edge.End.X, (float)edge.End.Y, 0));
            }
        }

        Gizmos.color = Color.red;
        if (path.Count > 1) {
            for (int i = 0; i < path.Count - 1; i++) {
                Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);
            }
        }

        if(startNode != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(startNode.transform.position, 0.3f);
        }


        if (endNode != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(endNode.transform.position, 0.3f);
        }
    }

    [Button]
    public void FindPath()
    {
        path = AstarNavigator.GetPath(startNode, endNode);
        Debug.Log(path.Count);
    }
}
