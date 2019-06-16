using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    #region Attributes
    private int index;

    private float angle = 45.0f;
    private float amplitude ;
    private float rand;
    private Vector3 floor;
    private bool jumping = false;
    private float freq;
    #endregion

    #region Properties
    public int Index
    {
        get { return index; }
        set { index = value; }
    }
    public float Height
    {
        get { return transform.localPosition.magnitude; }
    }
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        freq = Random.Range(0.75f, 1.5f);
        rand = Random.Range(0.0f, 5.0f);
        amplitude = Random.Range(0.1f, 1.0f);
        InvokeRepeating("JumpAgain", rand, 2.5f);
    }
    private void Update()
    {
        floor = transform.position;
        if (jumping)
            Jump();
    }
    #endregion

    #region Methods
    private void Jump()
    {
        float dt = Time.deltaTime;
        angle += dt * freq;
        float cos = Mathf.Cos(angle);

        if (jumping)
        {
            transform.position -= amplitude * 0.25f * transform.up * cos * 0.25f;
            if (Height < 12.5f)
            {
                jumping = false;
            }
        }
    }

    private void JumpAgain()
    {
        jumping = true;
    }
    #endregion
}
