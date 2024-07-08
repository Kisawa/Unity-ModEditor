using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ModEditor
{
    public class Copy : VertexCalcUtilBase
    {
        public override string Name => "Copy";

        public override string Tip => "Copy mesh data.";

        public override PassCount PassCount => currentType;

        PassCount currentType
        {
            get
            {
                switch (currentData)
                {
                    case DataType.VertexColor:
                        return PassCount.Color;
                    case DataType.Vertex:
                    case DataType.Normal:
                        return PassCount.Three;
                    case DataType.Tangent:
                        return PassCount.Four;
                }
                return PassCount.Four;
            }
        }

        DataType currentData { get => window.CopyCurrentData; set => window.CopyCurrentData = value; }

        Mesh originMesh { get => window.CopyOriginMesh; set => window.CopyOriginMesh = value; }

        public override Vector3[] ExecuteThree(Mesh mesh)
        {
            if (originMesh != null && originMesh.vertexCount != mesh.vertexCount)
                return null;
            switch (currentData)
            {
                case DataType.Vertex:
                    return originMesh == null ? mesh.vertices : originMesh.vertices;
                case DataType.Normal:
                    Debug.LogError(originMesh.vertexCount);
                    return originMesh == null ? mesh.normals : originMesh.normals;
            }
            return null;
        }

        public override Vector4[] ExecuteFour(Mesh mesh)
        {
            if (originMesh != null && originMesh.vertexCount != mesh.vertexCount)
                return null;
            if (currentData == DataType.Tangent)
                return originMesh == null ? mesh.tangents : originMesh.tangents;
            return null;
        }

        public override Color[] ExecuteColor(Mesh mesh)
        {
            if (originMesh != null && originMesh.vertexCount != mesh.vertexCount)
                return null;
            if (currentData == DataType.VertexColor)
            {
                Color[] colors = originMesh == null ? mesh.colors : originMesh.colors;
                if (colors.Length != mesh.vertexCount)
                    colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                return colors;
            }
            return null;
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Origin Mesh:", labelStyle, GUILayout.Width(100));
            originMesh = (Mesh)EditorGUILayout.ObjectField(originMesh, typeof(Mesh), false, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Data Select:", labelStyle, GUILayout.Width(100));
            currentData = (DataType)EditorGUILayout.EnumPopup(currentData, GUILayout.Width(140));
            EditorGUILayout.EndHorizontal();
        }

        public enum DataType
        {
            VertexColor,
            Vertex,
            Normal,
            Tangent
        }
    }
}