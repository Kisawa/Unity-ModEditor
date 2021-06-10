using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ModEditor
{
    public static class EditorEvent
    {
        public static Camera Camera { get; private set; }

        public static OnKey OnKey { get; } = new OnKey();

        public static OnMouse OnMouse { get; } = new OnMouse();

        public static OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();

        public static Control Control { get; } = new Control();

        public static Alt Alt { get; } = new Alt();

        public static ControlAndAlt ControlAndAlt { get; } = new ControlAndAlt();

        public static Shift Shift { get; } = new Shift();

        public static ShiftAndAlt ShiftAndAlt { get; } = new ShiftAndAlt();

        public static ShiftAndControl ShiftAndControl { get; } = new ShiftAndControl();

        public static ShiftAndControlAndAlt ShiftAndControlAndAlt { get; } = new ShiftAndControlAndAlt();

        public static void Update(Event @event, Camera camera)
        {
            Camera = camera;
            Key.UpdateControlState(@event.control);
            Key.UpdateAltState(@event.alt);
            Key.UpdateControlAndAltState(@event.control && @event.alt);
            Key.UpdateControlOrAltState(@event.control || @event.alt);
            Key.UpdateShiftState(@event.shift);
            Key.UpdateShiftAndControlState(@event.shift && @event.control);
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
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Tab.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.Tab.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Tab.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.Tab.InvokeDown())
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
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.BackQuote.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.BackQuote.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.BackQuote.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.BackQuote.InvokeDown())
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
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Space.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.Space.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Space.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.Space.InvokeDown())
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
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.CapsLock.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.CapsLock.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.CapsLock.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.V)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.V.InvokeDown();
                            if (Use.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.V.InvokeDown();
                            if (Use.Control.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.V.InvokeDown();
                            if (Use.Alt.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.V.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.V.InvokeDown();
                            if (Use.Shift.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.V.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.V.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.V.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.V.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Q)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Q.InvokeDown();
                            if (Use.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Q.InvokeDown();
                            if (Use.Control.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Q.InvokeDown();
                            if (Use.Alt.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Q.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Q.InvokeDown();
                            if (Use.Shift.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Q.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Q.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Q.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.Q.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.E)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.E.InvokeDown();
                            if (Use.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.E.InvokeDown();
                            if (Use.Control.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.E.InvokeDown();
                            if (Use.Alt.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.E.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.E.InvokeDown();
                            if (Use.Shift.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.E.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.E.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.E.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.E.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.A)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.A.InvokeDown();
                            if (Use.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.A.InvokeDown();
                            if (Use.Control.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.A.InvokeDown();
                            if (Use.Alt.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.A.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.A.InvokeDown();
                            if (Use.Shift.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.A.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.A.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.A.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.A.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.D)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.D.InvokeDown();
                            if (Use.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.D.InvokeDown();
                            if (Use.Control.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.D.InvokeDown();
                            if (Use.Alt.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.D.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.D.InvokeDown();
                            if (Use.Shift.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.D.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.D.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.D.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.D.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Z)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Z.InvokeDown();
                            if (Use.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Z.InvokeDown();
                            if (Use.Control.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Z.InvokeDown();
                            if (Use.Alt.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Z.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Z.InvokeDown();
                            if (Use.Shift.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Z.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Z.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Z.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.Z.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.C)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.C.InvokeDown();
                            if (Use.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.C.InvokeDown();
                            if (Use.Control.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.C.InvokeDown();
                            if (Use.Alt.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.C.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.C.InvokeDown();
                            if (Use.Shift.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.C.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.C.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.C.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.C.InvokeDown())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Return)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Enter.InvokeDown();
                            if (Use.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Enter.InvokeDown();
                            if (Use.Control.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Enter.InvokeDown();
                            if (Use.Alt.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Enter.InvokeDown();
                            if (Use.ControlAndAlt.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Enter.InvokeDown();
                            if (Use.Shift.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Enter.InvokeDown();
                            if (Use.ShiftAndAlt.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Enter.InvokeDown();
                            if (Use.ShiftAndControl.OnKey.Enter.InvokeDown())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Enter.InvokeDown();
                            if (Use.ShiftAndControlAndAlt.OnKey.Enter.InvokeDown())
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
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Tab.InvokeUp();
                            if (Use.Shift.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Tab.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Tab.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.Tab.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Tab.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.Tab.InvokeUp())
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
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.BackQuote.InvokeUp();
                            if (Use.Shift.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.BackQuote.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.BackQuote.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.BackQuote.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.BackQuote.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.BackQuote.InvokeUp())
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
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Space.InvokeUp();
                            if (Use.Shift.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Space.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Space.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.Space.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Space.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.Space.InvokeUp())
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
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.CapsLock.InvokeUp();
                            if (Use.Shift.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.CapsLock.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.CapsLock.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.CapsLock.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.CapsLock.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.V)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.V.InvokeUp();
                            if (Use.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.V.InvokeUp();
                            if (Use.Control.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.V.InvokeUp();
                            if (Use.Alt.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.V.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.V.InvokeUp();
                            if (Use.Shift.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.V.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.V.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.V.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.V.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Q)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Q.InvokeUp();
                            if (Use.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Q.InvokeUp();
                            if (Use.Control.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Q.InvokeUp();
                            if (Use.Alt.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Q.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Q.InvokeUp();
                            if (Use.Shift.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Q.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Q.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Q.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.Q.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.E)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.E.InvokeUp();
                            if (Use.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.E.InvokeUp();
                            if (Use.Control.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.E.InvokeUp();
                            if (Use.Alt.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.E.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.E.InvokeUp();
                            if (Use.Shift.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.E.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.E.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.E.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.E.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.A)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.A.InvokeUp();
                            if (Use.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.A.InvokeUp();
                            if (Use.Control.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.A.InvokeUp();
                            if (Use.Alt.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.A.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.A.InvokeUp();
                            if (Use.Shift.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.A.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.A.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.A.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.A.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.D)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.D.InvokeUp();
                            if (Use.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.D.InvokeUp();
                            if (Use.Control.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.D.InvokeUp();
                            if (Use.Alt.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.D.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.D.InvokeUp();
                            if (Use.Shift.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.D.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.D.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.D.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.D.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Z)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Z.InvokeUp();
                            if (Use.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Z.InvokeUp();
                            if (Use.Control.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Z.InvokeUp();
                            if (Use.Alt.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Z.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Z.InvokeUp();
                            if (Use.Shift.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Z.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Z.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Z.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.Z.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.C)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.C.InvokeUp();
                            if (Use.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.C.InvokeUp();
                            if (Use.Control.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.C.InvokeUp();
                            if (Use.Alt.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.C.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.C.InvokeUp();
                            if (Use.Shift.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.C.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.C.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.C.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.C.InvokeUp())
                                @event.Use();
                        }
                    }
                    if (@event.keyCode == KeyCode.Return)
                    {
                        if (!@event.control && !@event.alt && !@event.shift)
                        {
                            OnKey.Enter.InvokeUp();
                            if (Use.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && !@event.shift)
                        {
                            Control.OnKey.Enter.InvokeUp();
                            if (Use.Control.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (@event.alt && !@event.control && !@event.shift)
                        {
                            Alt.OnKey.Enter.InvokeUp();
                            if (Use.Alt.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && !@event.shift)
                        {
                            ControlAndAlt.OnKey.Enter.InvokeUp();
                            if (Use.ControlAndAlt.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && !@event.alt && @event.shift)
                        {
                            Shift.OnKey.Enter.InvokeUp();
                            if (Use.Shift.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (!@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndAlt.OnKey.Enter.InvokeUp();
                            if (Use.ShiftAndAlt.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && !@event.alt && @event.shift)
                        {
                            ShiftAndControl.OnKey.Enter.InvokeUp();
                            if (Use.ShiftAndControl.OnKey.Enter.InvokeUp())
                                @event.Use();
                        }
                        if (@event.control && @event.alt && @event.shift)
                        {
                            ShiftAndControlAndAlt.OnKey.Enter.InvokeUp();
                            if (Use.ShiftAndControlAndAlt.OnKey.Enter.InvokeUp())
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
                    if (@event.control && !@event.alt && @event.shift)
                    {
                        ShiftAndControl.OnMouse.InvokeMove();
                        if (Use.ShiftAndControl.OnMouse.InvokeMove())
                            @event.Use();
                    }
                    if (@event.control && @event.alt && @event.shift)
                    {
                        ShiftAndControlAndAlt.OnMouse.InvokeMove();
                        if (Use.ShiftAndControlAndAlt.OnMouse.InvokeMove())
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
                    if (@event.control && !@event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndControl.OnMouse.InvokeDownLeft();
                            if (Use.ShiftAndControl.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndControl.OnMouse.InvokeDownRight();
                            if (Use.ShiftAndControl.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndControl.OnMouse.InvokeDownScroll();
                            if (Use.ShiftAndControl.OnMouse.InvokeDownScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && @event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeDownLeft();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeDownLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeDownRight();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeDownRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeDownScroll();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeDownScroll())
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
                    if (@event.control && !@event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndControl.OnMouse.InvokeUpLeft();
                            if (Use.ShiftAndControl.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndControl.OnMouse.InvokeUpRight();
                            if (Use.ShiftAndControl.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndControl.OnMouse.InvokeUpScroll();
                            if (Use.ShiftAndControl.OnMouse.InvokeUpScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && @event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeUpLeft();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeUpLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeUpRight();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeUpRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeUpScroll();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeUpScroll())
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
                    if (@event.control && !@event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndControl.OnMouse.InvokeDragLeft();
                            if (Use.ShiftAndControl.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndControl.OnMouse.InvokeDragRight();
                            if (Use.ShiftAndControl.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndControl.OnMouse.InvokeDragScroll();
                            if (Use.ShiftAndControl.OnMouse.InvokeDragScroll())
                                @event.Use();
                        }
                    }
                    if (@event.control && @event.alt && @event.shift)
                    {
                        if (@event.button == 0)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeDragLeft();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeDragLeft())
                                @event.Use();
                        }
                        else if (@event.button == 1)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeDragRight();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeDragRight())
                                @event.Use();
                        }
                        else if (@event.button == 2)
                        {
                            ShiftAndControlAndAlt.OnMouse.InvokeDragScroll();
                            if (Use.ShiftAndControlAndAlt.OnMouse.InvokeDragScroll())
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
                if (@event.control && !@event.alt && @event.shift)
                {
                    ShiftAndControl.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.ShiftAndControl.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
                if (@event.control && @event.alt && @event.shift)
                {
                    ShiftAndControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y);
                    if (Use.ShiftAndControlAndAlt.OnScrollWheel.InvokeRoll(@event.delta.y))
                        @event.Use();
                }
            }
        }

        public static class Use
        {
            public static OnKey OnKey { get; } = new OnKey();

            public static OnMouse OnMouse { get; } = new OnMouse();

            public static OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();

            public static Control Control { get; } = new Control();

            public static Alt Alt { get; } = new Alt();

            public static ControlAndAlt ControlAndAlt { get; } = new ControlAndAlt();

            public static Shift Shift { get; } = new Shift();

            public static ShiftAndAlt ShiftAndAlt { get; } = new ShiftAndAlt();

            public static ShiftAndControl ShiftAndControl { get; } = new ShiftAndControl();

            public static ShiftAndControlAndAlt ShiftAndControlAndAlt { get; } = new ShiftAndControlAndAlt();
        }
    }

    public static class Key
    {
        public static bool Control { get; private set; }
        public static bool Alt { get; private set; }
        public static bool ControlAndAlt { get; private set; }
        public static bool ControlOrAlt { get; private set; }
        public static bool Shift { get; private set; }
        public static bool ShiftAndControl { get; private set; }
        public static bool CapsLock { get; private set; }
        public static event Action<bool> ControlStateChange;
        public static event Action<bool> AltStateChange;
        public static event Action<bool> ControlAndAltStateChange;
        public static event Action<bool> ControlOrAltStateChange;
        public static event Action<bool> ShiftStateChange;
        public static event Action<bool> ShiftAndControlStateChange;
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
        public static void UpdateShiftAndControlState(bool res)
        {
            if (ShiftAndControl == res)
                return;
            ShiftAndControl = res;
            ShiftAndControlStateChange?.Invoke(ShiftAndControl);
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

        public class Key_V : Key_Base { }

        public class Key_Q : Key_Base { }

        public class Key_E : Key_Base { }

        public class Key_A : Key_Base { }

        public class Key_D : Key_Base { }

        public class Key_Z : Key_Base { }

        public class Key_C : Key_Base { }

        public class Key_Enter : Key_Base { }
    }

    public class OnKey
    {
        public Key.Key_Tab Tab { get; } = new Key.Key_Tab();

        public Key.Key_BackQuote BackQuote { get; } = new Key.Key_BackQuote();

        public Key.Key_Space Space { get; } = new Key.Key_Space();

        public Key.Key_CapsLock CapsLock { get; } = new Key.Key_CapsLock();

        public Key.Key_V V { get; } = new Key.Key_V();

        public Key.Key_Q Q { get; } = new Key.Key_Q();

        public Key.Key_E E { get; } = new Key.Key_E();

        public Key.Key_A A { get; } = new Key.Key_A();

        public Key.Key_D D { get; } = new Key.Key_D();

        public Key.Key_Z Z { get; } = new Key.Key_Z();

        public Key.Key_C C { get; } = new Key.Key_C();

        public Key.Key_Enter Enter { get; } = new Key.Key_Enter();
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
        public static bool Is { get; private set; }
        public static event Action Update;
        public static void UpdateScrollWheelState(bool res)
        {
            Is = res;
            if (res)
                Update?.Invoke();
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
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }

    public class Alt
    {
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }

    public class ControlAndAlt
    {
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }

    public class Shift
    {
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }

    public class ShiftAndAlt
    {
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }

    public class ShiftAndControl
    {
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }

    public class ShiftAndControlAndAlt
    {
        public OnKey OnKey { get; } = new OnKey();

        public OnMouse OnMouse { get; } = new OnMouse();

        public OnScrollWheel OnScrollWheel { get; } = new OnScrollWheel();
    }
}