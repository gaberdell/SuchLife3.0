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
            //only attack living entities
            if (enemy.TryGetComponent<Mob>(out var mob))
            {
                if (enemy.TryGetComponent<Health>(out var health))
                {
                    health.TakeDamage(attackDamage);
                }
                //apply a knockback force to the mob
                //read force from some value associated with the attack or weapon
                float knockbackForce = 5f;
                //calculate direction to knockback based on player and enemy's position
                Vector3 enemyPos = enemy.transform.position;
                Vector3 playerPos = gameObject.transform.position;
                Vector3 diff = enemyPos - playerPos;
                Vector3 knockbackVector = new Vector3(diff.x * knockbackForce, diff.y * knockbackForce, 0);
                //calculate direction to knockback based on player's facing direction
                float playerFacing = gameObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                Vector3 playerFacingVector = new Vector3(Mathf.Sin(playerFacing), Mathf.Cos(playerFacing) * -1, 0);
                Vector3 newKnockbackVector = playerFacingVector * knockbackForce;
                //Debug.Log(playerFacingVector);

                
                mob.applyKnockback(newKnockbackVector, knockbackForce);
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