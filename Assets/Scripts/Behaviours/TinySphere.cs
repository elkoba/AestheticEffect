using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinySphere : MonoBehaviour
{
    #region Private Attributes
    [SerializeField] private int index;
    [SerializeField] private Vector3 vertex;
    #endregion

    #region Properties
    public Vector3 Vertex
    {
        get { return vertex; }
        set { vertex = value; }
    }

    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    #endregion

    void Awake()
    {
        transform.position = vertex;
    }

    void Update()
    {
        transform.position = vertex;
    }
}
