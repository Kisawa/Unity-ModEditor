using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public partial class TextureBrushTab
    {
        public event Action onCurrentDrawBoardChanged;

        List<Transform> drawBoards;
        Transform currentDrawBoard
        {
            get => window.TextureBrushTabCurrentDrawBoard;
            set
            {
                TextureManager.Clear();
                if (value != null)
                {
                    TextureManager.Init(value, window.Manager.TextureBaseColor);
                    collider = value.GetComponent<MeshCollider>();
                }
                onCurrentDrawBoardChanged?.Invoke();
                window.TextureBrushTabCurrentDrawBoard = value;
            }
        }
        MeshCollider collider;

        Texture baseTex { get => window.TextureBrushTabBaseTex; set => window.TextureBrushTabBaseTex = value; }

        bool textureView { get => window.TextureBrushTabTextureView; set => window.TextureBrushTabTextureView = value; }
        public TextureManager TextureManager { get; } = new TextureManager();

        public bool TexRender { get; private set; }

        public bool CursorOn { get; private set; }

        public Vector2 CursorTexcoord { get; private set; }

        public Vector3 CursorPos { get; private set; }

        public Vector3 CursorNormal { get; private set; }

        public float BrushRotation { get => window.TextureBrushTabBrushRotation; set => window.TextureBrushTabBrushRotation = value; }

        void refreshDrawBoards()
        {
            if (drawBoards == null)
                drawBoards = new List<Transform>();
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
                MeshCollider collider = obj.GetComponent<MeshCollider>();
                if (collider == null || !collider.enabled || collider.sharedMesh == null)
                    continue;
                drawBoards.Add(obj.transform);
            }
            if (!drawBoards.Contains(currentDrawBoard))
                currentDrawBoard = null;
        }

        TargetTextureType utilTargetTextureType { get => window.TextureBrushTabTargetTextureType; set => window.TextureBrushTabTargetTextureType = value; }
        Texture utilCustomOriginTex
        {
            get => window.TextureBrushTabUtilCustomOriginTex;
            set
            {
                if (window.TextureBrushTabUtilCustomOriginTex == value)
                    return;
                window.TextureBrushTabUtilCustomOriginTex = value;
                window.textureBrushTabUtilCustomResultTex = null;
            }
        }
        RenderTexture utilCustomResultTex { get => window.TextureBrushTabUtilCustomResultTex; set => window.TextureBrushTabUtilCustomResultTex = value; }
        bool utilCustomTexPassView { get => window.TextureBrushTabUtilCustomTexPassView; set => window.TextureBrushTabUtilCustomTexPassView = value; }
        ColorPass utilCustomViewPass { get => window.TextureBrushTabUtilCustomTexViewPass; set => window.TextureBrushTabUtilCustomTexViewPass = value; }
        RenderTexture utilCustomViewTex { get => window.TextureBrushTabUtilCustomViewTex; set => window.TextureBrushTabUtilCustomViewTex = value; }

        void excuteUtil(TextureUtilBase util)
        {
            RenderTexture tex = null;
            switch (utilTargetTextureType)
            {
                case TargetTextureType.Background:
                    if (TextureManager.IsAvailable)
                    {
                        TextureManager.Cache.SetBaseTextureUndo();
                        tex = TextureManager.Cache.BaseTexture;
                    }
                    break;
                case TargetTextureType.Foreground:
                    if (TextureManager.IsAvailable)
                    {
                        TextureManager.Cache.SetDrawTextureUndo();
                        tex = TextureManager.Cache.DrawTexture;
                    }
                    break;
                case TargetTextureType.Custom:
                    {
                        if (utilCustomOriginTex != null)
                        {
                            if (utilCustomResultTex == null)
                                utilCustomResultTex = DrawUtil.Self.Clone(utilCustomOriginTex);
                            else
                                utilCustomResultTex = DrawUtil.Self.Clone(utilCustomResultTex);
                            tex = utilCustomResultTex;
                        }
                    }
                    break;
            }
            if (tex != null)
                util.Excute(tex);

            switch (utilTargetTextureType)
            {
                case TargetTextureType.Background:
                case TargetTextureType.Foreground:
                    if (TextureManager.IsAvailable)
                        TextureManager.Merge();
                    break;
                case TargetTextureType.Custom:
                    refreshUtilCustomViewTex();
                    break;
            }
        }

        void refreshUtilCustomViewTex()
        {
            if (utilCustomOriginTex == null || !utilCustomTexPassView)
                return;
            int viewPass = utilCustomTexPassView ? (int)utilCustomViewPass : -1;
            utilCustomViewTex = new RenderTexture(utilCustomOriginTex.width, utilCustomOriginTex.height, 0);
            utilCustomViewTex.enableRandomWrite = true;
            utilCustomViewTex.Create();
            if (utilCustomResultTex == null)
                DrawUtil.Self.RefreshView(utilCustomOriginTex, utilCustomViewTex, viewPass);
            else
                DrawUtil.Self.RefreshView(utilCustomResultTex, utilCustomViewTex, viewPass);
        }

        private void OnMouse_Move()
        {
            if (collider == null)
            {
                CursorOn = false;
                return;
            }
            CursorOn = collider.Raycast(EditorEvent.Camera.ViewportPointToRay(Mouse.ScreenTexcoord), out RaycastHit hit, float.MaxValue);
            if (CursorOn)
            {
                CursorTexcoord = hit.textureCoord;
                CursorPos = hit.point;
                CursorNormal = hit.normal;
            }
        }

        private void OnMouse_DrawStart()
        {
            if (window.OnSceneGUI)
                return;
            OnMouse_Move();
            if (!CursorOn)
                return;
            TextureManager.DrawStart(window.Manager.TextureBrushColor, CursorTexcoord, window.Manager.TexBrushRange, BrushRotation, window.Manager.ColorMask);
            Repaint();
        }

        private void OnMouse_Draw()
        {
            OnMouse_Move();
            if (!CursorOn)
                return;
            TextureManager.Draw(window.Manager.TextureBrushColor, CursorTexcoord, window.Manager.TexBrushRange, BrushRotation, window.Manager.ColorMask);
            Repaint();
        }

        private void OnMouse_DrawEnd()
        {
            TextureManager.DrawEnd();
        }

        private void Control_OnMouse_DragLeft()
        {
            if (window.ToolType != ModEditorToolType.TextureBrush)
                return;
            Vector3 texBrushRange = window.Manager.TexBrushRange;
            Vector2 delta = Event.current.delta * 0.001f;
            texBrushRange.x += delta.x;
            texBrushRange.y += delta.y;
            window.Manager.TexBrushRange = texBrushRange;
            window.SceneHandleType = SceneHandleType.BrushSize;
            Repaint();
        }

        private void Control_OnMouse_DragRight()
        {
            if (window.ToolType != ModEditorToolType.TextureBrush)
                return;
            Vector3 texBrushRange = window.Manager.TexBrushRange;
            texBrushRange.z += Event.current.delta.x * 0.005f;
            window.Manager.TexBrushRange = texBrushRange;
            window.SceneHandleType = SceneHandleType.BrushSize;
            Repaint();
        }

        private void OnScrollWheel_Roll(float obj)
        {
            BrushRotation += (float)Math.PI * 0.01f * Math.Sign(obj);
        }

        private void V_Down()
        {
            TexRender = !TexRender;
        }

        void undoRedoPerformed()
        {
            currentDrawBoard = currentDrawBoard;
        }
    }
}