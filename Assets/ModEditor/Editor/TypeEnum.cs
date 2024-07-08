using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public enum UVType
    { 
        UV,
        UV2,
        UV3
    }

    public enum VertexBrushType
    {
        Color,
        TwoColorGradient,
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
        UV2,
        UV3,
        Custom
    }

    public enum CustomTargetType
    {
        None,
        VertexColor,
        Vertex,
        Normal,
        Tangent,
        UV2,
        UV3
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
        Four,
        Color,
        Other
    }

    public enum TargetTextureType
    { 
        Background,
        Foreground,
        Custom
    }

    public enum TexViewPass
    { 
        R,
        G,
        B,
        A,
        Gray
    }

    public enum BlendType
    {
        Add,
        Sub,
        RevSub,
        Min,
        Max
    }

    public enum BlendFactor
    { 
        One,
        Zero,
        BlendTexColor,
        BlendTexAlpha,
        OriginTexColor,
        OriginTexAlpha,
        OneMinusBlendTexColor,
        OneMinusBlendTexAlpha,
        OneMinusOriginTexColor,
        OneMinusOriginTexAlpha
    }
}