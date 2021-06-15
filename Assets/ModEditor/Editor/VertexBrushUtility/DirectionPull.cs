using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class DirectionPull : VertexBrushUtilBase
    {
        public override string Name => "DirectionPull";

        public override string Tip => "Stretch in the direction of the vector.";

        public override WriteTargetType UtilTarget => WriteTargetType.Vertex;

        public override bool BrushWrite(Mesh mesh, CalcManager data)
        {
            switch (TargetType)
            {
                case WriteTargetType.Vertex:
                    {
                        ComputeBuffer vertexBuffer3 = CalcUtil.Self.GetBuffer3(mesh.vertices);
                        calcShader.SetBuffer(kernel_DirectionPull_Vertex, "_Vertex", vertexBuffer3);

                        ComputeBuffer _Direction = null;
                        switch (directionType)
                        {
                            case DirectionType.Vertex:
                                _Direction = CalcUtil.Self.GetBuffer4(mesh.vertices);
                                break;
                            case DirectionType.Normal:
                                _Direction = CalcUtil.Self.GetBuffer4(mesh.normals);
                                break;
                            case DirectionType.Tangent:
                                _Direction = CalcUtil.Self.GetBuffer4(mesh.tangents);
                                break;
                            case DirectionType.VertexColor:
                                _Direction = CalcUtil.Self.GetBuffer4(mesh.colors);
                                break;
                        }
                        calcShader.SetBuffer(kernel_DirectionPull_Vertex, "_Direction", _Direction);
                        calcShader.SetBuffer(kernel_DirectionPull_Vertex, "RW_Result", data.Cache.RW_BrushResult);
                        calcShader.Dispatch(kernel_DirectionPull_Vertex, Mathf.CeilToInt((float)mesh.vertexCount / 1024), 1, 1);
                        if (calcNormal)
                            mesh.RecalculateNormals();
                        vertexBuffer3.Dispose();
                        _Direction.Dispose();
                    }
                    return true;
                default:
                    return false;
            }
        }

        ComputeShader calcShader;
        int kernel_DirectionPull_Vertex;

        DirectionType directionType { get => window.DirectionPullDirectionType; set => window.DirectionPullDirectionType = value; }
        bool calcNormal { get => window.DirectionPullCalcNormal; set => window.DirectionPullCalcNormal = value; }

        public DirectionPull()
        {
            calcShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/VertexBrushUtility/DirectionPull.compute");
            kernel_DirectionPull_Vertex = calcShader.FindKernel("DirectionPull_Vertex");
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Direction:", labelStyle, GUILayout.Width(100));
            directionType = (DirectionType)EditorGUILayout.EnumPopup(directionType, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Calc Normal:", labelStyle, GUILayout.Width(100));
            calcNormal = EditorGUILayout.Toggle(calcNormal, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();
        }

        public enum DirectionType
        { 
            Vertex,
            Normal,
            Tangent,
            VertexColor
        }
    }
}