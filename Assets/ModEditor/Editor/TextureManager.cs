using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class TextureManager
    {
        public Transform trans { get; private set; }
        public Renderer renderer { get; private set; }
        public DrawUtil.Cache Cache { get; private set; }

        public bool IsAvailable
        {
            get
            {
                if (trans == null || renderer == null || Cache == null || !Cache.IsAvailable)
                    return false;
                return true;
            }
        }

        public void Init(Transform trans, Color baseColor)
        {
            this.trans = trans;
            renderer = trans.GetComponent<Renderer>();
            if (renderer == null)
                return;
            Cache = DrawUtil.Self.GetCache(trans, baseColor);
            Undo.undoRedoPerformed += undoRedoPerformed;
        }

        public void ChangeBase(Color baseColor)
        {
            if (!IsAvailable)
                return;
            Cache.SetBaseTextureUndo();
            DrawUtil.Self.Init(baseColor, Cache);
        }

        public void ChangeBase(Texture tex)
        {
            if (!IsAvailable)
                return;
            Cache.SetBaseTextureUndo();
            DrawUtil.Self.Init(tex, Cache);
        }

        public void DrawStart(Color brushColor, Vector2 cursorTexcoord, Vector3 texBrushRange, float brushRotation)
        {
            if (!IsAvailable)
                return;
            Cache.SetDrawTextureUndo();
            DrawUtil.Self.Draw(Cache, brushColor, cursorTexcoord, texBrushRange, brushRotation);
        }

        public void Draw(Color brushColor, Vector2 cursorTexcoord, Vector3 texBrushRange, float brushRotation)
        {
            if (!IsAvailable)
                return;
            DrawUtil.Self.Draw(Cache, brushColor, cursorTexcoord, texBrushRange, brushRotation);
        }

        public void Clear()
        {
            trans = null;
            renderer = null;
            Cache = null;
            Undo.undoRedoPerformed -= undoRedoPerformed;
        }

        void undoRedoPerformed()
        {
            if (!IsAvailable)
                return;
            DrawUtil.Self.Merge(Cache);
        }

        public void Save(string name)
        {
            if (Cache == null || Cache.Texture == null)
                return;
            if (!AssetDatabase.IsValidFolder($"{ModEditorWindow.ModEditorPath}/Textures"))
                AssetDatabase.CreateFolder(ModEditorWindow.ModEditorPath, "Textures");
            string path = $"{ModEditorWindow.ModEditorPath}/Textures/{name}-Editing.png";

            Texture2D tex = new Texture2D(Cache.Texture.width, Cache.Texture.height, TextureFormat.ARGB32, false);
            RenderTexture pre = RenderTexture.active;
            RenderTexture.active = Cache.Texture;
            tex.ReadPixels(new Rect(0, 0, Cache.Texture.width, Cache.Texture.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter write = new BinaryWriter(file);
            write.Write(bytes);
            file.Close();

            Object.DestroyImmediate(tex);
            RenderTexture.active = pre;
        }
    }
}