using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class ExtraMaterialBase : MonoBehaviour
{
    protected abstract string ShaderPath { get; }

    [SerializeField]
    bool includeChildren;
    public bool IncludeChildren
    {
        get => includeChildren;
        set
        {
            includeChildren = value;
            if (!enabled)
                return;
            if (includeChildren)
            {
                MeshRenderer[] childrenRenderer = GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < childrenRenderer.Length; i++)
                    show(childrenRenderer[i]);
            }
            else
            {
                MeshRenderer[] childrenRenderer = GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < childrenRenderer.Length; i++)
                    hide(childrenRenderer[i]);
                show(GetComponent<MeshRenderer>());
            }
        }
    }

    protected Material material;

    bool currentIncludeChildren;

    protected virtual void Awake()
    {
        ClearMaterial();
        if (material == null)
        {
            material = new Material(Shader.Find(ShaderPath));
            material.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    protected virtual void OnEnable()
    {
        if (IncludeChildren)
        {
            MeshRenderer[] childrenRenderer = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < childrenRenderer.Length; i++)
                show(childrenRenderer[i]);
        }
        else
        {
            show(GetComponent<MeshRenderer>());
        }
    }

    protected virtual void OnDisable()
    {
        MeshRenderer[] childrenRenderer = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childrenRenderer.Length; i++)
            hide(childrenRenderer[i]);
    }

    protected virtual void Update()
    {
        if (currentIncludeChildren != IncludeChildren)
        {
            IncludeChildren = includeChildren;
            currentIncludeChildren = includeChildren;
        }
    }

    void show(MeshRenderer _renderer)
    {
        if (_renderer == null)
            return;
        List<Material> materials = new List<Material>();
        _renderer.GetSharedMaterials(materials);
        if (!materials.Contains(material))
            materials.Add(material);
        _renderer.sharedMaterials = materials.ToArray();
    }

    void hide(MeshRenderer _renderer)
    {
        if (_renderer == null)
            return;
        List<Material> materials = new List<Material>();
        _renderer.GetSharedMaterials(materials);
        materials.Remove(material);
        _renderer.sharedMaterials = materials.ToArray();
    }

    void ClearMaterial()
    {
        MeshRenderer[] childrenRenderer = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childrenRenderer.Length; i++)
        {
            List<Material> materials = new List<Material>();
            childrenRenderer[i].GetSharedMaterials(materials);
            for (int j = 0; j < materials.Count; j++)
            {
                if (materials[j].shader == Shader.Find(ShaderPath))
                    materials.RemoveAt(j);
            }
            childrenRenderer[i].sharedMaterials = materials.ToArray();
        }
    }
}