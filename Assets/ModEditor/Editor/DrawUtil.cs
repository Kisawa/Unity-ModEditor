using System.Collections;
using System.Collections.Generic;
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

        public ComputeShader DrawShader { get; private set; }
        public int kernel_Init { get; private set; }
        public int kernel_Merge { get; private set; }
        public int kernel_Clone { get; private set; }

        public DrawUtil()
        {
            DrawShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/DrawUtil.compute");
            kernel_Init = DrawShader.FindKernel("Init");
            kernel_Merge = DrawShader.FindKernel("Merge");
            kernel_Clone = DrawShader.FindKernel("Clone");
        }

        public Cache GetCache(Transform trans, Color baseColor)
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
                Init(baseColor, cache);
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
            }
            ModEditor.DrawUtilTransCache.Clear();
            ModEditor.DrawUtilCache.Clear();
        }

        public void Init(Color baseColor, Cache cache)
        {
            if (cache == null)
                cache = new Cache();
            if (cache.BaseTexture == null)
            {
                cache.BaseTexture = new RenderTexture(1080, 1080, 0);
                cache.BaseTexture.enableRandomWrite = true;
                cache.BaseTexture.Create();
            }
            if (cache.DrawTexture == null)
            {
                cache.DrawTexture = new RenderTexture(1080, 1080, 0);
                cache.DrawTexture.enableRandomWrite = true;
                cache.DrawTexture.Create();
            }
            if (cache.Texture == null)
            {
                cache.Texture = new RenderTexture(1080, 1080, 0);
                cache.Texture.enableRandomWrite = true;
                cache.Texture.Create();
            }
            DrawShader.SetVector("_BaseColor", baseColor);
            DrawShader.SetTexture(kernel_Init, "RW_BackgroundTexture", cache.BaseTexture);
            DrawShader.SetTexture(kernel_Init, "RW_ForegroundTexture", cache.DrawTexture);
            DrawShader.SetTexture(kernel_Init, "RW_Texture", cache.Texture);
            DrawShader.Dispatch(kernel_Init, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
        }

        public void Merge(Cache cache)
        {
            if (cache == null || !cache.IsAvailable)
                return;
            DrawShader.SetTexture(kernel_Merge, "RW_BackgroundTexture", cache.BaseTexture);
            DrawShader.SetTexture(kernel_Merge, "RW_ForegroundTexture", cache.DrawTexture);
            DrawShader.SetTexture(kernel_Merge, "RW_Texture", cache.Texture);
            DrawShader.Dispatch(kernel_Merge, Mathf.CeilToInt(cache.Texture.width / 32f), Mathf.CeilToInt(cache.Texture.height / 32f), 1);
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

        [System.Serializable]
        public class Cache
        {
            public RenderTexture BaseTexture;
            public RenderTexture DrawTexture;
            public RenderTexture Texture;

            public bool IsAvailable { get => BaseTexture != null && DrawTexture != null && Texture != null; }

            public void SetUndo()
            {
                RenderTexture baseTexture = Self.Clone(BaseTexture);
                RenderTexture drawTexture = Self.Clone(DrawTexture);
                Undo.RecordObject(ModEditor, "DrawUtil Cache Changed");
                BaseTexture = baseTexture;
                DrawTexture = drawTexture;
            }

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