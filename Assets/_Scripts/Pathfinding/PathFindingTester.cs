using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

public class PathFindingTester : MonoBehaviour
{
    public const float NODE_CONNECTION_RANGE = 4.0f;

    private Stack<NavigationNode> path = new Stack<NavigationNode>();

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

        Dictionary<Vector3, NavigationNode> navNodes = new Dictionary<Vector3, NavigationNode>();

        int i = 1;

        foreach(VEdge edge in edges) {
            Vector3 start = TransposeToZ(edge.Start);
            Vector3 end = TransposeToZ(edge.End);

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

    private Vector3 TransposeToZ(VPoint p)
    {
        return new Vector3((float)p.X, 0, (float)p.Y);
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
                Gizmos.DrawLine(new Vector3((float)edge.Start.X, 0, (float)edge.Start.Y), new Vector3((float)edge.End.X, 0, (float)edge.End.Y));
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
