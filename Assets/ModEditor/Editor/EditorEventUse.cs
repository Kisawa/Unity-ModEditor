using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public static class EditorEventUse
    {
        public static void Update(Event @event, Camera camera)
        {
            Key.UpdateAltState(@event.alt);
            Key.UpdateControlState(@event.control);
            Key.UpdateControlState(@event.control && @event.alt);
            Key.UpdateControlState(@event.control || @event.alt);
            if (@event.isKey)
            {
                if (@event.type == EventType.KeyDown)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.Tab.InvokeDown();
                            @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.BackQuote.InvokeDown();
                            @event.Use();
                        }
                    }
                }
                if (@event.type == EventType.KeyUp)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.Tab.InvokeUp();
                            @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.BackQuote.InvokeDown();
                            @event.Use();
                        }
                    }
                }
            }

            Mouse.UpdateMousePos(@event, camera);
            if (@event.isMouse)
            {
                if (@event.button == 0)
                {
                    if (@event.type == EventType.MouseDown)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeDownLeft();
                            @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeDownLeft();
                            @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeDownLeft();
                            @event.Use();
                        }
                    }
                    if (@event.type == EventType.MouseUp)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeUpLeft();
                            @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeUpLeft();
                            @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeUpLeft();
                            @event.Use();
                        }
                    }
                    if (@event.type == EventType.MouseMove)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeMove();
                            @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeMove();
                            @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeMove();
                            @event.Use();
                        }
                    }
                    if (@event.type == EventType.MouseDrag)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeDragLeft();
                            @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeDragLeft();
                            @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeDragLeft();
                            @event.Use();
                        }
                    }
                }
            }

            if (@event.isScrollWheel)
            {
                if (@event.control && !@event.alt)
                {
                    Control.OnScrollWheel.InvokeRoll(@event.delta.y);
                    @event.Use();
                }
                if (@event.alt && !@event.control)
                {
                    Alt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    @event.Use();
                }
            }
        }

        public static class Key
        {
            public static bool Control { get; private set; }
            public static bool Alt { get; private set; }
            public static bool ControlAndAlt { get; private set; }
            public static bool ControlOrAlt { get; private set; }
            public static event Action<bool> ControlStateChange;
            public static event Action<bool> AltStateChange;
            public static event Action<bool> ControlAndAltStateChange;
            public static event Action<bool> ControlOrAltStateChange;
            public static void UpdateControlState(bool res)
            {
                if (Control == res)
                    return;
                Control = res;
                ControlStateChange?.Invoke(Control);
            }
            public static void UpdateAltState(bool res)
            {
                if (Alt == res)
                    return;
                Alt = res;
                AltStateChange?.Invoke(Alt);
            }
            public static void UpdateControlAndAltState(bool res)
            {
                if (ControlAndAlt == res)
                    return;
                ControlAndAlt = res;
                ControlAndAltStateChange?.Invoke(ControlAndAlt);
            }
            public static void UpdateControlOrAltState(bool res)
            {
                if (ControlOrAlt == res)
                    return;
                ControlOrAlt = res;
                ControlOrAltStateChange?.Invoke(ControlOrAlt);
            }
        }

        public static class OnKey
        {
            public static class Tab
            {
                public static event Action Down;
                public static event Action Up;
                public static void InvokeDown()
                {
                    Down?.Invoke();
                }
                public static void InvokeUp()
                {
                    Up?.Invoke();
                }
            }

            public static class BackQuote
            {
                public static event Action Down;
                public static event Action Up;
                public static void InvokeDown()
                {
                    Down?.Invoke();
                }
                public static void InvokeUp()
                {
                    Up?.Invoke();
                }
            }
        }

        public static class Mouse
        {
            static Vector3 screenTexcoord;
            public static Vector3 ScreenTexcoord => screenTexcoord;
            public static void UpdateMousePos(Event @event, Camera camera)
            {
                screenTexcoord = camera.ScreenToViewportPoint(@event.mousePosition);
                screenTexcoord.y = 1 - ScreenTexcoord.y;
                screenTexcoord.z = (float)Screen.width / Screen.height;
            }
        }

        public static class OnMouse
        {
            public static event Action DownLeft;
            public static event Action UpLeft;
            public static event Action Move;
            public static event Action DragLeft;
            public static void InvokeDownLeft()
            {
                DownLeft?.Invoke();
            }
            public static void InvokeUpLeft()
            {
                UpLeft?.Invoke();
            }
            public static void InvokeMove()
            {
                Move?.Invoke();
            }
            public static void InvokeDragLeft()
            {
                DragLeft?.Invoke();
            }
        }

        public static class Control
        {
            public static class OnMouse
            {
                public static event Action DownLeft;
                public static event Action UpLeft;
                public static event Action Move;
                public static event Action DragLeft;
                public static void InvokeDownLeft()
                {
                    DownLeft?.Invoke();
                }
                public static void InvokeUpLeft()
                {
                    UpLeft?.Invoke();
                }
                public static void InvokeMove()
                {
                    Move?.Invoke();
                }
                public static void InvokeDragLeft()
                {
                    DragLeft?.Invoke();
                }
            }

            public static class OnScrollWheel
            {
                public static event Action<float> Roll;
                public static void InvokeRoll(float res)
                {
                    Roll?.Invoke(res);
                }
            }
        }

        public static class Alt
        {
            public static class OnMouse
            {
                public static event Action DownLeft;
                public static event Action UpLeft;
                public static event Action Move;
                public static event Action DragLeft;
                public static void InvokeDownLeft()
                {
                    DownLeft?.Invoke();
                }
                public static void InvokeUpLeft()
                {
                    UpLeft?.Invoke();
                }
                public static void InvokeMove()
                {
                    Move?.Invoke();
                }
                public static void InvokeDragLeft()
                {
                    DragLeft?.Invoke();
                }
            }

            public static class OnScrollWheel
            {
                public static event Action<float> Roll;
                public static void InvokeRoll(float res)
                {
                    Roll?.Invoke(res);
                }
            }
        }
    }
}