using System.Collections;
using UI;
using UnityEngine;
public class ChampionSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject champPrefab;
    private float respawnDelay = 5f;
    private GameObject currentChamp;
    private Champion championScript;
    private HpBarView hpBarView;
    private void Start()
    {
        currentChamp = Instantiate(champPrefab, transform.position, Quaternion.identity);

        championScript = currentChamp.GetComponent<Champion>();
        championScript.OnDeath += HandleChampDeath;

        hpBarView = HpBarManager.Instance.CreateHpBar(currentChamp.transform);
        hpBarView.Bind(championScript);
    }

    private void HandleChampDeath()
    {
        StartCoroutine(RespawnChamp());
    }
    

    private IEnumerator RespawnChamp()
    {
        currentChamp.SetActive(false);
        hpBarView.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        hpBarView.gameObject.SetActive(true);
        currentChamp.transform.position = transform.position; // 스폰 위치로 복귀
        championScript.ResetChampion();
    }
}
