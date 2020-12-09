Shader "ModEditor/ShowNormal"
{
    Properties
    {
		_NormalColor("Normal Color", Color) = (1, 0, 0, 1)
		_NormalLength("Normal Length", Range(0.01, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
			#pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal: NORMAL;
            };

			struct v2g
			{
				float4 worldPos: SV_POSITION;
				float3 worldNormal: TEXCOORD1;
			};

            struct g2f
            {
				float4 pos : SV_POSITION;
            };

			fixed4 _NormalColor;
			fixed _NormalLength;

            v2g vert(appdata v)
            {
                v2g o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

			[maxvertexcount(6)]
			void geom(triangle v2g input[3], inout LineStream<g2f> tristream)
			{
				for(int i = 0; i < 3; i++)
				{
					g2f o1;
					o1.pos = mul(UNITY_MATRIX_VP, input[i].worldPos);
					tristream.Append(o1);
					g2f o2;
					float4 underlapPos = input[i].worldPos + float4(input[i].worldNormal, 0) * _NormalLength;
					o2.pos = mul(UNITY_MATRIX_VP, underlapPos);
					tristream.Append(o2);
					tristream.RestartStrip();
				}
			}

            fixed4 frag(g2f i) : SV_Target
            {
                return _NormalColor;
            }
            ENDCG
        }
    }
}