using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Color[] colors = new Color[mesh.vertexCount];
        for (int i = 0; i < colors.Length; i++)
        {
            Color color = Color.red;
            color = Color.Lerp(color, Color.blue, (float)i / colors.Length);
            colors[i] = color;
        }
        mesh.colors = colors;
    }
}