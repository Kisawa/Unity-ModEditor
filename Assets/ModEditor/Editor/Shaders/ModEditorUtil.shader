Shader "Hidden/ModEditorUtil"
{
    SubShader
    {
        CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata
        {
            float4 vertex: POSITION;
			float3 normal: NORMAL;
			float4 tangent: TANGENT;
			float2 texcoord: TEXCOORD0;
			float4 color: COLOR;
        };

		struct appdata_UVView
		{
			float4 vertex: POSITION;
			float2 texcoord0: TEXCOORD0;
			float2 texcoord1: TEXCOORD1;
			float2 texcoord2: TEXCOORD2;
		};

		struct v2f_UVView
		{
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0;
		};

		struct v2f
		{
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0;
			float4 screenPos: TEXCOORD1;
		};

		struct v2g
		{
			float4 worldPos: SV_POSITION;
			float3 worldNormal: TEXCOORD0;
			float4 worldTangent: TEXCOORD1;
			float4 color: TEXCOORD2;
		};

        struct g2f
        {
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0;
			float4 color: TEXCOORD1;
			float4 normal: TEXCOORD2;
        };

		fixed4 _NormalColor;
		fixed _NormalLength;

		fixed4 _TangentColor;
		fixed _TangentLength;
		fixed _ArrowLength;
		fixed _ArrowSize;

		fixed4 _GridColor;
		bool _GridWithZTest;

		float _UVType;
		fixed _UVAlpha;

		float _DepthCompress;

		sampler2D _MapViewTex;
		int _MapViewPass;
		fixed4 _MapViewColorTint;

		float pow2(float res)
		{
			return res * res;
		}

		float2 rotate2(float2 res, float radian)
		{
			return mul(float2x2(cos(radian), -sin(radian), sin(radian), cos(radian)), res);
		}

		float remap(float num, float inMin, float inMax, float outMin, float outMax)
		{
			return outMin + (num - inMin) * (outMax - outMin) / (inMax - inMin);
		}

		v2g vertToGeom(appdata v)
        {
            v2g o;
			UNITY_INITIALIZE_OUTPUT(v2g, o);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
			o.worldTangent.xyz = UnityObjectToWorldDir(v.tangent);
			o.worldTangent.w = v.tangent.w;
            return o;
        }

		g2f vert(appdata v)
        {
            g2f o;
			UNITY_INITIALIZE_OUTPUT(g2f, o);
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
			o.color = v.color;
			COMPUTE_EYEDEPTH(o.normal.w);
			o.normal.xyz = UnityObjectToWorldNormal(v.normal);
            return o;
        }

		v2f_UVView vert_UVView(appdata_UVView v)
		{
			v2f_UVView o;
			UNITY_INITIALIZE_OUTPUT(v2f_UVView, o);
			o.pos = UnityObjectToClipPos(v.vertex);
			switch (_UVType)
			{
			case 1:
				o.uv = v.texcoord1;
				break;
			case 2:
				o.uv = v.texcoord2;
				break;
			default:
				o.uv = v.texcoord0;
				break;
			}
			return o;
		}

		v2f vert_ScreenMesh(appdata v)
		{
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f, o);
			o.pos = v.vertex;
			o.uv = v.texcoord;
			return o;
		}

		[maxvertexcount(6)]
		void geom_normalView(triangle v2g input[3], inout LineStream<g2f> tristream)
		{
			for(int i = 0; i < 3; i++)
			{
				g2f o1;
				UNITY_INITIALIZE_OUTPUT(g2f, o1);
				o1.pos = mul(UNITY_MATRIX_VP, input[i].worldPos);
				tristream.Append(o1);
				g2f o2;
				UNITY_INITIALIZE_OUTPUT(g2f, o2);
				float4 underlapPos = input[i].worldPos + float4(input[i].worldNormal, 0) * _NormalLength;
				o2.pos = mul(UNITY_MATRIX_VP, underlapPos);
				tristream.Append(o2);
				tristream.RestartStrip();
			}
		}

		[maxvertexcount(12)]
		void geom_tangentView(triangle v2g input[3], inout LineStream<g2f> tristream)
		{
			for(int i = 0; i < 3; i++)
			{
				g2f o1;
				UNITY_INITIALIZE_OUTPUT(g2f, o1);
				o1.pos = mul(UNITY_MATRIX_VP, input[i].worldPos);
				tristream.Append(o1);
				g2f o2;
				UNITY_INITIALIZE_OUTPUT(g2f, o2);
				float4 underlapPos = input[i].worldPos + float4(input[i].worldTangent.xyz, 0) * _TangentLength;
				o2.pos = mul(UNITY_MATRIX_VP, underlapPos);
				tristream.Append(o2);
				float4 arrowOffset = float4(cross(input[i].worldNormal, input[i].worldTangent.xyz) * input[i].worldTangent.w * _TangentLength * _ArrowSize * 0.5, 0);
				g2f o3;
				UNITY_INITIALIZE_OUTPUT(g2f, o3);
				float4 dirPos1 =  underlapPos - float4(input[i].worldTangent.xyz, 0) * _TangentLength * _ArrowLength + arrowOffset;
				o3.pos = mul(UNITY_MATRIX_VP, dirPos1);
				tristream.Append(o3);
				g2f o4;
				UNITY_INITIALIZE_OUTPUT(g2f, o4);
				float4 dirPos2 =  dirPos1 - arrowOffset;
				o4.pos = mul(UNITY_MATRIX_VP, dirPos2);
				tristream.Append(o4);
				tristream.RestartStrip();
			}
		}

		[maxvertexcount(4)]
		void geom_gridView(triangle v2g input[3], inout LineStream<g2f> tristream)
		{
			g2f o1;
			UNITY_INITIALIZE_OUTPUT(g2f, o1);
			o1.pos = mul(UNITY_MATRIX_VP, input[0].worldPos);
			g2f o2;
			UNITY_INITIALIZE_OUTPUT(g2f, o2);
			o2.pos = mul(UNITY_MATRIX_VP, input[1].worldPos);
			g2f o3;
			UNITY_INITIALIZE_OUTPUT(g2f, o3);
			o3.pos = mul(UNITY_MATRIX_VP, input[2].worldPos);
			tristream.Append(o1);
			tristream.Append(o2);
			tristream.Append(o3);
			tristream.Append(o1);
		}

        fixed4 frag_normalView(g2f i) : SV_Target
        {
            return _NormalColor;
        }

		fixed4 frag_tangentView(g2f i) : SV_Target
        {
            return _TangentColor;
        }

		fixed4 frag_gridView(g2f i) : SV_Target
        {
            return _GridColor;
        }

		fixed4 frag_UVView(g2f i) : SV_Target
        {
            return fixed4(i.uv, 0, _UVAlpha);
        }

		fixed4 frag_VertexColorView(g2f i) : SV_Target
        {
            return i.color;
        }

		fixed4 frag_DepthMapView(g2f i) : SV_Target
        {
			float depth = i.normal.w * _DepthCompress;
            return fixed4(depth, depth, depth, 1);
        }

		fixed4 frag_NormalMapView(g2f i) : SV_Target
        {
			float3 normal = mul(UNITY_MATRIX_V, float4(i.normal.xyz, 0));
            return fixed4(normal * 0.5 + 0.5, 1);
        }

		float _FromStep;
		float _ToStep;
		float3 _MouseTexcoord;
		float _BrushSize;
		fixed4 _BrushScopeViewColor;
		fixed4 frag_ScreenMesh(v2f i) : SV_Target
		{
			i.uv.x *= _MouseTexcoord.z;
			float2 mouseTexcoord = _MouseTexcoord.xy;
			mouseTexcoord.x *= _MouseTexcoord.z;
			float dis = distance(i.uv, mouseTexcoord);
			fixed4 col = lerp(0, _BrushScopeViewColor, min(1 - step(_BrushSize, dis), step(_BrushSize - 0.002, dis)));
			col = max(col, lerp(0, _BrushScopeViewColor, 1 - step(_BrushSize * _FromStep, dis)));
			col = max(col, lerp(0, _BrushScopeViewColor, step(_BrushSize * _ToStep, dis) - step(_BrushSize, dis)));
			return col;
		}

		int _TexRender;
		sampler2D _EditorTex;
		fixed4 frag_TexView(g2f i) : SV_Target
		{
			fixed4 col = tex2D(_EditorTex, i.uv) * _TexRender;
			return col;
		}

		fixed4 _TexBrushRangeViewColor;
		int _CursorOn;
		float2 _CursorTexcoord;
		float3 _TexBrushRange;
		float _BrushRotate;
		fixed4 frag_TexBrushRange(g2f i) : SV_Target
		{
			fixed4 col = 0;
			float2 dir = i.uv - _CursorTexcoord;
			dir = rotate2(dir, _BrushRotate);
			float res = pow2(dir.x) / pow2(_TexBrushRange.x) + pow2(dir.y) / pow2(_TexBrushRange.y);
			int inRange = 1 - step(1, res);
			int inRange1 = 1 - step(0.95, res);
			
			res = (1 - remap(res, min(_TexBrushRange.z, 0.999), 1, 0, 1)) * inRange;
			col = lerp(col, _TexBrushRangeViewColor, res);

			col = lerp(col, _TexBrushRangeViewColor, inRange - inRange1);
			inRange = 1 - step(1, res);
			inRange1 = 1 - step(1 - 0.1 * max(_TexBrushRange.z, 0.01), res);
			col = lerp(col, 1, inRange - inRange1);
			col *= _CursorOn;
			return col;
		}

		fixed4 frag_MapView(g2f i) : SV_Target
		{
			fixed4 color = tex2D(_MapViewTex, i.uv);
			switch (_MapViewPass)
			{
			case 0:
				color = color.r;
				break;
			case 1:
				color = color.g;
				break;
			case 2:
				color = color.b;
				break;
			case 3:
				color = color.a;
				break;
			case 4:
				fixed gray = dot(color.rgb, fixed3(0.299, 0.587, 0.114));
				color = gray;
				break;
			}
			color *= _MapViewColorTint;
			color.a = 1;
			return color;
		}
		ENDCG

		Pass
		{
			ZWrite On
			ColorMask 0
		}

        Pass
        {
            CGPROGRAM
			#pragma vertex vertToGeom
			#pragma geometry geom_normalView
			#pragma fragment frag_normalView
			ENDCG
        }

		Pass
        {
            CGPROGRAM
			#pragma vertex vertToGeom
			#pragma geometry geom_tangentView
			#pragma fragment frag_tangentView
			ENDCG
        }

		Pass
        {
			ZTest [_GridWithZTest]
            CGPROGRAM
			#pragma vertex vertToGeom
			#pragma geometry geom_gridView
			#pragma fragment frag_gridView
			ENDCG
        }

		Pass
        {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
			#pragma vertex vert_UVView
			#pragma fragment frag_UVView
			ENDCG
        }

		Pass
        {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_VertexColorView
			ENDCG
        }

		Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_DepthMapView
			ENDCG
        }

		Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_NormalMapView
			ENDCG
        }

		Pass
        {
			ZTest Off ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
			#pragma vertex vert_ScreenMesh
			#pragma fragment frag_ScreenMesh
			ENDCG
        }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_TexView
			ENDCG
		}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_TexBrushRange
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_MapView
			ENDCG
		}
    }
}