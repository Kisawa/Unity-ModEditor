using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEditor;

namespace ModEditor
{
    public class AvgNormal : VertexCalcUtilBase
    {
        public override string Name => "Avg normals";

        public override string Tip => "Calc the avg normal direction.";

        public override PassCount PassCount => PassCount.Three;

        public override Vector3[] ExecuteThree(Mesh mesh)
        {
            Vector3[] result;
            AvgNormalJob job = new AvgNormalJob()
            {
                vertexs = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob),
                normals = new NativeArray<Vector3>(mesh.normals, Allocator.TempJob),
                output = new NativeArray<Vector3>(mesh.vertexCount, Allocator.TempJob)
            };
            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();
            result = job.output.ToArray();
            job.output.Dispose();
            return result;
        }

        struct AvgNormalJob : IJob
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Vector3> vertexs;

            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Vector3> normals;

            public NativeArray<Vector3> output;

            public void Execute()
            {
                Dictionary<Vector3, Vector3> avgNormalDic = new Dictionary<Vector3, Vector3>();
                for (var i = 0; i < vertexs.Length; i++)
                {
                    Vector3 vertex = vertexs[i];
                    if (avgNormalDic.TryGetValue(vertex, out Vector3 avgNormal))
                        avgNormalDic[vertex] = (avgNormal + normals[i]).normalized;
                    else
                        avgNormalDic.Add(vertex, normals[i]);
                }
                for (int i = 0; i < vertexs.Length; i++)
                    output[i] = avgNormalDic[vertexs[i]];
            }
        }
    }
}