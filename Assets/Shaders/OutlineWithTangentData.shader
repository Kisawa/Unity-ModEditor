Shader "ModEditor/OutlineWithTangentData"{
	Properties{
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Main Tex", 2D) = "white"{}
		_Ramp("Ramp Tex", 2D) = "white"{}
		_Outline("Outline", Range(0, 0.1)) = 0.02
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_Specular("Specular", Color) = (1, 1, 1, 1)
		_SpecularScale("Specular Scale", Range(0, 0.1)) = 0.01
	}

	SubShader{
		Pass{
			NAME "OUTLINE"
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed _Outline;
			fixed4 _OutlineColor;

			float4 vert(float4 vertex: POSITION, float4 tangent: TANGENT): SV_POSITION{
				float3 pos = UnityObjectToViewPos(vertex);
				float3 v_normal = mul((float3x3)UNITY_MATRIX_IT_MV, tangent.xyz);
				v_normal.z = 0.5;
				pos = pos + normalize(v_normal) * _Outline;
				return UnityViewToClipPos(pos);
			}

			fixed4 frag(float4 pos: SV_POSITION): SV_Target{
				return fixed4(_OutlineColor.xyz, 1);
			}
			ENDCG
		}

		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			sampler2D _Ramp;
			fixed4 _Specular;
			fixed _SpecularScale;

			struct frag_data{
				float4 pos: SV_POSITION;
				float2 uv: TEXCOORD0;
				float3 worldNormal: TEXCOORD1;
				float3 worldPos: TEXCOORD2;
				SHADOW_COORDS(3)
			};

			frag_data vert(float4 vertex: POSITION, float2 texcoord: TEXCOORD0, float3 normal: NORMAL){
				frag_data o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uv = TRANSFORM_TEX(texcoord, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(normal);
				o.worldPos = mul(unity_ObjectToWorld, vertex);
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(frag_data i): SV_Target{
				fixed3 normal = normalize(i.worldNormal);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 albedo = tex2D(_MainTex, i.uv) * _Color;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT * albedo;
				fixed diff = dot(normal, lightDir) * 0.5 + 0.5;
				fixed3 diffuse = _LightColor0 * albedo * tex2D(_Ramp, float2(diff, diff));
				fixed spec = dot(normal, normalize(lightDir + viewDir));
				fixed w = fwidth(spec);
				fixed3 specular = _Specular * smoothstep(-w, w, spec + _SpecularScale - 1) * step(0.0001, _SpecularScale);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				return fixed4(ambient + (diffuse + specular) * atten, 1);
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}