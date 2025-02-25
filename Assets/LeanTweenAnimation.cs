using UnityEngine;

public class LeanTweenAnimation : MonoBehaviour
{
        Vector3 TitleSize = new Vector3(15f, 15f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     //   LeanTween.moveY(gameObject, -.5f, 2f);
        LeanTween.scale(gameObject, TitleSize, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
