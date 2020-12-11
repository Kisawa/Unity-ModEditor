using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowVertex : DrawRendererBase
{
    public Color VertexColor = Color.black;
    [Range(0.001f, 1)]
    public float VertexScale = 0.5f;

    protected override string ShaderPath => "ModEditor/ShowVertex";

    protected override void RenderHandle()
    {
        material.SetColor("_VertexColor", VertexColor);
        material.SetFloat("_VertexScale", VertexScale);
    }
}