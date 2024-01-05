using DatabaseLibrary;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using Debug = UnityEngine.Debug;

namespace ForwardTask
{
    public class ForwardTaskRunner
    {
        public void Execute(Database database)
        {
            var conductivities = new NativeArray<double>(database.Conductivities, Allocator.Persistent);
            var times = new NativeArray<double>(database.Times, Allocator.Persistent);
            var boundaryConditions = new NativeArray<double>(database.Temperatures, Allocator.Persistent);
            var result = new NativeArray<double>(database.SpaceSegmentsCount * database.TimeSegmentsCount, Allocator.Persistent);
            var p = new NativeArray<double>(database.SpaceSegmentsCount - 1, Allocator.Persistent);
            var q = new NativeArray<double>(database.SpaceSegmentsCount - 1, Allocator.Persistent);
            var error = new NativeArray<double>(1, Allocator.Persistent);

            var deltaSpace = database.Lenght / database.SpaceSegmentsCount;

            var thomasJob = new ThomasImplictJob()
            {
                DeltaSpace = deltaSpace,
                Conductivities = conductivities,
                Times = times,
                BoundaryConditions = boundaryConditions,
                ResultTemperatures = result,
                P = p,
                Q = q,
                Error = error
            };

            var watch = Stopwatch.StartNew();
            var handle = thomasJob.Schedule();
            handle.Complete();
            watch.Stop();
            Debug.Log($"Forward task on CPU executed in {watch.ElapsedMilliseconds} ms. Error is {error[0]}");

            conductivities.Dispose();
            times.Dispose();
            boundaryConditions.Dispose();
            result.Dispose();
            p.Dispose();
            q.Dispose();
        }
    }
}