using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    #region Public Attributes
    public GameObject a;
    public GameObject b;
    #endregion

    #region Private Attributes

    #endregion

    #region Properties
    public GameObject A
    {
        get { return a; }
        set { a = value; }
    }

    public GameObject B
    {
        get { return b; }
        set { b = value; }
    }

    public Vector3 AB
    {
        get { return (a.transform.position - b.transform.position); }
    }

    public Vector3 Pos
    {
        get { return 0.5f * (a.transform.position + b.transform.position); }
    }

    public Quaternion Rot
    {
        get
        {
            Vector3 newUp = AB;
            newUp.Normalize();
            Quaternion rot = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), newUp);

            return rot;
        }
    }

    public float Size
    {
        get { return AB.magnitude; }
    }
    #endregion

    #region Monobehaviour
    void Update()
    {
        UpdateTransform();
    }
    #endregion

    #region Methods
    public void UpdateTransform()
    {
        float size = Size;

        transform.position = Pos;
        transform.rotation = Rot;
        transform.localScale = new Vector3(1.0f, size, 1.0f);
    }
    #endregion
}
