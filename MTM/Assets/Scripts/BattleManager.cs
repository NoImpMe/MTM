using System.Collections;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }
        [SerializeField]
        private int CurrentGameSeconds = 0;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            StartCoroutine(tickGameSeconds());
        }
        IEnumerator tickGameSeconds()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                CurrentGameSeconds += 1;
            }
        }
        public int GetCurrentTime()
        {
            return CurrentGameSeconds;
        }
        
    }
}