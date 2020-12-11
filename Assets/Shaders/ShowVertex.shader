Shader "ModEditor/ShowVertex"
{
    Properties
    {
        _VertexColor("VertexColor", Color) = (0, 0, 0, 1)
		_VertexScale("VertexScale", Range(0.001, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        
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
            #pragma vertex vert
			#pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2g
            {
				float4 worldPos : SV_POSITION;
            };

			struct g2f
			{
				float4 pos: SV_POSITION;
			};

			fixed4 _VertexColor;
			float _VertexScale;

            v2g vert (appdata v)
            {
                v2g o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

			[maxvertexcount(15)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> tristream)
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

					tristream.Append(o1);
					tristream.Append(o4);
					tristream.Append(o2);
					tristream.Append(o3);
					tristream.Append(o1);
					tristream.RestartStrip();
				}
			}

            fixed4 frag (g2f i) : SV_Target
            {
                return _VertexColor;
            }
            ENDCG
        }
    }
}