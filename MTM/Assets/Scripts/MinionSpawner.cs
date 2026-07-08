using System.Collections;
using UI;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject minionPrefab;
    private float respawnDelay = 13f;
    private GameObject currentMinion;
    private HpBarView hpBarView;
    private Minion minionScript;
    private void Start()
    {
        SpawnMinion(); 
    }

    private void SpawnMinion()
    {
        currentMinion = Instantiate(minionPrefab, transform.position, Quaternion.identity);

        minionScript = currentMinion.GetComponent<Minion>();
        minionScript.OnDeath += HandleMinionDeath;

        hpBarView = HpBarManager.Instance.CreateHpBar(currentMinion.transform);
        hpBarView.Bind(minionScript);
    }

    private void HandleMinionDeath()
    {
        hpBarView.Unbind(minionScript);
        Destroy(hpBarView.gameObject);
        StartCoroutine(RespawnMinion());
    }

    private IEnumerator RespawnMinion()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnMinion();
    }

}
