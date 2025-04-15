using UnityEngine;

public class DropLeanTween : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeanTween.moveY(gameObject, 900f, .7f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
