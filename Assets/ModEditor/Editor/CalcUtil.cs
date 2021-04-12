using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
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
        public int kernel_CalcVertexsWithScreenScope { get; private set; }
        public int kernel_SpreadSelectInTirangle { get; private set; }
        public int kernel_RespreadSelectInTirangle { get; private set; }
        public int kernel_LockZone { get; private set; }
        public int kernel_AddZone { get; private set; }
        public int kernel_SubZone { get; private set; }
        public int kernel_WriteVertexColorUseSelectData { get; private set; }

        Dictionary<Transform, ComputeBuffer> ZoneCache;

        public CalcUtil()
        {
            CalcVertexShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/CalcViewVertex.compute");
            kernel_CalcVertexsWithScreenScope = CalcVertexShader.FindKernel("CalcVertexsWithScreenScope");
            kernel_SpreadSelectInTirangle = CalcVertexShader.FindKernel("SpreadSelectInTirangle");
            kernel_RespreadSelectInTirangle = CalcVertexShader.FindKernel("RespreadSelectInTirangle");
            kernel_LockZone = CalcVertexShader.FindKernel("LockZone");
            kernel_AddZone = CalcVertexShader.FindKernel("AddZone");
            kernel_SubZone = CalcVertexShader.FindKernel("SubZone");
            kernel_WriteVertexColorUseSelectData = CalcVertexShader.FindKernel("WriteVertexColorUseSelectData");
            ZoneCache = new Dictionary<Transform, ComputeBuffer>();
        }

        public ComputeBuffer GetZoneCache(Transform trans, int count)
        {
            if (ZoneCache.TryGetValue(trans, out ComputeBuffer RW_Zone))
            {
                if (RW_Zone.count != count)
                {
                    RW_Zone.Dispose();
                    RW_Zone = new ComputeBuffer(count, sizeof(int));
                    RW_Zone.SetData(Enumerable.Repeat(1, count).ToArray());
                }
            }
            else
            {
                RW_Zone = new ComputeBuffer(count, sizeof(int));
                RW_Zone.SetData(Enumerable.Repeat(1, count).ToArray());
                ZoneCache.Add(trans, RW_Zone);
            }
            return RW_Zone;
        }

        public void ClearZoneCache()
        {
            foreach (ComputeBuffer item in ZoneCache.Values)
                item.Dispose();
            ZoneCache.Clear();
        }

        public static Color[] WriteVertexColor_UseSelectData(Color color, ComputeBuffer _Selects, Color[] originColors)
        {
            if (originColors.Length == 0)
                return null;
            Self.CalcVertexShader.SetVector("_Color", color);
            Self.CalcVertexShader.SetBuffer(Self.kernel_WriteVertexColorUseSelectData, "RW_Selects", _Selects);
            ComputeBuffer RW_Colors = new ComputeBuffer(originColors.Length, sizeof(float) * 4);
            RW_Colors.SetData(originColors);
            Self.CalcVertexShader.SetBuffer(Self.kernel_WriteVertexColorUseSelectData, "RW_Colors", RW_Colors);
            Self.CalcVertexShader.Dispatch(Self.kernel_WriteVertexColorUseSelectData, Mathf.CeilToInt((float)originColors.Length / 1024), 1, 1);
            Color[] colors = new Color[originColors.Length];
            RW_Colors.GetData(colors);
            RW_Colors.Dispose();
            return colors;
        }
    }
}