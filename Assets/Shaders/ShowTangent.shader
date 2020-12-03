Shader "ModEditor/ShowTangent"
{
    Properties
    {
		_TangentColor("Tangent Color", Color) = (0, 1, 0, 1)
		_TangentLength("Tangent Length", Range(0.01, 1)) = 0.5
		[Header(Binormal)]
		[Space(10)]
		_ArrowLength("Arrow Length", Range(0, 1)) = 0.2
		_ArrowSize("Arrow Size", Range(0, 1)) = 0.1
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
				float4 tangent: TANGENT;
            };

			struct v2g
			{
				float4 worldPos: SV_POSITION;
				float3 worldNormal: TEXCOORD0;
				float4 worldTangent: TEXCOORD1;
			};

            struct g2f
            {
				float4 pos : SV_POSITION;
            };

			fixed4 _TangentColor;
			fixed _TangentLength;
			fixed _ArrowLength;
			fixed _ArrowSize;

            v2g vert(appdata v)
            {
                v2g o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldTangent.xyz = UnityObjectToWorldDir(v.tangent);
				o.worldTangent.w = v.tangent.w;
                return o;
            }

			[maxvertexcount(12)]
			void geom(triangle v2g input[3], inout LineStream<g2f> tristream)
			{
				for(int i = 0; i < 3; i++)
				{
					g2f o1;
					o1.pos = mul(UNITY_MATRIX_VP, input[i].worldPos);
					tristream.Append(o1);
					g2f o2;
					float4 underlapPos = input[i].worldPos + float4(input[i].worldTangent.xyz, 0) * _TangentLength;
					o2.pos = mul(UNITY_MATRIX_VP, underlapPos);
					tristream.Append(o2);
					float4 arrowOffset = float4(cross(input[i].worldNormal, input[i].worldTangent.xyz) * input[i].worldTangent.w * _TangentLength * _ArrowSize * 0.5, 0);
					g2f o3;
					float4 dirPos1 =  underlapPos - float4(input[i].worldTangent.xyz, 0) * _TangentLength * _ArrowLength + arrowOffset;
					o3.pos = mul(UNITY_MATRIX_VP, dirPos1);
					tristream.Append(o3);
					g2f o4;
					float4 dirPos2 =  dirPos1 - arrowOffset;
					o4.pos = mul(UNITY_MATRIX_VP, dirPos2);
					tristream.Append(o4);
					tristream.RestartStrip();
				}
			}

            fixed4 frag(g2f i) : SV_Target
            {
                return _TangentColor;
            }
            ENDCG
        }
    }
}