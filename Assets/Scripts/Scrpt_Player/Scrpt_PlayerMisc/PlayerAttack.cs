using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;

    private float _lastAttackTime;
    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;
    private float additiveDamageBoost = 0f;


    private void Update()
    {
        if (InputHandler.Instance == null)
        {
            Debug.LogError("InputHandler.Instance is null!");
            return;
        }

        if (InputHandler.Instance.IsAttacking && Time.time >= _lastAttackTime + attackCooldown)
        {
            //if holding a weapon, use that instead
            if (InputHandler.currSelectedContext == InputHandler.SelectedContext.Weapon) 
            {
                //look at item name
            } else
            {
                StartAttack();

            }
        }
    }

    private void StartAttack()
    {
        _lastAttackTime = Time.time;
        _isAttacking = true;
        
        animator.SetTrigger("Attack");

       Invoke("DetectHits", 0.5f);
    }
    public void EndAttack()
    {
        _isAttacking = false;
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

    private void DetectHits()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            //only attack living entities
            if (enemy.TryGetComponent<Mob>(out var mob))
            {
                if (enemy.TryGetComponent<Health>(out var health))
                {
                    int boostedDamage = Mathf.RoundToInt(attackDamage + additiveDamageBoost);
                    health.TakeDamage(boostedDamage);
                }
                //apply a knockback force to the mob
                //read force from some value associated with the attack or weapon
                float knockbackForce = 10f;
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                Vector2 knockbackVector = knockbackDir * knockbackForce;

                mob.applyKnockback(knockbackVector, knockbackForce);
                //Debug.Log("push");
                //Debug.Log(knockbackVector);
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}