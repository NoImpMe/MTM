using Core;
using System;

namespace Battle
{
    public abstract class Unit : DummyMonoBehaviour
    {
        public float maxHp = 100f;
        public float attackDamage = 10f;
        public float attackRange = 3f;
        public float attackDelay = 1.5f;

        protected float currentHp;

        // HP 변화만 알림. 누가 듣는지는 Unit이 모름 (HPBar 등과의 디커플링용)
        public event Action<float> OnHpChanged;

        // 죽음 알림. 부활/소멸/파괴 등 실제 처리는 구독자(외부)가 결정
        public event Action OnDeath;

        public override void DummyAwake()
        {
            currentHp = maxHp;
        }

        public override void DummyUpdate()
        {
            //타겟을 찾아 공격 실제로는 코루틴 사용
            Unit target = FindTarget();
            if (target != null)
            {
                Attack(target);
            }
        }

        //유닛마다 누구를 우선 공격할지 규칙이 다르므로 서브클래스에서 결정
        protected abstract Unit FindTarget();

        protected virtual void Attack(Unit target)
        {
            target.TakeDamage(attackDamage);
        }

        public virtual void TakeDamage(float damage)
        {
            currentHp -= damage;
            OnHpChanged?.Invoke(currentHp);

            if (currentHp <= 0)
            {
                Die();
            }
        }

        // 공통 동작: 죽음을 알림. 부활/소멸 등 구체적 사후 처리는 서브클래스가 override
        protected virtual void Die()
        {
            OnDeath?.Invoke();
        }
        public override void DummyDestroy()
        {
            //StopAllCoroutine() 으로 타겟 추적 중지 후 이벤트 삭제
            OnDeath = null;
            OnHpChanged = null;
            //Destroy(gameObject)로 오브젝트 파괴
        }
    }
}