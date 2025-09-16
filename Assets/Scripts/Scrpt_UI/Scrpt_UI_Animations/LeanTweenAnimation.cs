using UnityEngine;

public class LeanTweenAnimation : MonoBehaviour
{
    [SerializeField]
    RectTransform backGroundTransform;

    Vector3 originalPosition;

    Vector3 TitleSize = new Vector3(15f, 15f, 0f);

    float distTraveled = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = backGroundTransform.position;

     //   LeanTween.moveY(gameObject, -.5f, 2f);
        LeanTween.scale(gameObject, TitleSize, 1.5f);
    }

    private void Update()
    {
        backGroundTransform.position = originalPosition + 8f * new Vector3(Mathf.Cos(Time.time/3f), Mathf.Sin(Time.time/3f), 0);
    }
}
