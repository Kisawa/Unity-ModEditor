Shader "Seasar/ToonScenes2"
{
    Properties
    {
		_Color("Color Tint", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_MainTex("Main Tex", 2D) = "white"{}
		[Header(Outline)]
		[Space(10)]
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0, 1)) = 0.001
		[Header(Shadow)]
		[Space(10)]
		_ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
		_ShadowRange("Shadow Range", Range(0, 0.5)) = 0.1
		_ShadowSmooth("Shadow Smooth", Range(0, 0.5)) = 0.1
		_ShadowBlend("Shadow Blend", Range(0, 0.5)) = 0
		[Header(AO)]
		[Space(10)]
		[Toggle]_OpenAO("Open Ambient Occlusion", int) = 0
		[NoScaleOffset]_AOMap("Ambient Occlusion Map", 2D) = "white"{}
		[Header(Snow)]
		[Space(10)]
		_SnowColor("Snow Color", Color) = (1, 1, 1, 1)
		_SnowScale("Snow Scale", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Pass{
			Tags{ "LightMode" = "Always" }
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
			};

			float4 vert(appdata v): SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(v.vertex);
				float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.tangent.xyz);
				float3 ndcNormal = normalize(TransformViewToProjection(viewNormal.xyz)) * pos.w;//将法线变换到NDC空间
				float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));//将近裁剪面右上角位置的顶点变换到观察空间
				float aspect = abs(nearUpperRight.y / nearUpperRight.x);//求得屏幕宽高比
				ndcNormal.x *= aspect;
				pos.xy += 0.01 * _OutlineWidth * ndcNormal.xy;
				return pos;

				//float depth = UnityObjectToClipPos(v.vertex).z;
				//float3 pos = v.vertex + v.tangent.xyz * _OutlineWidth * depth;
				//return UnityObjectToClipPos(pos);
			}

			fixed4 frag(float4 pos: SV_POSITION): SV_Target
			{
				return fixed4(_OutlineColor.xyz, 1);
			}
			ENDCG
		}

        Pass
        {
			Tags{ "LightMode" = "ForwardBase" }
            CGPROGRAM
			#pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
				float3 worldPos: TEXCOORD1;
				float3 worldNormal: TEXCOORD2;
				SHADOW_COORDS(3)
            };

			fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _ShadowColor;
			fixed _ShadowRange;
			fixed _ShadowSmooth;
			fixed _ShadowBlend;
			bool _OpenAO;
			sampler2D _AOMap;
			fixed4 _SnowColor;
			fixed _SnowScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv) * _Color;
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				fixed3 normal = normalize(i.worldNormal);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed diff = dot(normal, lightDir) * 0.5 + 0.5;
				fixed d = saturate(1 + _ShadowBlend - smoothstep(0.5 + _ShadowSmooth + 0.1, 0.5 - _ShadowSmooth, diff * atten - _ShadowRange + 0.5));
				fixed3 diffuse = lerp(_ShadowColor, color * _LightColor0, d);
				if(_OpenAO)
					diffuse *= tex2D(_AOMap, i.uv);
				fixed snow = dot(normal, fixed3(0, 1, 0));
				diffuse = lerp(diffuse, _SnowColor, snow * _SnowScale);
                return fixed4(diffuse, 1);
            }
            ENDCG
        }
    }

	Fallback "VertexLit"
}