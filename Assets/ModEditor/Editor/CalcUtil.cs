using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public sealed class CalcUtil
    {
        static CalcUtil self;
        public static CalcUtil Self
        {
            get 
            {
                if (self == null)
                    self = new CalcUtil();
                return self;
            }
        }

        public ComputeShader CalcVertexShader { get; private set; }
        public int kernel_SelectWithScreenScope { get; private set; }
        public int kernel_SpreadSelect { get; private set; }
        public int kernel_RespreadSelect { get; private set; }
        public int kernel_LockZone { get; private set; }
        public int kernel_AddZone { get; private set; }
        public int kernel_SubZone { get; private set; }
        public int kernel_Calc { get; private set; }

        Dictionary<Transform, CalcData.Cache> Cache;

        public CalcUtil()
        {
            CalcVertexShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/CalcViewVertex.compute");
            kernel_SelectWithScreenScope = CalcVertexShader.FindKernel("SelectWithScreenScope");
            kernel_SpreadSelect = CalcVertexShader.FindKernel("SpreadSelect");
            kernel_RespreadSelect = CalcVertexShader.FindKernel("RespreadSelect");
            kernel_LockZone = CalcVertexShader.FindKernel("LockZone");
            kernel_AddZone = CalcVertexShader.FindKernel("AddZone");
            kernel_SubZone = CalcVertexShader.FindKernel("SubZone");
            kernel_Calc = CalcVertexShader.FindKernel("Calc");
            Cache = new Dictionary<Transform, CalcData.Cache>();
        }

        public CalcData.Cache GetCache(Transform trans, int count)
        {
            if (!Cache.TryGetValue(trans, out CalcData.Cache cache))
            {
                cache = new CalcData.Cache();
                Cache.Add(trans, cache);
            }
            if (cache.RW_Zone == null || cache.RW_Zone.count != count)
            {
                if (cache.RW_Zone != null)
                    cache.RW_Zone.Dispose();
                cache.RW_Zone = new ComputeBuffer(count, sizeof(int));
                cache.RW_Zone.SetData(Enumerable.Repeat(1, count).ToArray());
            }
            if (cache.RW_Selects == null || cache.RW_Selects.count != count)
            {
                if (cache.RW_Selects != null)
                    cache.RW_Selects.Dispose();
                cache.RW_Selects = new ComputeBuffer(count, sizeof(float));
                cache.RW_Selects.SetData(Enumerable.Repeat(0, count).ToArray());
            }
            return cache;
        }

        public void ClearCache()
        {
            foreach (CalcData.Cache item in Cache.Values)
            {
                if (item.RW_Zone != null)
                    item.RW_Zone.Dispose();
                if (item.RW_Selects != null)
                    item.RW_Selects.Dispose();
            }
            Cache.Clear();
        }
    }
}