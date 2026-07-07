using System.Threading.Tasks;

namespace Battle
{
    public class Champion : Unit
    {
        public float respawnDelay = 10f;
        int level = 0;

        public override void DummyAwake()
        {
            base.DummyAwake();
            attackDamage = GetProperAttackDamageBy(level);
            attackDelay = GetProperAttackDelayBy(level);
            attackRange = GetProperAttackRangeBy(level);
        }


        protected override void Die()
        {
            base.Die();
            ScheduleRespawn(respawnDelay);
        }
        
        protected float GetProperAttackDamageBy(int level) => 0;
        protected float GetProperAttackDelayBy(int level) => 0;
        protected float GetProperAttackRangeBy(int level) => 0;

        //코루틴은 생략, 지정 시간 후 Respawn() 호출
        protected virtual void ScheduleRespawn(float delay) 
        {
            //yield return new WaitforSeconds(delay)
            Respawn();
        }

        protected virtual void Respawn()
        {
            currentHp = maxHp;
            //위치 초기화 등 부활 관련 처리
        }

        protected override Unit FindTarget() => null; //탐색 로직 생략
    }
}