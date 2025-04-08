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
        //EventManager.PlayerHandUpdate += playerHandUpdate;
    }

    private void OnDisable()
    {
        //EventManager.PlayerHandUpdate -= playerHandUpdate;
    }

    private void playerHandUpdate(Item newItem)
    {
        CurrentlyHeldItem = newItem;

        //switch rotation when that gets added

        currentlyHeld.sprite = CurrentlyHeldItem.icon;
    }
}
