using UnityEngine;

public class HandleDungeonEntrance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputHandler inputHandler;
    bool interact;
    private void Start()
    {
        inputHandler = InputHandler.Instance;
        interact = true;
    }
    private void Update()
    {
        //bool interact = inputHandler.InteractTriggered;
        if(interact){
            //load dungeon and disable this object
            print("interact triggered");
            //newDungeon.StartRender();
            gameObject.SetActive(false);
            interact = false;
        }
    }
    
        
    
}
