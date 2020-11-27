using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public abstract class DrawRendererBase : MonoBehaviour
{
    protected abstract string ShaderPath { get; }

    [SerializeField]
    CameraEvent cameraEvent = CameraEvent.AfterForwardOpaque;

    protected Material material;
    protected new Renderer renderer;

    CameraEvent currentCameraEvent = CameraEvent.AfterForwardOpaque;
    Dictionary<Camera, CommandBuffer> cameras;

    protected virtual void Awake()
    {
        material = new Material(Shader.Find(ShaderPath));
        material.hideFlags = HideFlags.HideAndDontSave;
    }

    protected virtual void OnDisable()
    {
        if (cameras == null)
            return;
        foreach (var item in cameras)
            item.Value.Clear();
    }

    protected virtual void OnDestroy()
    {
        removeBuffer();
    }

    void removeBuffer()
    {
        if (cameras != null)
        {
            foreach (var item in cameras)
                item.Key.RemoveCommandBuffer(currentCameraEvent, item.Value);
            cameras.Clear();
        }
        currentCameraEvent = cameraEvent;
    }

    private void OnWillRenderObject()
    {
        if (cameras == null)
            cameras = new Dictionary<Camera, CommandBuffer>();

        if (currentCameraEvent != cameraEvent)
            removeBuffer();

        if (renderer == null)
            renderer = GetComponent<Renderer>();

        Camera cam = Camera.current;
        if (cameras.TryGetValue(cam, out CommandBuffer buffer))
        {
            buffer.Clear();
        }
        else
        {
            buffer = new CommandBuffer();
            buffer.name = $"{GetType().Name} - {transform.name}";
            cam.AddCommandBuffer(cameraEvent, buffer);
            cameras.Add(cam, buffer);
        }

        RenderHandle();
        buffer.DrawRenderer(renderer, material);
    }

    protected abstract void RenderHandle();
}