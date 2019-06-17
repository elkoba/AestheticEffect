using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulges : MonoBehaviour
{
    #region Public Attributes
    [Range(1.0f, 2.5f)]
    public float maxBulgeMagnitude = 1.0f;
    [Range(1.0f, 3.0f)]
    public float maxAreaOfEffect = 1.0f;
    [Range(0.25f, 0.75f)]
    public float bulgeProbability = 0.25f;
    [Range(1.0f, 2.5f)]
    public float bulgeSpeed = 1.0f;
    #endregion

    #region Private Attributes
    private static int _totalVertices;
    private static int _tinySphereTotalPositions;
    private static Vector3[] _mainSphereVerticesOriginalPositions;
    private static Vector3[] _wireSphereVerticesOriginalPositions;
    private static Vector3[] _tinySpheresOriginalPositions;
    private static Vector3[] _normals;
    private Mesh _mainSphereMesh;
    private Mesh _wireSphereMesh;
    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        // Variables to initialize and set the vertices and normals and tiny sphere positions and directions
        Transform mainSphere = transform.GetChild(0);
        Transform wireSphere = transform.GetChild(1);
        Transform tinySpheres = transform.GetChild(2);

        MeshFilter msmf = mainSphere.GetChild(0).GetComponent<MeshFilter>();
        MeshFilter wsmf = wireSphere.GetChild(0).GetComponent<MeshFilter>();

        _mainSphereMesh = msmf.mesh;
        _wireSphereMesh = wsmf.mesh;

        int totalTinySpheres = tinySpheres.childCount;

        // Initialize the arrays and set their values
        _totalVertices = msmf.mesh.vertices.Length;

        _mainSphereVerticesOriginalPositions = msmf.mesh.vertices;
        _wireSphereVerticesOriginalPositions = wsmf.mesh.vertices;
        _normals = msmf.mesh.normals;

        _tinySphereTotalPositions = tinySpheres.childCount;
        _tinySpheresOriginalPositions = new Vector3[totalTinySpheres];
        for (int i = 0; i < totalTinySpheres; i++)
        {
            _tinySpheresOriginalPositions[i] = tinySpheres.GetChild(i).transform.position;
        }
    }

    private void Update()
    {
        bool bulge = MakeBulge();

        if (bulge)
        {
            int i = Random.Range(0, _totalVertices);

            Bulge(i);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// This method handles the bulge behaviour
    /// </summary>
    /// <param name="index">
    /// The index of the centre of the bulge, which will be the index of the vertex of our sphere meshes
    /// </param>
    private void Bulge(int index)
    {
        Vector3 mainSphereCentre = _mainSphereVerticesOriginalPositions[index];
        Vector3 wireSphereCentre = _wireSphereVerticesOriginalPositions[index];
        Vector3 direction = _normals[index];
        float aoe = Random.Range(0.5f, maxAreaOfEffect);
        
        for (int i = 0; i < _totalVertices; i++)
        {
            float dist = DistToVertex(index, i);

            if (dist < aoe)
            {
                float height = dist / aoe;
                float dt = Time.deltaTime;
                float y = 0.1f;
                y += (-(height * height) + 1) * dt;

                while (y > 0.0f)
                {
                    _mainSphereMesh.vertices[i] += _normals[i] * bulgeSpeed * y;
                }
            }
        }
    }

    private float DistToVertex(int centre, int checking)
    {
        Vector3 bulgeCentre = _mainSphereVerticesOriginalPositions[centre];
        Vector3 checkingVertex = _wireSphereVerticesOriginalPositions[checking];

        float dist = (checkingVertex - bulgeCentre).magnitude;

        return dist;
    }

    /// <summary>
    /// This method handles the chances of making a bulge
    /// </summary>
    /// <returns>
    /// Returns if there will be a bulge or not
    /// </returns>
    private bool MakeBulge()
    {
        float p = Random.Range(0.0f, 1.0f);

        if (p < bulgeProbability)
            return true;
        else
            return false;
    }
    #endregion
}
