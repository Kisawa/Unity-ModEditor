using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public sealed class CalcShaderData
    {
        public abstract class CalcVertexsData
        {
            public Transform trans { get; protected set; }
            public Renderer renderer { get; protected set; }
            public Material material { get; private set; }
            public ComputeBuffer _Vertexs { get; private set; }
            public ComputeBuffer _Triangles { get; private set; }
            public ComputeBuffer RW_Selects { get; private set; }
            public virtual bool IsAvailable
            {
                get 
                {
                    if (trans == null || renderer == null || material == null || !_Vertexs.IsValid() || !_Triangles.IsValid() || !RW_Selects.IsValid())
                        return false;
                    return true;
                }
            }

            bool _clearSpread;
            protected bool clearSpread
            {
                get
                {
                    if (_clearSpread)
                    {
                        _clearSpread = false;
                        return true;
                    }
                    return false;
                }
            }

            public CalcVertexsData(Renderer renderer, Vector3[] vertexs, int[] triangles)
            {
                this.renderer = renderer;
                _Vertexs = new ComputeBuffer(vertexs.Length, sizeof(float) * 3);
                _Vertexs.SetData(vertexs);
                _Triangles = new ComputeBuffer(triangles.Length, sizeof(int));
                _Triangles.SetData(triangles);
                RW_Selects = new ComputeBuffer(vertexs.Length, sizeof(float));
            }

            public abstract void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth);

            public virtual void SpreadSelects(int spread)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelectInTirangle, "RW_Selects", RW_Selects);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelectInTirangle, "_Triangles", _Triangles);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_SpreadSelectInTirangle, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
            }

            public virtual void BindMaterial(Material material)
            {
                this.material = material;
                material.SetBuffer("_Selects", RW_Selects);
            }

            public void ClearSpread()
            {
                _clearSpread = true;
            }

            public virtual void Clear()
            {
                _Vertexs.Dispose();
                _Triangles.Dispose();
                RW_Selects.Dispose();
                if (material != null)
                    Object.DestroyImmediate(material);
            }
        }

        public abstract class CalcMeshVertexsData : CalcVertexsData
        { 
            public MeshFilter meshFilter { get; private set; }

            public override bool IsAvailable
            {
                get 
                {
                    if (trans == null || renderer == null || meshFilter == null || meshFilter.sharedMesh == null || material == null || !_Vertexs.IsValid() || !_Triangles.IsValid() || !RW_Selects.IsValid())
                        return false;
                    return true;
                }
            }

            public CalcMeshVertexsData(Renderer renderer, MeshFilter meshFilter): base(renderer, meshFilter.sharedMesh.vertices, meshFilter.sharedMesh.triangles)
            {
                this.meshFilter = meshFilter;
                trans = meshFilter.transform;
            }
        }

        public abstract class CalcSkinnedMeshMeshVertexsData : CalcVertexsData
        {
            public SkinnedMeshRenderer skinnedMesh { get; private set; }

            public override bool IsAvailable
            {
                get
                {
                    if (trans == null || renderer == null || skinnedMesh == null || skinnedMesh.sharedMesh == null || material == null || !_Vertexs.IsValid() || !_Triangles.IsValid() || !RW_Selects.IsValid())
                        return false;
                    return true;
                }
            }

            protected Mesh bakedMesh;

            public CalcSkinnedMeshMeshVertexsData(SkinnedMeshRenderer skinnedMesh) : base(skinnedMesh, skinnedMesh.sharedMesh.vertices, skinnedMesh.sharedMesh.triangles)
            {
                this.skinnedMesh = skinnedMesh;
                trans = skinnedMesh.transform;
                bakedMesh = new Mesh();
            }
        }

        public class CalcMeshVertexsData_ScreenScope : CalcMeshVertexsData
        {
            public CalcMeshVertexsData_ScreenScope(Renderer renderer, MeshFilter meshFilter) : base(renderer, meshFilter) { }

            public override void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                if (camera == null)
                    return;
                CalcUtil.Self.CalcVertexShader.SetVector("_MouseTexcoord", mouseTexcoord);
                CalcUtil.Self.CalcVertexShader.SetFloat("_Size", brushSize);
                CalcUtil.Self.CalcVertexShader.SetFloat("_Depth", brushDepth);
                CalcUtil.Self.CalcVertexShader.SetMatrix("_MV", camera.worldToCameraMatrix * trans.localToWorldMatrix);
                CalcUtil.Self.CalcVertexShader.SetMatrix("_P", camera.projectionMatrix);
                CalcUtil.Self.CalcVertexShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_CalcVertexsWithScreenScope, "_Vertexs", _Vertexs);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_CalcVertexsWithScreenScope, "RW_Selects", RW_Selects);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_CalcVertexsWithScreenScope, Mathf.CeilToInt((float)_Vertexs.count / 1024), 1, 1);
            }
        }

        public class CalcSkinnedMeshVertexsData_ScreenScope : CalcSkinnedMeshMeshVertexsData
        {
            public CalcSkinnedMeshVertexsData_ScreenScope(SkinnedMeshRenderer skinnedMesh) : base(skinnedMesh) { }

            public override void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                if (camera == null)
                    return;
                skinnedMesh.BakeMesh(bakedMesh);
                _Vertexs.SetData(bakedMesh.vertices);
                CalcUtil.Self.CalcVertexShader.SetVector("_MouseTexcoord", mouseTexcoord);
                CalcUtil.Self.CalcVertexShader.SetFloat("_Size", brushSize);
                CalcUtil.Self.CalcVertexShader.SetFloat("_Depth", brushDepth);
                CalcUtil.Self.CalcVertexShader.SetMatrix("_MV", camera.worldToCameraMatrix * trans.localToWorldMatrix);
                CalcUtil.Self.CalcVertexShader.SetMatrix("_P", camera.projectionMatrix);
                CalcUtil.Self.CalcVertexShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_CalcVertexsWithScreenScope, "_Vertexs", _Vertexs);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_CalcVertexsWithScreenScope, "RW_Selects", RW_Selects);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_CalcVertexsWithScreenScope, Mathf.CeilToInt((float)_Vertexs.count / 1024), 1, 1);
            }
        }
    }
}