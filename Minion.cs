namespace Battle
{
    public class Minion : Unit
    {
        public override void DummyAwake()
        {
            base.DummyAwake();
            attackDamage = GetProperAttackDamageBy(BattleManager.CurrentGameSeconds);
        }

        // 게임 진행 시간에 비례한 공격력 산정 (구체 계산 로직 생략)
        protected float GetProperAttackDamageBy(int gameSeconds) => 0;

        protected override void Die()
        {
            base.Die(); 
            DummyDestroy();
        }
        public override void DummyDestroy()
        {
            base.DummyDestroy();
            //Minion이 죽을 때 골드 지급, 경험치 지급 등 로직
        }
        protected override Unit FindTarget() => null;
    }
}