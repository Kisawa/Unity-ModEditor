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
        public int kernel_Result1 { get; private set; }
        public int kernel_Result2 { get; private set; }
        public int kernel_Result3 { get; private set; }
        public int kernel_Result4 { get; private set; }
        public int kernel_Origin3To1 { get; private set; }
        public int kernel_Origin4To1 { get; private set; }
        public int kernel_Origin1To3 { get; private set; }
        public int kernel_Origin1To4 { get; private set; }
        public int kernel_Origin2To4 { get; private set; }
        public int kernel_Origin3To4 { get; private set; }

        Dictionary<Transform, CalcData.Cache> Cache;

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
            kernel_Result1 = CalcShader.FindKernel("Result1");
            kernel_Result2 = CalcShader.FindKernel("Result2");
            kernel_Result3 = CalcShader.FindKernel("Result3");
            kernel_Result4 = CalcShader.FindKernel("Result4");
            kernel_Origin3To1 = CalcShader.FindKernel("Origin3To1");
            kernel_Origin4To1 = CalcShader.FindKernel("Origin4To1");
            kernel_Origin1To3 = CalcShader.FindKernel("Origin1To3");
            kernel_Origin1To4 = CalcShader.FindKernel("Origin1To4");
            kernel_Origin2To4 = CalcShader.FindKernel("Origin2To4");
            kernel_Origin3To4 = CalcShader.FindKernel("Origin3To4");
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
            CalcShader.Dispatch(kernel_Origin4To1, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin3To1, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin4To1, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer1_from3(ComputeBuffer _buffer3, TargetPassType originPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer3.count, sizeof(float));
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin3To1, "_Origin3", _buffer3);
            CalcShader.SetBuffer(kernel_Origin3To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin3To1, Mathf.CeilToInt((float)_buffer3.count / 1024), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer1_from4(ComputeBuffer _buffer4, TargetPassType originPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer4.count, sizeof(float));
            CalcShader.SetInt("_OriginPass", (int)originPass);
            CalcShader.SetBuffer(kernel_Origin4To1, "_Origin4", _buffer4);
            CalcShader.SetBuffer(kernel_Origin4To1, "RW_Result1", _result);
            CalcShader.Dispatch(kernel_Origin4To1, Mathf.CeilToInt((float)_buffer4.count / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer3_from1(ComputeBuffer _buffer1)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 3);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer3_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer3)
        {
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _buffer3);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
        }

        public ComputeBuffer GetBuffer3(float[] values, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer3_from1(ComputeBuffer _buffer1, TargetPassType resultPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 3);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _result);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer3_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer3, TargetPassType resultPass)
        {
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To3, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To3, "RW_Result3", _buffer3);
            CalcShader.Dispatch(kernel_Origin1To3, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
        }

        public ComputeBuffer GetBuffer3(Vector3[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 3);
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
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4_from1(ComputeBuffer _buffer1)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 4);
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer4)
        {
            CalcShader.SetInt("_ResultPass", -1);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)_buffer4.count / 1024), 1, 1);
        }

        public ComputeBuffer GetBuffer4(float[] values, TargetPassType resultPass)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float));
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _values);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4_from1(ComputeBuffer _buffer1, TargetPassType resultPass)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer1.count, sizeof(float) * 4);
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public void GetBuffer4_from1(ComputeBuffer _buffer1, ComputeBuffer _buffer4, TargetPassType resultPass)
        {
            CalcShader.SetInt("_ResultPass", (int)resultPass);
            CalcShader.SetBuffer(kernel_Origin1To4, "_Origin1", _buffer1);
            CalcShader.SetBuffer(kernel_Origin1To4, "RW_Result4", _buffer4);
            CalcShader.Dispatch(kernel_Origin1To4, Mathf.CeilToInt((float)_buffer1.count / 1024), 1, 1);
        }

        public ComputeBuffer GetBuffer4(Vector2[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 2);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetInt("_ResultPass", 0);
            CalcShader.SetBuffer(kernel_Origin2To4, "_Origin2", _values);
            CalcShader.SetBuffer(kernel_Origin2To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin2To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(Vector3[] values)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4_from3(ComputeBuffer _buffer3)
        {
            ComputeBuffer _result = new ComputeBuffer(_buffer3.count, sizeof(float) * 4);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _buffer3);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt((float)_buffer3.count / 1024), 1, 1);
            return _result;
        }

        public ComputeBuffer GetBuffer4(Vector3[] values, Color[] origin)
        {
            ComputeBuffer _values = new ComputeBuffer(values.Length, sizeof(float) * 3);
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _values.SetData(values);
            _result.SetData(origin);
            CalcShader.SetBuffer(kernel_Origin3To4, "_Origin3", _values);
            CalcShader.SetBuffer(kernel_Origin3To4, "RW_Result4", _result);
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
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
            CalcShader.Dispatch(kernel_Origin3To4, Mathf.CeilToInt((float)values.Length / 1024), 1, 1);
            _values.Dispose();
            return _result;
        }

        public ComputeBuffer GetBuffer4(Color[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _result.SetData(values);
            return _result;
        }

        public ComputeBuffer GetBuffer4(Vector4[] values)
        {
            ComputeBuffer _result = new ComputeBuffer(values.Length, sizeof(float) * 4);
            _result.SetData(values);
            return _result;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, float[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Color[] res = GetResultColor(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Vector2[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Vector3[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Vector4[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, Color[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultColor(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultColor(WriteType type, Color[] origin, ComputeBuffer values, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Saturate", 1);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result4, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result4, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 4);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result4, "RW_Result4", RW_Result);
            CalcShader.Dispatch(kernel_Result4, Mathf.CeilToInt((float)RW_Result.count / 1024), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, float[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector3[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Vector2[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Vector3[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Vector4[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, Color[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResult(WriteType type, Vector3[] origin, ComputeBuffer values, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Saturate", 0);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result3, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result3, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 3);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result3, "RW_Result3", RW_Result);
            CalcShader.Dispatch(kernel_Result3, Mathf.CeilToInt((float)RW_Result.count / 1024), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, float[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector4[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Vector2[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Vector3[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Vector4[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, Color[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResult(type, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResult(WriteType type, Vector4[] origin, ComputeBuffer values, ComputeBuffer _Select = null)
        {
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Saturate", 0);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result4, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result4, "_Origin4", values);
            ComputeBuffer RW_Result = new ComputeBuffer(origin.Length, sizeof(float) * 4);
            RW_Result.SetData(origin);
            CalcShader.SetBuffer(kernel_Result4, "RW_Result4", RW_Result);
            CalcShader.Dispatch(kernel_Result4, Mathf.CeilToInt((float)RW_Result.count / 1024), 1, 1);
            RW_Result.GetData(origin);
            RW_Result.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, float[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Vector2[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Vector3[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Vector4[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, Color[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Color[] res = GetResultCustom(type, outType, inPass, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Color[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Color[] origin, ComputeBuffer values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer4 = GetBuffer4(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from4(_buffer4, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Saturate", 0);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt((float)origin.Length / 1024), 1, 1);
            GetBuffer4_from1(_buffer1, _buffer4, outType);
            _buffer4.GetData(origin);
            _buffer4.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, float[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Vector2[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Vector3[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Vector4[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, Color[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector3[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector3[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector3[] origin, ComputeBuffer values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer3 = GetBuffer3(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from3(_buffer3, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Saturate", 0);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt((float)origin.Length / 1024), 1, 1);
            GetBuffer3_from1(_buffer1, _buffer3, outType);
            _buffer3.GetData(origin);
            _buffer3.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, float[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values, TargetPassType.X);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Vector2[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Vector3[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Vector4[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, Color[] values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _values = GetBuffer4(values);
            Vector4[] res = GetResultCustom(type, inPass, outType, origin, _values, _Select);
            _values.Dispose();
            return res;
        }

        public Vector4[] GetResultCustom(WriteType type, TargetPassType inPass, TargetPassType outType, Vector4[] origin, ComputeBuffer values, ComputeBuffer _Select = null)
        {
            ComputeBuffer _buffer4 = GetBuffer4(origin);
            ComputeBuffer _buffer1 = GetBuffer1_from4(_buffer4, outType);
            CalcShader.SetInt("_WriteType", (int)type);
            CalcShader.SetInt("_Saturate", 0);
            CalcShader.SetInt("_OriginPass", (int)inPass);
            _Select = check_Select(_Select, origin.Length, out bool clearSelect);
            CalcShader.SetBuffer(kernel_Result1, "RW_Selects", _Select);
            CalcShader.SetBuffer(kernel_Result1, "_Origin4", values);
            CalcShader.SetBuffer(kernel_Result1, "RW_Result1", _buffer1);
            CalcShader.Dispatch(kernel_Result1, Mathf.CeilToInt((float)origin.Length / 1024), 1, 1);
            GetBuffer4_from1(_buffer1, _buffer4, outType);
            _buffer4.GetData(origin);
            _buffer4.Dispose();
            _buffer1.Dispose();
            if (clearSelect)
                _Select.Dispose();
            return origin;
        }
    }
}