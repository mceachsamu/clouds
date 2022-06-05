Shader "Unlit/underwater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaterLevel("water level", float) = 0.0
        _WaterFade("water fade", Range(0.0, 0.1)) = 0.0
        _WaterFadeExp("water fade exp", Range(0.0, 5.0)) = 0.0
        _MinLevel("min level", Range(-1.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform float _WaterLevel;
            uniform float _WaterFade;
            uniform float _WaterFadeExp;
            uniform float _MinLevel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float diff = _WaterLevel - i.wpos.y;

                float waterFade = pow(diff, _WaterFadeExp) + (diff * _WaterFade) + _MinLevel;

                return float4(waterFade, waterFade, waterFade, 1.0);
            }
            ENDCG
        }
    }
}
