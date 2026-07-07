using Core;

namespace UI
{
    public class HPBar : DummyMonoBehaviour
    {
        private float _currentHP;

        public void RefreshHP(float newHP)
        {
            _currentHP = newHP;
        }
    }
}
