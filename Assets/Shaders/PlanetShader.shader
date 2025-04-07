Shader "Custom/PlanetShader"
{
    Properties
    {
        [Header(Surface settings)]
        [MainColor] _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
        _BandDivider("Band Divider", Float) = 0.4
        [Header(Texture settings)]
        [MainTexture] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Brightness("Brightness", Float) = 0
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            Name "ForwardLit" // For debugging
            Tags{"LightMode" = "UniversalForward"}
            LOD 200

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            // _MAIN_LIGHT_SHADOWS_CASCADE implies that _MAIN_LIGHT_SHADOWS is on
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            // We can save compile time by signaling that the _SHADOWS_SOFT keyword is
            // only used in the fragment stage
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #define _ADDITIONAL_LIGHTS
            #define _ADDITIONAL_LIGHT_SHADOWS

            #include "PlanetShaderForwardLitPass.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "PlanetShaderShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
}
