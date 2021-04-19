using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ModEditor
{
    public partial class VertexBrushTab
    {
        List<(Transform, Mesh)> objInOperation;

        List<(Transform, Mesh)> recordObjInOperation()
        {
            if (window.camera == null)
                return null;
            List<(Transform, Mesh)> _objInOperation = new List<(Transform, Mesh)>();
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
                    _objInOperation.Add((target.transform, meshFilter.sharedMesh));
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    _objInOperation.Add((target.transform, skinnedMeshRenderer.sharedMesh));
                }
            }
            return _objInOperation;
        }

        private void OnMouse_DownLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.VertexView)
            {
                objInOperation = recordObjInOperation();
                write();
            }
        }

        private void OnMouse_UpLeft()
        {
            objInOperation = null;
        }

        private void OnMouse_DragLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.VertexView)
                write();
        }

        private void OnMouse_Scroll() { }

        private void Alt_OnScrollWheel_Roll(float obj)
        {
            if (window.OnSceneGUI || !window.VertexView)
                return;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].SpreadSelects(obj < 0);
        }

        private void Shift_OnMouse_Left()
        {
            if (window.OnSceneGUI || !window.BrushLock)
                return;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].AddZoneFromSelect();
        }

        private void Shift_OnMouse_Right()
        {
            if (window.OnSceneGUI || !window.BrushLock)
                return;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].SubZoneFromSelect();
        }

        private void Space_Down()
        {
            if (!window.VertexView)
                return;
            float depth = float.MaxValue;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
            {
                float _depth = window.CalcShaderDatas[i].GetMinDepth(window.camera, Mouse.ScreenTexcoord, window.Manager.BrushSize);
                if (depth > _depth)
                    depth = _depth;
            }
            window.Manager.BrushDepth = depth + 0.0001f;
            Mouse_Update();
        }

        private void CapsLock_Down()
        {
            window.BrushLock = !window.BrushLock;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].LockZoneFromSelect(window.BrushLock);
        }

        private void Control_OnMouse_DragLeft()
        {
            if (!window.VertexView)
                return;
            window.Manager.BrushSize += Event.current.delta.x * 0.01f;
            window.SceneHandleType = SceneHandleType.BrushSize;
        }

        private void Control_OnScrollWheel_Roll(float obj)
        {
            if (!window.VertexView)
                return;
            window.Manager.BrushDepth -= Event.current.delta.y * 0.01f;
            window.SceneHandleType = SceneHandleType.BrushDepth;
        }

        private void Mouse_Update()
        {
            if (Mouse.IsButton)
                return;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].ClearSpread();
        }

        private void ScrollWheel_Update()
        {
            if (Key.Alt)
                return;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].ClearSpread();
        }

        private void V_Down()
        {
            window.BrushColorView = !window.BrushColorView;
        }

        private void C_Down()
        {
            window.Manager.BrushColorToStep += 0.01f;
            Repaint();
        }

        private void Z_Down()
        {
            window.Manager.BrushColorToStep -= 0.01f;
            Repaint();
        }

        private void D_Down()
        {
            window.Manager.BrushColorFromStep += 0.01f;
            Repaint();
        }

        private void A_Down()
        {
            window.Manager.BrushColorFromStep -= 0.01f;
            Repaint();
        }

        void write()
        {
            if (objInOperation == null)
                return;
            switch (window.Manager.BrushType)
            {
                case BrushType.ScreenScope:
                    writeVertexColor_ScreenScope();
                    break;
            }
        }

        void writeVertexColor_ScreenScope()
        {
            for (int i = 0; i < objInOperation.Count; i++)
            {
                Transform trans = objInOperation[i].Item1;
                Mesh mesh = objInOperation[i].Item2;
                if (trans == null || mesh == null)
                    continue;
                CalcShaderData.CalcVertexsData data = window.CalcShaderDatas.FirstOrDefault(x => x.trans == trans);
                if (data != null && data.IsAvailable)
                {
                    data.Cala(window.Manager.BrushColorFrom, window.Manager.BrushColorTo, window.Manager.BrushColorFromStep, window.Manager.BrushColorToStep);
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = data.GetResultColor(window.Manager.WriteType, window.Manager.WriteTargetType, colors);
                }
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