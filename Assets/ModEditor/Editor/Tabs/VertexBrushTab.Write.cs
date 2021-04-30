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
                if (target == null || !target.activeSelf)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                    _objInOperation.Add((target.transform, window.SetEditingMesh(target, meshFilter)));
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    _objInOperation.Add((target.transform, window.SetEditingMesh(target, skinnedMeshRenderer)));
            }
            return _objInOperation;
        }

        private void OnMouse_DownLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.VertexView && !BrushDisable())
            {
                objInOperation = recordObjInOperation();
                write_ScreenScope();
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
            if (window.VertexView && !BrushDisable())
                write_ScreenScope();
        }

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
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.BrushColorToStep += 0.01f;
                Repaint();
            }
        }

        private void Z_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.BrushColorToStep -= 0.01f;
                Repaint();
            }
        }

        private void D_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.BrushColorFromStep += 0.01f;
                Repaint();
            }
        }

        private void A_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.BrushColorFromStep -= 0.01f;
                Repaint();
            }
        }

        public bool BrushDisable()
        {
            if (window.Manager.WriteTargetType == WriteTargetType.None)
                return true;
            if (window.Manager.WriteTargetType == WriteTargetType.Custom)
            {
                if (window.Manager.CustomTargetType_X == CustomTargetType.None &&
                    window.Manager.CustomTargetType_Y == CustomTargetType.None &&
                    window.Manager.CustomTargetType_Z == CustomTargetType.None &&
                    window.Manager.CustomTargetType_W == CustomTargetType.None)
                    return true;
            }
            return false;
        }

        void executeCalcUtil(VertexCalcUtilBase util)
        {
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null || !target.activeSelf)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                Mesh mesh = null;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                    mesh = window.SetEditingMesh(target, meshFilter);
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    mesh = window.SetEditingMesh(target, skinnedMeshRenderer);
                if (mesh != null)
                {
                    switch (util.PassCount)
                    {
                        case PassCount.One:
                            {
                                float[] result = util.ExecuteOne(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result);
                            }
                            break;
                        case PassCount.Two:
                            {
                                Vector2[] result = util.ExecuteTwo(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result);
                            }
                            break;
                        case PassCount.Three:
                            {
                                Vector3[] result = util.ExecuteThree(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result);
                            }
                            break;
                        case PassCount.Four:
                            {
                                Vector4[] result = util.ExecuteFour(mesh);
                                if(result != null && result.Length > 0)
                                    write_Data(mesh, result);
                            }
                            break;
                        case PassCount.Color:
                            {
                                Color[] result = util.ExecuteColor(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result);
                            }
                            break;
                    }
                }
            }
        }

        void write_Data(Mesh mesh, float[] result)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, colors, result);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.tangents, result);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, window.Manager.CustomTargetPass_X, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Vector2[] result)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, colors, result);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.tangents, result);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Vector3[] result)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, colors, result);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.tangents, result);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Vector4[] result)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, colors, result);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.tangents, result);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_W, TargetPassType.W, window.Manager.CustomTargetPass_W, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Color[] result)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, colors, result);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.tangents, result);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_W, TargetPassType.W, window.Manager.CustomTargetPass_W, mesh, result);
                    break;
            }
        }

        void write_ScreenScope()
        {
            if (objInOperation == null || objInOperation.Count == 0)
                return;
            for (int i = 0; i < objInOperation.Count; i++)
            {
                Transform trans = objInOperation[i].Item1;
                Mesh mesh = objInOperation[i].Item2;
                if (trans == null || mesh == null)
                    continue;
                CalcShaderData.CalcVertexsData data = window.CalcShaderDatas.FirstOrDefault(x => x.trans == trans);
                if (data != null && data.IsAvailable)
                {
                    switch (window.Manager.VertexBrushType)
                    {
                        case VertexBrushType.Color:
                            data.Cala(window.Manager.BrushColor, window.Manager.BrushStrength);
                            break;
                        case VertexBrushType.TwoColorGradient:
                            data.Cala(window.Manager.BrushColorFrom, window.Manager.BrushColorTo, window.Manager.BrushColorFromStep, window.Manager.BrushColorToStep, window.Manager.BrushStrength);
                            break;
                    }
                    WriteType writeType = window.Manager.WriteType;
                    if (writeType == WriteType.OtherUtil)
                    {
                        VertexBrushUtilBase util = brushUtilInstances[window.Manager.BrushUtilIndex];
                        util.TargetType = window.Manager.WriteTargetType;
                        util.CustomTarget_X = window.Manager.CustomTargetType_X;
                        util.CustomTarget_Y = window.Manager.CustomTargetType_Y;
                        util.CustomTarget_Z = window.Manager.CustomTargetType_Z;
                        util.CustomTarget_W = window.Manager.CustomTargetType_W;
                        util.CustomPass_X = window.Manager.CustomTargetPass_X;
                        util.CustomPass_Y = window.Manager.CustomTargetPass_Y;
                        util.CustomPass_Z = window.Manager.CustomTargetPass_Z;
                        util.CustomPass_W = window.Manager.CustomTargetPass_W;
                        if (util.BrushWrite(mesh, data))
                            writeType = WriteType.Replace;
                        else
                            continue;
                    }
                    switch (window.Manager.WriteTargetType)
                    {
                        case WriteTargetType.VertexColor:
                            Color[] colors = mesh.colors;
                            if (colors.Length != mesh.vertexCount)
                                colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                            mesh.colors = data.GetResultColor(writeType, colors);
                            break;
                        case WriteTargetType.Vertex:
                            mesh.vertices = data.GetResult(writeType, mesh.vertices);
                            data.UpdateVertex(mesh.vertices);
                            break;
                        case WriteTargetType.Normal:
                            mesh.normals = data.GetResult(writeType, mesh.normals);
                            break;
                        case WriteTargetType.Tangent:
                            mesh.tangents = data.GetResult(writeType, mesh.tangents);
                            break;
                        case WriteTargetType.Custom:
                            setCustomData(writeType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, data);
                            setCustomData(writeType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, data);
                            setCustomData(writeType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, data);
                            setCustomData(writeType, window.Manager.CustomTargetType_W, TargetPassType.W, window.Manager.CustomTargetPass_W, mesh, data);
                            break;
                    }
                }
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, CalcShaderData.CalcVertexsData data)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = data.GetResultCustom(writeType, inPass, outPass, colors);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = data.GetResultCustom(writeType, inPass, outPass, mesh.vertices);
                    data.UpdateVertex(mesh.vertices);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = data.GetResultCustom(writeType, inPass, outPass, mesh.normals);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = data.GetResultCustom(writeType, inPass, outPass, mesh.tangents);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType outPass, Mesh mesh, float[] result)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, colors, result);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, mesh.vertices, result);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, mesh.normals, result);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, mesh.tangents, result);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Vector2[] result)
        {
            if (inPass == TargetPassType.Z || inPass == TargetPassType.W)
            {
                Debug.LogWarning($"ModEditor VertexBrush: CustomData dont have \"{inPass}\" pass");
                return;
            }
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, colors, result);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.tangents, result);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Vector3[] result)
        {
            if (inPass == TargetPassType.W)
            {
                Debug.LogWarning($"ModEditor VertexBrush: CustomData dont have \"{inPass}\" pass");
                return;
            }
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, colors, result);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.tangents, result);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Vector4[] result)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, colors, result);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.tangents, result);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Color[] result)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    Color[] colors = mesh.colors;
                    if (colors.Length != mesh.vertexCount)
                        colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, colors, result);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.tangents, result);
                    break;
            }
        }
    }
}