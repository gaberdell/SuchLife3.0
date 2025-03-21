using UnityEngine;

public class HandleDungeonExit : MonoBehaviour
{
    GameObject entrance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setEntrance(GameObject entrance)
    {
        this.entrance = entrance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //move player back to entrance
        Debug.Log("interact triggered");
        //newDungeon.StartRender();
        //gameObject.SetActive(false);
        //move player to starting room in dungeon (0,0 point)
        GameObject.Find("Player").transform.position = entrance.transform.position; 
    }
}
