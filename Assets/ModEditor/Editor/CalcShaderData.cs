using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ModEditor
{
    public sealed class CalcShaderData
    {
        public static ModEditorWindow ModEditor => ModEditorWindow.Self;
        public abstract class CalcVertexsData
        {
            public Transform trans { get; protected set; }
            public Renderer renderer { get; protected set; }
            public Material material { get; private set; }
            public ComputeBuffer _Vertexs { get; private set; }
            public ComputeBuffer _Triangles { get; private set; }
            public ComputeBuffer RW_Depths { get; private set; }
            public ComputeBuffer _OriginColors { get; private set; }
            public ComputeBuffer RW_Colors { get; private set; }
            public CalcData.Cache Cache { get; private set; }
            public virtual bool IsAvailable
            {
                get 
                {
                    if (trans == null || renderer == null || material == null || !_Vertexs.IsValid() || !_Triangles.IsValid() || !RW_Depths.IsValid() || !Cache.RW_Selects.IsValid() || !Cache.RW_Zone.IsValid())
                        return false;
                    return true;
                }
            }

            Vector3[] _vertexs;

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
                trans = renderer.transform;
                _vertexs = vertexs;
                _Vertexs = new ComputeBuffer(vertexs.Length, sizeof(float) * 3);
                _Vertexs.SetData(vertexs);
                _Triangles = new ComputeBuffer(triangles.Length, sizeof(int));
                _Triangles.SetData(triangles);
                RW_Depths = new ComputeBuffer(vertexs.Length, sizeof(float));
                RW_Depths.SetData(Enumerable.Repeat(0, vertexs.Length).ToArray());
                _OriginColors = new ComputeBuffer(vertexs.Length, sizeof(float) * 4);
                RW_Colors = new ComputeBuffer(vertexs.Length, sizeof(float) * 4);
                Cache = CalcUtil.Self.GetCache(trans, vertexs.Length);
            }

            public abstract void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth);

            public virtual float GetMinDepth(Camera camera, Vector3 mouseTexcoord, float brushSize)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return ModEditorConstants.BrushMaxDepth;
                }
                Update(camera, mouseTexcoord, brushSize, ModEditorConstants.BrushMaxDepth);
                float[] _depths = new float[RW_Depths.count];
                RW_Depths.GetData(_depths);
                float[] _selects = new float[Cache.RW_Selects.count];
                Cache.RW_Selects.GetData(_selects);
                MinJob job = new MinJob()
                {
                    nums = new NativeArray<float>(_depths, Allocator.TempJob),
                    selects = new NativeArray<float>(_selects, Allocator.TempJob),
                    min = new NativeArray<float>(new float[] { ModEditorConstants.BrushMaxDepth }, Allocator.TempJob)
                };
                JobHandle jobHandle = job.Schedule();
                jobHandle.Complete();
                float res = job.min[0];
                job.min.Dispose();
                return res;
            }
            
            public virtual void SpreadSelects(bool spread)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                if (spread)
                {
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelect, "RW_Selects", Cache.RW_Selects);
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelect, "_Triangles", _Triangles);
                    CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_SpreadSelect, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
                    Cache.SpreadLevel++;
                    float[] _selects = new float[Cache.RW_Selects.count];
                    Cache.RW_Selects.GetData(_selects);
                    UnifyOverlapVertexJob job = new UnifyOverlapVertexJob()
                    {
                        vertexs = new NativeArray<Vector3>(_vertexs, Allocator.TempJob),
                        selects = new NativeArray<float>(_selects, Allocator.TempJob)
                    };
                    JobHandle jobHandle = job.Schedule();
                    jobHandle.Complete();
                    Cache.RW_Selects.SetData(job.selects);
                    job.selects.Dispose();
                }
                else
                {
                    Cache.SpreadLevel--;
                    CalcUtil.Self.CalcVertexShader.SetInt("_SpreadLevel", Cache.SpreadLevel);
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_RespreadSelect, "RW_Selects", Cache.RW_Selects);
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_RespreadSelect, "_Triangles", _Triangles);
                    CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_RespreadSelect, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
                }
            }

            public virtual void LockZoneFromSelect(bool lockZone)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                if (lockZone)
                {
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_LockZone, "RW_Selects", Cache.RW_Selects);
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_LockZone, "RW_Zone", Cache.RW_Zone);
                    CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_LockZone, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
                }
                else
                {
                    Cache.RW_Zone.SetData(Enumerable.Repeat(1, Cache.RW_Zone.count).ToArray());
                }
                Cache.SpreadLevel = 0;
            }

            public virtual void AddZoneFromSelect()
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_AddZone, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_AddZone, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_AddZone, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
            }

            public virtual void SubZoneFromSelect()
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SubZone, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SubZone, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_SubZone, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
            }

            public void Cala(Color color)
            {
                CalcUtil.Self.CalcVertexShader.SetVector("_Color", color);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_Calc, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_Calc, "_OriginColors", _OriginColors);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_Calc, "RW_Colors", RW_Colors);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_Calc, Mathf.CeilToInt((float)RW_Colors.count / 1024), 1, 1);
            }

            public void RefreshColor()
            {
                Color[] colors = new Color[RW_Colors.count];
                RW_Colors.GetData(colors);
                _OriginColors.SetData(colors);
            }

            public virtual void BindMaterial(Material material)
            {
                this.material = material;
                material.SetBuffer("_Selects", Cache.RW_Selects);
                material.SetBuffer("_Zone", Cache.RW_Zone);
            }

            public void ClearSpread()
            {
                _clearSpread = true;
                Cache.SpreadLevel = 0;
            }

            public virtual void Clear()
            {
                _Vertexs.Dispose();
                _Triangles.Dispose();
                RW_Depths.Dispose();
                _OriginColors.Dispose();
                RW_Colors.Dispose();
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
                    if (trans == null || renderer == null || meshFilter == null || meshFilter.sharedMesh == null || material == null || !_Vertexs.IsValid() || !_Triangles.IsValid() || !RW_Depths.IsValid() || !Cache.RW_Selects.IsValid() || !Cache.RW_Zone.IsValid())
                        return false;
                    return true;
                }
            }

            public CalcMeshVertexsData(Renderer renderer, MeshFilter meshFilter): base(renderer, meshFilter.sharedMesh.vertices, meshFilter.sharedMesh.triangles)
            {
                this.meshFilter = meshFilter;
                Color[] colors = meshFilter.sharedMesh.colors;
                if (meshFilter.sharedMesh.colors.Length != meshFilter.sharedMesh.vertexCount)
                    colors = Enumerable.Repeat(Color.white, meshFilter.sharedMesh.vertexCount).ToArray();
                _OriginColors.SetData(colors);
            }
        }

        public abstract class CalcSkinnedMeshMeshVertexsData : CalcVertexsData
        {
            public SkinnedMeshRenderer skinnedMesh { get; private set; }

            public override bool IsAvailable
            {
                get
                {
                    if (trans == null || renderer == null || skinnedMesh == null || skinnedMesh.sharedMesh == null || material == null || !_Vertexs.IsValid() || !_Triangles.IsValid() || !RW_Depths.IsValid())
                        return false;
                    return true;
                }
            }

            protected Mesh bakedMesh;

            public CalcSkinnedMeshMeshVertexsData(SkinnedMeshRenderer skinnedMesh) : base(skinnedMesh, skinnedMesh.sharedMesh.vertices, skinnedMesh.sharedMesh.triangles)
            {
                this.skinnedMesh = skinnedMesh;
                bakedMesh = new Mesh();
                Color[] colors = skinnedMesh.sharedMesh.colors;
                if (skinnedMesh.sharedMesh.colors.Length != skinnedMesh.sharedMesh.vertexCount)
                    colors = Enumerable.Repeat(Color.white, skinnedMesh.sharedMesh.vertexCount).ToArray();
                _OriginColors.SetData(colors);
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
                CalcUtil.Self.CalcVertexShader.SetMatrix("_P", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
                CalcUtil.Self.CalcVertexShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetInt("_OnlyZone", Key.Shift ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetInt("_ZoneInSelect", ModEditor.BrushLock && Key.Shift && Key.Control ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "_Vertexs", _Vertexs);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Depths", RW_Depths);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_SelectWithScreenScope, Mathf.CeilToInt((float)_Vertexs.count / 1024), 1, 1);
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
                CalcUtil.Self.CalcVertexShader.SetMatrix("_P", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
                CalcUtil.Self.CalcVertexShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetInt("_OnlyZone", Key.Shift ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetInt("_ZoneInSelect", ModEditor.BrushLock && Key.Shift && Key.Control ? 1 : 0);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "_Vertexs", _Vertexs);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Depths", RW_Depths);
                CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_SelectWithScreenScope, Mathf.CeilToInt((float)_Vertexs.count / 1024), 1, 1);
            }
        }
    }
}