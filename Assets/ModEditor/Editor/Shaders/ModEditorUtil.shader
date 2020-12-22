Shader "Hidden/ModEditorUtil"
{
	Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata
        {
            float4 vertex : POSITION;
			float3 normal: NORMAL;
			float4 tangent: TANGENT;
			float2 texcoord : TEXCOORD0;
			float4 color: COLOR;
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
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 color: TEXCOORD1;
			float4 normal: TEXCOORD2;
        };

		struct v2f_moreSample
        {
            float2 uv[9] : TEXCOORD0;
            float4 pos : SV_POSITION;
        };

		fixed4 _NormalColor;
		fixed _NormalLength;

		fixed4 _TangentColor;
		fixed _TangentLength;
		fixed _ArrowLength;
		fixed _ArrowSize;

		fixed4 _GridColor;

		fixed _UVAlpha;

		float _DepthCompress;

		fixed4 _VertexColor;
		float _VertexScale;
		sampler2D _DepthMap;
		float3 _MousePos;
		float _SelcetDis;

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		float _Spread;

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

		v2f_moreSample vert_moreSample(appdata v)
        {
            v2f_moreSample o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv[0] = v.texcoord;
			o.uv[1] = v.texcoord + _MainTex_TexelSize * float2(-_Spread, -_Spread);
			o.uv[2] = v.texcoord + _MainTex_TexelSize * float2(0, -_Spread);
			o.uv[3] = v.texcoord + _MainTex_TexelSize * float2(_Spread, -_Spread);
			o.uv[4] = v.texcoord + _MainTex_TexelSize * float2(-_Spread, 0);
			o.uv[5] = v.texcoord + _MainTex_TexelSize * float2(_Spread, 0);
			o.uv[6] = v.texcoord + _MainTex_TexelSize * float2(-_Spread, _Spread);
			o.uv[7] = v.texcoord + _MainTex_TexelSize * float2(0, _Spread);
			o.uv[8] = v.texcoord + _MainTex_TexelSize * float2(_Spread, _Spread);
            return o;
        }

		[maxvertexcount(15)]
		void geom_vertexView(triangle v2g input[3], inout TriangleStream<g2f> tristream)
		{
			for(int i = 0; i < 3; i++)
			{
				int i_cw = fmod(i + 1, 3);
				int i_ccw = fmod(i + 2, 3);
				float len = clamp(min(length(input[i].worldPos - input[i_cw].worldPos), length(input[i].worldPos - input[i_ccw].worldPos)) * _VertexScale * 0.1, 0.0005, 0.02);

				g2f o1;
				UNITY_INITIALIZE_OUTPUT(g2f, o1);
				o1.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) - float3(1, 0, 0) * len);
				g2f o2;
				UNITY_INITIALIZE_OUTPUT(g2f, o2);
				o2.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) + float3(1, 0, 0) * len);
				g2f o3;
				UNITY_INITIALIZE_OUTPUT(g2f, o3);
				o3.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) - float3(0, 1, 0) * len);
				g2f o4;
				UNITY_INITIALIZE_OUTPUT(g2f, o4);
				o4.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) + float3(0, 1, 0) * len);

				float4 screenPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, input[i].worldPos));
				float2 vertexTexcoord = screenPos.xy / screenPos.w;
				vertexTexcoord.x *= _MousePos.z;
				float2 mouse = _MousePos.xy;
				mouse.x *= _MousePos.z;
				int res = 1 - step(_SelcetDis, length(vertexTexcoord - mouse));
				fixed4 col = _VertexColor;
				col *= res;
				col.rgb += abs(res - 1);
				col.a += 0.05;
				o1.color = col;
				o2.color = col;
				o3.color = col;
				o4.color = col;

				tristream.Append(o1);
				tristream.Append(o4);
				tristream.Append(o2);
				tristream.Append(o3);
				tristream.Append(o1);
				tristream.RestartStrip();
			}
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

		fixed4 frag_vertexView(g2f i) : SV_Target
        {
            return i.color;
        }

        fixed4 frag_normalView(g2f i) : SV_Target
        {
            return _NormalColor;
        }

		fixed4 frag_tangentView(g2f i) : SV_Target
        {
            return _TangentColor;
        }

		fixed4 frag_gridView (g2f i) : SV_Target
        {
            return _GridColor;
        }

		fixed4 frag_UVView (g2f i) : SV_Target
        {
            return fixed4(i.uv, 0, _UVAlpha);
        }

		fixed4 frag_VertexColorView (g2f i) : SV_Target
        {
            return i.color;
        }

		fixed4 frag_DepthMapView (g2f i) : SV_Target
        {
			float depth = i.normal.w * _DepthCompress;
            return fixed4(depth, depth, depth, 1);
        }

		fixed4 frag_NormalMapView (g2f i) : SV_Target
        {
            return fixed4(i.normal.xyz, 1);
        }

		float4 frag_normDepth (g2f i) : SV_Target
		{
			return float4(i.normal.w, 0, 0, 1);
		}

		float4 frag_expandEdge (v2f_moreSample i) : SV_Target
        {
            float res = tex2D(_MainTex, i.uv[0]).r;
			for(int j = 1; j < 9; j++)
				res = max(tex2D(_MainTex, i.uv[j]).r, res);
			return float4(res, 0, 0, 1);
        }
		ENDCG

		Pass
		{
			ZWrite On
			ColorMask 0
		}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vertToGeom
			#pragma geometry geom_vertexView
			#pragma fragment frag_vertexView
			ENDCG
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
			#pragma vertex vert
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
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_normDepth
			ENDCG
        }

		Pass
        {
			Cull Off ZWrite Off ZTest Always
            CGPROGRAM
			#pragma vertex vert_moreSample
			#pragma fragment frag_expandEdge
			ENDCG
        }
    }
}