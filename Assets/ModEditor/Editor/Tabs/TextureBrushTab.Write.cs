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
        List<int> drawBoardSubCount;
        List<bool> drawBoardSubUnfold;
        Transform currentDrawBoard
        {
            get => window.TextureBrushTabCurrentDrawBoard;
            set
            {
                TextureManager.Clear();
                collider = null;
                if (value != null)
                {
                    TextureManager.Init(value, subNum, window.Manager.TextureBaseColor);
                    collider = value.GetComponent<MeshCollider>();
                }
                onCurrentDrawBoardChanged?.Invoke();
                window.TextureBrushTabCurrentDrawBoard = value;
            }
        }
        MeshCollider collider;
        int subNum 
        {
            get => window.TextureBrushTabCurrentDrawBoardSubNum;
            set
            {
                if (window.TextureBrushTabCurrentDrawBoardSubNum == value)
                    return;
                window.TextureBrushTabCurrentDrawBoardSubNum = value;
                onCurrentDrawBoardChanged?.Invoke();
            }
        }

        Texture baseTex { get => window.TextureBrushTabBaseTex; set => window.TextureBrushTabBaseTex = value; }

        bool textureView { get => window.TextureBrushTabTextureView; set => window.TextureBrushTabTextureView = value; }
        public TextureManager TextureManager { get; } = new TextureManager();

        public bool TexRender { get; private set; }

        public bool CursorOn { get; private set; }

        public Vector2 CursorTexcoord { get; private set; }

        public Vector3 CursorPos { get; private set; }

        public Vector3 CursorNormal { get; private set; }

        public float CursorDistance { get; private set; }

        public float BrushRotation { get => window.TextureBrushTabBrushRotation; set => window.TextureBrushTabBrushRotation = value; }

        void refreshDrawBoards()
        {
            if (drawBoards == null)
                drawBoards = new List<Transform>();
            else
                drawBoards.Clear();
            if (drawBoardSubCount == null)
                drawBoardSubCount = new List<int>();
            else
                drawBoardSubCount.Clear();
            if (drawBoardSubUnfold == null)
                drawBoardSubUnfold = new List<bool>();
            else
                drawBoardSubUnfold.Clear();
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
                drawBoardSubCount.Add(collider.sharedMesh.subMeshCount);
                drawBoardSubUnfold.Add(true);
            }
            if (!drawBoards.Contains(currentDrawBoard))
            {
                subNum = 0;
                currentDrawBoard = null;
            }
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
                utilCustomViewTexPanel.BindTexture(value);
            }
        }
        RenderTexture utilCustomResultTex { get => window.TextureBrushTabUtilCustomResultTex; set => window.TextureBrushTabUtilCustomResultTex = value; }

        TexturePanel utilCustomViewTexPanel { get => window.TextureBrushTabuUtilCustomViewTexPanel; set => window.TextureBrushTabuUtilCustomViewTexPanel = value; }

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
                        if (utilCustomResultTex == null)
                        {
                            if (utilCustomOriginTex == null)
                            {
                                utilCustomResultTex = new RenderTexture(window.Manager.TexBrushTexelSize.x, window.Manager.TexBrushTexelSize.y, 0);
                                utilCustomResultTex.enableRandomWrite = true;
                                utilCustomResultTex.Create();
                            }
                            else
                                utilCustomResultTex = DrawUtil.Self.Clone(utilCustomOriginTex);
                        }
                        else
                            utilCustomResultTex = DrawUtil.Self.Clone(utilCustomResultTex);
                        tex = utilCustomResultTex;
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
                    utilCustomViewTexPanel.BindTexture(utilCustomResultTex);
                    break;
            }
        }

        private void OnMouse_Move()
        {
            if (collider == null)
            {
                CursorOn = false;
                return;
            }
            bool hitRes = collider.Raycast(EditorEvent.Camera.ViewportPointToRay(Mouse.ScreenTexcoord), out RaycastHit hit, float.MaxValue);
            if (hitRes)
            {
                var subMesh = collider.sharedMesh.GetSubMesh(subNum);
                int index = (hit.triangleIndex + 1) * 3;
                if (index >= subMesh.indexStart && index <= subMesh.indexStart + subMesh.indexCount)
                {
                    CursorOn = true;
                    CursorTexcoord = hit.textureCoord;
                    CursorPos = hit.point;
                    CursorNormal = hit.normal;
                    CursorDistance = hit.distance;
                    return;
                }
            }
            CursorOn = false;
        }

        private void OnScrollWheel_Move(float obj)
        {
            OnMouse_Move();
        }

        bool startDraw;
        private void OnMouse_DrawStart()
        {
            if (window.OnSceneGUI)
                return;
            OnMouse_Move();
            if (!CursorOn)
                return;
            TextureManager.DrawStart(window.Manager.TextureBrushColor, CursorTexcoord, window.Manager.TexBrushRange, BrushRotation, window.Manager.ColorMask);
            startDraw = true;
            Repaint();
        }

        private void OnMouse_Draw()
        {
            OnMouse_Move();
            if (!CursorOn)
                return;
            if (startDraw)
                TextureManager.Draw(window.Manager.TextureBrushColor, CursorTexcoord, window.Manager.TexBrushRange, BrushRotation, window.Manager.ColorMask);
            else
            {
                TextureManager.DrawStart(window.Manager.TextureBrushColor, CursorTexcoord, window.Manager.TexBrushRange, BrushRotation, window.Manager.ColorMask);
                startDraw = true;
            }
            Repaint();
        }

        private void OnMouse_DrawEnd()
        {
            TextureManager.DrawEnd();
            startDraw = false;
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
            if (utilCustomResultTex == null)
                utilCustomViewTexPanel.BindTexture(utilCustomOriginTex);
            else
                utilCustomViewTexPanel.BindTexture(utilCustomResultTex);
        }
    }
}