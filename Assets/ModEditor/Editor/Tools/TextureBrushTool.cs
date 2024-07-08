using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ModEditor
{
    public class TextureBrushTool : SceneToolBase
    {
        bool texRender;
        public bool TexRender
        {
            get
            {
                if (ModEditor != null)
                    texRender = ModEditor.Tab_TextureBrush.TexRender;
                return texRender;
            }
        }

        bool cursorOn;
        public bool CursorOn
        {
            get
            {
                if (ModEditor != null)
                    cursorOn = ModEditor.Tab_TextureBrush.CursorOn;
                return cursorOn;
            }
        }

        Vector3 cursorPos;
        public Vector3 CursorPos
        {
            get
            {
                if (ModEditor != null)
                    cursorPos = ModEditor.Tab_TextureBrush.CursorPos;
                return cursorPos;
            }
        }

        Vector3 cursorNormal;
        public Vector3 CursorNormal
        {
            get
            {
                if (ModEditor != null)
                    cursorNormal = ModEditor.Tab_TextureBrush.CursorNormal;
                return cursorNormal;
            }
        }

        Color texBrushRangeViewColor = Color.green;
        Color TexBrushRangeViewColor
        {
            get
            {
                if (ModEditor != null)
                    texBrushRangeViewColor = ModEditor.Manager.TexBrushRangeViewColor;
                return texBrushRangeViewColor;
            }
            set
            {
                if (ModEditor == null)
                    return;
                texBrushRangeViewColor = value;
                ModEditor.Manager.TexBrushRangeViewColor = texBrushRangeViewColor;
            }
        }

        Vector2 cursorTexcoord;
        Vector2 CursorTexcoord
        {
            get
            {
                if (ModEditor != null)
                    cursorTexcoord = ModEditor.Tab_TextureBrush.CursorTexcoord;
                return cursorTexcoord;
            }
        }

        float cursorDistance;
        float CursorDistance
        {
            get
            {
                if (ModEditor != null)
                    cursorDistance = ModEditor.Tab_TextureBrush.CursorDistance;
                return cursorDistance;
            }
        }

        Vector3 texBrushRange;
        Vector3 TexBrushRange
        {
            get
            {
                if (ModEditor != null)
                    texBrushRange = ModEditor.Manager.TexBrushRange;
                return texBrushRange;
            }
            set
            {
                if (ModEditor == null || texBrushRange == value)
                    return;
                texBrushRange = value;
                ModEditor.Manager.TexBrushRange = texBrushRange;
                ModEditor.Repaint();
            }
        }

        float brushRotation = 0;
        float BrushRotation
        {
            get
            {
                if (ModEditor != null)
                    brushRotation = ModEditor.Tab_TextureBrush.BrushRotation;
                return brushRotation;
            }
            set
            {
                if (ModEditor == null)
                    return;
                brushRotation = value;
                ModEditor.Tab_TextureBrush.BrushRotation = brushRotation;
            }
        }

        public override void OnSceneGUI(EditorWindow window)
        {
            base.OnSceneGUI(window);
            if (CursorOn)
            {
                Handles.color = TexBrushRangeViewColor;
                float size = CursorDistance * 0.1f;
                Handles.DrawLine(CursorPos, CursorPos + CursorNormal * size);
            }
        }

        public override Rect Draw(EditorWindow window, GUIStyle txtStyle, GUIStyle hotKeyStyle, GUIStyle msgStyle)
        {
            Rect rect = new Rect(window.position.width - 250, window.position.height - 175, 240, 145);
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Texture Brush", "LODRenderersText");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("BrushRange View Color", txtStyle, GUILayout.Width(140));
            TexBrushRangeViewColor = EditorGUILayout.ColorField(new GUIContent(), TexBrushRangeViewColor, true, true, false, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("BurshRange Width:", txtStyle, GUILayout.Width(100));
            GUILayout.Label(TexBrushRange.x.ToString("F3"), txtStyle, GUILayout.Width(32));
            GUILayout.Label("/Ctrl-DragLeft-X", hotKeyStyle, GUILayout.Width(95));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("BurshRange Height:", txtStyle, GUILayout.Width(100));
            GUILayout.Label(TexBrushRange.y.ToString("F3"), txtStyle, GUILayout.Width(32));
            GUILayout.Label("/Ctrl-DragLeft-Y", hotKeyStyle, GUILayout.Width(95));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("BurshRange Inner:", txtStyle, GUILayout.Width(100));
            GUILayout.Label(TexBrushRange.z.ToString("F3"), txtStyle, GUILayout.Width(32));
            GUILayout.Label("/Ctrl-DragRight-X", hotKeyStyle, GUILayout.Width(95));
            EditorGUILayout.EndHorizontal();

            Vector3 range = TexBrushRange;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Align Width", "EditModeSingleButton", GUILayout.Width(105)))
                range.y = range.x;
            GUILayout.Space(12);
            if (GUILayout.Button("Align Height", "EditModeSingleButton", GUILayout.Width(105)))
                range.x = range.y;
            EditorGUILayout.EndHorizontal();
            TexBrushRange = range;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Bursh Rotation:", txtStyle, GUILayout.Width(100));
            GUILayout.Label((BrushRotation * Mathf.Rad2Deg).ToString("F1"), txtStyle, GUILayout.Width(32));
            GUILayout.Label("/Ctrl-ScrollWheel", hotKeyStyle, GUILayout.Width(95));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Reset brush rotation", "EditModeSingleButton", GUILayout.Width(225)))
                BrushRotation = 0;

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            updateTextureViewBuffer();
            return rect;
        }

        public override bool IsAvailable()
        {
            if (ModEditor.TabType == ModEditorTabType.TextureBrush && ModEditor.Tab_TextureBrush.TextureManager.IsAvailable)
                return true;
            return false;
        }

        void updateTextureViewBuffer()
        {
            if (ModEditor == null || ModEditor.camera == null || !IsAvailable())
                return;
            ModEditor.Mat_Util.SetTexture("_EditorTex", ModEditor.Tab_TextureBrush.TextureManager.Cache.Texture);
            SetTargetEditorTex();
            ModEditor.Mat_Util.SetInt("_TexRender", TexRender ? 1 : 0);
            ModEditor.Mat_Util.SetColor("_TexBrushRangeViewColor", TexBrushRangeViewColor);
            ModEditor.Mat_Util.SetInt("_CursorOn", CursorOn ? 1 : 0);
            ModEditor.Mat_Util.SetVector("_CursorTexcoord", CursorTexcoord);
            ModEditor.Mat_Util.SetVector("_TexBrushRange", TexBrushRange);
            ModEditor.Mat_Util.SetFloat("_BrushRotate", BrushRotation);
            SceneView.RepaintAll();
        }

        void SetTargetEditorTex()
        {
            if (ModEditor.Manager.Target == null)
                return;
            for (int i = 0; i < ModEditor.Manager.TargetChildren.Count; i++)
            {
                GameObject target = ModEditor.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!ModEditor.Manager.ActionableDic[target])
                    continue;
                Renderer renderer = target.GetComponent<Renderer>();
                if (renderer == null)
                    continue;
                for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                {
                    Material material = renderer.sharedMaterials[j];
                    if (material == null)
                        continue;
                    material.SetTexture("_EditorTex", ModEditor.Tab_TextureBrush.TextureManager.Cache.Texture);
                }
            }
        }
    }
}