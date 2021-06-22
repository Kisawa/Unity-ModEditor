using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class TextureManager
    {
        ModEditorWindow ModEditor => ModEditorWindow.Self;
        int viewTexPass => ModEditor.Manager.TexturePassView ? (int)ModEditor.Manager.TextureViewPass : -1;
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

        public void Init(Transform trans, int subNum, Color baseColor)
        {
            this.trans = trans;
            renderer = trans.GetComponent<Renderer>();
            if (renderer == null)
                return;
            Cache = DrawUtil.Self.GetCache(trans, subNum, baseColor, viewTexPass);
            Undo.undoRedoPerformed += Merge;
        }

        public void ChangeBase(Color baseColor)
        {
            if (!IsAvailable)
                return;
            Cache.SetBaseTextureUndo();
            DrawUtil.Self.Init(baseColor, Cache, viewTexPass);
        }

        public void ChangeBase(Texture tex)
        {
            if (!IsAvailable)
                return;
            Cache.SetBaseTextureUndo();
            DrawUtil.Self.Init(tex, Cache, viewTexPass);
        }

        public void DrawStart(Color brushColor, Vector2 cursorTexcoord, Vector3 texBrushRange, float brushRotation, Color colorMask)
        {
            if (!IsAvailable)
                return;
            Cache.SetDrawTextureUndo();
            DrawUtil.Self.DrawStart(Cache, brushColor, cursorTexcoord, texBrushRange, brushRotation, colorMask, viewTexPass);
        }

        public void Draw(Color brushColor, Vector2 cursorTexcoord, Vector3 texBrushRange, float brushRotation, Color colorMask)
        {
            if (!IsAvailable)
                return;
            DrawUtil.Self.Draw(Cache, brushColor, cursorTexcoord, texBrushRange, brushRotation, colorMask, viewTexPass);
        }

        public void DrawEnd()
        {
            DrawUtil.Self.DrawEnd();
        }

        public void Merge()
        {
            if (!IsAvailable)
                return;
            DrawUtil.Self.Merge(Cache, viewTexPass);
        }

        public void RefreshView()
        {
            if (!IsAvailable || viewTexPass < 0)
                return;
            DrawUtil.Self.RefreshView(Cache, viewTexPass);
        }

        public void ClearDraw()
        {
            if (!IsAvailable)
                return;
            Cache.SetDrawTextureUndo();
            DrawUtil.Self.Write(Cache.DrawTexture, Color.clear);
            Merge();
        }

        public void Clear()
        {
            trans = null;
            renderer = null;
            Cache = null;
            Undo.undoRedoPerformed -= Merge;
        }

        public void Save(string name)
        {
            if (Cache == null || Cache.Texture == null)
                return;
            DrawUtil.Self.Save(Cache.Texture, name);
        }
    }
}