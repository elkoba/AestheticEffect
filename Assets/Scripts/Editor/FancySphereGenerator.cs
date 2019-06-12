using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FancySphereGenerator : EditorWindow
{
    #region Public Attributes
    public GameObject mainSphere;
    public GameObject wiredSphere;
    public List<GameObject> miniSpheres;
    #endregion

    #region Private Attributes
    private static string _name = "Fancy Sphere";
    private static float _radius = 10.0f;
    private static int _totalMiniSpheres = 100;
    private int _totalMiniSphereMaterials = 2;
    private Mesh _mainSphereMesh;
    private Mesh _tinySphereMesh;
    private Material _mainSphereMaterial;
    private List<Material> _tinySpheresMaterials = new List<Material>();
    #endregion

    #region Properties

    #endregion

    #region Editor
    [MenuItem("Prefab Generator/Fancy Sphere Prefab Generator")]
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
        _totalMiniSpheres = EditorGUILayout.IntSlider("Number of Tiny Spheres", _totalMiniSpheres, 50, 150);

        EditorGUILayout.Separator();
        GUILayout.Label("Meshes for the Object", EditorStyles.boldLabel);
        _mainSphereMesh = EditorGUILayout.ObjectField(string.Format("Main Sphere Mesh"), _mainSphereMesh, typeof(Mesh), false) as Mesh;
        _tinySphereMesh = EditorGUILayout.ObjectField(string.Format("Tiny Sphere Mesh"), _tinySphereMesh, typeof(Mesh), false) as Mesh;

        EditorGUILayout.Separator();
        GUILayout.Label("Materials for the Object", EditorStyles.boldLabel);
        _mainSphereMaterial = EditorGUILayout.ObjectField(string.Format("Main Sphere Material"), _mainSphereMaterial, typeof(Material), false) as Material;
        EditorGUILayout.Separator();
        _totalMiniSphereMaterials = EditorGUILayout.IntSlider("Mini Sphere Materials", _totalMiniSphereMaterials, 1, 10);
        for (int i = 0; i < _totalMiniSphereMaterials; i++) 
        {
            Material mat = EditorGUILayout.ObjectField(string.Format("Material " + (i + 1)), _mainSphereMaterial, typeof(Material), false) as Material;
            _tinySpheresMaterials.Add(mat);
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
    private bool CreateFancySphere()
    {
        bool created = true;

        try
        {

        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("The Fancy Sphere could not be successfully generated. Here is why: {0}", e.ToString());
        }

        return created;
    }
    #endregion
}
