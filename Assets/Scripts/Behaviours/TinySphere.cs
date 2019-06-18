using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinySphere : MonoBehaviour
{
    #region Private Attributes
    private int index;
    public Vector3 originalPos;
    public Vector3 currentPos;
    Transform bigSphere;
    Mesh bigSphereMesh;
    #endregion

    #region Properties
    public Vector3 Vertex
    {
        get { return originalPos; }
        set { originalPos = value; }
    }

    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    #endregion

    void Awake()
    {
        Transform parent = transform.parent;
        Transform fancySphere = parent.parent;
        bigSphere = fancySphere.GetChild(0).GetChild(0);
        bigSphereMesh = bigSphere.GetComponent<MeshFilter>().mesh;

        currentPos = Vertex;
    }

    void Update()
    {
        Vector3 offset = (currentPos - originalPos);
        
        float dist = offset.magnitude;

        transform.position += transform.up * dist;
    }
}