using UnityEngine;

public class WiredSphere : MonoBehaviour
{
    private float textureSpeed = -0.00075f;

    void Update()
    {
        float dt = Time.deltaTime;

        float offsetY = textureSpeed * dt;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.mainTextureOffset += new Vector2(0.0f, textureSpeed);
    }
}
