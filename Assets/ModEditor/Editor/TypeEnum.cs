﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public enum BrushType
    {
        ScreenScope
    }

    public enum WriteType
    {
        Replace,
        Add,
        Subtract,
        Multiply,
        OtherUtil
    }

    public enum WriteTargetType
    {
        None,
        VertexColor,
        Vertex,
        Normal,
        Tangent,
        Texture,
        Custom
    }

    public enum CustomTargetType
    {
        None,
        VertexColor,
        Vertex,
        Normal,
        Tangent
    }

    public enum TargetPassType
    {
        X,
        Y,
        Z,
        W
    }

    public enum PassCount
    { 
        One,
        Two,
        Three,
        Four
    }
}