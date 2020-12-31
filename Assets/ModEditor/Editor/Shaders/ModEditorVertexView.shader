Shader "Hidden/ModEditorVertexView"
{
    SubShader
    {
        Pass
        {
			ZWrite Off
			ZTest [_VertexWithZTest]
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
			#pragma geometry geom_vertexView
            #pragma fragment frag
            #include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex: POSITION;
			};

			struct v2g
			{
				float4 worldPos: SV_POSITION;
				float select: TEXCOORD0;
			};

            struct g2f
            {
                float4 pos : SV_POSITION;
				float4 color: TEXCOORD0;
            };

			fixed4 _VertexColor;
			float _VertexScale;
			int _BrushOn;
			int _HideNoSelectVertex;
			StructuredBuffer<float> _Selects;

            v2g vert (appdata v, uint id : SV_VertexID)
            {
                v2g o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.select = any(_Selects[id]);
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

					fixed4 col = lerp(fixed4(1, 1, 1, 0.1 * (1 - _HideNoSelectVertex)), _VertexColor, input[i].select * _BrushOn);
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

            fixed4 frag (g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
