﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public sealed class DrawUtil
    {
        static ModEditorWindow ModEditor => ModEditorWindow.Self;

        static DrawUtil self;
        public static DrawUtil Self
        {
            get
            {
                if (self == null)
                    self = new DrawUtil();
                return self;
            }
        }

        readonly static Vector2 TexelSize = Vector2.one * 1024;

        public ComputeShader DrawShader { get; private set; }
        public int kernel_Init { get; private set; }
        public int kernel_InitTex { get; private set; }
        public int kernel_Merge { get; private set; }
        public int kernel_RefreshView { get; private set; }
        public int kernel_Clone { get; private set; }
        public int kernel_Write { get; private set; }
        public int kernel_Draw { get; private set; }

        public DrawUtil()
        {
            DrawShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/DrawUtil.compute");
            kernel_Init = DrawShader.FindKernel("Init");
            kernel_InitTex = DrawShader.FindKernel("InitTex");
            kernel_Merge = DrawShader.FindKernel("Merge");
            kernel_RefreshView = DrawShader.FindKernel("RefreshView");
            kernel_Clone = DrawShader.FindKernel("Clone");
            kernel_Write = DrawShader.FindKernel("Write");
            kernel_Draw = DrawShader.FindKernel("Draw");
        }

        public Cache GetCache(Transform trans, Color baseColor, int viewPass = -1)
        {
            if (ModEditor == null)
                return null;
            Cache cache;
            int index = ModEditor.DrawUtilTransCache.IndexOf(trans);
            if (index == -1)
            {
                ModEditor.DrawUtilTransCache.Add(trans);
                cache = new Cache();
                ModEditor.DrawUtilCache.Add(cache);
            }
            else
                cache = ModEditor.DrawUtilCache[index];
            if (!cache.IsAvailable)
                Init(baseColor, cache, viewPass);
            return cache;
        }

        public void ClearCache()
        {
            if (ModEditor == null)
                return;
            for (int i = 0; i < ModEditor.DrawUtilCache.Count; i++)
            {
                Cache cache = ModEditor.DrawUtilCache[i];
                if (cache.BaseTexture != null)
                    Object.DestroyImmediate(cache.BaseTexture);
                if (cache.DrawTexture != null)
                    Object.DestroyImmediate(cache.DrawTexture);
                if (cache.Texture != null)
                    Object.DestroyImmediate(cache.Texture);
                if (cache.ViewTexture != null)
                    Object.DestroyImmediate(cache.ViewTexture);
            }
            ModEditor.DrawUtilTransCache.Clear();
            ModEditor.DrawUtilCache.Clear();
        }

        public void Init(Color baseColor, Cache cache, int viewPass = -1)
        {
            if (cache == null)
                cache = new Cache();
            if (cache.BaseTexture == null)
            {
                cache.BaseTexture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.BaseTexture.enableRandomWrite = true;
                cache.BaseTexture.Create();
            }
            if (cache.DrawTexture == null)
            {
                cache.DrawTexture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.DrawTexture.enableRandomWrite = true;
                cache.DrawTexture.Create();
            }
            if (cache.Texture == null)
            {
                cache.Texture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.Texture.enableRandomWrite = true;
                cache.Texture.Create();
            }
            if (cache.ViewTexture == null)
            {
                cache.ViewTexture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.ViewTexture.enableRandomWrite = true;
                cache.ViewTexture.Create();
            }
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetVector("_BaseColor", baseColor);
            DrawShader.SetTexture(kernel_Init, "RW_BackgroundTexture", cache.BaseTexture);
            DrawShader.SetTexture(kernel_Init, "RW_ForegroundTexture", cache.DrawTexture);
            DrawShader.SetTexture(kernel_Init, "RW_Texture", cache.Texture);
            DrawShader.SetTexture(kernel_Init, "RW_ViewTexture", cache.ViewTexture);
            DrawShader.Dispatch(kernel_Init, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
        }

        public void Init(Texture tex, Cache cache, int viewPass = -1)
        {
            RenderTexture _tex = RenderTexture.GetTemporary((int)TexelSize.x, (int)TexelSize.y, 0);
            _tex.enableRandomWrite = true;
            _tex.Create();
            Graphics.Blit(tex, _tex);
            if (cache == null)
                cache = new Cache();
            if (cache.BaseTexture == null)
            {
                cache.BaseTexture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.BaseTexture.enableRandomWrite = true;
                cache.BaseTexture.Create();
            }
            if (cache.DrawTexture == null)
            {
                cache.DrawTexture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.DrawTexture.enableRandomWrite = true;
                cache.DrawTexture.Create();
            }
            if (cache.Texture == null)
            {
                cache.Texture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.Texture.enableRandomWrite = true;
                cache.Texture.Create();
            }
            if (cache.ViewTexture == null)
            {
                cache.ViewTexture = new RenderTexture((int)TexelSize.x, (int)TexelSize.y, 0);
                cache.ViewTexture.enableRandomWrite = true;
                cache.ViewTexture.Create();
            }
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetVector("_BaseTexTexelSize", new Vector2(_tex.width, _tex.height));
            DrawShader.SetTexture(kernel_InitTex, "_BaseTexture", _tex);
            DrawShader.SetTexture(kernel_InitTex, "RW_BackgroundTexture", cache.BaseTexture);
            DrawShader.SetTexture(kernel_InitTex, "RW_ForegroundTexture", cache.DrawTexture);
            DrawShader.SetTexture(kernel_InitTex, "RW_Texture", cache.Texture);
            DrawShader.SetTexture(kernel_InitTex, "RW_ViewTexture", cache.ViewTexture);
            DrawShader.Dispatch(kernel_InitTex, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
            RenderTexture.ReleaseTemporary(_tex);
        }

        public void Merge(Cache cache, int viewPass = -1)
        {
            if (cache == null || !cache.IsAvailable)
                return;
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetTexture(kernel_Merge, "RW_BackgroundTexture", cache.BaseTexture);
            DrawShader.SetTexture(kernel_Merge, "RW_ForegroundTexture", cache.DrawTexture);
            DrawShader.SetTexture(kernel_Merge, "RW_Texture", cache.Texture);
            DrawShader.SetTexture(kernel_Merge, "RW_ViewTexture", cache.ViewTexture);
            DrawShader.Dispatch(kernel_Merge, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
        }

        public void RefreshView(Cache cache, int viewPass = -1)
        {
            if (cache == null || !cache.IsAvailable)
                return;
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetTexture(kernel_RefreshView, "RW_Texture", cache.Texture);
            DrawShader.SetTexture(kernel_RefreshView, "RW_ViewTexture", cache.ViewTexture);
            DrawShader.Dispatch(kernel_RefreshView, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
        }

        public void RefreshView(Texture texture, RenderTexture viewTexture, int viewPass = -1)
        {
            if (texture == null || viewTexture == null)
                return;
            RenderTexture _tex = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            _tex.enableRandomWrite = true;
            _tex.Create();
            Graphics.Blit(texture, _tex);
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetTexture(kernel_RefreshView, "RW_Texture", _tex);
            DrawShader.SetTexture(kernel_RefreshView, "RW_ViewTexture", viewTexture);
            DrawShader.Dispatch(kernel_RefreshView, Mathf.CeilToInt(viewTexture.width / 32f), Mathf.CeilToInt(viewTexture.height / 32f), 1);
        }

        public void RefreshView(RenderTexture texture, RenderTexture viewTexture, int viewPass = -1)
        {
            if (texture == null || viewTexture == null)
                return;
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetTexture(kernel_RefreshView, "RW_Texture", texture);
            DrawShader.SetTexture(kernel_RefreshView, "RW_ViewTexture", viewTexture);
            DrawShader.Dispatch(kernel_RefreshView, Mathf.CeilToInt(viewTexture.width / 32f), Mathf.CeilToInt(viewTexture.height / 32f), 1);
        }

        public void DownSample(RenderTexture texture, int downSample)
        {
            if (downSample <= 1)
                return;
            RenderTexture tex = RenderTexture.GetTemporary(texture.width / downSample, texture.height / downSample, texture.depth, texture.format);
            Graphics.Blit(texture, tex);
            Graphics.Blit(tex, texture);
        }

        public RenderTexture Clone(RenderTexture texture)
        {
            if (texture == null)
                return null;
            RenderTexture clone = new RenderTexture(texture.descriptor);
            clone.enableRandomWrite = true;
            clone.Create();
            DrawShader.SetTexture(kernel_Clone, "RW_ForegroundTexture", texture);
            DrawShader.SetTexture(kernel_Clone, "RW_Texture", clone);
            DrawShader.Dispatch(kernel_Clone, Mathf.CeilToInt(clone.width / 32f), Mathf.CeilToInt(clone.height / 32f), 1);
            return clone;
        }

        public RenderTexture Clone(Texture texture)
        {
            if (texture == null)
                return null;
            RenderTexture _tex = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            _tex.enableRandomWrite = true;
            _tex.Create();
            Graphics.Blit(texture, _tex);
            RenderTexture clone = new RenderTexture(texture.width, texture.height, 0);
            clone.enableRandomWrite = true;
            clone.Create();
            DrawShader.SetTexture(kernel_Clone, "RW_ForegroundTexture", _tex);
            DrawShader.SetTexture(kernel_Clone, "RW_Texture", clone);
            DrawShader.Dispatch(kernel_Clone, Mathf.CeilToInt(clone.width / 32f), Mathf.CeilToInt(clone.height / 32f), 1);
            RenderTexture.ReleaseTemporary(_tex);
            return clone;
        }

        public void Write(RenderTexture texture, Color color)
        {
            if (texture == null)
                return;
            DrawShader.SetVector("_Color", color);
            DrawShader.SetTexture(kernel_Write, "RW_Texture", texture);
            DrawShader.Dispatch(kernel_Write, Mathf.CeilToInt(texture.width / 32f), Mathf.CeilToInt(texture.height / 32f), 1);
        }

        public void Write(RenderTexture texture, RenderTexture origin)
        {
            if (texture == null || origin == null)
                return;
            DrawShader.SetTexture(kernel_Clone, "RW_ForegroundTexture", origin);
            DrawShader.SetTexture(kernel_Clone, "RW_Texture", texture);
            DrawShader.Dispatch(kernel_Clone, Mathf.CeilToInt(texture.width / 32f), Mathf.CeilToInt(texture.height / 32f), 1);
        }

        public void Write(RenderTexture texture, Texture origin)
        {
            if (texture == null || origin == null)
                return;
            RenderTexture _tex = RenderTexture.GetTemporary(origin.width, origin.height, 0);
            _tex.enableRandomWrite = true;
            _tex.Create();
            Graphics.Blit(origin, _tex);
            DrawShader.SetTexture(kernel_Clone, "RW_ForegroundTexture", _tex);
            DrawShader.SetTexture(kernel_Clone, "RW_Texture", texture);
            DrawShader.Dispatch(kernel_Clone, Mathf.CeilToInt(texture.width / 32f), Mathf.CeilToInt(texture.height / 32f), 1);
            RenderTexture.ReleaseTemporary(_tex);
        }

        public void DrawStart(Cache cache, Color brushColor, Vector2 cursorTexcoord, Vector3 texBrushRange, float brushRotation, Color colorMask, int viewPass = -1)
        {
            if (cache == null || !cache.IsAvailable)
                return;
            Draw(cache, brushColor, cursorTexcoord, texBrushRange, brushRotation, colorMask, viewPass);
        }

        public void Draw(Cache cache, Color brushColor, Vector2 cursorTexcoord, Vector3 texBrushRange, float brushRotation, Color colorMask, int viewPass = -1)
        {
            if (cache == null || !cache.IsAvailable)
                return;
            DrawShader.SetVector("_TexelSize", TexelSize);
            DrawShader.SetVector("_BrushColor", brushColor);
            DrawShader.SetVector("_CursorTexcoord", cursorTexcoord);
            DrawShader.SetVector("_TexBrushRange", texBrushRange);
            DrawShader.SetVector("_TexBrushRange", texBrushRange);
            DrawShader.SetFloat("_BrushRotate", brushRotation);
            DrawShader.SetVector("_ColorMask", colorMask);
            DrawShader.SetInt("_ViewType", viewPass);
            DrawShader.SetTexture(kernel_Draw, "RW_BackgroundTexture", cache.BaseTexture);
            DrawShader.SetTexture(kernel_Draw, "RW_ForegroundTexture", cache.DrawTexture);
            DrawShader.SetTexture(kernel_Draw, "RW_Texture", cache.Texture);
            DrawShader.SetTexture(kernel_Draw, "RW_ViewTexture", cache.ViewTexture);
            DrawShader.Dispatch(kernel_Draw, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
        }

        public void DrawEnd()
        {

        }

        public void Save(RenderTexture texture, string name)
        {
            if (texture == null)
                return;
            if (!AssetDatabase.IsValidFolder($"{ModEditorWindow.ModEditorPath}/Textures"))
                AssetDatabase.CreateFolder(ModEditorWindow.ModEditorPath, "Textures");
            string path = $"{ModEditorWindow.ModEditorPath}/Textures/{name}-Editing.png";

            Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture pre = RenderTexture.active;
            RenderTexture.active = texture;
            tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter write = new BinaryWriter(file);
            write.Write(bytes);
            file.Close();

            Object.DestroyImmediate(tex);
            RenderTexture.active = pre;
            AssetDatabase.ImportAsset(path);
        }

        [System.Serializable]
        public class Cache
        {
            public RenderTexture BaseTexture;
            public RenderTexture DrawTexture;
            public RenderTexture Texture;
            public RenderTexture ViewTexture;

            public bool IsAvailable { get => BaseTexture != null && DrawTexture != null && Texture != null && ViewTexture != null; }

            public void SetBaseTextureUndo()
            {
                RenderTexture texture = Self.Clone(BaseTexture);
                Undo.RecordObject(ModEditor, "DrawUtil Cache Changed");
                BaseTexture = texture;
            }

            public void SetDrawTextureUndo()
            {
                RenderTexture texture = Self.Clone(DrawTexture);
                Undo.RecordObject(ModEditor, "DrawUtil Cache Changed");
                DrawTexture = texture;
            }
        }
    }
}