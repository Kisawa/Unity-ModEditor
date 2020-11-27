Shader "ModEditor/ShowUV"
{
    Properties
    {
        _Alpha("Alpha", Range(0, 1)) = 1
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
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

			fixed _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.uv, 0, _Alpha);
            }
            ENDCG
        }
    }
}