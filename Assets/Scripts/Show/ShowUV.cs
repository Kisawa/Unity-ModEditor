using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUV : DrawRendererBase
{
    [Range(0, 1)]
    public float UVAlpha = 1;

    protected override string ShaderPath => "ModEditor/ShowUV";

    protected override void RenderHandle()
    {
        material.SetFloat("_UVAlpha", UVAlpha);
    }
}