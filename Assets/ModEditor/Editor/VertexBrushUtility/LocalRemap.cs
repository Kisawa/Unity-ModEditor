using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ModEditor
{
    public class LocalRemap : VertexBrushUtilBase
    {
        public override string Name => "LocalRemap";

        public override string Tip => "Calc the vertex local position relative to the object.";

        Dictionary<Transform, Mesh> originMeshDic;
        Transform targetTrans;
        bool available;
        List<Quaternion> rotation { get => window.LocalRemapRotation; set => window.LocalRemapRotation = value; }
        List<Vector3> coord { get => window.LocalRemapCoord; set => window.LocalRemapCoord = value; }

        ComputeShader calcShader;
        int kernel_LocalMix;
        int kernel_LocalRemap;

        public LocalRemap()
        {
            calcShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/VertexBrushUtility/LocalRemap.compute");
            kernel_LocalMix = calcShader.FindKernel("LocalMix");
            kernel_LocalRemap = calcShader.FindKernel("LocalRemap");
            originMeshDic = new Dictionary<Transform, Mesh>();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            window.onTargetChanged += Window_onTargetChanged;
            window.onSceneValidate += Window_onSceneValidate;
            available = window.Manager.Target != null;
            if (available)
                targetTrans = window.Manager.Target.transform;
            else
                targetTrans = null;
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onTargetChanged -= Window_onTargetChanged;
            window.onSceneValidate -= Window_onSceneValidate;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            clearMesh();
        }

        private void Window_onTargetChanged(GameObject obj)
        {
            available = obj != null;
            window.LocalRemapCoordClear();
            window.LocalRemapRotationClear();
            if (available)
            {
                targetTrans = window.Manager.Target.transform;
                rotation[0] = targetTrans.rotation;
                coord[0] = targetTrans.position;
            }
            else
                targetTrans = null;
            clearMesh();
        }

        private void Window_onSceneValidate(SceneView obj)
        {
            if (!available || window.ToolType != ModEditorToolType.VertexBrush)
                return;
            for (int i = 0; i < coord.Count; i++)
            {
                window.LocalRemapRotationSet(i, Handles.RotationHandle(rotation[i], coord[i]));
                window.LocalRemapCoordSet(i, Handles.PositionHandle(coord[i], rotation[i]));
            }
            if (GUI.changed)
                window.Repaint();
        }

        public override bool BrushWrite(Mesh mesh, CalcManager data)
        {
            if (!available)
                return false;
            ComputeBuffer _origin4 = new ComputeBuffer(mesh.vertexCount, sizeof(float) * 4);
            originMeshDic.TryGetValue(data.trans, out Mesh _mesh);
            if (_mesh == null)
                _mesh = mesh;
            switch (TargetType)
            {
                case WriteTargetType.VertexColor:
                    CalcUtil.Self.GetBuffer4(CalcUtil.CheckArrayNotNull(_mesh.colors, _mesh.vertexCount), _origin4);
                    break;
                case WriteTargetType.Vertex:
                    CalcUtil.Self.GetBuffer4(_mesh.vertices, _origin4);
                    break;
                case WriteTargetType.Normal:
                    CalcUtil.Self.GetBuffer4(_mesh.normals, _origin4);
                    break;
                case WriteTargetType.Tangent:
                    CalcUtil.Self.GetBuffer4(_mesh.tangents, _origin4);
                    break;
                case WriteTargetType.UV2:
                    CalcUtil.Self.GetBuffer4(CalcUtil.CheckArrayNotNull(_mesh.uv2, _mesh.vertexCount), _origin4);
                    break;
                case WriteTargetType.UV3:
                    CalcUtil.Self.GetBuffer4(CalcUtil.CheckArrayNotNull(_mesh.uv3, _mesh.vertexCount), _origin4);
                    break;
                case WriteTargetType.Custom:
                    setCustomBuffer(CustomTarget_X, CustomPass_X, TargetPassType.X, _mesh, _origin4);
                    setCustomBuffer(CustomTarget_Y, CustomPass_Y, TargetPassType.Y, _mesh, _origin4);
                    setCustomBuffer(CustomTarget_Z, CustomPass_Z, TargetPassType.Z, _mesh, _origin4);
                    setCustomBuffer(CustomTarget_W, CustomPass_W, TargetPassType.W, _mesh, _origin4);
                    break;
                default:
                    _origin4.Dispose();
                    return false;
            }

            ComputeBuffer _mix3 = new ComputeBuffer(mesh.vertexCount, sizeof(float) * 3);
            _mix3.SetData(Enumerable.Repeat(Vector3.zero, mesh.vertexCount).ToArray());
            ComputeBuffer _vertexBuffer3 = CalcUtil.Self.GetBuffer3(mesh.vertices);
            for (int i = 0; i < coord.Count; i++)
            {
                calcShader.SetMatrix("_WorldToLocal", data.TRS.inverse);
                calcShader.SetVector("_RelativeWorldPos", coord[i]);
                calcShader.SetBuffer(kernel_LocalMix, "_Vertex", _vertexBuffer3);
                calcShader.SetBuffer(kernel_LocalMix, "RW_Mix", _mix3);
                calcShader.Dispatch(kernel_LocalMix, Mathf.CeilToInt(mesh.vertexCount / 1024f), 1, 1);
            }

            calcShader.SetBuffer(kernel_LocalRemap, "RW_Mix", _mix3);
            calcShader.SetBuffer(kernel_LocalRemap, "_Origin", _origin4);
            calcShader.SetBuffer(kernel_LocalRemap, "RW_Result", data.Cache.RW_BrushResult);
            calcShader.Dispatch(kernel_LocalRemap, Mathf.CeilToInt((float)mesh.vertexCount / 1024), 1, 1);
            _origin4.Dispose();
            _vertexBuffer3.Dispose();
            _mix3.Dispose();
            return true;
        }

        void setCustomBuffer(CustomTargetType customType, TargetPassType inPass, TargetPassType outPass, Mesh mesh, ComputeBuffer _buffer4)
        {
            switch (customType)
            {
                case CustomTargetType.VertexColor:
                    CalcUtil.Self.GetBuffer4(CalcUtil.CheckArrayNotNull(mesh.colors, mesh.vertexCount), inPass, outPass, _buffer4);
                    break;
                case CustomTargetType.Vertex:
                    CalcUtil.Self.GetBuffer4(mesh.vertices, inPass, outPass, _buffer4);
                    break;
                case CustomTargetType.Normal:
                    CalcUtil.Self.GetBuffer4(mesh.normals, inPass, outPass, _buffer4);
                    break;
                case CustomTargetType.Tangent:
                    CalcUtil.Self.GetBuffer4(mesh.tangents, inPass, outPass, _buffer4);
                    break;
                case CustomTargetType.UV2:
                    CalcUtil.Self.GetBuffer4(CalcUtil.CheckArrayNotNull(mesh.uv2, mesh.vertexCount), inPass, outPass, _buffer4);
                    break;
                case CustomTargetType.UV3:
                    CalcUtil.Self.GetBuffer4(CalcUtil.CheckArrayNotNull(mesh.uv3, mesh.vertexCount), inPass, outPass, _buffer4);
                    break;
            }
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            if (targetTrans == null)
                return;
            EditorGUILayout.BeginHorizontal();
            if (window.ToolType == ModEditorToolType.VertexBrush)
            {
                if (checkOriginMesh())
                {
                    EditorGUILayout.LabelField("Raw mesh has been recorded.", labelStyle, GUILayout.Width(maxWidth - 55));
                    if (GUILayout.Button("Clear", GUILayout.Width(55)))
                        clearMesh();
                }
                else
                {
                    EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryWarnIconSmall"), GUILayout.Width(20));
                    EditorGUILayout.LabelField("No raw mesh was recorded.", labelStyle, GUILayout.Width(maxWidth - 75));
                    if (GUILayout.Button("Record", "EditModeSingleButton", GUILayout.Width(55)))
                        recordMesh();
                }
            }
            EditorGUILayout.EndHorizontal();

            Tab.BeginCheckWriteCommand();
            for (int i = 0; i < coord.Count; i++)
            {
                EditorGUILayout.BeginVertical("FrameBox");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Coord World Pos:", labelStyle, GUILayout.Width(120));
                if (GUILayout.Button("LookAt", "EditModeSingleButton", GUILayout.Width(60)))
                    SceneView.lastActiveSceneView.LookAt(coord[i]);
                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector3Field("", coord[i], GUILayout.Width(maxWidth));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Coord Local Pos:", Tab.CommandSyle, GUILayout.Width(maxWidth));
                EditorGUILayout.EndHorizontal();
                window.LocalRemapCoordSet(i, targetTrans.TransformPoint(EditorGUILayout.Vector3Field("", targetTrans.InverseTransformPoint(coord[i]), GUILayout.Width(maxWidth))));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30);
                if (GUILayout.Button("Reset rotation", "EditModeSingleButton" ,GUILayout.Width(maxWidth - 95)))
                    window.LocalRemapRotationSet(i, targetTrans.rotation);
                EditorGUI.BeginDisabledGroup(i == 0);
                if (GUILayout.Button("Remove", "EditModeSingleButton", GUILayout.Width(60)))
                {
                    window.LocalRemapCoordRemove(i);
                    window.LocalRemapRotationRemove(i);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Add Coord", "EditModeSingleButton", GUILayout.Width(maxWidth - 40)))
            {
                window.LocalRemapCoordSet(coord.Count, targetTrans.position);
                window.LocalRemapRotationSet(rotation.Count, targetTrans.rotation);
            }
            EditorGUILayout.EndHorizontal();
            Tab.EndCheckWriteCommand();
        }

        bool checkOriginMesh()
        {
            for (int i = 0; i < window.Tab_VertexBrush.CalcShaderDatas.Count; i++)
            {
                CalcManager data = window.Tab_VertexBrush.CalcShaderDatas[i];
                if (!originMeshDic.TryGetValue(data.trans, out Mesh mesh) || mesh == null || mesh.vertexCount != data._Vertices.count)
                {
                    clearMesh();
                    return false;
                }
            }
            return true;
        }

        void recordMesh()
        {
            originMeshDic.Clear();
            for (int i = 0; i < window.Tab_VertexBrush.CalcShaderDatas.Count; i++)
            {
                CalcManager data = window.Tab_VertexBrush.CalcShaderDatas[i];
                originMeshDic.Add(data.trans, Object.Instantiate(data.OriginMesh));
            }
        }

        void clearMesh()
        {
            if (originMeshDic.Count == 0)
                return;
            foreach (var item in originMeshDic.Values)
                Object.DestroyImmediate(item);
            originMeshDic.Clear();
        }
    }
}