namespace Battle
{
    public class Tower : Unit
    {

        protected override Unit FindTarget()
        {
            // 미니언과 챔피언 중 가까운 유닛 판별 로직 (구체 구현 생략)
            return null;
        }

        protected override void Die()
        {
            base.Die();
            DummyDestroy();
        }
        public override void DummyDestroy()
        {
            base.DummyDestroy();
            //Tower가 소멸될 때 골드 지급, 경험치 지급 등 로직
        }
    }
}