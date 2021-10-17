float4 getBaseLighting(float3 normC, float3 normal, float3 lightDir) {
    float NdotL = saturate(dot(normC , lightDir));

    float overall = smoothstep(0.0, 0.1, NdotL);
    if (overall < 0.0){
        overall = 0.5;
    }
    if (overall > 0.0){
        overall = 1.0;
    }

    return overall * _Color;
}

float4 getSpecularLighting(float3 lightDir, float3 normal, float3 viewDir) {
    float3 R = 2.0 * (normal) * dot((normal), normalize(lightDir)) - normalize(lightDir);
    float spec = pow(max(0, dot(R, normalize(viewDir))), _Glossiness);

    spec = smoothstep(0.005, 0.01, spec);

    return spec * _SpecularColor;
}

float4 getRimLighting(float3 normal, float3 viewDir) {
    float rimDot = clamp((1.0 - dot(normalize(viewDir), normalize(normal))), -50.0, 50.0);

    float rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);

    return rimIntensity * _Color;
}

float4 getBackLighting(float3 normal, float3 viewDir, float3 lightDir) {
    float b = dot(normalize(viewDir), -normalize(lightDir - normal * _BackLightNormalStrength));

    return b * _BacklightColor * _BackLightStrength;
}


float4 getLighting(float3 normal, float3 normalMapNormal, float3 viewDir)
{
    // we want to combine the mesh normal and the normal map normal for specific lighting
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

    float3 normC = normalize(normal + normalMapNormal);

    float4 baseColor = getBaseLighting(normC, normal, lightDir);

    float4 specularColor = getSpecularLighting(lightDir, normC, viewDir);

    float4 rimColor = getRimLighting(normal, viewDir);

    float4 backLighting = getBackLighting(normC, viewDir, lightDir);

    float4 col = _AmbientColor;
    col += baseColor;
    col += _Color;
    col += specularColor;
    col += rimColor;
    col += backLighting;
    return col * _Saturation;
}