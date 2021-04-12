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

        public static Shift Shift = new Shift();

        public static ShiftAndAlt ShiftAndAlt = new ShiftAndAlt();

        public static void Update(Event @event, Camera camera)
        {
            Key.UpdateControlState(@event.control);
            Key.UpdateAltState(@event.alt);
            Key.UpdateControlAndAltState(@event.control && @event.alt);
            Key.UpdateControlOrAltState(@event.control || @event.alt);
            Key.UpdateShiftState(@event.shift);
            Key.UpdateCapsLockState(@event.capsLock);
            if (@event.isKey)
            {
                if (@event.type == EventType.KeyDown)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Tab.InvokeDown();
                            if (Use.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Tab.InvokeDown();
                            if (Use.Control.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Tab.InvokeDown();
                            if (Use.Alt.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Tab.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Tab.InvokeDown();
                            if (Use.Shift.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Tab.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.BackQuote.InvokeDown();
                            if (Use.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.BackQuote.InvokeDown();
                            if (Use.Control.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.BackQuote.InvokeDown();
                            if (Use.Alt.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.BackQuote.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.BackQuote.InvokeDown();
                            if (Use.Shift.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.BackQuote.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Space)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Space.InvokeDown();
                            if (Use.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Space.InvokeDown();
                            if (Use.Control.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Space.InvokeDown();
                            if (Use.Alt.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Space.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Space.InvokeDown();
                            if (Use.Shift.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Space.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.CapsLock)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.CapsLock.InvokeDown();
                            if (Use.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.CapsLock.InvokeDown();
                            if (Use.Control.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.CapsLock.InvokeDown();
                            if (Use.Alt.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.CapsLock.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.CapsLock.InvokeDown();
                            if (Use.Shift.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.CapsLock.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                    }
                }
                if (@event.type == EventType.KeyUp)
                {
                    if (@event.keyCode == KeyCode.Tab)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Tab.InvokeUp();
                            if (Use.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Tab.InvokeUp();
                            if (Use.Control.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Tab.InvokeUp();
                            if (Use.Alt.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Tab.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.BackQuote)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.BackQuote.InvokeUp();
                            if (Use.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.BackQuote.InvokeUp();
                            if (Use.Control.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.BackQuote.InvokeUp();
                            if (Use.Alt.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.BackQuote.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Space)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Space.InvokeUp();
                            if (Use.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Space.InvokeUp();
                            if (Use.Control.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Space.InvokeUp();
                            if (Use.Alt.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Space.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.CapsLock)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.CapsLock.InvokeUp();
                            if (Use.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.CapsLock.InvokeUp();
                            if (Use.Control.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.CapsLock.InvokeUp();
                            if (Use.Alt.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.CapsLock.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                    }
                }
            }

            Mouse.UpdateMouseState(@event.isMouse, @event.type == EventType.MouseDown || @event.type == EventType.MouseUp);
            Mouse.UpdateMousePos(@event, camera);
            if (@event.isMouse)
            {
                if (@event.type == EventType.MouseMove)
                {
                    if (!@event.control && !@event.alt && !@event.shift)
                    {
                        OnMouse.InvokeMove();
                        if (Use.OnMouse.InvokeMove())
                            @event.Use();
                    }
                    if (@event.control && !@event.alt && !@event.shift)
                    {
                        Control.OnMouse.InvokeMove();
                        if (Use.Control.OnMouse.InvokeMove())
                            @event.Use();
                    }
                    if (@event.alt && !@event.control && !@event.shift)
                    {
                        Alt.OnMouse.InvokeMove();
                        if (Use.Alt.OnMouse.InvokeMove())
                            @event.Use();
                    }
                    if (@event.control && @event.alt && !@event.shift)
                    {
                        ControlAndAlt.OnMouse.InvokeMove();
                        if (Use.ControlAndAlt.OnMouse.InvokeMove())
                            @event.Use();
                    }
                    if (!@event.control && !@event.alt && @event.shift)
                    {
                        Shift.OnMouse.InvokeMove();
                        if (Use.Shift.OnMouse.InvokeMove())
                            @event.Use();
                    }
                    if (!@event.control && @event.alt && @event.shift)
                    {
                        ShiftAndAlt.OnMouse.InvokeMove();
                        if (Use.ShiftAndAlt.OnMouse.InvokeMove())
                            @event.Use();
                    }
                }
                if (@event.type == EventType.MouseDown)
                {
                    if (!@event.control && !@event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            OnMouse.InvokeDownLeft();
                            if (Use.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            OnMouse.InvokeDownRight();
                            if (Use.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            OnMouse.InvokeDownScroll();
                            if (Use.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && !@event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Control.OnMouse.InvokeDownLeft();
                            if (Use.Control.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Control.OnMouse.InvokeDownRight();
                            if (Use.Control.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Control.OnMouse.InvokeDownScroll();
                            if (Use.Control.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                    if (@event.alt && !@event.control && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Alt.OnMouse.InvokeDownLeft();
                            if (Use.Alt.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Alt.OnMouse.InvokeDownRight();
                            if (Use.Alt.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Alt.OnMouse.InvokeDownScroll();
                            if (Use.Alt.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && @event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ControlAndAlt.OnMouse.InvokeDownLeft();
                            if (Use.ControlAndAlt.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ControlAndAlt.OnMouse.InvokeDownRight();
                            if (Use.ControlAndAlt.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ControlAndAlt.OnMouse.InvokeDownScroll();
                            if (Use.ControlAndAlt.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                    if (!@event.control && !@event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Shift.OnMouse.InvokeDownLeft();
                            if (Use.Shift.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Shift.OnMouse.InvokeDownRight();
                            if (Use.Shift.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Shift.OnMouse.InvokeDownScroll();
                            if (Use.Shift.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                    if (!@event.control && @event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndAlt.OnMouse.InvokeDownLeft();
                            if (Use.ShiftAndAlt.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndAlt.OnMouse.InvokeDownRight();
                            if (Use.ShiftAndAlt.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndAlt.OnMouse.InvokeDownScroll();
                            if (Use.ShiftAndAlt.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                }
                if (@event.type == EventType.MouseUp)
                {
                    if (!@event.control && !@event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            OnMouse.InvokeUpLeft();
                            if (Use.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            OnMouse.InvokeUpRight();
                            if (Use.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            OnMouse.InvokeUpScroll();
                            if (Use.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && !@event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Control.OnMouse.InvokeUpLeft();
                            if (Use.Control.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Control.OnMouse.InvokeUpRight();
                            if (Use.Control.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Control.OnMouse.InvokeUpScroll();
                            if (Use.Control.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                    if (@event.alt && !@event.control && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Alt.OnMouse.InvokeUpLeft();
                            if (Use.Alt.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Alt.OnMouse.InvokeUpRight();
                            if (Use.Alt.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Alt.OnMouse.InvokeUpScroll();
                            if (Use.Alt.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && @event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ControlAndAlt.OnMouse.InvokeUpLeft();
                            if (Use.ControlAndAlt.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ControlAndAlt.OnMouse.InvokeUpRight();
                            if (Use.ControlAndAlt.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ControlAndAlt.OnMouse.InvokeUpScroll();
                            if (Use.ControlAndAlt.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                    if (!@event.control && !@event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Shift.OnMouse.InvokeUpLeft();
                            if (Use.Shift.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Shift.OnMouse.InvokeUpRight();
                            if (Use.Shift.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Shift.OnMouse.InvokeUpScroll();
                            if (Use.Shift.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                    if (!@event.control && @event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndAlt.OnMouse.InvokeUpLeft();
                            if (Use.ShiftAndAlt.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndAlt.OnMouse.InvokeUpRight();
                            if (Use.ShiftAndAlt.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndAlt.OnMouse.InvokeUpScroll();
                            if (Use.ShiftAndAlt.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                }
                if (@event.type == EventType.MouseDrag)
                {
                    if (!@event.control && !@event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            OnMouse.InvokeDragLeft();
                            if (Use.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            OnMouse.InvokeDragRight();
                            if (Use.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            OnMouse.InvokeDragScroll();
                            if (Use.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && !@event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Control.OnMouse.InvokeDragLeft();
                            if (Use.Control.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Control.OnMouse.InvokeDragRight();
                            if (Use.Control.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Control.OnMouse.InvokeDragScroll();
                            if (Use.Control.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                    if (@event.alt && !@event.control && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Alt.OnMouse.InvokeDragLeft();
                            if (Use.Alt.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Alt.OnMouse.InvokeDragRight();
                            if (Use.Alt.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Alt.OnMouse.InvokeDragScroll();
                            if (Use.Alt.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && @event.alt && !@event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ControlAndAlt.OnMouse.InvokeDragLeft();
                            if (Use.ControlAndAlt.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ControlAndAlt.OnMouse.InvokeDragRight();
                            if (Use.ControlAndAlt.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ControlAndAlt.OnMouse.InvokeDragScroll();
                            if (Use.ControlAndAlt.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                    if (!@event.control && !@event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            Shift.OnMouse.InvokeDragLeft();
                            if (Use.Shift.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            Shift.OnMouse.InvokeDragRight();
                            if (Use.Shift.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            Shift.OnMouse.InvokeDragScroll();
                            if (Use.Shift.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                    if (!@event.control && @event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndAlt.OnMouse.InvokeDragLeft();
                            if (Use.ShiftAndAlt.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndAlt.OnMouse.InvokeDragRight();
                            if (Use.ShiftAndAlt.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndAlt.OnMouse.InvokeDragScroll();
                            if (Use.ShiftAndAlt.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                }
            }

            ScrollWheel.UpdateScrollWheelState(@event.isScrollWheel);
            if (@event.isScrollWheel)
            {
                if (!@event.control && !@event.alt && !@event.shift)
                {
                    OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.control && !@event.alt && !@event.shift)
                {
                    Control.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.Control.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.alt && !@event.control && !@event.shift)
                {
                    Alt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.Alt.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.control && @event.alt && !@event.shift)
                {
                    ControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.ControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (!@event.control && !@event.alt && @event.shift)
                {
                    Shift.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.Shift.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (!@event.control && @event.alt && @event.shift)
                {
                    ShiftAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.ShiftAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y))
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

            public static Shift Shift = new Shift();

            public static ShiftAndAlt ShiftAndAlt = new ShiftAndAlt();
        }
    }

    public static class Key
    {
        public static bool Control { get; private set; }
        public static bool Alt { get; private set; }
        public static bool ControlAndAlt { get; private set; }
        public static bool ControlOrAlt { get; private set; }
        public static bool Shift { get; private set; }
        public static bool CapsLock { get; private set; }
        public static event Action<bool> ControlStateChange;
        public static event Action<bool> AltStateChange;
        public static event Action<bool> ControlAndAltStateChange;
        public static event Action<bool> ControlOrAltStateChange;
        public static event Action<bool> ShiftStateChange;
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
        public static void UpdateShiftState(bool res)
        {
            if (Shift == res)
                return;
            Shift = res;
            ShiftStateChange?.Invoke(Shift);
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
        public static bool IsButton { get; private set; }
        public static event Action Update;
        static Vector3 screenTexcoord;
        public static Vector3 ScreenTexcoord => screenTexcoord;
        public static void UpdateMouseState(bool res, bool buttonRes)
        {
            Is = res;
            IsButton = res && buttonRes;
            if (res)
                Update?.Invoke();
        }
        public static void UpdateMousePos(Event @event, Camera camera)
        {
            screenTexcoord = camera.ScreenToViewportPoint(@event.mousePosition);
            screenTexcoord.y = 1 - ScreenTexcoord.y;
            screenTexcoord.z = camera.aspect;
        }
    }

    public class OnMouse
    {
        public event Action DownLeft;
        public event Action UpLeft;
        public event Action DownScroll;
        public event Action UpScroll;
        public event Action DownRight;
        public event Action UpRight;
        public event Action Move;
        public event Action DragLeft;
        public event Action DragScroll;
        public event Action DragRight;
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
        public bool InvokeDownScroll()
        {
            if (DownScroll == null)
                return false;
            DownScroll.Invoke();
            return true;
        }
        public bool InvokeUpScroll()
        {
            if (UpScroll == null)
                return false;
            UpScroll.Invoke();
            return true;
        }
        public bool InvokeDownRight()
        {
            if (DownRight == null)
                return false;
            DownRight.Invoke();
            return true;
        }
        public bool InvokeUpRight()
        {
            if (UpRight == null)
                return false;
            UpRight.Invoke();
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
        public bool InvokeDragScroll()
        {
            if (DragScroll == null)
                return false;
            DragScroll.Invoke();
            return true;
        }
        public bool InvokeDragRight()
        {
            if (DragRight == null)
                return false;
            DragRight.Invoke();
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

    public class Shift
    {
        public OnKey OnKey = new OnKey();

        public OnMouse OnMouse = new OnMouse();

        public OnScrollWheel OnScrollWheel = new OnScrollWheel();
    }

    public class ShiftAndAlt
    {
        public OnKey OnKey = new OnKey();

        public OnMouse OnMouse = new OnMouse();

        public OnScrollWheel OnScrollWheel = new OnScrollWheel();
    }
}