#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
struct SurfaceVariables {
    float smoothness;
    float shininess;
    float rimLightThreshold;
    float rimLightFlatness;
    float rimLightIntensity;
    float specularHighlightThreshold;
    float3 normalWS;
    float3 viewDir;
};

float3 CalculateCelShading(SurfaceVariables s, Light l);
#endif

void CelShading_float(float smoothness, float rimLightThreshold, float rimLightFlatness, float rimLightIntensity, float specularHighlightThreshold,float3 positionWS, float3 normalWS, float3 viewDir, out float3 color)
{
#ifdef SHADERGRAPH_PREVIEW
    color = float3(1, 1, 1);
#else

    SurfaceVariables surface = (SurfaceVariables)0;
    surface.smoothness = smoothness;
    surface.shininess = exp2(10 * smoothness + 1);
    surface.rimLightThreshold = rimLightThreshold;
    surface.rimLightFlatness = rimLightFlatness;
    surface.rimLightIntensity = rimLightIntensity;
    surface.specularHighlightThreshold = specularHighlightThreshold;
    surface.normalWS = normalWS;
    surface.viewDir = viewDir;

#if SHADOWS_SCREEN
    float4 positionCS = TranformWorldToHClip(positionWS);
    float4 shadowCoord = ComputeScreenPos(positionCS);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
#endif

    Light mainLight = GetMainLight(shadowCoord);
    color = CalculateCelShading(surface, mainLight);

    int additionalLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < additionalLightCount; i++)
    {
        Light additionalLight = GetAdditionalLight(i, positionWS, 1);
        float shadow = AdditionalLightRealtimeShadow(i, positionWS, additionalLight.direction);
        color += CalculateCelShading(surface, additionalLight) * additionalLight.distanceAttenuation * shadow;
    }
#endif

}

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateCelShading(SurfaceVariables s, Light l)
{
    float attenuation = l.shadowAttenuation;
    float diffuse = saturate(dot(l.direction, s.normalWS));
    diffuse *= attenuation;
    float specular = 0;

    float3 halfwayVector = SafeNormalize(l.direction + s.viewDir);
    specular = saturate(dot(s.normalWS, halfwayVector));
    specular = pow(specular, s.shininess);

    specular *= diffuse * s.smoothness;

    float rimLight = 1 - dot(s.viewDir, s.normalWS);

    rimLight = rimLight * pow(diffuse, s.rimLightFlatness) < s.rimLightThreshold ? 0 : rimLight;
    rimLight *= s.rimLightIntensity;
    // Toon shading
    specular = specular > s.specularHighlightThreshold ? specular : 0;
    diffuse = diffuse > 0 ? 0.5 : 0;
    diffuse = (diffuse > 0.50) && (diffuse != 0) ? diffuse : diffuse * 0.5;
    float lighting = (diffuse + max(specular, rimLight));
    
    return lighting * l.color;
}
#endif
#endif