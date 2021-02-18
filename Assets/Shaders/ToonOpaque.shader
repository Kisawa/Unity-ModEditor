Shader "Toon/ToonOpaque"
{
    Properties
    {
		_Color("Color Tint", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_MainTex("Main Tex", 2D) = "white"{}
		[Header(Outline)]
		[Space(10)]
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0.001, 1)) = 0.001
    }
    SubShader
    {
        UsePass "Toon/ToonOutlineWithTangentData/Outline"

        Pass
        {
			Tags{ "RenderType"="Opaque" "LightMode" = "ForwardBase" }
			Cull Off
            CGPROGRAM
			#pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
				float2 uv: TEXCOORD0;
            };

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }

	Fallback "VertexLit"
}