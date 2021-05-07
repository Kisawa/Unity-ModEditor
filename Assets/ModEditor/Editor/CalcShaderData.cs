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
            /// <summary>
            /// Buffer type is "float3"
            /// </summary>
            public ComputeBuffer _Vertices { get; private set; }
            /// <summary>
            /// Buffer type is "int"
            /// </summary>
            public ComputeBuffer _Triangles { get; private set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Depths { get; private set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Sizes { get; private set; }
            /// <summary>
            /// Buffer type is "float4"
            /// </summary>
            public ComputeBuffer RW_BrushResult { get; private set; }
            public CalcData.Cache Cache { get; private set; }
            public virtual bool IsAvailable
            {
                get 
                {
                    if (trans == null || renderer == null || material == null || !_Vertices.IsValid() || !_Triangles.IsValid() || !RW_Depths.IsValid() || !RW_Sizes.IsValid() || !Cache.RW_Selects.IsValid() || !Cache.RW_Zone.IsValid())
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
                _Vertices = new ComputeBuffer(vertexs.Length, sizeof(float) * 3);
                _Vertices.SetData(vertexs);
                _Triangles = new ComputeBuffer(triangles.Length, sizeof(int));
                _Triangles.SetData(triangles);
                RW_Depths = new ComputeBuffer(vertexs.Length, sizeof(float));
                RW_Sizes = new ComputeBuffer(vertexs.Length, sizeof(float));
                RW_BrushResult = new ComputeBuffer(vertexs.Length, sizeof(float) * 4);
                Cache = CalcUtil.Self.GetCache(trans, vertexs.Length);
            }

            public abstract void Update(Camera camera, Vector3 mouseTexcoord, float brushSize, float brushDepth);

            public void UpdateVertex(Vector3[] vertex)
            {
                if (!IsAvailable)
                {
                    Clear();
                    return;
                }
                _vertexs = vertex;
                _Vertices.SetData(vertex);
            }

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
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelect, "RW_Selects", Cache.RW_Selects);
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelect, "_Triangles", _Triangles);
                    CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SpreadSelect, Mathf.CeilToInt((float)_Triangles.count / 1024), 1, 1);
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
                Cache.SpreadLevel = 0;
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

            public Color[] GetResultColor(WriteType type, Color[] origin)
            {
                return CalcUtil.Self.GetResultColor(type, origin, RW_BrushResult, Cache.RW_Selects);
            }

            public Vector3[] GetResult(WriteType type, Vector3[] origin)
            {
                return CalcUtil.Self.GetResult(type, origin, RW_BrushResult, Cache.RW_Selects);
            }

            public Vector4[] GetResult(WriteType type, Vector4[] origin)
            {
                return CalcUtil.Self.GetResult(type, origin, RW_BrushResult, Cache.RW_Selects);
            }

            public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Color[] origin)
            {
                return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, RW_BrushResult, Cache.RW_Selects);
            }

            public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Vector3[] origin)
            {
                return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, RW_BrushResult, Cache.RW_Selects);
            }

            public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outPass, Vector4[] origin)
            {
                return CalcUtil.Self.GetResultCustom(type, inPass, outPass, origin, RW_BrushResult, Cache.RW_Selects);
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
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSpread, "RW_Result4", RW_BrushResult);
                    CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSpread, Mathf.CeilToInt((float)RW_BrushResult.count / 1024), 1, 1);
                }
                else
                {
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Selects", Cache.RW_Selects);
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Sizes", RW_Sizes);
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Result4", RW_BrushResult);
                    CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSize, Mathf.CeilToInt((float)RW_BrushResult.count / 1024), 1, 1);
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
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSpread, "RW_Result4", RW_BrushResult);
                    CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSpread, Mathf.CeilToInt((float)RW_BrushResult.count / 1024), 1, 1);
                }
                else
                {
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Selects", Cache.RW_Selects);
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Sizes", RW_Sizes);
                    CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_CalcWithSize, "RW_Result4", RW_BrushResult);
                    CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_CalcWithSize, Mathf.CeilToInt((float)RW_BrushResult.count / 1024), 1, 1);
                }
            }

            public virtual void BindMaterial(Material material)
            {
                this.material = material;
                material.SetBuffer("_Selects", Cache.RW_Selects);
                material.SetBuffer("_Zone", Cache.RW_Zone);
                material.SetBuffer("_Colors", RW_BrushResult);
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
                RW_Depths.Dispose();
                RW_Sizes.Dispose();
                RW_BrushResult.Dispose();
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
                    if (trans == null || renderer == null || meshFilter == null || meshFilter.sharedMesh == null || material == null || !_Vertices.IsValid() || !_Triangles.IsValid() || !RW_Depths.IsValid() || !RW_Sizes.IsValid() || !Cache.RW_Selects.IsValid() || !Cache.RW_Zone.IsValid())
                        return false;
                    return true;
                }
            }

            public CalcMeshVertexsData(Renderer renderer, MeshFilter meshFilter): base(renderer, meshFilter.sharedMesh.vertices, meshFilter.sharedMesh.triangles)
            {
                this.meshFilter = meshFilter;
            }
        }

        public abstract class CalcSkinnedMeshMeshVertexsData : CalcVertexsData
        {
            public SkinnedMeshRenderer skinnedMesh { get; private set; }

            public override bool IsAvailable
            {
                get
                {
                    if (trans == null || renderer == null || skinnedMesh == null || skinnedMesh.sharedMesh == null || material == null || !_Vertices.IsValid() || !_Triangles.IsValid() || !RW_Depths.IsValid())
                        return false;
                    return true;
                }
            }

            protected Mesh bakedMesh;

            public CalcSkinnedMeshMeshVertexsData(SkinnedMeshRenderer skinnedMesh) : base(skinnedMesh, skinnedMesh.sharedMesh.vertices, skinnedMesh.sharedMesh.triangles)
            {
                this.skinnedMesh = skinnedMesh;
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
                CalcUtil.Self.CalcShader.SetVector("_MouseTexcoord", mouseTexcoord);
                CalcUtil.Self.CalcShader.SetFloat("_Size", brushSize);
                CalcUtil.Self.CalcShader.SetFloat("_Depth", brushDepth);
                CalcUtil.Self.CalcShader.SetMatrix("_MV", camera.worldToCameraMatrix * trans.localToWorldMatrix);
                CalcUtil.Self.CalcShader.SetMatrix("_P", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
                CalcUtil.Self.CalcShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
                CalcUtil.Self.CalcShader.SetInt("_OnlyZone", Key.Shift ? 1 : 0);
                CalcUtil.Self.CalcShader.SetInt("_ZoneInSelect", ModEditor.BrushLock && Key.ShiftAndControl ? 1 : 0);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "_Vertices", _Vertices);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Depths", RW_Depths);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Sizes", RW_Sizes);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SelectWithScreenScope, Mathf.CeilToInt((float)_Vertices.count / 1024), 1, 1);
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
                _Vertices.SetData(bakedMesh.vertices);
                CalcUtil.Self.CalcShader.SetVector("_MouseTexcoord", mouseTexcoord);
                CalcUtil.Self.CalcShader.SetFloat("_Size", brushSize);
                CalcUtil.Self.CalcShader.SetFloat("_Depth", brushDepth);
                CalcUtil.Self.CalcShader.SetMatrix("_MV", camera.worldToCameraMatrix * trans.localToWorldMatrix);
                CalcUtil.Self.CalcShader.SetMatrix("_P", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
                CalcUtil.Self.CalcShader.SetInt("_ClearSpread", clearSpread ? 1 : 0);
                CalcUtil.Self.CalcShader.SetInt("_OnlyZone", Key.Shift ? 1 : 0);
                CalcUtil.Self.CalcShader.SetInt("_ZoneInSelect", ModEditor.BrushLock && Key.ShiftAndControl ? 1 : 0);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "_Vertices", _Vertices);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Selects", Cache.RW_Selects);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Depths", RW_Depths);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Sizes", RW_Sizes);
                CalcUtil.Self.CalcShader.SetBuffer(CalcUtil.Self.kernel_SelectWithScreenScope, "RW_Zone", Cache.RW_Zone);
                CalcUtil.Self.CalcShader.Dispatch(CalcUtil.Self.kernel_SelectWithScreenScope, Mathf.CeilToInt((float)_Vertices.count / 1024), 1, 1);
            }
        }
    }
}