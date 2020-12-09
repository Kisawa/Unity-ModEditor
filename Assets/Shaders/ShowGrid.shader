Shader "ModEditor/ShowGrid"
{
    Properties
    {
        _GridColor("Grid Color", Color) = (0.75, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
			#pragma	geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

			struct v2g
			{
				float4 worldPos: SV_POSITION;
			};

            struct g2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _GridColor;

            v2g vert (appdata v)
            {
                v2g o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

			[maxvertexcount(4)]
			void geom(triangle v2g input[3], inout LineStream<g2f> tristream)
			{
				g2f o1;
				o1.pos = mul(UNITY_MATRIX_VP, input[0].worldPos);
				g2f o2;
				o2.pos = mul(UNITY_MATRIX_VP, input[1].worldPos);
				g2f o3;
				o3.pos = mul(UNITY_MATRIX_VP, input[2].worldPos);
				tristream.Append(o1);
				tristream.Append(o2);
				tristream.Append(o3);
				tristream.Append(o1);
			}

            fixed4 frag (g2f i) : SV_Target
            {
                return _GridColor;
            }
            ENDCG
        }
    }
}
