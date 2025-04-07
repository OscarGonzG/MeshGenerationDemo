#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// Material properties
float _BandDivider;
float4 _Color;
float4 _ShadowColor;
float _Brightness;
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

// UV tiling
float4 _MainTex_ST;

struct VertexInput
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
};

struct VertToFrag
{
    float4 positionCS : SV_POSITION;

    float3 positionWS : TEXCOORD0;
    float3 positionOS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float3 normalOS : TEXCOORD3;
};


VertToFrag Vertex(VertexInput input)
{
    VertToFrag output;
    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
    
    VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS); 
    
    output.positionCS = posInputs.positionCS;
    output.positionWS = posInputs.positionWS + normalInputs.normalWS * 0.035f;
    output.positionOS = input.positionOS;
    output.normalWS = normalInputs.normalWS;
    output.normalOS = input.normalOS;
    return output;
}

float4 Fragment(VertToFrag input) : SV_TARGET
{
    uint lightCount = GetAdditionalLightsCount();
    float4 finalColor = float4(0, 0, 0, 1);

    half lightIntensity = 0;
    half totalLightIntensity = 0;

    // Calculates light intensity
    for (uint i = 0; i < lightCount; i++)
    {
        Light light = GetAdditionalLight(i, input.positionWS);
        lightIntensity += (dot(light.direction, input.normalWS)) / (length(light.direction) * length(input.normalWS)) + 1;
        lightIntensity *= AdditionalLightRealtimeShadow(i, input.positionWS, light.direction);
        totalLightIntensity += lightIntensity;
    }
    totalLightIntensity *= 0.5;
    totalLightIntensity /= _BandDivider;
    totalLightIntensity = floor(totalLightIntensity) * _BandDivider;

    // Samples texture
    float3 textureSampleCoords = frac(input.positionOS);
    float4 sampleX = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, textureSampleCoords.yz);
    float4 sampleY = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, textureSampleCoords.xz);
    float4 sampleZ = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, textureSampleCoords.xy);
    float3 weights = input.normalOS;
    float4 textureSample = sampleX * weights.x + sampleY * weights.y + sampleZ * weights.z;

    return (_Color * (textureSample * _Brightness)) * totalLightIntensity + _ShadowColor * textureSample * floor(1 - totalLightIntensity);
}