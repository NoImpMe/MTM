using System;
using System.Collections;
using UnityEngine;
using Battle;

public abstract class Unit : MonoBehaviour
{
    public float attackRange = 3f;
    public float attackDelay = 1.5f; 
    public float attackDamage = 10;
    public float maxHp = 100f;
    [SerializeField]
    protected float currentHp;
    [SerializeField]
    private GameObject effectPrefab;
    [SerializeField]
    private GameObject hitEffectPrefab;
    [SerializeField]
    private float effectLifetime;
    [SerializeField]
    private Transform firePoint;
    protected Transform currentTarget;
    public event Action OnDeath;
    public event Action<float, float> HpChanged; 
    protected virtual void Awake()
    {
        
        currentHp = maxHp;
        StartCoroutine(Active());
    }

    protected virtual IEnumerator Active()
    {
        while (true)
        {
            
            if (currentTarget == null || !IsValidTarget(currentTarget))
            {
                currentTarget = FindTarget();
            }

            if (currentTarget != null)
            {
                Attack(currentTarget);
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);  
            }
            
        }
        
    }

    protected abstract Transform FindTarget();

    protected virtual bool IsValidTarget(Transform target)
    {
        return target != null && target.gameObject.activeInHierarchy;
    }

    protected virtual void Attack(Transform target)
    {
        
        if (effectPrefab != null)
        {
            FireProjectile(target);
        }
        else
        {
            target.GetComponent<Unit>().TakeDamage(attackDamage);
        }

    }
    private void FireProjectile(Transform target)
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject obj = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
        Projectile projectile = obj.GetComponent<Projectile>();

        Unit targetUnit = target.GetComponent<Unit>();
        projectile.Launch(target, attackDamage, () =>
        {
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(attackDamage);
            }
            SpawnHitEffect(target.position);
        });
    }
    private void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab == null) return;

        GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(effect, effectLifetime);
    }
    protected void EventOnDeath()
    {
        OnDeath?.Invoke();
    }
    protected void EventOnHpChanged()
    {
        HpChanged?.Invoke(currentHp, maxHp);
    }
    public virtual void TakeDamage(float dmg)
    {
        currentHp -= dmg;
        EventOnHpChanged();
        if (currentHp <= 0) Die();
    }

    protected virtual void Die()
    {
        StopAllCoroutines();
        EventOnDeath();
        Destroy(gameObject);
    }
}
