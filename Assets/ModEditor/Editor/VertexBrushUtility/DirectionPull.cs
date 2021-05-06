using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class DirectionPull : VertexBrushUtilBase
    {
        public override string Name => "DirectionPull";

        public override string Tip => "Stretch in the direction of the vector";

        public override bool BrushWrite(Mesh mesh, CalcShaderData.CalcVertexsData data)
        {
            switch (TargetType)
            {
                case WriteTargetType.Vertex:
                    {
                        ComputeBuffer vertexBuffer4 = CalcUtil.Self.GetBuffer4(mesh.vertices);
                        calcShader.SetBuffer(kernel_DirectionPull, "_Origin", vertexBuffer4);

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
                        calcShader.SetBuffer(kernel_DirectionPull, "_Direction", _Direction);
                        calcShader.SetBuffer(kernel_DirectionPull, "RW_Result", data.RW_BrushResult);
                        calcShader.Dispatch(kernel_DirectionPull, Mathf.CeilToInt((float)data.RW_BrushResult.count / 1024), 1, 1);
                        vertexBuffer4.Dispose();
                        _Direction.Dispose();
                    }
                    return true;
                default:
                    return false;
            }
        }

        ComputeShader calcShader;
        public int kernel_DirectionPull;

        DirectionType directionType = DirectionType.Normal;

        public DirectionPull()
        {
            calcShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/VertexBrushUtility/DirectionPull.compute");
            kernel_DirectionPull = calcShader.FindKernel("DirectionPull");
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.LabelField("Direction:", labelStyle, GUILayout.Width(maxWidth));
            directionType = (DirectionType)EditorGUILayout.EnumPopup(directionType, GUILayout.Width(maxWidth));
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