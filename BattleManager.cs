namespace Battle
{
    public static class BattleManager : DummyMonobehaviour
    {
        public static int CurrentGameSeconds => 0;

        public override void DummyAwake() 
        {
            Timer();
        }

        private void Timer() //실제로는 코루틴 사용
        {
            while (true)
            {
                //yield return new WaitforSeconds(1f)
                CurrentGameSeconds += 1;
            }
        }
    }
}