using UnityEngine;
using UnityEngine.Tilemaps;

public class HandleDangerTiles : MonoBehaviour
{
    Tilemap thisT;
    void Start()
    {
        thisT = this.GetComponent<Tilemap>();
        //thisT.SetTileAnimationFlags();
    }

    // Update is called once per frame
    void Update()
    {
        float f = this.GetComponent<Tilemap>().GetAnimationFrame(new Vector3Int(0, 0, 0));
        //Debug.Log(f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("player triggered with ground tilemap");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("player collided with ground tilemap");
        }
    }
}
