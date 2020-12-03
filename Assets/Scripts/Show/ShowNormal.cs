using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNormal : DrawRendererBase
{
    public Color NormalColor = Color.red;
    [Range(0.01f, 1)]
    public float NormalLength = 0.5f;

    protected override string ShaderPath => "ModEditor/ShowNormal";

    protected override void RenderHandle()
    {
        material.SetColor("_NormalColor", NormalColor);
        material.SetFloat("_NormalLength", NormalLength);
    }
}