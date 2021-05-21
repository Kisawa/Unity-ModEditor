using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class TextureBrushTab : WindowTabBase
    {
        new ModEditorWindow window;

        public TextureBrushTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        List<GameObject> drawBoards;
        bool textureView { get => window.TextureBrushTabTextureView; set => window.TextureBrushTabTextureView = value; }
        RenderTexture texture;

        public override void OnFocus()
        {
            base.OnFocus();
            window.onRefreshTargetDic += refreshDrawBoards;
            refreshDrawBoards();
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onRefreshTargetDic -= refreshDrawBoards;
        }

        public override void OnDiable()
        {
            base.OnDiable();
            if (texture != null)
                Object.DestroyImmediate(texture);
        }

        public override void Draw()
        {
            GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
            EditorGUILayout.BeginVertical("flow background");
            if (drawBoards == null || drawBoards.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryWarnIconSmall"), GUILayout.Width(20));
                EditorGUILayout.LabelField("No Drawing Board", labelStyle);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < drawBoards.Count; i++)
                {
                    GameObject obj = drawBoards[i];
                    EditorGUILayout.BeginHorizontal();
                    bool boardSwitch = window.TextureBrushTabBoardSwitchsCheck(obj);
                    if (GUILayout.Button(boardSwitch ? window.toggleOnContent : window.toggleContent, "ObjectPickerTab"))
                        window.TextureBrushTabBoardSwitchsSet(obj, !boardSwitch);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(textureView ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                textureView = !textureView;
            texture = (RenderTexture)EditorGUILayout.ObjectField(texture, typeof(RenderTexture), false);
            if (GUILayout.Button("New", GUILayout.Width(40)))
            {
                newTexture();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save"))
                saveTexture();

            if(texture != null && textureView)
                EditorGUI.DrawPreviewTexture(new Rect(0, window.position.height - window.position.width, window.position.width, window.position.width), texture);
        }

        void refreshDrawBoards()
        {
            if (drawBoards == null)
                drawBoards = new List<GameObject>();
            else
                drawBoards.Clear();
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject obj = window.Manager.TargetChildren[i];
                if (!window.Manager.ActionableDic[obj])
                    continue;
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer == null)
                    continue;
                Collider collider = obj.GetComponent<Collider>();
                if (collider == null)
                    continue;
                drawBoards.Add(obj);
                window.TextureBrushTabBoardSwitchsCheck(obj);
            }
        }

        void newTexture()
        {
            if (texture != null)
                Object.DestroyImmediate(texture);
            RenderTexture tex = DrawUtil.Self.Init();
            texture = tex;
        }

        void saveTexture()
        {
            if (texture == null)
                return;
            if (!AssetDatabase.IsValidFolder($"{ModEditorWindow.ModEditorPath}/Textures"))
                AssetDatabase.CreateFolder(ModEditorWindow.ModEditorPath, "Textures");
            string path = $"{ModEditorWindow.ModEditorPath}/Textures/{window.Manager.Target.name}-Editing.png";

            Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture pre = RenderTexture.active;
            RenderTexture.active = texture;
            tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter write = new BinaryWriter(file);
            write.Write(bytes);
            file.Close();

            //tex.Apply();
            //AssetDatabase.CreateAsset(tex, path);
            //AssetDatabase.ImportAsset(path);

            Object.DestroyImmediate(tex);
            RenderTexture.active = pre;
        }
    }
}