using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class VertexBrushTab
    {
        bool brushColorView;
        public bool BrushColorView
        {
            get => brushColorView;
            set
            {
                if (brushColorView == value)
                    return;
                brushColorView = value;
            }
        }

        public bool VertexZoneLock { get; set; }

        public bool VertexBrushLock { get; set; }

        List<CalcManager> calcShaderDatas;
        public List<CalcManager> CalcShaderDatas
        {
            get
            {
                if (calcShaderDatas == null)
                    calcShaderDatas = new List<CalcManager>();
                return calcShaderDatas;
            }
        }

        List<(Transform, Mesh, Mesh)> objInOperation;

        public CalcManager AddCalcShaderRender(Renderer renderer, MeshFilter meshFilter)
        {
            if (renderer == null || meshFilter == null || meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertexCount == 0)
                return null;
            Material material = new Material(Shader.Find("Hidden/ModEditorVertexView"));
            CalcManager data = new CalcManager_Mesh(renderer, meshFilter);
            data.BindMaterial(material);
            CalcShaderDatas.Add(data);
            return data;
        }

        public CalcManager AddCalcShaderRender(SkinnedMeshRenderer skinnedMesh)
        {
            if (skinnedMesh == null || skinnedMesh.sharedMesh == null || skinnedMesh.sharedMesh.vertexCount == 0)
                return null;
            Material material = new Material(Shader.Find("Hidden/ModEditorVertexView"));
            CalcManager data = new CalcManager_SkinnedMesh(skinnedMesh);
            data.BindMaterial(material);
            CalcShaderDatas.Add(data);
            return data;
        }

        public void ClearCalcShaderData()
        {
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].Clear();
            CalcShaderDatas.Clear();
        }

        public void RecordObjInOperation()
        {
            if (window.camera == null || (objInOperation != null && objInOperation.Count != 0))
                return;
            List<(Transform, Mesh, Mesh)> _objInOperation = new List<(Transform, Mesh, Mesh)>();
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                    _objInOperation.Add((target.transform, window.SetEditingMesh(target, meshFilter), null));
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    Mesh bakedMesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(bakedMesh);
                    _objInOperation.Add((target.transform, window.SetEditingMesh(target, skinnedMeshRenderer), bakedMesh));
                }
            }
            objInOperation = _objInOperation;
        }

        public void ClearObjInOperation()
        {
            if(objInOperation != null)
                objInOperation.Clear();
        }

        private void Tab_Down()
        {
            if (window.ToolType == ModEditorToolType.VertexBrush)
                window.Manager.VertexWithZTest = !window.Manager.VertexWithZTest;
        }

        private void OnMouse_DownLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.ToolType == ModEditorToolType.VertexBrush && !BrushDisable())
            {
                RecordObjInOperation();
                BrushWrite();
            }
        }

        private void OnMouse_UpLeft()
        {
            if (WriteCommand)
                return;
            ClearObjInOperation();
        }

        private void OnMouse_DragLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.ToolType == ModEditorToolType.VertexBrush && !BrushDisable())
                BrushWrite();
        }

        private void Alt_OnScrollWheel_Roll(float obj)
        {
            if (window.OnSceneGUI || window.ToolType != ModEditorToolType.VertexBrush)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].SpreadSelects(obj < 0);
        }

        private void Shift_OnMouse_Left()
        {
            if (window.OnSceneGUI || !VertexZoneLock || VertexBrushLock)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].AddZoneFromSelect();
        }

        private void Shift_OnMouse_Right()
        {
            if (window.OnSceneGUI || !VertexZoneLock || VertexBrushLock)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].SubZoneFromSelect();
        }

        private void Space_Down()
        {
            if (window.ToolType != ModEditorToolType.VertexBrush || VertexBrushLock)
                return;
            float depth = float.MaxValue;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
            {
                float _depth = CalcShaderDatas[i].GetMinDepth(window.camera, Mouse.ScreenTexcoord, window.Manager.VertexBrushSize);
                if (depth > _depth)
                    depth = _depth;
            }
            window.Manager.VertexBrushDepth = depth + 0.0001f;
            Mouse_Update();
        }

        private void CapsLock_Down()
        {
            if (VertexBrushLock)
                return;
            VertexZoneLock = !VertexZoneLock;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].LockZoneFromSelect(VertexZoneLock);
        }

        private void Alt_CapsLock_Down()
        {
            VertexBrushLock = !VertexBrushLock;
            Repaint();
        }

        private void Control_OnMouse_DragLeft()
        {
            if (window.ToolType != ModEditorToolType.VertexBrush || VertexBrushLock)
                return;
            window.Manager.VertexBrushSize += Event.current.delta.x * 0.01f;
            window.SceneHandleType = SceneHandleType.BrushSize;
        }

        private void Control_OnScrollWheel_Roll(float obj)
        {
            if (window.ToolType != ModEditorToolType.VertexBrush || VertexBrushLock)
                return;
            window.Manager.VertexBrushDepth -= Event.current.delta.y * 0.01f;
            window.SceneHandleType = SceneHandleType.BrushDepth;
        }

        private void Mouse_Update()
        {
            if (Mouse.IsButton || VertexBrushLock)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].ClearSpread();
        }

        private void ScrollWheel_Update()
        {
            if (Key.Alt || VertexBrushLock)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].ClearSpread();
        }

        private void V_Down()
        {
            BrushColorView = !BrushColorView;
        }

        private void C_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.VertexBrushColorToStep += 0.01f;
                Repaint();
            }
        }

        private void Z_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.VertexBrushColorToStep -= 0.01f;
                Repaint();
            }
        }

        private void D_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.VertexBrushColorFromStep += 0.01f;
                Repaint();
            }
        }

        private void A_Down()
        {
            if (window.Manager.VertexBrushType == VertexBrushType.TwoColorGradient)
            {
                window.Manager.VertexBrushColorFromStep -= 0.01f;
                Repaint();
            }
        }

        private void Enter_Down()
        {
            VertexCalcUtilBase util = calcUtilInstances[window.Manager.CalcUtilIndex];
            if (util.WithSelect)
                excuteCalcUtil(util);
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

        public void BrushWrite()
        {
            if (objInOperation == null || objInOperation.Count == 0)
                return;
            for (int i = 0; i < objInOperation.Count; i++)
            {
                Transform trans = objInOperation[i].Item1;
                Mesh mesh = objInOperation[i].Item2;
                if (trans == null || mesh == null)
                    continue;
                CalcManager data = CalcShaderDatas.FirstOrDefault(x => x.trans == trans);
                if (data != null && data.IsAvailable)
                {
                    switch (window.Manager.VertexBrushType)
                    {
                        case VertexBrushType.Color:
                            data.Cala(window.Manager.VertexBrushColor, window.Manager.VertexBrushStrength);
                            break;
                        case VertexBrushType.TwoColorGradient:
                            data.Cala(window.Manager.VertexBrushColorFrom, window.Manager.VertexBrushColorTo, window.Manager.VertexBrushColorFromStep, window.Manager.VertexBrushColorToStep, window.Manager.VertexBrushStrength);
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
                        if (util.BrushWrite(objInOperation[i].Item3 == null ? objInOperation[i].Item2 : objInOperation[i].Item3, data))
                            writeType = WriteType.Replace;
                        else
                            continue;
                    }
                    switch (window.Manager.WriteTargetType)
                    {
                        case WriteTargetType.VertexColor:
                            mesh.colors = data.GetResultColor(writeType, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                            break;
                        case WriteTargetType.Vertex:
                            mesh.vertices = data.GetResult(writeType, mesh.vertices, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                            data.UpdateVertex(mesh.vertices);
                            break;
                        case WriteTargetType.Normal:
                            mesh.normals = data.GetResult(writeType, mesh.normals, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                            break;
                        case WriteTargetType.Tangent:
                            mesh.tangents = data.GetResult(writeType, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                            break;
                        case WriteTargetType.UV2:
                            mesh.uv2 = data.GetResult(writeType, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                            break;
                        case WriteTargetType.UV3:
                            mesh.uv3 = data.GetResult(writeType, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
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

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, CalcManager data)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    mesh.colors = data.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = data.GetResultCustom(writeType, inPass, outPass, mesh.vertices, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                    data.UpdateVertex(mesh.vertices);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = data.GetResultCustom(writeType, inPass, outPass, mesh.normals, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = data.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                    break;
                case CustomTargetType.UV2:
                    mesh.uv2 = data.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                    break;
                case CustomTargetType.UV3:
                    mesh.uv3 = data.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax);
                    break;
            }
        }

        void excuteCalcUtil(VertexCalcUtilBase util)
        {
            if (BrushDisable())
                return;
            WriteCommand = false;
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
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
                if (mesh != null && mesh.vertexCount > 0)
                {
                    ComputeBuffer _Select = null;
                    if (util.WithSelect)
                    {
                        CalcManager data = CalcShaderDatas.FirstOrDefault(x => x.trans == target.transform);
                        _Select = data.Cache.RW_Selects;
                    }
                    switch (util.PassCount)
                    {
                        case PassCount.One:
                            {
                                float[] result = util.ExecuteOne(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result, _Select);
                            }
                            break;
                        case PassCount.Two:
                            {
                                Vector2[] result = util.ExecuteTwo(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result, _Select);
                            }
                            break;
                        case PassCount.Three:
                            {
                                Vector3[] result = util.ExecuteThree(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result, _Select);
                            }
                            break;
                        case PassCount.Four:
                            {
                                Vector4[] result = util.ExecuteFour(mesh);
                                if(result != null && result.Length > 0)
                                    write_Data(mesh, result, _Select);
                            }
                            break;
                        case PassCount.Color:
                            {
                                Color[] result = util.ExecuteColor(mesh);
                                if (result != null && result.Length > 0)
                                    write_Data(mesh, result, _Select);
                            }
                            break;
                        case PassCount.Other:
                            {
                                util.ExecuteOther(mesh);
                            }
                            break;
                    }
                }
            }
        }

        void write_Data(Mesh mesh, float[] result, ComputeBuffer _Select = null)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, window.Manager.CustomTargetPass_X, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Vector2[] result, ComputeBuffer _Select = null)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Vector3[] result, ComputeBuffer _Select = null)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Vector4[] result, ComputeBuffer _Select = null)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_W, TargetPassType.W, window.Manager.CustomTargetPass_W, mesh, result);
                    break;
            }
        }

        void write_Data(Mesh mesh, Color[] result, ComputeBuffer _Select = null)
        {
            switch (window.Manager.WriteTargetType)
            {
                case WriteTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultColor(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResult(window.Manager.WriteType, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResult(window.Manager.WriteType, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case WriteTargetType.Custom:
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_X, TargetPassType.X, window.Manager.CustomTargetPass_X, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Y, TargetPassType.Y, window.Manager.CustomTargetPass_Y, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_Z, TargetPassType.Z, window.Manager.CustomTargetPass_Z, mesh, result);
                    setCustomData(window.Manager.WriteType, window.Manager.CustomTargetType_W, TargetPassType.W, window.Manager.CustomTargetPass_W, mesh, result);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType outPass, Mesh mesh, float[] result, ComputeBuffer _Select = null)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResultCustom(writeType, TargetPassType.X, outPass, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Vector2[] result, ComputeBuffer _Select = null)
        {
            if (inPass == TargetPassType.Z || inPass == TargetPassType.W)
            {
                Debug.LogWarning($"ModEditor VertexBrush: CustomData dont have \"{inPass}\" pass");
                return;
            }
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Vector3[] result, ComputeBuffer _Select = null)
        {
            if (inPass == TargetPassType.W)
            {
                Debug.LogWarning($"ModEditor VertexBrush: CustomData dont have \"{inPass}\" pass");
                return;
            }
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Vector4[] result, ComputeBuffer _Select = null)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
            }
        }

        void setCustomData(WriteType writeType, CustomTargetType customTarget, TargetPassType inPass, TargetPassType outPass, Mesh mesh, Color[] result, ComputeBuffer _Select = null)
        {
            switch (customTarget)
            {
                case CustomTargetType.VertexColor:
                    mesh.colors = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Vertex:
                    mesh.vertices = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.vertices, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Normal:
                    mesh.normals = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, mesh.normals, result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.Tangent:
                    mesh.tangents = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.tangents, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV2:
                    mesh.uv2 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
                case CustomTargetType.UV3:
                    mesh.uv3 = CalcUtil.Self.GetResultCustom(writeType, inPass, outPass, CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), result, window.Manager.VertexBrushClamp, window.Manager.VertexBrushClampMin, window.Manager.VertexBrushClampMax, _Select);
                    break;
            }
        }
    }
}