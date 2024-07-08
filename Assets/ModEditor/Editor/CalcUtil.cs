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

        public ComputeShader CalcShader { get; private set; }
        public int kernel_SelectWithScreenScope { get; private set; }
        public int kernel_SpreadSelect { get; private set; }
        public int kernel_RespreadSelect { get; private set; }
        public int kernel_LockZone { get; private set; }
        public int kernel_AddZone { get; private set; }
        public int kernel_SubZone { get; private set; }
        public int kernel_CalcWithSize { get; private set; }
        public int kernel_CalcWithSpread { get; private set; }
        public int kernel_MappedTo01 { get; private set; }
        public int kernel_Result1 { get; private set; }
        public int kernel_Result2 { get; private set; }
        public int kernel_Result3 { get; private set; }
        public int kernel_Result4 { get; private set; }
        public int kernel_Origin2To1 { get; private set; }
        public int kernel_Origin3To1 { get; private set; }
        public int kernel_Origin4To1 { get; private set; }
        public int kernel_Origin4To2 { get; private set; }
        public int kernel_Origin4To3 { get; private set; }
        public int kernel_Origin1To2 { get; private set; }
        public int kernel_Origin1To3 { get; private set; }
        public int kernel_Origin1To4 { get; private set; }
        public int kernel_Origin2To4 { get; private set; }
        public int kernel_Origin3To4 { get; private set; }

        Dictionary<Transform, Cache> ManagerCache;

        public CalcUtil()
        {
            CalcShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/CalcUtil.compute");
            kernel_SelectWithScreenScope = CalcShader.FindKernel("SelectWithScreenScope");
            kernel_SpreadSelect = CalcShader.FindKernel("SpreadSelect");
            kernel_RespreadSelect = CalcShader.FindKernel("RespreadSelect");
            kernel_LockZone = CalcShader.FindKernel("LockZone");
            kernel_AddZone = CalcShader.FindKernel("AddZone");
            kernel_SubZone = CalcShader.FindKernel("SubZone");
            kernel_CalcWithSize = CalcShader.FindKernel("CalcWithSize");
            kernel_CalcWithSpread = CalcShader.FindKernel("CalcWithSpread");
            kernel_MappedTo01 = CalcShader.FindKernel("MappedTo01");
            kernel_Result1 = CalcShader.FindKernel("Result1");
            kernel_Result2 = CalcShader.FindKernel("Result2");
            kernel_Result3 = CalcShader.FindKernel("Result3");
            kernel_Result4 = CalcShader.FindKernel("Result4");
            kernel_Origin2To1 = CalcShader.FindKernel("Origin2To1");
            kernel_Origin3To1 = CalcShader.FindKernel("Origin3To1");
            kernel_Origin4To1 = CalcShader.FindKernel("Origin4To1");
            kernel_Origin4To2 = CalcShader.FindKernel("Origin4To2");
            kernel_Origin4To3 = CalcShader.FindKernel("Origin4To3");
            kernel_Origin1To3 = CalcShader.FindKernel("Origin1To3");
            kernel_Origin1To2 = CalcShader.FindKernel("Origin1To2");
            kernel_Origin1To4 = CalcShader.FindKernel("Origin1To4");
            kernel_Origin2To4 = CalcShader.FindKernel("Origin2To4");
            kernel_Origin3To4 = CalcShader.FindKernel("Origin3To4");
            ManagerCache = new Dictionary<Transform, Cache>();
        }

        public Cache GetCache(Transform trans, int count)
        {
            if (!ManagerCache.TryGetValue(trans, out Cache cache))
            {
                cache = new Cache();
                ManagerCache.Add(trans, cache);
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
            if (cache.RW_Depths == null || cache.RW_Depths.count != count)
            {
                if (cache.RW_Depths != null)
                    cache.RW_Depths.Dispose();
                cache.RW_Depths = new ComputeBuffer(count, sizeof(float));
            }
            if (cache.RW_Sizes == null || cache.RW_Sizes.count != count)
            {
                if (cache.RW_Sizes != null)
                    cache.RW_Sizes.Dispose();
                cache.RW_Sizes = new ComputeBuffer(count, sizeof(float));
            }
            if (cache.RW_BrushResult == null || cache.RW_BrushResult.count != count)
            {
                if (cache.RW_BrushResult != null)
                    cache.RW_BrushResult.Dispose();
                cache.RW_BrushResult = new ComputeBuffer(count, sizeof(float) * 4);
            }
            return cache;
        }

        public void ClearCache()
        {
            foreach (Cache item in ManagerCache.Values)
            {
                if (item.RW_Zone != null)
                    item.RW_Zone.Dispose();
                if (item.RW_Selects != null)
                    item.RW_Selects.Dispose();
                if (item.RW_Depths != null)
                    item.RW_Depths.Dispose();
                if (item.RW_Sizes != null)
                    item.RW_Sizes.Dispose();
                if (item.RW_BrushResult != null)
                    item.RW_BrushResult.Dispose();
            }
            ManagerCache.Clear();
        }

        ComputeBuffer check_Select(ComputeBuffer _Select, int count, out bool clearSelect)
        {
            if (_Select == null || !_Select.IsValid())
            {
                _Select = new ComputeBuffer(count, sizeof(float));
                _Select.SetData(Enumerable.Repeat(-1f, count).ToArray());
                clearSelect =  true;
            }
            else
                clearSelect = false;
            return _Select;
        }

        public ComputeBuffer GetBuffer1(Color[] values, TargetPassType originPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 4);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float));
            _values.SetData(values);
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin4To1, "_Origin4", _values);
            CalcShader.SetBuffer(kernel_Origin4To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin4To1, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer1(Vector2[] values, TargetPassType originPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 2);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float));
            _values.SetData(values);
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin2To1, "_Origin2", _values);
            CalcShader.SetBuffer(kernel_Origin2To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin2To1, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer1(Vector3[] values, TargetPassType originPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float));
            _values.SetData(values);
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin3To1, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin3To1, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer1(Vector4[] values, TargetPassType originPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 4);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float));
            _values.SetData(values);
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin4To1, "_Origin4", _values);
            CalcShader.SetBuffer(kernel_Origin4To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin4To1, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer1_from2(ComputeBuffer _buffer2, TargetPassType originPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer2.count, sizeof(float));
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin2To1, "_Origin2", _buffer2);
            CalcShader.SetBuffer(kernel_Origin2To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin2To1, Mathf.CeilToInt(_buffer2.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer1_from3(ComputeBuffer _buffer3, TargetPassType originPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer3.count, sizeof(float));
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin3To1, "_Origin3", _buffer3);
            CalcShader.SetBuffer(kernel_Origin3To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin3To1, Mathf.CeilToInt(_buffer3.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer1_from4(ComputeBuffer _buffer4, TargetPassType originPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer4.count, sizeof(float));
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin4To1, "_Origin4", _buffer4);
            CalcShader.SetBuffer(kernel_Origin4To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin4To1, Mathf.CeilToInt(_buffer4.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer2_from4(ComputeBuffer _buffer4)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer4.count, sizeof(float) * 2);
            CalcShader.SetBuffer(kernel_Origin4To2, "_Origin4", _buffer4);
            CalcShader.SetBuffer(kernel_Origin4To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin4To2, Mathf.CeilToInt(_buffer4.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer3_from4(ComputeBuffer _buffer4)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer4.count, sizeof(float) * 3);
            CalcShader.SetBuffer(kernel_Origin4To3, "_Origin4", _buffer4);
            CalcShader.SetBuffer(kernel_Origin4To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin4To3, Mathf.CeilToInt(_buffer4.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer3(float[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer3_from1(ComputeBuffer _buffer1)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 3);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer3(float[] values, Vector3[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer3_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer3)
        {
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _buffer3);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer3(float[] values, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer3_from1(ComputeBuffer _buffer1, TargetPassType resultPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 3);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer3(float[] values, Vector3[] origin, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer3_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer3, TargetPassType resultPass)
        {
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _buffer3);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer3(Vector3[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _result.SetData(values);
            return _result;
        }

        public ComputeBuffer GetBuffer2(float[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 2);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer2_from1(ComputeBuffer _buffer1)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 2);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer2(float[] values, Vector3[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 2);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer2_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer2)
        {
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _buffer2);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer2(float[] values, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 2);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer2_from1(ComputeBuffer _buffer1, TargetPassType resultPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 2);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer2(float[] values, Vector2[] origin, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 2);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _result);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer2_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer2, TargetPassType resultPass)
        {
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To2, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To2, "RW_Result2", _buffer2);
            CalcShader.Dispatch(kernel_Origin1To2, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer2(Vector2[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 2);
            _result.SetData(values);
            return _result;
        }

        public ComputeBuffer GetBuffer4(float[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4_from1(ComputeBuffer _buffer1)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 4);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer4(float[] values, Color[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(float[] values, Vector4[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer4)
        {
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(_buffer4.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer4(float[] values, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4_from1(ComputeBuffer _buffer1, TargetPassType resultPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 4);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer4(float[] values, Color[] origin, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(float[] values, Vector4[] origin, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer4, TargetPassType resultPass)
        {
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt(_buffer1.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer4(Vector2[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 2);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", 0);
            CalcShader.SetBuffer(kernel_Origin2To4, "_Origin2", _values);
            CalcShader.SetBuffer(kernel_Origin2To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(Vector2[] values, Color[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 2);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", 0);
            CalcShader.SetBuffer(kernel_Origin2To4, "_Origin2", _values);
            CalcShader.SetBuffer(kernel_Origin2To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(Vector2[] values, Vector4[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 2);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetInt("_ResultPass", 0);
            CalcShader.SetBuffer(kernel_Origin2To4, "_Origin2", _values);
            CalcShader.SetBuffer(kernel_Origin2To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4(Vector2[] values, ComputeBuffer _buffer4)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 2);
            _values.SetData(values);
            CalcShader.SetBuffer(kernel_Origin2To4, "_Origin2", _values);
            CalcShader.SetBuffer(kernel_Origin2To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
        }

        public ComputeBuffer GetBuffer4(Vector3[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4(Vector3[] values, ComputeBuffer _buffer4)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _values.SetData(values);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
        }

        public ComputeBuffer GetBuffer4_from3(ComputeBuffer _buffer3)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer3.count, sizeof(float) * 4);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _buffer3);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt(_buffer3.count / 1024f), 1, 1);
            return _result;
        }

        public void GetBuffer4_from3(ComputeBuffer _buffer3, ComputeBuffer _buffer4)
        {
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _buffer3);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt(_buffer3.count / 1024f), 1, 1);
        }

        public ComputeBuffer GetBuffer4(Vector3[] values, Color[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(Vector3[] values, Vector4[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt(values.Length / 1024f), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4(Vector3[] values, TargetPassType inPass, TargetPassType outPass, ComputeBuffer _buffer4)
        {
            ComputeBuffer _buffer1 = GetBuffer1(values, inPass);
            GetBuffer4_from1(_buffer1, _buffer4, outPass);
            _buffer1.Dispose();
        }

        public ComputeBuffer GetBuffer4(Color[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _result.SetData(values);
            return _result;
        }

        public void GetBuffer4(Color[] values, ComputeBuffer _buffer4)
        {
            _buffer4.SetData(values);
        }

        public void GetBuffer4(Color[] values, TargetPassType inPass, TargetPassType outPass, ComputeBuffer _buffer4)
        {
            ComputeBuffer _buffer1 = GetBuffer1(values, inPass);
            GetBuffer4_from1(_buffer1, _buffer4, outPass);
            _buffer1.Dispose();
        }

        public ComputeBuffer GetBuffer4(Vector4[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _result.SetData(values);
            return _result;
        }

        public void GetBuffer4(Vector4[] values, ComputeBuffer _buffer4)
        {
            _buffer4.SetData(values);
        }

        public void GetBuffer4(Vector4[] values, TargetPassType inPass, TargetPassType outPass, ComputeBuffer _buffer4)
        {
            ComputeBuffer _buffer1 = GetBuffer1(values, inPass);
            GetBuffer4_from1(_buffer1, _buffer4, outPass);
            _buffer1.Dispose();
        }

        public void GetBuffer4(Vector2[] values, TargetPassType inPass, TargetPassType outPass, ComputeBuffer _buffer4)
        {
            ComputeBuffer _buffer1 = GetBuffer1(values, inPass);
            GetBuffer4_from1(_buffer1, _buffer4, outPass);
            _buffer1.Dispose();
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Color[] res = GetResultColor(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result4, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result4, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 4);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result4, "RW_Result4", RW_Result);
            CalcShader.Dispatch(kernel_Result4, Mathf.CeilToInt(RW_Result.count / 1024f), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector2[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResult(WriteType type, Vector2[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result2, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result2, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 2);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result2, "RW_Result2", RW_Result);
            CalcShader.Dispatch(kernel_Result2, Mathf.CeilToInt(RW_Result.count / 1024f), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector3[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result3, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result3, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 3);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result3, "RW_Result3", RW_Result);
            CalcShader.Dispatch(kernel_Result3, Mathf.CeilToInt(RW_Result.count / 1024f), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector4[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result4, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result4, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 4);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result4, "RW_Result4", RW_Result);
            CalcShader.Dispatch(kernel_Result4, Mathf.CeilToInt(RW_Result.count / 1024f), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer4 = GetBuffer4(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from4(_buffer4, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            GetBuffer4_from1(_buffer1, _buffer4, outType);
            _buffer4.GetData(origin);
            _buffer4.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector2[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector2[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector2[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector2[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector2[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector2[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector2[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector2[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector2[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer2 = GetBuffer2(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from2(_buffer2, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            GetBuffer2_from1(_buffer1, _buffer2, outType);
            _buffer2.GetData(origin);
            _buffer2.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer3 = GetBuffer3(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from3(_buffer3, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            GetBuffer3_from1(_buffer1, _buffer3, outType);
            _buffer3.GetData(origin);
            _buffer3.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, float[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Vector2[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Vector3[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Vector4[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Color[] values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, clamp, min, max, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, ComputeBuffer values, bool clamp = false, float min = 0, float max = 1, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer4 = GetBuffer4(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from4(_buffer4, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Clamp", clamp ? 1 : 0);
            CalcShader.SetFloat("_ClampMin", min);
            CalcShader.SetFloat("_ClampMax", max);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            GetBuffer4_from1(_buffer1, _buffer4, outType);
            _buffer4.GetData(origin);
            _buffer4.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public float[] MappedTo01(float[] origin)
        {
            ComputeBuffer buffer = GetBuffer4(origin);
            CalcShader.SetBuffer(kernel_MappedTo01, "RW_Result4", buffer);
            CalcShader.Dispatch(kernel_MappedTo01, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            ComputeBuffer result = GetBuffer1_from4(buffer, TargetPassType.X);
            result.GetData(origin);
            buffer.Dispose();
            result.Dispose();
            return origin;
        }

        public Color[] MappedTo01(Color[] origin)
        {
            ComputeBuffer buffer = GetBuffer4(origin);
            CalcShader.SetBuffer(kernel_MappedTo01, "RW_Result4", buffer);
            CalcShader.Dispatch(kernel_MappedTo01, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            buffer.GetData(origin);
            buffer.Dispose();
            return origin;
        }

        public Vector2[] MappedTo01(Vector2[] origin)
        {
            ComputeBuffer buffer = GetBuffer4(origin);
            CalcShader.SetBuffer(kernel_MappedTo01, "RW_Result4", buffer);
            CalcShader.Dispatch(kernel_MappedTo01, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            ComputeBuffer result = GetBuffer2_from4(buffer);
            result.GetData(origin);
            buffer.Dispose();
            result.Dispose();
            return origin;
        }

        public Vector3[] MappedTo01(Vector3[] origin)
        {
            ComputeBuffer buffer = GetBuffer4(origin);
            CalcShader.SetBuffer(kernel_MappedTo01, "RW_Result4", buffer);
            CalcShader.Dispatch(kernel_MappedTo01, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            ComputeBuffer result = GetBuffer3_from4(buffer);
            result.GetData(origin);
            buffer.Dispose();
            result.Dispose();
            return origin;
        }

        public Vector4[] MappedTo01(Vector4[] origin)
        {
            ComputeBuffer buffer = GetBuffer4(origin);
            CalcShader.SetBuffer(kernel_MappedTo01, "RW_Result4", buffer);
            CalcShader.Dispatch(kernel_MappedTo01, Mathf.CeilToInt(origin.Length / 1024f), 1, 1);
            buffer.GetData(origin);
            buffer.Dispose();
            return origin;
        }

        public static Vector2[] CheckArrayNotNull(Vector2[] origin, int count)
        {
            if (origin == null || origin.Length != count)
                return Enumerable.Repeat(Vector2.zero, count).ToArray();
            return origin;
        }
        public static Vector3[] CheckArrayNotNull(Vector3[] origin, int count)
        {
            if (origin == null || origin.Length != count)
                return Enumerable.Repeat(Vector3.zero, count).ToArray();
            return origin;
        }
        public static Vector4[] CheckArrayNotNull(Vector4[] origin, int count)
        {
            if (origin == null || origin.Length != count)
                return Enumerable.Repeat(Vector4.zero, count).ToArray();
            return origin;
        }
        public static Color[] CheckArrayNotNull(Color[] origin, int count)
        {
            if (origin == null || origin.Length != count)
                return Enumerable.Repeat(Color.white, count).ToArray();
            return origin;
        }

        public sealed class Cache
        {
            int spreadLevel = 0;
            public int SpreadLevel
            {
                get => spreadLevel;
                set
                {
                    if (value < 0)
                        value = 0;
                    spreadLevel = value;
                }
            }
            /// <summary>
            /// Buffer type is "int"
            /// </summary>
            public ComputeBuffer RW_Zone { get; set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Selects { get; set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Depths { get; set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Sizes { get; set; }
            /// <summary>
            /// Buffer type is "float4"
            /// </summary>
            public ComputeBuffer RW_BrushResult { get; set; }
            public bool IsAvailable { get => !RW_Depths.IsValid() || !RW_Sizes.IsValid() || !RW_Selects.IsValid() || !RW_Zone.IsValid() || !RW_BrushResult.IsValid(); }
        }
    }
}