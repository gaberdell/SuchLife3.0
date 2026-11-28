using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInfo : MonoBehaviour
{
    //data class to store attributes concerning the player so multiple scripts can access
    //put attributes like health in here instead of the health class

    public GameObject Player { get; private set; }
    private string username;
 
    [Header("Held item objects (the sprite shown in each hand)")]
    //the old single "currentlyHeld" reference becomes the left hand; assign the right hand object in the inspector
    [FormerlySerializedAs("currentlyHeld")]
    [SerializeField] private GameObject currentlyHeldLeft;
    [SerializeField] private GameObject currentlyHeldRight;
 
    //used to make weapons rotate towards the mouse slightly
    [Header("Held item tilt")]
    [SerializeField] private float maxTiltAngle = 10f;
    [FormerlySerializedAs("restLocalAngle")]
    [SerializeField] private float restLocalAngleLeft = 180f;
    [SerializeField] private float restLocalAngleRight = 180f;
 
    //Everything below is per hand, indexed with (int)Hand
    private readonly GameObject[] heldObjects = new GameObject[2];
    private readonly SpriteRenderer[] heldRenderers = new SpriteRenderer[2];
    private readonly Item[] heldItems = new Item[2];
    private readonly int[] heldItemIndices = { -1, -1 };
    private readonly bool[] isAnimated = new bool[2];
    private readonly float[] currentAngle = new float[2];
    private readonly float[] angleVelocity = new float[2];
 
    //Unique behavior of NOT being replicated between clients
    //Server grabs GUID from a tcp request but never shares it to other clients
    public Guid playerGuid;

    private void Awake()
    {
        Player = gameObject;
 
        heldObjects[(int)Hand.Left] = currentlyHeldLeft;
        heldObjects[(int)Hand.Right] = currentlyHeldRight;
 
        for (int h = 0; h < 2; h++)
        {
            if (heldObjects[h] != null)
            {
                heldRenderers[h] = heldObjects[h].GetComponent<SpriteRenderer>();
            }
        }
    }

    private void Start()
    {
        for (int h = 0; h < 2; h++)
        {
            if (heldObjects[h] != null)
            {
                currentAngle[h] = heldObjects[h].transform.eulerAngles.z;
            }
        }
    }

    public Item GetHeldItem(Hand hand)
    {
        return heldItems[(int)hand];
    }
 
    //inventory slot the hand's item came from, -1 if the hand is empty
    public int GetHeldItemIndex(Hand hand)
    {
        return heldItemIndices[(int)hand];
    }
 
    public SpriteRenderer GetHandRenderer(Hand hand)
    {
        return heldRenderers[(int)hand];
    }
 
    //true while an attack animation owns the hand's rotation
    public void SetAnimated(Hand hand, bool value)
    {
        isAnimated[(int)hand] = value;
    }
 
    //called by PlayerInventory whenever what a hand is holding changes
    public void SetHeldItem(Hand hand, Item newItem, int slotIndex)
    {
        int h = (int)hand;
        heldItems[h] = newItem;
        heldItemIndices[h] = newItem != null ? slotIndex : -1;
 
        //when held item is changed, change the renderer accordingly
        if (heldRenderers[h] != null)
        {
            heldRenderers[h].sprite = newItem != null ? newItem.icon : null;
        }
    }
 
    public void Update()
    {
        UpdateHandRotation(Hand.Left);
        UpdateHandRotation(Hand.Right);
    }
 
    private void UpdateHandRotation(Hand hand)
    {
        int h = (int)hand;
        Item item = heldItems[h];
 
        if (item == null || heldObjects[h] == null)
        {
            return;
        }
 
        Transform held = heldObjects[h].transform;
        float restLocalAngle = hand == Hand.Left ? restLocalAngleLeft : restLocalAngleRight;
        float parentAngle = held.parent != null ? held.parent.eulerAngles.z : 0f;
        float restAngle = parentAngle + restLocalAngle;
 
        if (item.useFlag == 4 && !isAnimated[h])
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = mouseWorld - held.position;
            float mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
 
            float offsetFromRest = Mathf.DeltaAngle(restAngle, mouseAngle);
            float targetAngle = restAngle + Mathf.Clamp(offsetFromRest, -maxTiltAngle, maxTiltAngle);
 
            currentAngle[h] = Mathf.SmoothDampAngle(currentAngle[h], targetAngle, ref angleVelocity[h], 0.05f);
            held.rotation = Quaternion.Euler(0f, 0f, currentAngle[h]);
        }
        else if (item.useFlag == 4)
        {
            held.rotation = Quaternion.Euler(0f, 0f, currentAngle[h]);
        }
        else
        {
            // Resting angle, relative to the player's orientation
            held.localRotation = Quaternion.Euler(0, 0, restLocalAngle);
            currentAngle[h] = restAngle;
            angleVelocity[h] = 0f;
        }
    }
}