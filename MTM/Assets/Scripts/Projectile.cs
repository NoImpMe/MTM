using UnityEngine;
using System;
using System.Collections;

namespace Battle
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;

        private Transform target;
        private float damage;
        private Action onHit; // 명중 시 실행할 콜백 (데미지 적용은 호출부가 결정)

        public void Launch(Transform target, float damage, Action onHit)
        {
            this.target = target;
            this.damage = damage;
            this.onHit = onHit;
            StartCoroutine(MoveToTarget());
        }

        private IEnumerator MoveToTarget()
        {
            while (target != null && Vector3.Distance(transform.position, target.position) > 0.1f)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                // 화살 머리가 진행 방향을 보게 회전 (선택 사항, 화살 스프라이트가 오른쪽을 향해 그려져 있다고 가정)
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            if (target != null)
            {
                onHit?.Invoke();
            }

            Destroy(gameObject);
        }
    }
}