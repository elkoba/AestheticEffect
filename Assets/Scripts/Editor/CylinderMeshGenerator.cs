using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CylinderMeshGenerator : EditorWindow
{
    #region Public Attributes
    public const string assetsFolder = "Assets/Models/Cylinder";
    #endregion

    #region Private Attributes
    private static string _name = "Cylinder_00";
    private static float _length = 1.0f;
    private static float _radius = 0.25f;
    private static int _divisions = 32;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private int[] _triangles;
    #endregion

    #region Properties
    public float Height
    {
        get { return _length * 0.5f; }
    }

    public float Pi
    {
        get { return Mathf.PI; }
    }
    #endregion

    #region Editor
    [MenuItem("MeshGenerator/Cylinder Mesh Generator")]
    public static void ShowMenu()
    {
        CylinderMeshGenerator window = EditorWindow.GetWindow<CylinderMeshGenerator>("Cylinder Window");
        window.minSize = new Vector2(150.0f, 200.0f);
        window.Show();
    }

    private void OnGUI()
    {
        // Receive the name the user wants for the cylinder mesh
        EditorGUILayout.Separator();
        _name = EditorGUILayout.TextField("Mesh Name", _name);

        // Get the data for the cylinder creation
        EditorGUILayout.Separator();
        _divisions = EditorGUILayout.IntSlider("Radial Divisions", _divisions, 16, 48);
        _radius = EditorGUILayout.Slider("Radius", _radius, 0.15f, 0.75f);

        EditorGUILayout.Separator();
        if (GUILayout.Button("Generate Cylinder"))
        {
            if (CreateCylinder())
            {
                Debug.Log("You successfully created a cylinder of radius " + _radius + " and " + _divisions + " divisions!");
                Close();
            }
            else
            {
                Debug.LogWarning("Error creating the cylinder.");
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a cylinder mesh
    /// </summary>
    /// <returns>
    /// True if the cylinder mesh was successfully created, false if the cylinder mesh could not be created
    /// </returns>
    private bool CreateCylinder()
    {
        bool created = true;

        try
        {
            #region Vertices
            int totalVertices = 2 + (4 * _divisions);
            _vertices = new Vector3[totalVertices];

            _vertices[0] = Vector3.up * Height;

            for (int layer = 0; layer < 2; layer++)
            {
                switch (layer)
                {
                    case 0:
                        for (int div = 0; div < _divisions; div++)
                        {
                            float a = 2.0f * Pi * (float)(div == _divisions ? 0 : div) / _divisions;
                            float sin = Mathf.Sin(a);
                            float cos = Mathf.Cos(a);

                            _vertices[div + 1] = new Vector3(_radius * cos, Height, _radius * sin);
                            _vertices[_divisions + div + 1] = new Vector3(_radius * cos, Height, _radius * sin);
                        }
                        break;

                    case 1:
                        for (int div = 0; div < _divisions; div++)
                        {
                            float a = 2.0f * Pi * (float)(div == _divisions ? 0 : div) / _divisions;
                            float sin = Mathf.Sin(a);
                            float cos = Mathf.Cos(a);

                            _vertices[2 * _divisions + div + 1] = new Vector3(_radius * cos, -Height, _radius * sin);
                            _vertices[3 * _divisions + div + 1] = new Vector3(_radius * cos, -Height, _radius * sin);
                        }
                        break;
                }
            }

            _vertices[_vertices.Length - 1] = -Vector3.up * Height;
            #endregion

            #region Normals
            _normals = new Vector3[totalVertices];

            _normals[0] = Vector3.up;

            for (int layer = 0; layer < 2; layer++)
            {
                switch (layer)
                {
                    case 0:
                        for (int div = 0; div < _divisions; div++)
                        {
                            float a = 2.0f * Pi * (float)(div == _divisions ? 0 : div) / _divisions;
                            float sin = Mathf.Sin(a);
                            float cos = Mathf.Cos(a);

                            _normals[div + 1] = Vector3.up;
                            _normals[_divisions + div + 1] = new Vector3(_radius * cos, 0.0f, _radius * sin).normalized;
                        }
                        break;

                    case 1:
                        for (int div = 0; div < _divisions; div++)
                        {
                            float a = 2.0f * Pi * (float)(div == _divisions ? 0 : div) / _divisions;
                            float sin = Mathf.Sin(a);
                            float cos = Mathf.Cos(a);

                            _normals[2 * _divisions + div + 1] = new Vector3(_radius * cos, 0.0f, _radius * sin).normalized;
                            _normals[3 * _divisions + div + 1] = -Vector3.up;
                        }
                        break;
                }
            }

            _normals[_vertices.Length - 1] = -Vector3.up;
            #endregion

            #region Triangles
            int totalTriangles = (2 * _divisions) + (3 * (2 * _divisions));
            int totalIndexes = 3 * totalTriangles;
            _triangles = new int[totalIndexes];
            int i = 0;

            // Top Cap
            for (int div = 0; div < _divisions; div++)
            {
                int centre = 0;
                int current = div + 1;
                int next = (current % _divisions) + 1;

                _triangles[i++] = centre;
                _triangles[i++] = next;
                _triangles[i++] = current;
            }

            // Middle layer, which will have 3 sets of rings, the two borders and the middle layer
            for (int layer = 0; layer < 3; layer++)
            { 
                for (int div = 0; div < _divisions; div++)
                {
                    int topCurrent = (layer * _divisions) + div + 1;
                    int topNext = (topCurrent % _divisions) + (layer * _divisions) + 1;
                    int botCurrent = topCurrent + _divisions;
                    int botNext = topNext + _divisions;

                    _triangles[i++] = topCurrent;
                    _triangles[i++] = topNext;
                    _triangles[i++] = botNext;

                    _triangles[i++] = topCurrent;
                    _triangles[i++] = botNext;
                    _triangles[i++] = botCurrent;
                }
            }

            // Bottom Cap
            for (int div = 0; div < _divisions; div++)
            {
                int centre = _vertices.Length - 1;
                int current = _vertices.Length - 1 - (div + 1);
                int next = _vertices.Length - 1 - (div + 2);
                if (next < (3 * _divisions) + 1)
                {
                    next += _divisions;
                }

                _triangles[i++] = centre;
                _triangles[i++] = next;
                _triangles[i++] = current;
            }
            #endregion

            #region Mesh
            _mesh = new Mesh();

            _mesh.vertices = _vertices;
            _mesh.normals = _normals;
            _mesh.triangles = _triangles;
            #endregion

            #region Directory Management
            string path = Path.Combine(assetsFolder, _name + ".asset");

            if (!Directory.Exists(assetsFolder))
            {
                Directory.CreateDirectory(assetsFolder);
            }

            AssetDatabase.CreateAsset(_mesh, path);
            AssetDatabase.Refresh();
            #endregion
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("The cylinder could not be succesfully generated. Here is why: {0}", e.ToString());
            created = false;
        }

        return created;
    }
    #endregion
}
