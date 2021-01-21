using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    public readonly struct EnsembleShiftCorrelationData
    {
        public IParticle Particle1 { get; }

        public IParticle Particle2 { get; }

        public int EnsembleSize1 { get; }

        public int EnsembleSize2 { get; }

        public double RiRj { get; }

        public double DeltaRiDeltaRj { get; }

        public EnsembleShiftCorrelationData(double riRj, double deltaRiDeltaRj, int ensembleSize1, int ensembleSize2, IParticle particle1, IParticle particle2)
        {
            RiRj = riRj;
            EnsembleSize1 = ensembleSize1;
            Particle1 = particle1;
            Particle2 = particle2;
            DeltaRiDeltaRj = deltaRiDeltaRj;
            EnsembleSize2 = ensembleSize2;
        }
    }
}