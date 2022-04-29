using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEditor;
using System;

namespace ModEditor
{
    public class AvgNormal : VertexCalcUtilBase
    {
        public override string Name => "Avg normals";

        public override string Tip => "Calc the avg normal direction.";

        public override PassCount PassCount => PassCount.Three;

        static float ApproximateRefer;
        float approximateRefer { get => window.ApproximateRefer; set => window.ApproximateRefer = value; }

        bool useUnityNormal { get => window.UseUnityNormal; set => window.UseUnityNormal = value; }

        bool mappedTo01 { get => window.MappedTo01; set => window.MappedTo01 = value; }

        public override Vector3[] ExecuteThree(Mesh mesh)
        {
            ApproximateRefer = approximateRefer;
            Vector3[] result;
            Mesh _mesh = mesh;
            if (useUnityNormal)
            {
                _mesh = UnityEngine.Object.Instantiate(mesh);
                _mesh.RecalculateNormals();
            }
            AvgNormalJob job = new AvgNormalJob()
            {
                vertexs = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob),
                normals = new NativeArray<Vector3>(_mesh.normals, Allocator.TempJob),
                output = new NativeArray<Vector3>(mesh.vertexCount, Allocator.TempJob)
            };
            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();
            result = job.output.ToArray();
            job.output.Dispose();
            if (mappedTo01)
                result = CalcUtil.Self.MappedTo01(result);
            return result;
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use Unity Normal:", labelStyle, GUILayout.Width(130));
            if (GUILayout.Button(useUnityNormal ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
                useUnityNormal = !useUnityNormal;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mapped To 01:", labelStyle, GUILayout.Width(130));
            if (GUILayout.Button(mappedTo01 ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
                mappedTo01 = !mappedTo01;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Approximate Refer:", labelStyle, GUILayout.Width(120));
            approximateRefer = EditorGUILayout.Slider(approximateRefer, 0, .1f, GUILayout.Width(maxWidth - 160));
            EditorGUILayout.EndHorizontal();
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
                Dictionary<ApproximateVector3, Vector3> avgNormalDic = new Dictionary<ApproximateVector3, Vector3>();
                for (var i = 0; i < vertexs.Length; i++)
                {
                    ApproximateVector3 vertex = new ApproximateVector3(vertexs[i]);
                    if (avgNormalDic.TryGetValue(vertex, out Vector3 avgNormal))
                        avgNormalDic[vertex] = (avgNormal + normals[i]).normalized;
                    else
                        avgNormalDic.Add(vertex, normals[i]);
                }
                for (int i = 0; i < vertexs.Length; i++)
                    output[i] = avgNormalDic[new ApproximateVector3(vertexs[i])];
            }
        }

        struct ApproximateVector3 : IEquatable<ApproximateVector3>
        {
            public Vector3 vec;

            public ApproximateVector3(Vector3 vec)
            {
                this.vec = vec;
            }

            public static bool operator ==(ApproximateVector3 vec0, ApproximateVector3 vec1)
            {
                return Vector3.Distance(vec0.vec, vec1.vec) <= ApproximateRefer;
            }

            public static bool operator !=(ApproximateVector3 vec0, ApproximateVector3 vec1)
            {
                return !(vec0 == vec1);
            }

            public override bool Equals(object obj)
            {
                return this == (ApproximateVector3)obj;
            }

            public override int GetHashCode()
            {
                return ApproximateRefer <= 0 ? vec.GetHashCode() : ((int)(Vector3.Distance(Vector3.zero, vec) / ApproximateRefer)).GetHashCode();
            }

            public bool Equals(ApproximateVector3 other)
            {
                return this == other;
            }
        }
    }
}