using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ModEditor
{
    public static class EditorEvent
    {
        public static OnKey OnKey = new OnKey();

        public static OnMouse OnMouse = new OnMouse();

        public static OnScrollWheel OnScrollWheel = new OnScrollWheel();

        public static Control Control = new Control();

        public static Alt Alt = new Alt();

        public static ControlAndAlt ControlAndAlt = new ControlAndAlt();

        public static void Update(Event @event, Camera camera)
        {
            Key.UpdateControlState(@event.control);
            Key.UpdateAltState(@event.alt);
            Key.UpdateControlAndAltState(@event.control && @event.alt);
            Key.UpdateControlOrAltState(@event.control || @event.alt);
            Key.UpdateCapsLockState(@event.capsLock);
            if (@event.isKey)
            {
                if (@event.type == EventType.KeyDown)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.Tab.InvokeDown();
                            if (Use.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.Tab.InvokeDown();
                            if (Use.Control.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.Tab.InvokeDown();
                            if (Use.Alt.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.Tab.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.BackQuote.InvokeDown();
                            if (Use.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.BackQuote.InvokeDown();
                            if (Use.Control.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.BackQuote.InvokeDown();
                            if (Use.Alt.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.BackQuote.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Space)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.Space.InvokeDown();
                            if (Use.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.Space.InvokeDown();
                            if (Use.Control.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.Space.InvokeDown();
                            if (Use.Alt.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.Space.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.CapsLock)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.CapsLock.InvokeDown();
                            if (Use.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.CapsLock.InvokeDown();
                            if (Use.Control.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.CapsLock.InvokeDown();
                            if (Use.Alt.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.CapsLock.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.CapsLock.InvokeDown())
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
                            if (Use.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.Tab.InvokeUp();
                            if (Use.Control.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.Tab.InvokeUp();
                            if (Use.Alt.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.Tab.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.BackQuote.InvokeUp();
                            if (Use.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.BackQuote.InvokeUp();
                            if (Use.Control.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.BackQuote.InvokeUp();
                            if (Use.Alt.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.BackQuote.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Space)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.Space.InvokeUp();
                            if (Use.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.Space.InvokeUp();
                            if (Use.Control.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.Space.InvokeUp();
                            if (Use.Alt.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.Space.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.CapsLock)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnKey.CapsLock.InvokeUp();
                            if (Use.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnKey.CapsLock.InvokeUp();
                            if (Use.Control.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnKey.CapsLock.InvokeUp();
                            if (Use.Alt.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnKey.CapsLock.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                    }
                }
            }

            Mouse.UpdateMouseState(@event.isMouse);
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
                            if (Use.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeDownLeft();
                            if (Use.Control.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeDownLeft();
                            if (Use.Alt.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnMouse.InvokeDownLeft();
                            if (Use.ControlAndAlt.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                    }
                    if (@event.type == EventType.MouseUp)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeUpLeft();
                            if (Use.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeUpLeft();
                            if (Use.Control.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeUpLeft();
                            if (Use.Alt.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnMouse.InvokeUpLeft();
                            if (Use.ControlAndAlt.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                    }
                    if (@event.type == EventType.MouseMove)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeMove();
                            if (Use.OnMouse.InvokeMove())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeMove();
                            if (Use.Control.OnMouse.InvokeMove())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeMove();
                            if (Use.Alt.OnMouse.InvokeMove())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnMouse.InvokeMove();
                            if (Use.ControlAndAlt.OnMouse.InvokeMove())
                                @event.Use();
                        }
                    }
                    if (@event.type == EventType.MouseDrag)
                    {
                        if (!@event.control && !@event.alt)
                        {
                            OnMouse.InvokeDragLeft();
                            if (Use.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt)
                        {
                            Control.OnMouse.InvokeDragLeft();
                            if (Use.Control.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control)
                        {
                            Alt.OnMouse.InvokeDragLeft();
                            if (Use.Alt.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        if (@event.control && @event.alt)
                        {
                            ControlAndAlt.OnMouse.InvokeDragLeft();
                            if (Use.ControlAndAlt.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                    }
                }
            }

            ScrollWheel.UpdateScrollWheelState(@event.isScrollWheel);
            if (@event.isScrollWheel)
            {
                if (!@event.control && !@event.alt)
                {
                    OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.control && !@event.alt)
                {
                    Control.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.Control.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.alt && !@event.control)
                {
                    Alt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.Alt.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.control && @event.alt)
                {
                    ControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.ControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
            }
        }

        public static class Use
        {
            public static OnKey OnKey = new OnKey();

            public static OnMouse OnMouse = new OnMouse();

            public static OnScrollWheel OnScrollWheel = new OnScrollWheel();

            public static Control Control = new Control();

            public static Alt Alt = new Alt();

            public static ControlAndAlt ControlAndAlt = new ControlAndAlt();
        }
    }

    public static class Key
    {
        public static bool Control { get; private set; }
        public static bool Alt { get; private set; }
        public static bool ControlAndAlt { get; private set; }
        public static bool ControlOrAlt { get; private set; }
        public static bool CapsLock { get; private set; }
        public static event Action<bool> ControlStateChange;
        public static event Action<bool> AltStateChange;
        public static event Action<bool> ControlAndAltStateChange;
        public static event Action<bool> ControlOrAltStateChange;
        public static event Action<bool> CapsLockStateChange;
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
        public static void UpdateCapsLockState(bool res)
        {
            if (CapsLock == res)
                return;
            CapsLock = res;
            CapsLockStateChange?.Invoke(CapsLock);
        }

        public abstract class Key_Base
        {
            public event Action Down;
            public event Action Up;
            public bool InvokeDown()
            {
                if (Down == null)
                    return false;
                Down.Invoke();
                return true;
            }
            public bool InvokeUp()
            {
                if (Up == null)
                    return false;
                Up.Invoke();
                return true;
            }
        }

        public class Key_Tab : Key_Base { }

        public class Key_BackQuote : Key_Base { }

        public class Key_Space : Key_Base { }

        public class Key_CapsLock : Key_Base { }
    }

    public class OnKey
    {
        public Key.Key_Tab Tab = new Key.Key_Tab();

        public Key.Key_BackQuote BackQuote = new Key.Key_BackQuote();

        public Key.Key_Space Space = new Key.Key_Space();

        public Key.Key_CapsLock CapsLock = new Key.Key_CapsLock();
    }

    public static class Mouse
    {
        public static bool Is { get; private set; }
        public static event Action Update;
        static Vector3 screenTexcoord;
        public static Vector3 ScreenTexcoord => screenTexcoord;
        public static void UpdateMouseState(bool res)
        {
            Is = res;
            if (res)
                Update?.Invoke();
        }
        public static void UpdateMousePos(Event @event, Camera camera)
        {
            screenTexcoord = camera.ScreenToViewportPoint(@event.mousePosition);
            screenTexcoord.y = 1 - ScreenTexcoord.y;
            screenTexcoord.z = (float)Screen.width / Screen.height;
        }
    }

    public class OnMouse
    {
        public event Action DownLeft;
        public event Action UpLeft;
        public event Action Move;
        public event Action DragLeft;
        public bool InvokeDownLeft()
        {
            if (DownLeft == null)
                return false;
            DownLeft.Invoke();
            return true;
        }
        public bool InvokeUpLeft()
        {
            if (UpLeft == null)
                return false;
            UpLeft.Invoke();
            return true;
        }
        public bool InvokeMove()
        {
            if (Move == null)
                return false;
            Move.Invoke();
            return true;
        }
        public bool InvokeDragLeft()
        {
            if (DragLeft == null)
                return false;
            DragLeft.Invoke();
            return true;
        }
    }

    public static class ScrollWheel
    {
        public static bool Is { get; set; }
        public static event Action Update;
        public static void UpdateScrollWheelState(bool res)
        {
            Is = res;
            if (res)
                Update.Invoke();
        }
    }

    public class OnScrollWheel
    {
        public event Action<float> Roll;
        public bool InvokeRoll(float res)
        {
            if (Roll == null)
                return false;
            Roll.Invoke(res);
            return true;
        }
    }

    public class Control
    {
        public OnKey OnKey = new OnKey();

        public OnMouse OnMouse = new OnMouse();

        public OnScrollWheel OnScrollWheel = new OnScrollWheel();
    }

    public class Alt
    {
        public OnKey OnKey = new OnKey();

        public OnMouse OnMouse = new OnMouse();

        public OnScrollWheel OnScrollWheel = new OnScrollWheel();
    }

    public class ControlAndAlt
    {
        public OnKey OnKey = new OnKey();

        public OnMouse OnMouse = new OnMouse();

        public OnScrollWheel OnScrollWheel = new OnScrollWheel();
    }
}