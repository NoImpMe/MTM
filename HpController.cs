using UI;

namespace Battle
{
    
    public class HpController
    {
        private readonly Unit _unit;
        private readonly HPBar _hpBar;

        //유닛이 생성될 때 연결
        public HpController(Unit unit, HPBar hpBar) 
        {
            _unit = unit;
            _hpBar = hpBar;

            _unit.OnHpChanged += HandleHpChanged;
        }

        private void HandleHpChanged(float newHp)
        {
            _hpBar.RefreshHP(newHp);
        }

        //이 이벤트 구독만 제거하는 안전장치용 함수 현재는 사용X
        public void Unbind()
        {
            _unit.OnHpChanged -= HandleHpChanged;
        }
    }
}