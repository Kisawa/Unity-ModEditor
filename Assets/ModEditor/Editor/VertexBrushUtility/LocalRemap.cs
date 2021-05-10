using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModEditor
{
    public class LocalRemap : VertexBrushUtilBase
    {
        public override string Name => "LocalRemap";

        public override string Tip => "Calc the vertex local position relative to the object.";

        Transform targetTrans;
        bool available;
        Quaternion rotation { get => window.LocalRemapRotation; set => window.LocalRemapRotation = value; }
        Vector3 coord { get => window.LocalRemapCoord; set => window.LocalRemapCoord = value; }

        ComputeShader calcShader;
        int kernel_LocalRemap;

        public LocalRemap()
        {
            calcShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/VertexBrushUtility/LocalRemap.compute");
            kernel_LocalRemap = calcShader.FindKernel("LocalRemap");
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
            window.onTargetChanged += Window_onTargetChanged;
            window.onSceneValidate -= Window_onSceneValidate;
        }

        private void Window_onTargetChanged(GameObject obj)
        {
            available = obj != null;
            if (available)
            {
                targetTrans = window.Manager.Target.transform;
                window.localRemapRotation = targetTrans.rotation;
                window.localRemapCoord = targetTrans.position;
            }
            else
                targetTrans = null;
        }

        private void Window_onSceneValidate(SceneView obj)
        {
            if (!available || !window.VertexView)
                return;
            rotation = Handles.RotationHandle(rotation, coord);
            coord = Handles.PositionHandle(coord, rotation);
            if (GUI.changed)
                window.Repaint();
        }

        public override bool BrushWrite(Mesh mesh, CalcShaderData.CalcVertexsData data)
        {
            if (!available)
                return false;
            calcShader.SetMatrix("_LocalToWorld", data.trans.localToWorldMatrix);
            calcShader.SetVector("_RelativeWorldPos", coord);
            ComputeBuffer _vertexBuffer3 = CalcUtil.Self.GetBuffer3(mesh.vertices);
            calcShader.SetBuffer(kernel_LocalRemap, "_Vertex", _vertexBuffer3);
            calcShader.SetBuffer(kernel_LocalRemap, "RW_Result", data.Cache.RW_BrushResult);
            calcShader.Dispatch(kernel_LocalRemap, Mathf.CeilToInt((float)mesh.vertexCount / 1024), 1, 1);
            _vertexBuffer3.Dispose();
            return true;
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            if (targetTrans == null)
                return;
            EditorGUILayout.LabelField("Coord World Pos:", labelStyle, GUILayout.Width(maxWidth));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector3Field("", coord, GUILayout.Width(maxWidth));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.LabelField("Coord Local Pos:", labelStyle, GUILayout.Width(maxWidth));
            coord = targetTrans.TransformPoint(EditorGUILayout.Vector3Field("", targetTrans.InverseTransformPoint(coord), GUILayout.Width(maxWidth)));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (GUILayout.Button("Reset rotation relative to the object.", GUILayout.Width(maxWidth - 30)))
                rotation = targetTrans.rotation;
            EditorGUILayout.EndHorizontal();
        }
    }
}