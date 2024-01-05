using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ForwardTask
{
    public struct ThomasImplictJob : IJob
    {
        public double DeltaSpace;
        public NativeArray<double> Conductivities;
        public NativeArray<double> Times;
        public NativeArray<double> BoundaryConditions;
        public NativeArray<double> ResultTemperatures;

        private int _spaceSegments;
        private int _timeSegments;
        private NativeArray<double> _p;
        private NativeArray<double> _q;

        public void Execute()
        {
            _spaceSegments = Conductivities.Length;
            _timeSegments = Times.Length;
            _p = new NativeArray<double>(_spaceSegments, Allocator.Temp);
            _q = new NativeArray<double>(_spaceSegments, Allocator.Temp);
        }

        private void CalculateOnZeroTime()
        {
            var sigma = CalculateSigma(1);

        }

        private double CalculateSigma(int layer)
        {
            var deltaTime = Times[layer] - Times[layer-1];

            return -deltaTime / (2 * math.pow(DeltaSpace, 2));
        }

        private void CalculateFirstPoint(int layer, double sigma)
        {
            var condCenter = Conductivities[0];
            var condRight = Conductivities[1];
            var b = -sigma * (2 * condCenter + condRight) + 1;
            var c = sigma * (condCenter + condRight);

        }
    }
}
