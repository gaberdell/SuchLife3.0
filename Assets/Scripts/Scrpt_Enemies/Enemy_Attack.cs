using UnityEngine;
using System.Collections;
using Pathfinding;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackCooldown = 1.0f;
    public float dashDistance = 0.8f;
    public float dashDuration = 0.12f;
    public float returnDuration = 0.2f;
    public float hitTime = 0.08f;

    private bool playerInRange = false;
    private bool canAttack = true;

    private Health targetHealth;
    private bool isAttacking = false;
    private bool hasDealtDamage = false;
    private Vector3 attackStartPos;
    private Transform targetTransform;
    private AIPath aiPath;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
    }

    void Update()
{
    if (playerInRange && canAttack && !isAttacking)
    {
        StartCoroutine(AttackRoutine());
    }
}

    public void DealDamage()
    {
        if (playerInRange && targetHealth != null)
        {
            targetHealth.TakeDamage(damage, true, gameObject);
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        isAttacking = true;
        hasDealtDamage = false;
        if (aiPath != null)
        {
            aiPath.enabled = false;
        }

        attackStartPos = transform.position;

        Vector3 dashDir = Vector3.right;
        if (targetTransform != null)
        {
            dashDir = (targetTransform.position - transform.position).normalized;
        }

        Vector3 attackTargetPos = attackStartPos + dashDir * dashDistance;

        float timer = 0f;

        // attack dash
        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            float t = timer / dashDuration;
            transform.position = Vector3.Lerp(attackStartPos, attackTargetPos, t);

            if (!hasDealtDamage && timer >= hitTime)
            {
                DealDamage();
                hasDealtDamage = true;
            }

            yield return null;
        }

        transform.position = attackTargetPos;
        if (!hasDealtDamage)
        {
            DealDamage();
            hasDealtDamage = true;
        }

        // return to start
        timer = 0f;
        Vector3 currentPos = transform.position;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float t = timer / returnDuration;
            transform.position = Vector3.Lerp(currentPos, attackStartPos, t);
            yield return null;
        }

        transform.position = attackStartPos;
         if (aiPath != null)
        {
            aiPath.enabled = true;
        }
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            targetHealth = other.GetComponent<Health>();
            targetTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            targetHealth = null;
            targetTransform = null;
        }
    }
}