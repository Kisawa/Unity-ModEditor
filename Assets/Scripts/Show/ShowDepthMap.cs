using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDepthMap : DrawRendererBase
{
    [Range(0.001f, 1)]
    public float DepthCompress = 0.1f;

    protected override string ShaderPath => "ModEditor/ShowDepthMap";

    protected override void RenderHandle()
    {
        material.SetFloat("_DepthCompress", DepthCompress);
    }
}