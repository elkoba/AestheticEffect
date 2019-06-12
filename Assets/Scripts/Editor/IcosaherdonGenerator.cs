using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IcosahedronGenerator : EditorWindow
{
    #region Public Attributes
    public const string assetsFolder = "Assets/Prefabs/Icosahedron";
    public GameObject ico;
    public GameObject nod;
    public GameObject edg;
    #endregion

    #region Private Attributes
    private static string _name = "Icosahedron";
    private static float _radius = 13.0f;
    private Mesh _nodeMesh;
    private Mesh _edgeMesh;
    private Material _icosahedronMaterial;
    private Vector3[] _vertices;
    private List<GameObject> _nodes;
    private List<GameObject> _edges;
    #endregion

    #region Properties
    public float SideSize
    {
        get { return (_nodes[0].transform.position - _nodes[1].transform.position).magnitude; }
    }
    #endregion

    #region Editor
    [MenuItem("Prefab Generator/Icosahedron Prefab Generator")]
    public static void ShowMenu()
    {
        IcosahedronGenerator window = EditorWindow.GetWindow<IcosahedronGenerator>("Icosahedron Generator");
        window.minSize = new Vector2(250.0f, 100.0f);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Separator();
        GUILayout.Label("Name of your Object", EditorStyles.boldLabel);
        _name = EditorGUILayout.TextField("Icosahedron Name", _name);

        EditorGUILayout.Separator();
        GUILayout.Label("Object info", EditorStyles.boldLabel);
        _radius = EditorGUILayout.Slider("Icosahedron Radius", _radius, 10.0f, 20.0f);

        EditorGUILayout.Separator();
        GUILayout.Label("Meshes for the Object", EditorStyles.boldLabel);
        _nodeMesh = EditorGUILayout.ObjectField(string.Format("Node Mesh"), _nodeMesh, typeof(Mesh), false) as Mesh;
        _edgeMesh = EditorGUILayout.ObjectField(string.Format("Edge Mesh"), _edgeMesh, typeof(Mesh), false) as Mesh;

        EditorGUILayout.Separator();
        GUILayout.Label("Materials for the Object", EditorStyles.boldLabel);
        _icosahedronMaterial = EditorGUILayout.ObjectField(string.Format("Icosahedron Material"), _icosahedronMaterial, typeof(Material), false) as Material;

        EditorGUILayout.Separator();
        if (GUILayout.Button("Generate Icoshaedron"))
        {
            if (CreateIcosahedron())
            {
                Debug.Log("You successfully created an icosahedron of radius " + _radius + "!");
            }
            else
            {
                Debug.LogWarning("Error creating the icosahedron.");
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates an icosahedron
    /// </summary>
    /// <returns>
    /// True if the icosahedron was successfully created, false if the icosahedron could not be created
    /// </returns>
    private bool CreateIcosahedron()
    {
        bool created = true;

        try
        {
            // Initialize the objects for the hierarchy
            ico = new GameObject();
            nod = new GameObject();
            edg = new GameObject();

            // Set the hierarchy
            ico.name = "Icosahedron";
            nod.name = "Nodes";
            edg.name = "Edges";
            nod.transform.parent = ico.transform;
            edg.transform.parent = ico.transform;

            // Set each part of the icosahedron
            SetNodes();
            SetEdges();
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("The Icosahedron could not be successfully generated. Here is why: {0}", e.ToString());
        }

        return created;
    }

    /// <summary>
    /// This method handles the edges creation
    /// </summary>
    private void SetEdges()
    {
        _edges = new List<GameObject>();

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                // Instantiate the object and adjust its transform
                GameObject newEdge = new GameObject();

                // Add the components
                MeshFilter mf = newEdge.AddComponent<MeshFilter>();
                MeshRenderer mr = newEdge.AddComponent<MeshRenderer>();
                Edge ec = newEdge.AddComponent<Edge>();

                // Set the edge component info
                ec.A = _nodes[i];
                ec.B = _nodes[j];

                // Add the proper mesh and material
                mf.mesh = _edgeMesh;
                mr.material = _icosahedronMaterial;

                // Adjust its transform
                newEdge.name = "Edge_" + ec.A.GetComponent<Node>().Index + "_" + ec.B.GetComponent<Node>().Index;
                newEdge.transform.parent = edg.transform;
                ec.UpdateTransform();

                // Add the edge to the list of edges
                _edges.Add(newEdge);
            }
        }

        FilterEdges();
    }

    /// <summary>
    ///  This method filters the non-valid edges from the list of edges
    /// </summary>
    private void FilterEdges()
    {
        for (int i = _edges.Count - 1; i >= 0; i--)
        {
            bool isValid = true;
            GameObject edgeObj = _edges[i];
            Edge edge = edgeObj.GetComponent<Edge>();

            // Mark as not valid the edges that has both extremes the same node
            if (edge.A.GetComponent<Node>().Index == edge.B.GetComponent<Node>().Index)
            {
                isValid = false;
            }
            // Mark as not valid the edges that don't connect adjacent nodes
            else if (Mathf.Abs(edge.Size - SideSize) > 0.01f)
            {
                isValid = false;
            }
            // Mark as not valid the edges that are repeated (i.e.: Edge_0_1 and Edge_1_0 are repeated)
            else
            {
                foreach (GameObject e in _edges)
                {
                    if (edge.A.GetComponent<Node>().Index == e.GetComponent<Edge>().B.GetComponent<Node>().Index &&
                        edge.B.GetComponent<Node>().Index == e.GetComponent<Edge>().A.GetComponent<Node>().Index)
                    {
                        isValid = false;
                    }
                }
            }

            // Remove the non-valid edges
            if (!isValid)
            {
                _edges.RemoveAt(i);
                DestroyImmediate(edgeObj);
            }
        }
    }

    /// <summary>
    /// This method handles the node creation
    /// </summary>
    private void SetNodes()
    {
        // Set the nodes position, which will be the vertices position of an icosahedron
        GenerateNodePositions();
        _nodes = new List<GameObject>();

        // Generate all the nodes
        for (int n = 0; n < 12; n++)
        {
            // Create the node
            GameObject newNode = new GameObject();

            // Add the proper components
            MeshFilter mf = newNode.AddComponent<MeshFilter>();
            MeshRenderer mr = newNode.AddComponent<MeshRenderer>();
            Node nc = newNode.AddComponent<Node>();

            // Set the node component info
            nc.Index = n;

            // Add the proper mesh and material
            mf.mesh = _nodeMesh;
            mr.material = _icosahedronMaterial;

            // Adjust its transform
            newNode.name = "Node_" + n;
            newNode.transform.parent = nod.transform;
            newNode.transform.position = _vertices[n];
            newNode.transform.up = _vertices[n];

            // Add the node to the list of nodes
            _nodes.Add(newNode);
        }
    }

    /// <summary>
    ///  This method generates the positions of the nodes
    /// </summary>
    private void GenerateNodePositions()
    {
        float goldenRatio = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
        int i = 0;
        _vertices = new Vector3[12];

        // XY Plane Vertices
        _vertices[i++] = new Vector3(-1.0f, goldenRatio, 0.0f).normalized * _radius;
        _vertices[i++] = new Vector3(1.0f, goldenRatio, 0.0f).normalized * _radius;
        _vertices[i++] = new Vector3(-1.0f, -goldenRatio, 0.0f).normalized * _radius;
        _vertices[i++] = new Vector3(1.0f, -goldenRatio, 0.0f).normalized * _radius;

        // YZ Plane Vertices
        _vertices[i++] = new Vector3(0.0f, -1.0f, goldenRatio).normalized * _radius;
        _vertices[i++] = new Vector3(0.0f, 1.0f, goldenRatio).normalized * _radius;
        _vertices[i++] = new Vector3(0.0f, -1.0f, -goldenRatio).normalized * _radius;
        _vertices[i++] = new Vector3(0.0f, 1.0f, -goldenRatio).normalized * _radius;

        // XZ Plane Vertices
        _vertices[i++] = new Vector3(goldenRatio, 0.0f, -1.0f).normalized * _radius;
        _vertices[i++] = new Vector3(goldenRatio, 0.0f, 1.0f).normalized * _radius;
        _vertices[i++] = new Vector3(-goldenRatio, 0.0f, -1.0f).normalized * _radius;
        _vertices[i++] = new Vector3(-goldenRatio, 0.0f, 1.0f).normalized * _radius;
    }
    #endregion
}
