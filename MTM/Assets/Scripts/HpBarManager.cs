using UnityEngine;

namespace UI
{
    public class HpBarManager : MonoBehaviour
    {
        public static HpBarManager Instance { get; private set; }

        [SerializeField] private Transform hudCanvasRoot; // HUD_Canvas¿« Transform
        [SerializeField] private GameObject hpBarPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public HpBarView CreateHpBar(Transform target)
        {
            GameObject go = Instantiate(hpBarPrefab, hudCanvasRoot);
            HpBarView view = go.GetComponent<HpBarView>();
            view.SetFollowTarget(target);
            return view;
        }
    }
}