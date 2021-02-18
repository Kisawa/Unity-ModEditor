Shader "Toon/ToonOutlineWithTangentData"
{
    Properties
    {
		[Header(Outline)]
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0.001, 1)) = 0.001
    }
    SubShader
    {
		Pass{
			Name "Outline"
			Tags{ "RenderType"="Opaque" "LightMode" = "Always" }
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
				float outlineWidth = clamp(_OutlineWidth * 0.01 * depth, 0.001, 0.03);
				float3 pos = v.vertex + v.tangent.xyz * outlineWidth;
				return UnityObjectToClipPos(pos);
			}

			fixed4 frag(float4 pos: SV_POSITION): SV_Target
			{
				return fixed4(_OutlineColor.xyz, 1);
			}
			ENDCG
		}
    }
}