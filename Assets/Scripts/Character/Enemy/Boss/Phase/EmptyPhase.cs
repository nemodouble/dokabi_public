using System.Collections;

namespace Boss.Phase
{
    public class EmptyPhase : BossPhase
    {
        public EmptyPhase(string phaseName, int rarity = 1) : base(phaseName, rarity)
        {
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            yield break;
        }
    }
}