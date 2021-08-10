Shader "Hidden/ModEditorVertexView"
{
    SubShader
    {
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex: POSITION;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
				float4 color: TEXCOORD0;
			};

			int _HideColorView;
			StructuredBuffer<float> _Selects;
			StructuredBuffer<float4> _Colors;

			v2f vert(appdata v, uint id: SV_VertexID)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = _Colors[id] * any(_Selects[id]) * (1 - _HideColorView);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}

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
				float2 select: TEXCOORD0;
			};

            struct g2f
            {
                float4 pos : SV_POSITION;
				float4 color: TEXCOORD0;
            };

			fixed4 _UnselectedVertexColor;
			fixed4 _SelectedVertexColor;
			int _HideNoSelectVertex;
			int _BrushOn;
			float _VertexScale;
			StructuredBuffer<float> _Selects;
			StructuredBuffer<int> _Zone;

            v2g vert (appdata v, uint id: SV_VertexID)
            {
                v2g o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				int select = any(_Selects[id]);
				int zone = any(_Zone[id]);
				o.select.x = select * _BrushOn;
				o.select.y = zone;
                return o;
            }

			[maxvertexcount(12)]
			void geom_vertexView(triangle v2g input[3], inout TriangleStream<g2f> tristream)
			{
				for(int i = 0; i < 3; i++)
				{
					int i_cw = fmod(i + 1, 3);
					int i_ccw = fmod(i + 2, 3);
					float len = clamp(max(distance(input[i].worldPos, input[i_cw].worldPos), distance(input[i].worldPos, input[i_ccw].worldPos)) * _VertexScale * 0.15, 0.001, 0.03);
					g2f o1;
					o1.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) + float3(0, 1, 0) * len);
					g2f o2;
					o2.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) + float3(sqrt(3) * 0.5, -0.5, 0) * len);
					g2f o3;
					o3.pos = UnityViewToClipPos(UnityWorldToViewPos(input[i].worldPos.xyz) + float3(-sqrt(3) * 0.5, -0.5, 0) * len);

					fixed4 col = lerp(fixed4(_UnselectedVertexColor.rgb, _UnselectedVertexColor.a * (1 - _HideNoSelectVertex) * input[i].select.y), _SelectedVertexColor, input[i].select.x);
					o1.color = col;
					o2.color = col;
					o3.color = col;

					tristream.Append(o1);
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