using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModEditor
{
    public class RecalcMesh : VertexCalcUtilBase
    {
        public override string Name => "RecalcMesh";

        public override string Tip => "Recalc normals or tangents from mesh data.";

        public override bool AllowSelect
        {
            get
            {
                switch (calcType)
                {
                    case CalcType.Bounds:
                        return false;
                }
                return true;
            }
        }

        public override PassCount PassCount
        {
            get
            {
                switch (calcType)
                {
                    case CalcType.Normals:
                        return PassCount.Three;
                    case CalcType.Tangents:
                        return PassCount.Four;
                }
                return PassCount.Other;
            }
        }
        CalcType calcType { get => window.RecalcMeshCalcType; set => window.RecalcMeshCalcType = value; }

        public override Vector3[] ExecuteThree(Mesh mesh)
        {
            Mesh meshClone = Object.Instantiate(mesh);
            meshClone.RecalculateNormals();
            return meshClone.normals;
        }

        public override Vector4[] ExecuteFour(Mesh mesh)
        {
            Mesh meshClone = Object.Instantiate(mesh);
            meshClone.RecalculateTangents();
            return meshClone.tangents;
        }

        public override void ExecuteOther(Mesh mesh)
        {
            mesh.RecalculateBounds();
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Data Select:", labelStyle, GUILayout.Width(100));
            calcType = (CalcType)EditorGUILayout.EnumPopup(calcType, GUILayout.Width(140));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryWarnIconSmall"), GUILayout.Width(20));
            switch (calcType)
            {
                case CalcType.Normals:
                    EditorGUILayout.LabelField("From the triangles and vertices.", labelStyle, GUILayout.Width(maxWidth - 30));
                    break;
                case CalcType.Tangents:
                    EditorGUILayout.LabelField("From the normals and texcoord.", labelStyle, GUILayout.Width(maxWidth - 30));
                    break;
                case CalcType.Bounds:
                    EditorGUILayout.LabelField("From the vertices.", labelStyle, GUILayout.Width(maxWidth - 30));
                    break;
            }
            EditorGUILayout.EndHorizontal();
        }

        public enum CalcType
        {
            Normals,
            Tangents,
            Bounds,
        }
    }
}