using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ModEditor
{
    public partial class NormalEditorTab
    {
        void write()
        {
            switch (window.Manager.BrushType)
            {
                case BrushType.ScreenScope:
                    writeVertexColor_ScreenScope();
                    break;
            }
        }

        void writeVertexColor_ScreenScope()
        {
            if (objInOperation == null)
                return;
            foreach (var item in objInOperation)
            {
                if (item.Key == null || item.Value == null)
                    continue;
                CalcShaderData.CalcVertexsData data = ModEditorTool.CalcShaderDatas.FirstOrDefault(x => x.trans == item.Key);
                if (data != null && data.IsAvailable)
                    item.Value.colors = CalcUtil.WriteVertexColor_UseSelectData(window.Manager.BrushColor, data.RW_Selects, item.Value.colors);
            }
        }

        void writeAcgNormalToTangent()
        {
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    window.SetEditingMesh(target, meshFilter);
                    writeAcgNormalToTangent(meshFilter.sharedMesh);
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    writeAcgNormalToTangent(skinnedMeshRenderer.sharedMesh);
                }
            }
        }

        void writeAcgNormalToTangent(Mesh mesh)
        {
            if (mesh == null)
                return;
            AvgNormalJob job = new AvgNormalJob()
            {
                vertexs = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob),
                normals = new NativeArray<Vector3>(mesh.normals, Allocator.TempJob),
                output = new NativeArray<Vector4>(mesh.vertexCount, Allocator.TempJob)
            };
            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();
            mesh.tangents = job.output.ToArray();
            job.output.Dispose();
        }
    }
}