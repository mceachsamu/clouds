Shader "Unlit/smoke"
{
     Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _NormalMap("normal map", 2D) = "white" {}
        _UseNormalMap("Use normal map", int) = 0.0
        _NormalFrequency("normal frequency", range(1.0,100.0)) = 1.0
        _NormalStrength("normal strength", range(0.0, 1.0)) = 1.0

        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
        _Saturation("Saturation", range(0,1)) = 1.0

        _Transparency("transparenct", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        LOD 100

        Pass
        {

            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" "Queue" = "Transparent" "RenderType"="Transparent"}
            // Cull Off
            ZWrite On
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha 
            CGPROGRAM
            #pragma target 3.0

            #include "cellShading.cginc"

            #pragma multi_compile_shadowcaster
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 customData : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                half3 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z
                float4 smokeColor : TEXCOORD7;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            uniform float _NormalFrequency;
            uniform float _NormalStrength;

            uniform float _Glossiness;
            uniform float4 _SpecularColor;
            uniform float4 _RimColor;
            uniform float _RimAmount;
            uniform float4 _AmbientColor;
            uniform int _UseNormalMap;
            uniform float _Saturation;
            uniform float _Transparency;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal =  UnityObjectToWorldNormal(v.normal);
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(o.worldNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, o.worldNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, o.worldNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, o.worldNormal.z);

                o.smokeColor = v.customData;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half2 uv_NormalMap = TRANSFORM_TEX (i.uv, _NormalMap);

                half3 tnormal = UnpackNormal(tex2D(_NormalMap, uv_NormalMap * _NormalFrequency)) * _NormalStrength;
                 // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);
                worldNormal *= _NormalStrength;

                worldNormal += i.worldNormal;
                worldNormal = normalize(worldNormal);
                //check if we should disabled normal mapping
                if (!_UseNormalMap){
                    worldNormal = i.worldNormal;
                }

                float4 col = i.smokeColor;

                //apply saturation
                col.rgb = col.rgb * _Saturation;

                float4 shading = GetCellShading(i.wpos, _WorldSpaceLightPos0.xyzw, worldNormal, i.viewDir, col, _LightColor0, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                shading.a = _Transparency;
                return shading * col;
            }
            ENDCG
        }

        Pass {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }

            CGPROGRAM
            #pragma target 3.0

            #include "cellShading.cginc"

            #pragma multi_compile_shadowcaster
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                half3 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z
                SHADOW_COORDS(7)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            uniform int _UseColor;
            uniform float _NormalFrequency;

            uniform float4 _Color;
            uniform float _Glossiness;
            uniform float4 _SpecularColor;
            uniform float4 _RimColor;
            uniform float _RimAmount;
            uniform float4 _AmbientColor;
            uniform int _UseNormalMap;
            uniform float _Saturation;

            v2f vert (appdata_tan v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal =  UnityObjectToWorldNormal(v.normal);
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(o.worldNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, o.worldNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, o.worldNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, o.worldNormal.z);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half2 uv_NormalMap = TRANSFORM_TEX (i.uv, _NormalMap);

                half3 tnormal = UnpackNormal(tex2D(_NormalMap, uv_NormalMap * _NormalFrequency));
                 // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);

                //check if we should disabled normal mapping
                if (!_UseNormalMap){
                    worldNormal = i.worldNormal;
                }

                fixed4 col = tex2D(_MainTex, i.uv);
                if(_UseColor == 1){
                    col = _Color;
                }

                //apply saturation
                col.rgb = col.rgb * _Saturation;

                float4 shading = GetCellShading(i.wpos, _WorldSpaceLightPos0.xyzw, worldNormal, i.viewDir, col, _LightColor0, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                float shadow = SHADOW_ATTENUATION(i);
                col.xyz *= shadow;
                return col * shading;
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
                    if (i.position.x > 0.5){
                        return 0.0;
                    }
                    return UnityEncodeCubeShadowDepth(depth);
                }
            #else
                float4 vert (VertexData v) : SV_POSITION {
                    float4 position =
                        UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
                    return UnityApplyLinearShadowBias(position);
                }

                half4 frag () : SV_TARGET {
                    return 0;
                }
            #endif

            #endif
			ENDCG
		}
    }
}
