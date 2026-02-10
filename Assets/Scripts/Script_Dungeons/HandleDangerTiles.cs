using UnityEngine;
using UnityEngine.Tilemaps;

public class HandleDangerTiles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float f = this.GetComponent<Tilemap>().animationFrameRate;
        Debug.Log(f);   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("player collided with groudn tilemap");
        }
    }
}
