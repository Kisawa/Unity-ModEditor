using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class TextureManager
    {
        public RenderTexture Texture => texture;
        RenderTexture texture;
        RenderTexture baseTexture;
        RenderTexture drawTexture;

        public void Init(Color baseColor)
        {
            Destroy();
            DrawUtil.Self.Init(baseColor, out baseTexture, out drawTexture, out texture);
        }

        public void ChangeBase(Color baseColor)
        {
            DrawUtil.Self.ChangeBase(baseColor, baseTexture, drawTexture, texture);
        }

        public void Destroy()
        {
            if (baseTexture != null)
                Object.DestroyImmediate(baseTexture);
            if (drawTexture != null)
                Object.DestroyImmediate(drawTexture);
            if (texture != null)
                Object.DestroyImmediate(texture);
        }

        public void Save(string name)
        {
            if (Texture == null)
                return;
            if (!AssetDatabase.IsValidFolder($"{ModEditorWindow.ModEditorPath}/Textures"))
                AssetDatabase.CreateFolder(ModEditorWindow.ModEditorPath, "Textures");
            string path = $"{ModEditorWindow.ModEditorPath}/Textures/{name}-Editing.png";

            Texture2D tex = new Texture2D(Texture.width, Texture.height, TextureFormat.ARGB32, false);
            RenderTexture pre = RenderTexture.active;
            RenderTexture.active = Texture;
            tex.ReadPixels(new Rect(0, 0, Texture.width, Texture.height), 0, 0);

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