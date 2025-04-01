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

    private void Update()
    {
        if (InputHandler.Instance.IsAttacking && Time.time >= _lastAttackTime + attackCooldown)
        {
            StartAttack();
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

    private void DetectHits()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(attackDamage);
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