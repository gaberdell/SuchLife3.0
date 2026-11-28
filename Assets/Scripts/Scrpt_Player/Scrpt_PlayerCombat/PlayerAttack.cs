using UnityEngine;
using System.Collections.Generic;
using System.Collections;
 
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask enemyLayer;
 
    [Header("References")]
    [SerializeField] private Transform attackPointLeft;
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInfo SelfInfo;
 
    [Header("Crossbow Settings")]
    [SerializeField] private float shotSpawnDistance = 0.6f;
    [SerializeField] private GameObject crossbowProjectile;
    [SerializeField] private Transform crossbowShotPointLeft;
    [SerializeField] private Transform crossbowShotPointRight;
    [SerializeField] private float timeToFullCharge = 1f;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite chargingSprite;
    [SerializeField] private Sprite chargedSprite;
 
    //Crossbow state, one entry per hand (index with (int)Hand)
    private readonly float[] holdStartTime = new float[2];
    private readonly bool[] isHolding = new bool[2];
    private readonly bool[] isCharged = new bool[2];
    private readonly Item[] lastHeldItem = new Item[2];
 
    [Header("Aiming")]
    private Vector2 aimDirection = Vector2.right;
 
    //Animator trigger names, per hand
    private static readonly string[] punchTriggers = { "AttackLeft", "AttackRight" };
    private static readonly string[] swordTriggers = { "SwordLeft", "SwordRight" };
    private static readonly string[] spearTriggers = { "SpearLeft", "SpearRight" };
 
    [Header("Detecting hits")]
    //hit detection state is shared by both hands on purpose, so only one melee attack can be
    //in progress at a time (that is the current behavior, to be decided later)
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>(); 
    private bool DetectingHits;
    private Collider2D[] hitObjects = new Collider2D[10];
 
    private readonly float[] lastAttackTime = new float[2];
    private float additiveDamageBoost = 0f;
 
    private void Update()
    {
        if (InputHandler.Instance == null)
        {
            Debug.LogError("InputHandler.Instance is null!");
            return;
        }
 
        HandleHand(Hand.Left);
        HandleHand(Hand.Right);
    }
 
    private void HandleHand(Hand hand)
    {
        int h = (int)hand;
        InputHandler input = InputHandler.Instance;
        InputHandler.SelectedContext context = InputHandler.GetSelectedContext(hand);
        Item held = SelfInfo.GetHeldItem(hand);
 
        //swapping what is in the hand throws away any crossbow charge
        if (held != lastHeldItem[h])
        {
            lastHeldItem[h] = held;
            isHolding[h] = false;
            isCharged[h] = false;
        }
 
        Weapon weapon = context == InputHandler.SelectedContext.Weapon ? held as Weapon : null;
 
        //Crossbow: hold the hand's button to charge, press again once charged to fire
        if (weapon != null && weapon.AnimationType == 3)
        {
            HandleCrossbow(hand);
            return;
        }
 
        //placing blocks and drinking potions use the hand's button, so they take priority over attacking
        if (context == InputHandler.SelectedContext.Block || context == InputHandler.SelectedContext.Consumable)
        {
            return;
        }
 
        if (!input.IsAttacking(hand) || Time.time < lastAttackTime[h] + attackCooldown)
        {
            return;
        }
 
        if (weapon != null)
        {
            int weaponDamage = Mathf.RoundToInt(attackDamage * weapon.DamageMult);
 
            switch (weapon.AnimationType)
            {
                case 1: StartMelee(hand, swordTriggers[h], weaponDamage); break; // swing
                case 2: StartMelee(hand, spearTriggers[h], weaponDamage); break; // stab
                default: StartMelee(hand, punchTriggers[h], attackDamage); break;
            }
        }
        else
        {
            //empty hand (or tool) punches
            StartMelee(hand, punchTriggers[h], attackDamage);
        }
    }
 
    private void HandleCrossbow(Hand hand)
    {
        int h = (int)hand;
        bool held = InputHandler.Instance.IsAttacking(hand);
        bool pressed = InputHandler.Instance.WasAttackPressed(hand);
        SpriteRenderer sr = SelfInfo.GetHandRenderer(hand);
 
        if (pressed)
        {
            if (isCharged[h])
            {
                if (Time.time >= lastAttackTime[h] + attackCooldown)
                {
                    FireCrossbow(hand);
                }
            }
            else
            {
                isHolding[h] = true;
                holdStartTime[h] = Time.time;
                SetSprite(sr, idleSprite);
            }
        }
 
        if (isHolding[h] && !isCharged[h])
        {
            if (!held)
            {
                //let go before fully charged
                isHolding[h] = false;
                SetSprite(sr, idleSprite);
            }
            else
            {
                float heldTime = Time.time - holdStartTime[h];
 
                if (heldTime >= timeToFullCharge)
                {
                    isCharged[h] = true;
                    SetSprite(sr, chargedSprite);
                }
                else if (heldTime >= timeToFullCharge * 0.5f)
                {
                    SetSprite(sr, chargingSprite);
                }
                else
                {
                    SetSprite(sr, idleSprite);
                }
            }
        }
    }
 
    private void FireCrossbow(Hand hand)
    {
        int h = (int)hand;
        lastAttackTime[h] = Time.time;
 
        Vector2 dir = GetDirectionToMouse(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
 
        Vector2 spawnPosLeft = (Vector2)crossbowShotPointLeft.position + dir * shotSpawnDistance;
        Vector2 spawnPosRight = (Vector2)crossbowShotPointRight.position + dir * shotSpawnDistance;
 
 
        if (h == 0)
        {
            Instantiate(crossbowProjectile, spawnPosLeft, Quaternion.Euler(0f, 0f, angle - 90f));
        }
        else 
        {
            Instantiate(crossbowProjectile, spawnPosRight, Quaternion.Euler(0f, 0f, angle - 90f));
        }
 
        isCharged[h] = false;
        isHolding[h] = false;
        SetSprite(SelfInfo.GetHandRenderer(hand), idleSprite);
    }
 
    private void SetSprite(SpriteRenderer sr, Sprite sprite)
    {
        if (sr != null && sprite != null)
        {
            sr.sprite = sprite;
        }
    }
 
    private void StartMelee(Hand hand, string trigger, int damage)
    {
        if (DetectingHits) 
        {
            return;
        }
        aimDirection = GetDirectionToMouse(transform.position);
        SelfInfo.SetAnimated(hand, true);
        lastAttackTime[(int)hand] = Time.time;
        animator.SetTrigger(trigger);
        StartCoroutine(DetectHits(hand, damage));
    }
 
    public void EndAttack() 
    {
    }
 
    public void ApplyDamageBoost(float amount, float duration)
    {
        additiveDamageBoost = amount;
        Debug.Log($"Applied +{amount} damage boost for {duration} seconds.");
        Invoke(nameof(RemoveDamageBoost), duration);
    }
 
    private void RemoveDamageBoost()
    {
        additiveDamageBoost = 0f;
        Debug.Log("Damage boost ended.");
    }
 
    private IEnumerator DetectHits(Hand hand, int damage)
    {
        DetectingHits = true;
        alreadyHit.Clear();
        Transform attackPoint = hand == Hand.Left ? attackPointLeft : attackPointRight;
        float elapsed = 0f;
 
        while (elapsed < 1.0)
        {
            Vector2 currentPos = attackPoint.position;
            int count = Physics2D.OverlapCircleNonAlloc(currentPos, 0.5f, hitObjects);
 
            for (int i = 0; i < count; i++)
            {
                Collider2D hitObject = hitObjects[i];
                
                if (!alreadyHit.Contains(hitObject))
                {
                    alreadyHit.Add(hitObject);
                    ApplyHits(hitObject, damage, hand);
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
 
        DetectingHits = false;
 
        //make sure the hand is never left stuck in the animated state if nothing was hit
        SelfInfo.SetAnimated(hand, false);
    }
 
    private void ApplyHits(Collider2D enemy, int Damage, Hand hand)
    {
        if (enemy.TryGetComponent<Mob>(out var mob))
        {
            if (IsBlockedByWall(enemy) == true)
            {
                return;
            }
 
            if (enemy.TryGetComponent<Health>(out var health))
            {
                int boostedDamage = Mathf.RoundToInt(Damage + additiveDamageBoost);
                health.TakeDamage(boostedDamage, true, gameObject);
            }
                
            //apply a knockback force to the mob
            //read force from some value associated with the attack or weapon
            float knockbackForce = 10f;
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            Vector2 knockbackVector = knockbackDir * knockbackForce;
            mob.applyKnockback(knockbackVector, knockbackForce);
            //Debug.Log(knockbackVector);
        }
        SelfInfo.SetAnimated(hand, false);   
    }
 
    private bool IsBlockedByWall(Collider2D target)
    {
        Vector2 origin = transform.position;
        Vector2 targetPoint = target.ClosestPoint(origin);
 
        RaycastHit2D[] hits = Physics2D.LinecastAll(origin, targetPoint);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger) continue;
            if (hit.collider.CompareTag("PlaceTileMap")) return true;
        }
        return false;
    }
 
    private void OnDrawGizmosSelected()
    {
        if (attackPointLeft == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
 
    private Vector2 GetDirectionToMouse(Vector2 from)
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - from;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : aimDirection;
    }
}