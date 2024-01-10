using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ForwardTask
{
    [BurstCompile(CompileSynchronously = true, OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
    public struct ThomasImplictJob : IJob
    {
        [ReadOnly] public double DeltaSpace;
        [ReadOnly] public NativeArray<double> Conductivities;
        [ReadOnly] public NativeArray<double> Times;
        [ReadOnly] public NativeArray<double> BoundaryConditions;
        public NativeArray<double> ResultTemperatures;
        public NativeArray<double> P;
        public NativeArray<double> Q;
        public NativeArray<double> Error;

        private int _spaceSegments;
        private int _timeSegments;

        public void Execute()
        {
            _spaceSegments = Conductivities.Length;
            _timeSegments = Times.Length;

            CopyFirstLayer();
            CalculateOnLayer(1);

            for (int i = 2; i < _timeSegments; i++)
            {
                CalculateOnLayer(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculateOnLayer(in int layer)
        {
            var sigma = CalculateSigma(layer);
            CalculatePQOfFirstPoint(layer);
            CalculatePQOfOtherPoints(sigma);
            BackPropagation(layer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double CalculateSigma(in int layer)
        {
            var deltaTime = Times[layer] - Times[layer - 1];

            return deltaTime / (2 * math.pow(DeltaSpace, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculatePQOfFirstPoint(in int layer)
        {
            P[0] = 0;
            Q[0] = BoundaryConditions[GetTemperatureIndex(layer - 1, 0)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculatePQOfOtherPoints(in double sigma)
        {
            for (int i = 1; i < _spaceSegments - 1; i++)
            {
                var condLeft = Conductivities[i - 1];
                var condCenter = Conductivities[i];
                var condRight = Conductivities[i + 1];

                var a = -sigma * (condCenter + condLeft);
                var b = 1 + sigma * (condLeft + 2 * condCenter + condRight);
                var c = -sigma * (condCenter + condRight);
                var d = ResultTemperatures[i];

                P[i] = -c / (a * P[i - 1] + b);
                Q[i] = (d - a * Q[i - 1]) / (a * P[i - 1] + b);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BackPropagation(in int layer)
        {
            for (int i = _spaceSegments - 2; i > 0; i--)
            {
                ResultTemperatures[i] = P[i] * ResultTemperatures[i + 1] + Q[i];

                EvaluateError(layer, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly int GetTemperatureIndex(in int layer, in int spaceNumber)
        {
            return layer * _spaceSegments + spaceNumber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EvaluateError(in int layer, in int index)
        {
            var temperatureIndex = GetTemperatureIndex(layer, index);

            if (double.IsNegative(BoundaryConditions[temperatureIndex]))
                return;

            Error[0] += math.pow(ResultTemperatures[index] - BoundaryConditions[temperatureIndex], 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CopyFirstLayer()
        {
            for (int i = 0; i < _spaceSegments; i++)
            {
                ResultTemperatures[i] = BoundaryConditions[i];
            }
        }
    }
}
