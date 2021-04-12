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

    public struct MinJob : IJob
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<float> selects;

        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<float> nums;

        public NativeArray<float> min;

        public void Execute()
        {
            for (int i = 0; i < selects.Length; i++)
            {
                float num = nums[i];
                float select = selects[i];
                if (select < 0 && min[0] > num)
                    min[0] = num;
            }
        }
    }

    public struct UnifyOverlapVertexJob : IJob
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<Vector3> vertexs;

        public NativeArray<float> selects;

        public void Execute()
        {
            Dictionary<Vector3, float> vertexDic = new Dictionary<Vector3, float>();
            for (var i = 0; i < vertexs.Length; i++)
            {
                Vector3 vertex = vertexs[i];
                float select = selects[i];
                if (vertexDic.TryGetValue(vertex, out float res))
                {
                    if (res < select)
                        vertexDic[vertex] = select;
                }
                else
                    vertexDic.Add(vertex, select);
            }
            for (int i = 0; i < selects.Length; i++)
            {
                if(selects[i] == 0)
                    selects[i] = vertexDic[vertexs[i]];
            }
        }
    }
}