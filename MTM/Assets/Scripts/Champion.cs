using System.Linq;
using UnityEngine;

public class Champion : Unit
{

    public LayerMask towerLayer;
    public float respawnDelay = 7f;

    protected override Transform FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, towerLayer);
        return hits
            .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
            .Select(h => h.transform)
            .FirstOrDefault();
    }
    protected override void Die()
    {
        StopAllCoroutines();
        EventOnDeath();
    }
    public void ResetChampion()
    {
        currentHp = maxHp;
        currentTarget = null;
        EventOnHpChanged();
        gameObject.SetActive(true);
        StartCoroutine(Active()); // SetActive(false)로 멈췄던 코루틴 재시작
    }
}
