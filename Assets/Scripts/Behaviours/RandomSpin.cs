using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpin : MonoBehaviour
{
    #region Attributes
    Vector3 currentAxis;
    private float angleSpeed = 60.0f;
    private float current = 0.0f;
    private float period = 20.0f;
    #endregion

    #region Properties
    
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        currentAxis = NewAxis();
    }

    private void Update()
    {
        Vector3 nextAxis = NewAxis();

        current += Time.deltaTime / period;

        currentAxis = Vector3.Lerp(currentAxis, nextAxis, current);

        if (current < 1.0f)
        {
            nextAxis = NewAxis();
            current = 0.0f;
        }

        float angle = 0.0f;
        angle += angleSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(angle, currentAxis);
    }
    #endregion

    #region Methods
    private Vector3 NewAxis()
    {
        return new Vector3(
            Random.Range(-360.0f, 360.0f),
            Random.Range(-360.0f, 360.0f),
            Random.Range(-360.0f, 360.0f)
            );
    }
    #endregion
}
