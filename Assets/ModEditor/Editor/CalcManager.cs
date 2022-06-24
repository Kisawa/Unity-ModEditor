using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ModEditor
{
    public abstract class CalcManager
    {
        protected static ModEditorWindow ModEditor => ModEditorWindow.Self;

        public Transform trans { get; protected set; }
        public Renderer renderer { get; protected set; }
        public Mesh OriginMesh { get; private set; }
        public Material Material { get; private set; }
        /// <summary>
        /// Buffer type is "float3"
        /// </summary>
        public ComputeBuffer _Vertices { get; private set; }
        /// <summary>
        /// Buffer type is "int"
        /// </summary>
        public ComputeBuffer _Triangles { get; private set; }
        public CalcUtil.Cache Cache { get; private set; }
        public virtual bool IsAvailable
        {
            get
            {
                if (trans == null || renderer == null || Material == null || !_Vertices.IsValid() || !_Triangles.IsValid() || Cache.IsAvailable)
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

        public virtual Matrix4x4 TRS { get; }

        public CalcManager(Renderer renderer, Mesh originMesh)
        {
            trans = renderer.transform;
            this.renderer = renderer;
            OriginMesh = originMesh;
            _Vertices = new ComputeBuffer(originMesh.vertexCount, sizeof(float) * 3);
            _Vertices.SetData(originMesh.vertices);
            _Triangles = new ComputeBuffer(originMesh.triangles.Length, sizeof(int));
            _Triangles.SetData(originMesh.triangles);
            Cache = CalcUtil.Self.GetCache(trans, originMesh.vertexCount);
        }

        public abstract void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth);

        public void UpdateVertex(Vector3[] vertex)
        {
            if (!IsAvailable)
            {
                Clear();
                return;
            }
            _Vertices.SetData(vertex);
        }

        public virtual float GetMinDepth(Camera camera, Vector3 mouseTexcoord, float brushSize)
        {
            if (!IsAvailable)
            {
                Clear();
                return ModEditorConstants.VertexBrushMaxDepth;
            }
            Update(camera, mouseTexcoord, brushSize, ModEditorConstants.VertexBrushMaxDepth);
            float[] _depths = new float[Cache.RW_Depths.count];
            Cache.RW_Depths.GetData(_depths);
            float[] _selects = new float[Cache.RW_Selects.count];
            Cache.RW_Selects.GetData(_selects);
            MinJob job = new MinJob()
            {
                nums = new NativeArray<float>(_depths, Allocator.TempJob),
                selects = new NativeArray<float>(_selects, Allocator.TempJob),
                min = new NativeArray<float>(new float[] { ModEditorConstants.VertexBrushMaxDepth }, Allocator.TempJob)
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
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelect, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelect, "_Triangles", _Triangles);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SpreadSelect, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
                Cache.SpreadLevel++;
                float[] _selects = new float[Cache.RW_Selects.count];
                Cache.RW_Selects.GetData(_selects);
                Vector3[] vertices = new Vector3[_Vertices.count];
                _Vertices.GetData(vertices);
                UnifyOverlapVertexJob job = new UnifyOverlapVertexJob()
                {
                    vertexs = new NativeArray<Vector3>(vertices, Allocator.TempJob),
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
                CalcUtil.Self.CalcShader.SetInt("_SpreadLevel", Cache.SpreadLevel);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_RespreadSelect, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_RespreadSelect, "_Triangles", _Triangles);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_RespreadSelect, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
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
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_LockZone, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_LockZone, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_LockZone, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
            }
            else
            {
                Cache.RW_Zone.SetData(Enumerable.Repeat(1, Cache.RW_Zone.count).ToArray());
            }
        }

        public virtual void AddZoneFromSelect()
        {
            if (!IsAvailable)
            {
                Clear();
                return;
            }
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_AddZone, "RW_Selects", Cache.RW_Selects);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_AddZone, "RW_Zone", Cache.RW_Zone);
            CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_AddZone, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
        }

        public virtual void SubZoneFromSelect()
        {
            if (!IsAvailable)
            {
                Clear();
                return;
            }
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SubZone, "RW_Selects", Cache.RW_Selects);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SubZone, "RW_Zone", Cache.RW_Zone);
            CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SubZone, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResultColor(type, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResult(type, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResult(type, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResult(type, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Color[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Vector2[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Vector3[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Vector4[] origin, bool clamp = false, float min = 0, float max = 1)
        {
            return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, Cache.RW_BrushResult, clamp, min, max, Cache.RW_Selects);
        }

        public void Cala(Color col, float strength)
        {
            CalcUtil.Self.CalcShader.SetVector("_From", col * strength);
            CalcUtil.Self.CalcShader.SetVector("_To", col * strength);
            CalcUtil.Self.CalcShader.SetFloat("_FromStep", 0f);
            CalcUtil.Self.CalcShader.SetFloat("_ToStep", 1f);
            if (Cache.SpreadLevel > 0)
            {
                CalcUtil.Self.CalcShader.SetInt("_SpreadLevel", Cache.SpreadLevel);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSpread, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSpread, "RW_Result4", Cache.RW_BrushResult);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSpread, Mathf.CeilToInt((float)Cache.RW_BrushResult.count / 1024), 1, 1);
            }
            else
            {
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Sizes", Cache.RW_Sizes);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Result4", Cache.RW_BrushResult);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSize, Mathf.CeilToInt((float)Cache.RW_BrushResult.count / 1024), 1, 1);
            }
        }

        public void Cala(Color from, Color to, float fromStep, float toStep, float strength)
        {
            CalcUtil.Self.CalcShader.SetVector("_From", from * strength);
            CalcUtil.Self.CalcShader.SetVector("_To", to * strength);
            CalcUtil.Self.CalcShader.SetFloat("_FromStep", fromStep);
            CalcUtil.Self.CalcShader.SetFloat("_ToStep", toStep);
            if (Cache.SpreadLevel > 0)
            {
                CalcUtil.Self.CalcShader.SetInt("_SpreadLevel", Cache.SpreadLevel);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSpread, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSpread, "RW_Result4", Cache.RW_BrushResult);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSpread, Mathf.CeilToInt((float)Cache.RW_BrushResult.count / 1024), 1, 1);
            }
            else
            {
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Sizes", Cache.RW_Sizes);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Result4", Cache.RW_BrushResult);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSize, Mathf.CeilToInt((float)Cache.RW_BrushResult.count / 1024), 1, 1);
            }
        }

        public virtual void BindMaterial(Material material)
        {
            this.Material = material;
            material.SetBuffer("_Selects", Cache.RW_Selects);
            material.SetBuffer("_Zone", Cache.RW_Zone);
            material.SetBuffer("_Colors", Cache.RW_BrushResult);
        }

        public void ClearSpread()
        {
            _clearSpread = true;
            Cache.SpreadLevel = 0;
        }

        public virtual void Clear()
        {
            _Vertices.Dispose();
            _Triangles.Dispose();
            if (Material != null)
                Object.DestroyImmediate(Material);
        }
    }

    public class CalcManager_Mesh : CalcManager
    {
        public MeshFilter meshFilter { get; private set; }

        public override bool IsAvailable
        {
            get
            {
                if (trans == null || renderer == null || meshFilter == null || meshFilter.sharedMesh == null || Material == null || !_Vertices.IsValid() || !_Triangles.IsValid() || Cache.IsAvailable)
                    return false;
                return true;
            }
        }

        public override Matrix4x4 TRS => trans.localToWorldMatrix;

        public CalcManager_Mesh(Renderer renderer, MeshFilter meshFilter) : base(renderer, meshFilter.sharedMesh)
        {
            this.meshFilter = meshFilter;
        }

        public override void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth)
        {
            if (!IsAvailable)
            {
                Clear();
                return;
            }
            if (camera == null)
                return;
            CalcUtil.Self.CalcShader.SetVector("_MouseTexcoord", mouseTexcoord);
            CalcUtil.Self.CalcShader.SetFloat("_Size", brushSize);
            CalcUtil.Self.CalcShader.SetFloat("_Depth", brushDepth);
            CalcUtil.Self.CalcShader.SetMatrix("_MV", camera.worldToCameraMatrix * TRS);
            CalcUtil.Self.CalcShader.SetMatrix("_P", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
            CalcUtil.Self.CalcShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
            CalcUtil.Self.CalcShader.SetInt("_OnlyZone", Key.Shift ? 1 : 0);
            CalcUtil.Self.CalcShader.SetInt("_ZoneInSelect", ModEditor.Tab_VertexBrush.VertexZoneLock && Key.ShiftAndControl ? 1 : 0);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "_Vertices", _Vertices);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Selects", Cache.RW_Selects);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Depths", Cache.RW_Depths);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Sizes", Cache.RW_Sizes);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Zone", Cache.RW_Zone);
            CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SelectWithScreenScope, Mathf.CeilToInt((float)_Vertices.count / 1024), 1, 1);
        }
    }

    public class CalcManager_SkinnedMesh : CalcManager
    {
        public SkinnedMeshRenderer skinnedMesh { get; private set; }

        public override bool IsAvailable
        {
            get
            {
                if (trans == null || renderer == null || skinnedMesh == null || skinnedMesh.sharedMesh == null || Material == null || !_Vertices.IsValid() || !_Triangles.IsValid() || Cache.IsAvailable)
                    return false;
                return true;
            }
        }

        public override Matrix4x4 TRS => Matrix4x4.TRS(trans.position, trans.rotation, Vector3.one);

        protected Mesh bakedMesh;

        public CalcManager_SkinnedMesh(SkinnedMeshRenderer skinnedMesh) : base(skinnedMesh, skinnedMesh.sharedMesh)
        {
            this.skinnedMesh = skinnedMesh;
            bakedMesh = new Mesh();
        }

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
            _Vertices.SetData(bakedMesh.vertices);
            CalcUtil.Self.CalcShader.SetVector("_MouseTexcoord", mouseTexcoord);
            CalcUtil.Self.CalcShader.SetFloat("_Size", brushSize);
            CalcUtil.Self.CalcShader.SetFloat("_Depth", brushDepth);
            CalcUtil.Self.CalcShader.SetMatrix("_MV", camera.worldToCameraMatrix * TRS);
            CalcUtil.Self.CalcShader.SetMatrix("_P", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
            CalcUtil.Self.CalcShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
            CalcUtil.Self.CalcShader.SetInt("_OnlyZone", Key.Shift ? 1 : 0);
            CalcUtil.Self.CalcShader.SetInt("_ZoneInSelect", ModEditor.Tab_VertexBrush.VertexZoneLock && Key.ShiftAndControl ? 1 : 0);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "_Vertices", _Vertices);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Selects", Cache.RW_Selects);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Depths", Cache.RW_Depths);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Sizes", Cache.RW_Sizes);
            CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Zone", Cache.RW_Zone);
            CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SelectWithScreenScope, Mathf.CeilToInt((float)_Vertices.count / 1024), 1, 1);
        }
    }
}