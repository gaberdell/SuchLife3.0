using UnityEngine;

public class RotateLeanTween : MonoBehaviour
{
    Vector3 TitleRotation = new Vector3(30f, 30f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeanTween.rotateAroundLocal(gameObject, Vector3.forward, 360f, .5f);

      // LeanTween.rotateY(gameObject, 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
