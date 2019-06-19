using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulges : MonoBehaviour
{
    #region Typedefs

    public struct Bulge
    {
        public Vector3 center;
        public float aoe;
        public float duration;
        public float timer;
    }

    #endregion

    #region Public Attributes

    [Range(0.5f, 1.0f)]
    public float minAreaOfEffect = 0.5f;
    [Range(1.5f, 3.0f)]
    public float maxAreaOfEffect = 1.0f;
    [Range(1.0f, 2.5f)]
    public float bulgeSpeed = 1.0f;

    public float minTimeBetweenBulges = 0.5f;
    public float maxTimeBetweenBulges = 1.0f;

    public float minBulgeDuration = 1.5f;
    public float maxBulgeDuration = 3.0f;

    #endregion

    #region Private Attributes

    private int _totalVertices;
    private int _tinySphereTotalPositions;
    private int _totalTinySpheres;
    private Vector3[] _mainSphereVerticesOriginalPositions;
    private Vector3[] _wireSphereVerticesOriginalPositions;
    private Vector3[] _tinySpheresOriginalPositions;
    private Vector3[] _normals;
    private Transform[] _tinySpheres;
    private Mesh _mainSphereMesh;
    private Mesh _wireSphereMesh;
    private float sphereRadius = 10.0f;

    private MeshFilter mainSphereMF;
    private MeshFilter wireSphereMF;

    private Vector3[] mainVerts;
    private Vector3[] wireVerts;
    private Vector3[] tsPositions;

    private float timer = 0.0f;
    private float timeNextBulge = 0.0f;
    private List<Bulge> activeBulges = new List<Bulge>();

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

        mainSphereMF = mainSphere.GetChild(0).GetComponent<MeshFilter>();
        wireSphereMF = wireSphere.GetChild(0).GetComponent<MeshFilter>();

        _mainSphereMesh = mainSphereMF.mesh;
        _wireSphereMesh = wireSphereMF.mesh;

        _totalTinySpheres = tinySpheres.childCount;

        // Initialize the arrays and set their values
        _totalVertices = _mainSphereMesh.vertices.Length;

        // copy the arrays to be sure we don't keep referencing the arrays we're changing
        _mainSphereVerticesOriginalPositions = new Vector3[_totalVertices];
        _wireSphereVerticesOriginalPositions = new Vector3[_totalVertices];
        _normals = new Vector3[_totalVertices];
        Array.Copy(_mainSphereMesh.vertices, _mainSphereVerticesOriginalPositions, _totalVertices);
        Array.Copy(_wireSphereMesh.vertices, _wireSphereVerticesOriginalPositions, _totalVertices);
        Array.Copy(_mainSphereMesh.normals, _normals, _totalVertices);

        // get the radius from one of the vertices of the sphere (the first one for instance)
        sphereRadius = _mainSphereVerticesOriginalPositions[0].magnitude;

        // get the list of tiny spheres' centers
        _tinySphereTotalPositions = _totalTinySpheres;
        _tinySpheresOriginalPositions = new Vector3[_totalTinySpheres];
        _tinySpheres = new Transform[_totalTinySpheres];
        for (int i = 0; i < _totalTinySpheres; i++)
        {
            _tinySpheres[i] = tinySpheres.GetChild(i);
            _tinySpheresOriginalPositions[i] = _tinySpheres[i].localPosition;
        }

        // create the list of vertices we'll be modifying each tick
        mainVerts = new Vector3[_totalVertices];
        wireVerts = new Vector3[_totalVertices];
        tsPositions = new Vector3[_totalTinySpheres];
        Array.Copy(_mainSphereVerticesOriginalPositions, mainVerts, _totalVertices);
        Array.Copy(_wireSphereVerticesOriginalPositions, wireVerts, _totalVertices);
        Array.Copy(_tinySpheresOriginalPositions, tsPositions, _totalTinySpheres);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        timer += dt;

        // we may want to add bulges
        if (timer >= timeNextBulge)
            AddBulge();

        // always start resetting the vertices to their original positions
        Array.Copy(_mainSphereVerticesOriginalPositions, mainVerts, _totalVertices);
        Array.Copy(_wireSphereVerticesOriginalPositions, wireVerts, _totalVertices);
        Array.Copy(_tinySpheresOriginalPositions, tsPositions, _totalTinySpheres);

        // update the vertices according to the active bulges
        for (int i = activeBulges.Count - 1; i >= 0; --i)
        {
            // update the bulge first
            Bulge b = activeBulges[i];
            UpdateBulge(dt, ref b);

            // if it's over let's remove it from the list, otherwise update it (remember we have to do this because it's a struct instead of a class)
            if (b.timer < b.duration)
                activeBulges[i] = b;
            else
                activeBulges.RemoveAt(i);
        }

        // finally update the mesh
        _mainSphereMesh.vertices = mainVerts;
        _wireSphereMesh.vertices = wireVerts;
        for (int i = 0; i < _totalTinySpheres; i++)
        {
            _tinySpheres[i].localPosition = tsPositions[i];
        }
        mainSphereMF.mesh = _mainSphereMesh;
        wireSphereMF.mesh = _wireSphereMesh;
    }
    #endregion

    #region Methods

    /// <summary>
    /// Updates the given bulge and adapts the vertices of the spheres according to its current position and time
    /// </summary>
    /// <param name="b"></param>
    private void UpdateBulge(float dt, ref Bulge b)
    {
        // always update the time first
        b.timer += dt;

        // we can early quit if the bulge is over
        if (b.timer >= b.duration)
            return;

        // calculate the intensity with the current time
        float intensity = 0.0f;
        if (b.timer < 0.5f * b.duration)
        {
            // the bulge is growing
            intensity = b.timer / (0.5f * b.duration);
        }
        else if (b.timer < b.duration)
        {
            // only update if it's not over
            intensity = (b.duration - b.timer) / (0.5f * b.duration);
        }

        // now with the intensity we can calculate the area of effect
        float areaOfEffect = intensity * b.aoe;
        float aoeSq = areaOfEffect * areaOfEffect;

        // and with that we can update the vertices in the spheres
        Vector3 dir = b.center.normalized;
        for (int i = 0; i < _totalVertices; ++i)
        {
            Vector3 origV = _mainSphereVerticesOriginalPositions[i];
            float distSq = (b.center - origV).sqrMagnitude;
            if (distSq < aoeSq)
            {
                // we're close enough to be affected by the bulge
                float dist = Mathf.Sqrt(distSq);
                float t = Mathf.Clamp01(dist / areaOfEffect) * 0.5f * Mathf.PI;      //< normalize in the range [0..PI/2] because cos(0) = 1, cos(PI/2) = 0
                float height = Mathf.Cos(t) * 0.5f * areaOfEffect;

                // add that height to the vertex
                // NOTE: [Barkley] Not sure if it's nicer with this line uncommented. The effect seems subtle so I left it commented out, but I think it looks slightly nicer with it uncommented
                dir = origV.normalized;
                mainVerts[i] += dir * height;
                wireVerts[i] += dir * height;
            }
        }

        // I tried to adpat the code above but it doesn't work, balls don' come back to their original place, they stay still on the
        // highest position of the bulge once they get there. I don't really understand why, tho

        Transform parent = transform.parent;
        Transform tinySpheres = transform.GetChild(2);
        int totalMiniSpheres = tinySpheres.childCount;
        for (int i = 0; i < totalMiniSpheres; i++)
        {
            Vector3 origS = tinySpheres.GetChild(i).transform.localPosition;
            float distSq = (b.center - origS).sqrMagnitude;
            int vertIndex = tinySpheres.GetChild(i).GetComponent<TinySphere>().Index;
            if (distSq < aoeSq)
            {
                // we're close enough to be affected by the bulge
                float dist = Mathf.Sqrt(distSq);
                float t = Mathf.Clamp01(dist / areaOfEffect) * 0.5f * Mathf.PI;      //< normalize in the range [0..PI/2] because cos(0) = 1, cos(PI/2) = 0
                float height = Mathf.Cos(t) * 0.5f * areaOfEffect;

                // add that height to the vertex
                // NOTE: [Barkley] Not sure if it's nicer with this line uncommented. The effect seems subtle so I left it commented out, but I think it looks slightly nicer with it uncommented
                dir = origS.normalized;
                tsPositions[i] += dir * height;
            }
        }
    }

    /// <summary>
    /// Adds a new bulge, with random values for the center, the duration and (maybe) other stuff, to the list
    /// </summary>
    private void AddBulge()
    {
        // create the new bulge
        Bulge b = new Bulge();
        b.center = UnityEngine.Random.onUnitSphere * sphereRadius;
        b.duration = UnityEngine.Random.Range(minBulgeDuration, maxBulgeDuration);
        b.timer = 0.0f;
        b.aoe = UnityEngine.Random.Range(minAreaOfEffect, maxAreaOfEffect);

        // add it to the list
        activeBulges.Add(b);

        // calculate the time for the new bulge
        float interval = UnityEngine.Random.Range(minTimeBetweenBulges, maxTimeBetweenBulges);
        timeNextBulge += interval;
    }

    #endregion
}
