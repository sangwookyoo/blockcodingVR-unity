// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Particles/Alpha Blended + Add"
{
Properties
{
	cTintColorv4 ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	tMainTex2D ("Texture", 2D) = "white" {}
	fSoftFactor ("Soft Factor", Range(0.01,3.0)) = 1.0
}

Category
{
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }	
	ColorMask RGB
	Cull Off
	Lighting Off
	ZWrite Off
	Fog { Color (0,0,0,0) }	
	SubShader
	{
		Pass
			{
				Name "ADDITIVE"
				
				Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
				Blend SrcAlpha One				
				ColorMask RGB
				Cull Off
				Lighting Off
				ZWrite Off
				Fog { Color (0,0,0,0) }				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles				
				#include "UnityCG.cginc"				
				sampler2D tMainTex2D;
				fixed4 cTintColorv4;
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD1;
					#endif
				};
				
				float4 tMainTex2D_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
					o.projPos = ComputeScreenPos (o.vertex);
					COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,tMainTex2D);
					return o;
				}
				
				sampler2D _CameraDepthTexture;
				float fSoftFactor;
				
				fixed4 frag (v2f i) : COLOR
				{
					#ifdef SOFTPARTICLES_ON
					float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					float partZ = i.projPos.z;
					float fade = saturate (fSoftFactor * (sceneZ-partZ));
					i.color.a *= fade;
					#endif
					
					return 2.0 * i.color * cTintColorv4 * tex2D(tMainTex2D, i.texcoord).a;
				}
				ENDCG 
			}
		Pass
			{
				Name "ALPHA_BLENDED"
				
				Tags { "Queue"="Transparent-5" "IgnoreProjector"="True" "RenderType"="Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha				
				ColorMask RGB
				Cull Off
				Lighting Off
				ZWrite Off
				Fog { Color (0,0,0,0) }				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles				
				#include "UnityCG.cginc"
				#define TEST				
				sampler2D tMainTex2D;
				fixed4 cBaseColorV4;
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD1;
					#endif
				};
				
				float4 tMainTex2D_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
					o.projPos = ComputeScreenPos (o.vertex);
					COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,tMainTex2D);
					return o;
				}
				
				sampler2D _CameraDepthTexture;
				float fSoftFactor;
				
				fixed4 frag (v2f i) : COLOR
				{
					#ifdef SOFTPARTICLES_ON
					float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					float partZ = i.projPos.z;
					float fade = saturate (fSoftFactor * (sceneZ-partZ));
					i.color.a *= fade;
					#endif
					
					fixed alpha = tex2D(tMainTex2D, i.texcoord).a * i.color.a * cBaseColorV4.a * 2.0;
					return fixed4(2.0 * cBaseColorV4.rgb, alpha);
				}
				ENDCG 
			}
	}	
}
}
