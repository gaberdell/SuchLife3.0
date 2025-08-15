using UnityEngine;

public class BuffIconFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset = new Vector3(0.5f, -0.5f, 0f);

    void Update()
    {
        if (playerTransform == null) return;

        transform.position = playerTransform.position + offset;
        transform.rotation = Camera.main.transform.rotation; // Always face camera
    }
}