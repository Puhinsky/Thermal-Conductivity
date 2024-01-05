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
            var deltaTime = Times[layer] - Times[layer - 1];

            return deltaTime / (2 * math.pow(DeltaSpace, 2));
        }

        private void CalculateFirstPoint(int layer, double sigma)
        {
            var condCenter = Conductivities[0];
            var condRight = Conductivities[1];
            var b = 1 + sigma * (2 * condCenter + condRight);
            var c = -sigma * (condCenter + condRight);
            var d = BoundaryConditions[GetTemperatureIndex(layer, 0)];
            _p[0] = -c / b;
            _q[0] = d / b;
        }

        private void CalculateMiddlePoints(int layer, double sigma)
        {
            for (int i = 1; i < _spaceSegments - 1; i++)
            {
                var condLeft = Conductivities[i-1];
                var condCenter = Conductivities[i];
                var condRight = Conductivities[i+1];
                var a = sigma * ()
                var b = 1 + sigma * (2 * condCenter + condRight);
                var c = -sigma * (condCenter + condRight);
                var d = BoundaryConditions[GetTemperatureIndex(layer, 0)];
            }
        }

        private int GetTemperatureIndex(int layer, int spaceNumber)
        {
            return layer * _spaceSegments + spaceNumber;
        }
    }
}
