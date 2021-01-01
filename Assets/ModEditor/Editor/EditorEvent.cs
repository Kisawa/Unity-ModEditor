using System;
using System.Collections;
using System.Collections.Generic;
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
            if (@event.isKey)
            {
                if (@event.type == EventType.KeyDown)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt)
                            OnKey.Tab.InvokeDown();
                        if (@event.control && !@event.alt)
                            Control.OnKey.Tab.InvokeDown();
                        if (@event.alt && !@event.control)
                            Alt.OnKey.Tab.InvokeDown();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnKey.Tab.InvokeDown();
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt)
                            OnKey.BackQuote.InvokeDown();
                        if (@event.control && !@event.alt)
                            Control.OnKey.BackQuote.InvokeDown();
                        if (@event.alt && !@event.control)
                            Alt.OnKey.BackQuote.InvokeDown();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnKey.BackQuote.InvokeDown();
                    }
                }
                if (@event.type == EventType.KeyUp)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt)
                            OnKey.Tab.InvokeUp();
                        if (@event.control && !@event.alt)
                            Control.OnKey.Tab.InvokeUp();
                        if (@event.alt && !@event.control)
                            Alt.OnKey.Tab.InvokeUp();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnKey.Tab.InvokeUp();
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt)
                            OnKey.BackQuote.InvokeUp();
                        if (@event.control && !@event.alt)
                            Control.OnKey.BackQuote.InvokeUp();
                        if (@event.alt && !@event.control)
                            Alt.OnKey.BackQuote.InvokeUp();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnKey.BackQuote.InvokeUp();
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
                            OnMouse.InvokeDownLeft();
                        if (@event.control && !@event.alt)
                            Control.OnMouse.InvokeDownLeft();
                        if (@event.alt && !@event.control)
                            Alt.OnMouse.InvokeDownLeft();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnMouse.InvokeDownLeft();
                    }
                    if (@event.type == EventType.MouseUp)
                    {
                        if (!@event.control && !@event.alt)
                            OnMouse.InvokeUpLeft();
                        if (@event.control && !@event.alt)
                            Control.OnMouse.InvokeUpLeft();
                        if (@event.alt && !@event.control)
                            Alt.OnMouse.InvokeUpLeft();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnMouse.InvokeUpLeft();
                    }
                    if (@event.type == EventType.MouseMove)
                    {
                        if (!@event.control && !@event.alt)
                            OnMouse.InvokeMove();
                        if (@event.control && !@event.alt)
                            Control.OnMouse.InvokeMove();
                        if (@event.alt && !@event.control)
                            Alt.OnMouse.InvokeMove();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnMouse.InvokeMove();
                    }
                    if (@event.type == EventType.MouseDrag)
                    {
                        if (!@event.control && !@event.alt)
                            OnMouse.InvokeDragLeft();
                        if (@event.control && !@event.alt)
                            Control.OnMouse.InvokeDragLeft();
                        if (@event.alt && !@event.control)
                            Alt.OnMouse.InvokeDragLeft();
                        if (@event.control && @event.alt)
                            ControlAndAlt.OnMouse.InvokeDragLeft();
                    }
                }
            }

            if (@event.isScrollWheel)
            {
                if (!@event.control && !@event.alt)
                    OnScrollWheel.InvokeRoll(@event.delta.y);
                if (@event.control && !@event.alt)
                    Control.OnScrollWheel.InvokeRoll(@event.delta.y);
                if (@event.alt && !@event.control)
                    Alt.OnScrollWheel.InvokeRoll(@event.delta.y);
                if (@event.control && @event.alt)
                    ControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y);
            }

            Use.Update(@event);
        }

        public static class Use
        {
            public static OnKey OnKey = new OnKey();

            public static OnMouse OnMouse = new OnMouse();

            public static OnScrollWheel OnScrollWheel = new OnScrollWheel();

            public static Control Control = new Control();

            public static Alt Alt = new Alt();

            public static ControlAndAlt ControlAndAlt = new ControlAndAlt();

            public static void Update(Event @event)
            {
                if (@event.isKey)
                {
                    if (@event.type == EventType.KeyDown)
                    {
                        if (@event.keyCode == KeyCode.Tab)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnKey.Tab.InvokeDown())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnKey.Tab.InvokeDown())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnKey.Tab.InvokeDown())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnKey.Tab.InvokeDown())
                                    @event.Use();
                            }
                        }
                        if (@event.keyCode == KeyCode.BackQuote)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnKey.BackQuote.InvokeDown())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnKey.BackQuote.InvokeDown())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnKey.BackQuote.InvokeDown())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnKey.BackQuote.InvokeDown())
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
                                if (OnKey.Tab.InvokeUp())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnKey.Tab.InvokeUp())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnKey.Tab.InvokeUp())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnKey.Tab.InvokeUp())
                                    @event.Use();
                            }
                        }
                        if (@event.keyCode == KeyCode.BackQuote)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnKey.BackQuote.InvokeUp())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnKey.BackQuote.InvokeUp())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnKey.BackQuote.InvokeUp())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnKey.BackQuote.InvokeUp())
                                    @event.Use();
                            }
                        }
                    }
                }

                if (@event.isMouse)
                {
                    if (@event.button == 0)
                    {
                        if (@event.type == EventType.MouseDown)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnMouse.InvokeDownLeft())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnMouse.InvokeDownLeft())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnMouse.InvokeDownLeft())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnMouse.InvokeDownLeft())
                                    @event.Use();
                            }
                        }
                        if (@event.type == EventType.MouseUp)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnMouse.InvokeUpLeft())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnMouse.InvokeUpLeft())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnMouse.InvokeUpLeft())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnMouse.InvokeUpLeft())
                                    @event.Use();
                            }
                        }
                        if (@event.type == EventType.MouseMove)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnMouse.InvokeMove())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnMouse.InvokeMove())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnMouse.InvokeMove())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnMouse.InvokeMove())
                                    @event.Use();
                            }
                        }
                        if (@event.type == EventType.MouseDrag)
                        {
                            if (!@event.control && !@event.alt)
                            {
                                if (OnMouse.InvokeDragLeft())
                                    @event.Use();
                            }
                            if (@event.control && !@event.alt)
                            {
                                if (Control.OnMouse.InvokeDragLeft())
                                    @event.Use();
                            }
                            if (@event.alt && !@event.control)
                            {
                                if (Alt.OnMouse.InvokeDragLeft())
                                    @event.Use();
                            }
                            if (@event.control && @event.alt)
                            {
                                if (ControlAndAlt.OnMouse.InvokeDragLeft())
                                    @event.Use();
                            }
                        }
                    }
                }

                if (@event.isScrollWheel)
                {
                    if (!@event.control && !@event.alt)
                    {
                        if (OnScrollWheel.InvokeRoll(@event.delta.y))
                            @event.Use();
                    }
                    if (@event.control && !@event.alt)
                    {
                        if (Control.OnScrollWheel.InvokeRoll(@event.delta.y))
                            @event.Use();
                    }
                    if (@event.alt && !@event.control)
                    {
                        if (Alt.OnScrollWheel.InvokeRoll(@event.delta.y))
                            @event.Use();
                    }
                    if (@event.control && @event.alt)
                    {
                        if (ControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y))
                            @event.Use();
                    }
                }
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

        public class Tab
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

        public class BackQuote
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
    }

    public class OnKey
    {
        public Key.Tab Tab = new Key.Tab();

        public Key.BackQuote BackQuote = new Key.BackQuote();
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