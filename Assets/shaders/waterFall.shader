Shader "Unlit/waterFall"
{
    Properties
    {
        _MainTex ("main texture", 2D) = "white" {}
        _Texture ("water texture", 2D) = "white" {}
        _TextureFrequency("texture frequency", range(1, 100)) = 1.0
        _Displacement("displacement", range(0, 5)) = 1.0
        _UnderWaterTexture ("underwater texture", 2D) = "white" {}
        _Color ("surface color", Color) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        
        LOD 100

        Pass
        {
            Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }

            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha 
            CGPROGRAM
            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodynlightmap novertexlight
            
            #include "UnityCG.cginc"
			#include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;

                SHADOW_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Texture;
            float4 _Texture_ST;

            sampler2D _UnderWaterTexture;
            float4 _UnderWaterTexture_ST;

            uniform float4 _Color;
            uniform float _TextureFrequency;
            uniform float _Displacement;

            v2f vert (appdata_tan v)
            {
                v2f o;

                v.vertex.xz *= 1 + sin(_Time.x * 500 + v.vertex.z * 5000) * 0.005;
                v.vertex.xz *= 1 + sin(_Time.x * 300 + v.vertex.x * 5000) * 0.005;

                float4 height = tex2Dlod (_MainTex, float4(float2(v.texcoord.x, v.texcoord.y),0,0));
                v.vertex.xy *= 1.0 + height.r * _Displacement;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _Texture);
                o.screenPos = ComputeScreenPos(o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 waterTex = tex2D(_Texture, float2(i.uv.x, i.uv.y + _Time.x * 0.7) * _TextureFrequency);

                float2 renderTexUV = float2(i.screenPos.x + waterTex.r, i.screenPos.y + waterTex.r) / i.screenPos.w;

                fixed4 underwater = tex2D(_UnderWaterTexture, renderTexUV);
                fixed4 main = tex2D(_MainTex, i.uv);

                float4 shadow = SHADOW_ATTENUATION(i);
                shadow.a = 1.0;

                float4 col = _Color * (1.0 + underwater/2.0) * shadow;
                col.a = 1.0;

                return col + main / 2.0;
            }
            ENDCG
        }

        Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_shadowcaster

			#pragma vertex vert
			#pragma fragment frag

            #if !defined(MY_SHADOWS_INCLUDED)
            #define MY_SHADOWS_INCLUDED

            #include "UnityCG.cginc"

            uniform int _CastsShadows;

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            #if defined(SHADOWS_CUBE)
                struct Interpolators {
                    float4 position : SV_POSITION;
                    float3 lightVec : TEXCOORD0;
                };

                Interpolators vert (VertexData v) {
                    Interpolators i;
                    i.position = UnityObjectToClipPos(v.position);
                    i.lightVec = mul(unity_ObjectToWorld, i.position).xyz - _LightPositionRange.xyz;
                    return i;
                }

                float4 frag (Interpolators i) : SV_TARGET {
                    float depth = length(i.lightVec) + unity_LightShadowBias.x;
                    depth *= _LightPositionRange.w;
                    return 0.0;
                }
            #else
                float4 vert (VertexData v) : SV_POSITION {
                    float4 position =
                        UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
                    
                    return UnityApplyLinearShadowBias(position);
                }

                half4 frag () : SV_TARGET {
                    return 0.0;
                }
            #endif

            #endif
			ENDCG
		}
    }
}
