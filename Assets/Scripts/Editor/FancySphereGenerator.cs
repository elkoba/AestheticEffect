using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEditor;

public class FancySphereGenerator : EditorWindow
{
    #region Public Attributes
    public GameObject fancy;
    public GameObject big;
    public GameObject wired;
    public GameObject mini;
    #endregion

    #region Private Attributes
    private static string _name = "Fancy Sphere";
    private static float _radius = 10.0f;
    private static float _minMiniSphereScale = 0.1f;
    private static float _maxMiniSphereScale = 1.0f;
    private static int _totalMiniSpheres = 150;
    private int _totalMiniSphereMaterials = 2;
    private Mesh _mainSphereMesh;
    private Mesh _tinySphereMesh;
    private Material _bigSphereMaterial;
    private Material _wiredSphereMaterial;
    private Material[] _tinySpheresMaterials = new Material[_totalMiniSpheres];
    private List<GameObject> _miniSpheres;
    #endregion

    #region Properties

    #endregion

    #region Editor
    [MenuItem("Complex Object Generator/Fancy Sphere")]
    private static void ShowMenu()
    {
        FancySphereGenerator window = EditorWindow.GetWindow<FancySphereGenerator>("Fancy Sphere Generator");
        window.minSize = new Vector2(250.0f, 100.0f);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Separator();
        GUILayout.Label("Name of your Object", EditorStyles.boldLabel);
        _name = EditorGUILayout.TextField("Fancy Sphere Name", _name);

        EditorGUILayout.Separator();
        GUILayout.Label("Object info", EditorStyles.boldLabel);
        _radius = EditorGUILayout.Slider("Fancy Sphere Radius", _radius, 5.0f, 15.0f);
        _totalMiniSpheres = EditorGUILayout.IntSlider("Number of Tiny Spheres", _totalMiniSpheres, 100, 200);

        EditorGUILayout.Separator();
        GUILayout.Label("Meshes for the Object", EditorStyles.boldLabel);
        _mainSphereMesh = EditorGUILayout.ObjectField(string.Format("Main Sphere Mesh"), _mainSphereMesh, typeof(Mesh), false) as Mesh;
        _tinySphereMesh = EditorGUILayout.ObjectField(string.Format("Tiny Sphere Mesh"), _tinySphereMesh, typeof(Mesh), false) as Mesh;

        EditorGUILayout.Separator();
        GUILayout.Label("Materials for the Object", EditorStyles.boldLabel);
        _bigSphereMaterial = EditorGUILayout.ObjectField(string.Format("Main Sphere Material"), _bigSphereMaterial, typeof(Material), false) as Material;
        _wiredSphereMaterial = EditorGUILayout.ObjectField(string.Format("Wired Sphere Material"), _wiredSphereMaterial, typeof(Material), false) as Material;
        EditorGUILayout.Separator();
        _totalMiniSphereMaterials = EditorGUILayout.IntSlider("Mini Sphere Materials", _totalMiniSphereMaterials, 1, 10);
        for (int i = 0; i < _totalMiniSphereMaterials; i++) 
        {
            _tinySpheresMaterials[i] = EditorGUILayout.ObjectField(string.Format("Material " + (i + 1)), _tinySpheresMaterials[i], typeof(Material), false) as Material;
        }
        

        EditorGUILayout.Separator();
        if (GUILayout.Button("Generate Fancy Sphere"))
        {
            if (CreateFancySphere())
            {
                Debug.Log("You successfully created a fancy sphere of radius " + _radius + " and " + _totalMiniSpheres + " mini spheres!");
            }
            else
            {
                Debug.LogWarning("Error creating the fancy sphere.");
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a fancy sphere
    /// </summary>
    /// <returns>
    /// True if the fancy sphere was successfully created, false if the fancy sphere could not be created
    /// </returns>
    private bool CreateFancySphere()
    {
        bool created = true;

        try
        {
            // Initialize the objects for the hierarchy
            fancy = new GameObject();
            big = new GameObject();
            wired = new GameObject();
            mini = new GameObject();

            // Stablish the hierarchy
            fancy.name = "Fancy Sphere";
            big.name = "Big Sphere";
            wired.name = "Wired Sphere";
            mini.name = "Tiny Spheres";
            big.transform.parent = fancy.transform;
            wired.transform.parent = fancy.transform;
            mini.transform.parent = fancy.transform;

            // Give the parent the proper component
            FancySphere sph = fancy.AddComponent<FancySphere>();

            // Set each part of the fancy sphere
            SetBigSphere();
            SetWiredSphere();
            SetMiniBalls();
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("The Fancy Sphere could not be successfully generated. Here is why: {0}", e.ToString());
        }

        return created;
    }

    /// <summary>
    /// This method handles the tiny spheres creation
    /// </summary>
    private void SetMiniBalls()
    {
        for (int i = 0; i < _totalMiniSpheres; i++)
        {
            // Create the object
            GameObject newMiniSphere = new GameObject();

            // Add the propet components
            MeshFilter mf = newMiniSphere.AddComponent<MeshFilter>();
            MeshRenderer mr = newMiniSphere.AddComponent<MeshRenderer>();
            TinySphere ts = newMiniSphere.AddComponent<TinySphere>();

            // Add the proper mesh and material
            mf.mesh = _tinySphereMesh;
            mr.material = _tinySpheresMaterials[Random.Range(0, _totalMiniSphereMaterials)];

            // Set the tiny sphere component info
            HandleTinySphereData(ts);

            // Adjust its transform
            newMiniSphere.transform.name = "Tiny Ball " + i;
            newMiniSphere.transform.parent = mini.transform;
            newMiniSphere.transform.position = ts.Vertex;
            newMiniSphere.transform.localScale *= Random.Range(_minMiniSphereScale, _maxMiniSphereScale);
        }
    }

    /// <summary>
    /// This method sets a random position on the surface of the big sphere
    /// </summary>
    /// <returns>.
    /// This method returns the Vector3 with the sphereical coordinates the tiny ball must have to be on a random spot on the surface of the big sphere
    /// </returns>
    private void HandleTinySphereData(TinySphere _ts)
    {
        Mesh bigSphereMesh = big.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        int totalVertices = bigSphereMesh.vertexCount;
        int randomVertex = Random.Range(250, totalVertices - 250);

        _ts.Index = randomVertex;
        _ts.Vertex = bigSphereMesh.vertices[randomVertex];
    }

    /// <summary>
    /// This method handles the big sphere creation
    /// </summary>
    private void SetWiredSphere()
    {
        // Set the object
        GameObject wiredSphere = new GameObject();

        // Add the proper components
        MeshFilter mf = wiredSphere.AddComponent<MeshFilter>();
        MeshRenderer mr = wiredSphere.AddComponent<MeshRenderer>();
        WiredSphere ws = wiredSphere.AddComponent<WiredSphere>();

        // Add the proper mesh and material
        mf.mesh = _mainSphereMesh;
        mr.material = _wiredSphereMaterial;

        // Adjust its transform
        wiredSphere.name = "Wired_Sphere";
        wiredSphere.transform.parent = wired.transform;
        wiredSphere.transform.position = Vector3.zero;
        wiredSphere.transform.localScale *= 1.025f;
    }

    /// <summary>
    /// This method handles the big sphere creation
    /// </summary>
    private void SetBigSphere()
    {
        // Set the object
        GameObject bigSphere = new GameObject();

        // Add the proper components
        MeshFilter mf = bigSphere.AddComponent<MeshFilter>();
        MeshRenderer mr = bigSphere.AddComponent<MeshRenderer>();

        // Adjust its transform
        bigSphere.name = "Big_Sphere";
        bigSphere.transform.parent = big.transform;
        bigSphere.transform.position = Vector3.zero;

        // Add the proper mesh and material
        mf.mesh = _mainSphereMesh;
        mr.material = _bigSphereMaterial;
    }
    #endregion
}
