using UnityEngine;

public class PlayerItemHoldView : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    SpriteRenderer currentlyHeld; 

    [SerializeField]
    Quaternion normalRotation;

    [SerializeField]
    Quaternion toolRotation;

    public Item CurrentlyHeldItem { get; private set; }


    private void OnEnable()
    {
        currentlyHeld = gameObject.GetComponent<SpriteRenderer>();
        //EventManager.PlayerHandUpdate += playerHandUpdate;
    }

    private void OnDisable()
    {
        //EventManager.PlayerHandUpdate -= playerHandUpdate;
    }

    public void playerHandUpdate(Item newItem)
    {
        CurrentlyHeldItem = newItem;

        //switch rotation when that gets added
        if (newItem != null)
        {
            currentlyHeld.sprite = CurrentlyHeldItem.icon;
        } else
        {
            currentlyHeld.sprite = null;
        }
    }
}
