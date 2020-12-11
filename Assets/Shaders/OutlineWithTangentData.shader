﻿Shader "Projector/OutlineWithTangentData"
{
    Properties
    {
		_Color("Color Tint", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_MainTex("Main Tex", 2D) = "white"{}
		[Header(Outline)]
		[Space(10)]
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0, 1)) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

		Pass{
			Tags{ "LightMode" = "Always" }
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _OutlineColor;
			fixed _OutlineWidth;

			struct appdata
			{
				float4 vertex: POSITION;
				float4 tangent: TANGENT;
				float4 color: COLOR;
			};

			float4 vert(appdata v): SV_POSITION
			{
				float depth;
				COMPUTE_EYEDEPTH(depth);
				float outlineWidth = clamp(_OutlineWidth * 0.01 * depth * v.color.a, 0.003, 0.03);
				float3 pos = v.vertex + v.tangent.xyz * outlineWidth;
				return UnityObjectToClipPos(pos);
			}

			fixed4 frag(float4 pos: SV_POSITION): SV_Target
			{
				return fixed4(_OutlineColor.xyz, 1);
			}
			ENDCG
		}

        Pass
        {
			Tags{ "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }

	Fallback "VertexLit"
}