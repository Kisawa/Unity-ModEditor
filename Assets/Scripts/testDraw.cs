using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class testDraw : MonoBehaviour
{
    public Material material;
    ComputeBuffer vertexs;
    //ComputeBuffer matrices_m;
    int count;
    Dictionary<Camera, CommandBuffer> cameras;

    private void Awake()
    {
        cameras = new Dictionary<Camera, CommandBuffer>();
    }

    private void OnEnable()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        count = mesh.vertexCount;
        vertexs = new ComputeBuffer(count, sizeof(float) * 3);
        vertexs.SetData(mesh.vertices);
        material.SetBuffer("_Vertexs", vertexs);
        //matrices_m = new ComputeBuffer(count, sizeof(float) * 16, ComputeBufferType.Default);
        //matrices_m.SetData(Enumerable.Repeat(transform.localToWorldMatrix, count).ToArray());
        //material.SetBuffer("_M", matrices_m);
    }

    private void OnRenderObject()
    {
        Camera cam = Camera.current;
        if (cameras.ContainsKey(cam))
            cameras[cam].Clear();
        else
        {
            CommandBuffer buffer = new CommandBuffer();
            cam.AddCommandBuffer(CameraEvent.AfterForwardAlpha, buffer);
            cameras.Add(cam, buffer);
        }
        //matrices_m.SetData(Enumerable.Repeat(transform.localToWorldMatrix, count).ToArray());
        cameras[cam].DrawProcedural(transform.localToWorldMatrix, material, 0, MeshTopology.Lines, count);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("asd"))
        {
            if (vertexs != null)
            {
                vertexs.Release();
                vertexs = null;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var item in cameras)
        {
            if(item.Key != null)
                item.Key.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, item.Value);
        }
        cameras.Clear();
        if (vertexs != null)
        {
            vertexs.Release();
            vertexs = null;
        }
        //if (matrices_m != null)
        //{
        //    matrices_m.Release();
        //    matrices_m = null;
        //}
    }
}
