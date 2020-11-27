using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTangent : DrawRendererBase
{
    public Color TangentColor = Color.green;
    [Range(0.01f, 1)]
    public float TangentLength = 0.5f;
    [Header("Binormal")]
    [Range(0, 1)]
    public float ArrowLength = 0.2f;
    [Range(0, 1)]
    public float ArrowSize = 0.1f;

    protected override string ShaderPath => "ModEditor/ShowTangent";

    protected override void RenderHandle()
    {
        material.SetColor("_TangentColor", TangentColor);
        material.SetFloat("_TangentLength", TangentLength);
        material.SetFloat("_ArrowLength", ArrowLength);
        material.SetFloat("_ArrowSize", ArrowSize);
    }
}