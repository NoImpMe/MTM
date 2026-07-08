using System.Linq;
using UI;
using UnityEngine;

public class Tower : Unit
{
    public LayerMask targetLayer;
    private HpBarView hpBarView;
    [SerializeField]
    private GameObject tower;
    private Tower towerScript;
    protected override void Awake()
    {
        base.Awake();
        towerScript = tower.GetComponent<Tower>();
        towerScript.OnDeath += HandleTowerDestroyed;
        
    }
    private void Start()
    {
        hpBarView = HpBarManager.Instance.CreateHpBar(tower.transform);
        hpBarView.Bind(towerScript);
    }
    protected override Transform FindTarget()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

        if (hits.Length == 0) return null;

        Transform best = hits
            .Select(h => h.transform)
            .OrderBy(t => GetPriority(t))
            .ThenBy(t => Vector2.Distance(transform.position, t.position))
            .FirstOrDefault();

        return best;
    }

    private int GetPriority(Transform t)
    {
        if (t.GetComponent<Minion>() != null) return 0; 
        if (t.GetComponent<Champion>() != null) return 1; 
        return 99;
    }

    private void HandleTowerDestroyed()
    {
        hpBarView.Unbind(towerScript);
        Destroy(hpBarView.gameObject);
    }
}
