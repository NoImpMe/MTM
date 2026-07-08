using Battle;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Minion : Unit
{
   
    int currentTime = 0;
    public LayerMask towerLayer;

    protected override void Awake()
    {
        currentTime = BattleManager.Instance.GetCurrentTime();
        attackDamage = GetProperAttackDamageMinion(currentTime);
        base.Awake();
    }

    protected override Transform FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, towerLayer);
        return hits
            .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
            .Select(h => h.transform)
            .FirstOrDefault();
    }
    private float GetProperAttackDamageMinion(int time)
    {
        return time * 0.5f;
    }
}
