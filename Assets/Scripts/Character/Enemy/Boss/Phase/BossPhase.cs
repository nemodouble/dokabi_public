using System.Collections;

namespace Boss.Phase
{
    public abstract class BossPhase
    {
        public readonly string PhaseName;
        public readonly int Rarity;

        protected BossPhase(string phaseName, int rarity = 1)
        {
            PhaseName = phaseName;
            Rarity = rarity;
        }

        protected internal abstract IEnumerator DoPhase(BossController bossController);
    }
}