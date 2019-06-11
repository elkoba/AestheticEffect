using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SphereMeshGenerator : EditorWindow
{
    #region Public Attributes
    public const string assetsFolder = "Assets/Models/Sphere";
    #endregion

    #region Private Attributes
    private static string _name = "Sphere_00";
    private static float _radius = 10.0f;
    private static int _longitudes = 16;
    private static int _latitudes = 12;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private int[] _triangles;
    #endregion

    #region Properties
    public float Pi
    {
        get { return Mathf.PI; }
    }
    #endregion

    #region Editor
    [MenuItem("MeshGenerator/Sphere Mesh Generator")]
    public static void ShowMenu()
    {
        SphereMeshGenerator window = EditorWindow.GetWindow<SphereMeshGenerator>("Shpere Window");
        window.minSize = new Vector2(150.0f, 200.0f);
        window.Show();
    }

    private void OnGUI()
    {
        // Receive the name the user wants for the sphere mesh
        EditorGUILayout.Separator();
        _name = EditorGUILayout.TextField("Mesh name", _name);

        // Get the data for the sphere creation
        EditorGUILayout.Separator();
        _radius = EditorGUILayout.Slider("Radius", _radius, 1.0f, 15.0f);
        _longitudes = EditorGUILayout.IntSlider("Meridians", _longitudes, 16, 128);
        _latitudes = EditorGUILayout.IntSlider("Parallels", _latitudes, 12, 64);

        // Button to create the sphere
        EditorGUILayout.Separator();
        if (GUILayout.Button("Generate Sphere"))
        {
            if (CreateSphere())
            {
                Debug.Log("You successfully created a sphere of radius " + _radius + "!");
                Close();
            }
            else
            {
                Debug.LogWarning("Error creating the sphere.");
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a sphere mesh
    /// </summary>
    /// <returns>
    /// True if the sphere mesh was successfully created, false if the sphere mesh could not be created
    /// </returns>
    private bool CreateSphere()
    {
        bool created = true;

        try
        {
            #region Vertices
            int totalVertices = (_longitudes + 1) * _latitudes + 2;
            _vertices = new Vector3[totalVertices];

            _vertices[0] = Vector3.up * _radius;

            for (int lat = 0; lat < _latitudes; lat++)
            {
                float phi = Pi * (float)(lat + 1) / (float)(_latitudes + 1);
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                for (int lon = 0; lon <= _longitudes; lon++)
                {
                    float theta = 2.0f * Pi * (float)(lon == _longitudes ? 0 : lon) / _longitudes;
                    float sinTheta = Mathf.Sin(theta);
                    float cosTheta = Mathf.Cos(theta);

                    _vertices[lon + lat * (_longitudes + 1) + 1] = new Vector3(sinPhi * cosTheta, cosPhi, sinPhi * sinTheta) * _radius;
                }
            }

            _vertices[_vertices.Length - 1] = -Vector3.up * _radius;
            #endregion

            #region Normals
            _normals = new Vector3[_vertices.Length];

            for (int n = 0; n < _vertices.Length; n++)
            {
                _normals[n] = _vertices[n].normalized;
            }
            #endregion

            #region Triangles
            int totalFaces = _vertices.Length;
            int totalTriangles = totalFaces * 2;
            int totalIndexes = totalTriangles * 3;
            _triangles = new int[totalIndexes];

            int i = 0;

            // Top cap triangles
            for (int lon = 0; lon < _longitudes; lon++)
            {
                _triangles[i++] = lon + 2;
                _triangles[i++] = lon + 1;
                _triangles[i++] = 0;
            }

            // Middle quads
            for (int lat = 0; lat < _latitudes - 1; lat++)
            {
                for (int lon = 0; lon < _longitudes; lon++)
                {
                    int current = lon + lat * (_longitudes + 1) + 1;
                    int next = current + _longitudes + 1;

                    _triangles[i++] = current;
                    _triangles[i++] = current + 1;
                    _triangles[i++] = next + 1;

                    _triangles[i++] = current;
                    _triangles[i++] = next + 1;
                    _triangles[i++] = next;
                }
            }

            // Bottom cap triangles
            for (int lon = 0; lon < _longitudes; lon++)
            {
                _triangles[i++] = _vertices.Length - 1;
                _triangles[i++] = _vertices.Length - (lon + 2) - 1;
                _triangles[i++] = _vertices.Length - (lon + 1) - 1;
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
            Debug.LogWarningFormat("The sphere could not be succesfully generated. Here is why: {0}", e.ToString());
            created = false;
        }

        return created;
    }
    #endregion
}
