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

        DataType currentData = DataType.VertexColor;

        public override Vector3[] ExecuteThree(Mesh mesh)
        {
            switch (currentData)
            {
                case DataType.Vertex:
                    return mesh.vertices;
                case DataType.Normal:
                    return mesh.normals;
            }
            return null;
        }

        public override Vector4[] ExecuteFour(Mesh mesh)
        {
            if (currentData == DataType.Tangent)
                return mesh.tangents;
            return null;
        }

        public override Color[] ExecuteColor(Mesh mesh)
        {
            if (currentData == DataType.VertexColor)
            {
                Color[] colors = mesh.colors;
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