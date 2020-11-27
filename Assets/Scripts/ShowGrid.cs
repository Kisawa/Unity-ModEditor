using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGrid : DrawRendererBase
{
    public Color GridColor = Color.white;

    protected override string ShaderPath => "ModEditor/ShowGrid";

    protected override void RenderHandle()
    {
        material.SetColor("_GridColor", GridColor);
    }
}