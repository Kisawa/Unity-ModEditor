using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ModEditor
{
    public struct AvgNormalJob : IJob
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<Vector3> vertexs;

        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<Vector3> normals;

        public NativeArray<Vector4> output;

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