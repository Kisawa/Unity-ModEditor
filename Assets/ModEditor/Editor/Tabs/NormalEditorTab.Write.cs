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
        RenderTexture _expandEdgeTexture;
        RenderTexture expandEdgeTexture
        {
            get
            {
                if (_expandEdgeTexture == null)
                {
                    _expandEdgeTexture = RenderTexture.GetTemporary(4096, 4096, 24);
                    _expandEdgeTexture.format = RenderTextureFormat.RFloat;
                    _expandEdgeTexture.hideFlags = HideFlags.HideAndDontSave;
                }
                if (texture != null)
                {
                    window.Mat_viewUtil.SetFloat("_Spread", 2);
                    Graphics.Blit(texture, _expandEdgeTexture, window.Mat_viewUtil, 10);
                }
                return _expandEdgeTexture;
            }
        }

        void write()
        {
            switch (window.Manager.BrushType)
            {
                case BrushType.Penetrate:
                    writeVertexColor_Penetrate();
                    break;
                case BrushType.DepthCull:
                    writeVertexColor_DepthCull();
                    break;
                default:
                    break;
            }
        }

        void writeVertexColor_Penetrate()
        {
            if (objInOperation == null || objInOperation.Count == 0 || window.camera == null || texture == null)
                return;
            List<Mesh> meshes = new List<Mesh>();
            List<Vector3> vertexs = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Matrix4x4> matrixs_mvp = new List<Matrix4x4>();
            for (int i = 0; i < objInOperation.Count; i++)
            {
                Transform trans = objInOperation[i].Item1;
                Mesh mesh = objInOperation[i].Item2;
                SkinnedMeshRenderer skinnedMeshRenderer = objInOperation[i].Item3;
                if (trans == null || mesh == null || (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != mesh))
                    continue;

                if (mesh.colors.Length != mesh.vertexCount)
                    mesh.colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                Mesh calcMesh = mesh;
                if (skinnedMeshRenderer != null)
                {
                    calcMesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(calcMesh);
                }
                meshes.Add(mesh);
                vertexs.AddRange(calcMesh.vertices);
                colors.AddRange(calcMesh.colors);
                Matrix4x4 _mvp = window.camera.projectionMatrix * window.camera.worldToCameraMatrix * trans.localToWorldMatrix;
                matrixs_mvp.AddRange(Enumerable.Repeat(_mvp, mesh.vertexCount).ToArray());
            }
            Color[] rw_colors = writeVertexColor_Penetrate(vertexs.ToArray(), colors.ToArray(), matrixs_mvp.ToArray());
            int step = 0;
            for (int i = 0; i < meshes.Count; i++)
            {
                Mesh mesh = meshes[i];
                mesh.colors = rw_colors.Skip(step).Take(mesh.vertexCount).ToArray();
                step += mesh.vertexCount;
            }
        }

        void writeVertexColor_DepthCull()
        {
            if (objInOperation == null || objInOperation.Count == 0 || window.camera == null || texture == null)
                return;
            List<Mesh> meshes = new List<Mesh>();
            List<Vector3> vertexs = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Matrix4x4> matrixs_mv = new List<Matrix4x4>();
            for (int i = 0; i < objInOperation.Count; i++)
            {
                Transform trans = objInOperation[i].Item1;
                Mesh mesh = objInOperation[i].Item2;
                SkinnedMeshRenderer skinnedMeshRenderer = objInOperation[i].Item3;
                if (trans == null || mesh == null || (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != mesh))
                    continue;

                if (mesh.colors.Length != mesh.vertexCount)
                    mesh.colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                Mesh calcMesh = mesh;
                if (skinnedMeshRenderer != null)
                {
                    calcMesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(calcMesh);
                }
                meshes.Add(mesh);
                vertexs.AddRange(calcMesh.vertices);
                colors.AddRange(calcMesh.colors);
                Matrix4x4 _mv = window.camera.worldToCameraMatrix * trans.localToWorldMatrix;
                matrixs_mv.AddRange(Enumerable.Repeat(_mv, mesh.vertexCount).ToArray());
            }
            Color[] rw_colors = writeVertexColor_DepthCull(vertexs.ToArray(), colors.ToArray(), matrixs_mv.ToArray());
            int step = 0;
            for (int i = 0; i < meshes.Count; i++)
            {
                Mesh mesh = meshes[i];
                mesh.colors = rw_colors.Skip(step).Take(mesh.vertexCount).ToArray();
                step += mesh.vertexCount;
            }
        }

        Color[] writeVertexColor_DepthCull(Vector3[] vertexs, Color[] colors, Matrix4x4[] matrixs_mv)
        {
            CalcData.CalcVertexColorWithDepthCull data = new CalcData.CalcVertexColorWithDepthCull();
            data._MV = matrixs_mv;
            data._P = window.camera.projectionMatrix;
            data._Vertexs = vertexs;
            data._Colors = colors;
            data._MouseTexcoord = screenTexcoord;
            data._Size = window.Manager.BrushSize;
            data._DepthMap = expandEdgeTexture;
            Color[] select = CalcUtil.WriteVertexColor_DepthCull(data, window.Manager.BrushColor);
            return select;
        }

        Color[] writeVertexColor_Penetrate(Vector3[] vertexs, Color[] colors, Matrix4x4[] matrixs_mvp)
        {
            CalcData.CalcVertexColorWithPenetrate data = new CalcData.CalcVertexColorWithPenetrate();
            data._MVP = matrixs_mvp;
            data._Vertexs = vertexs;
            data._Colors = colors;
            data._MouseTexcoord = screenTexcoord;
            data._Size = window.Manager.BrushSize;
            Color[] select = CalcUtil.WriteVertexColor_Penetrate(data, window.Manager.BrushColor);
            return select;
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