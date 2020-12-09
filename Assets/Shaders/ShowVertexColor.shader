Shader "ModEditor/ShowVertexColor"
{
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
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 color: COLOR;
            };

            struct v2f
            {
				float4 pos : SV_POSITION;
				float4 color: TEXCOORD0;
            };

			fixed _VertexColorAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}