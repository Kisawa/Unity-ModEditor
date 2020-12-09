Shader "ModEditor/ShowDepth"
{
	Properties
	{
		_DepthCompress("Depth Compress", Range(0, 1)) = 0.1
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
				float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

			float _DepthCompress;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				COMPUTE_EYEDEPTH(o.depth);
				o.depth *= _DepthCompress;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.depth, i.depth, i.depth, 1);
            }
            ENDCG
        }
    }
}