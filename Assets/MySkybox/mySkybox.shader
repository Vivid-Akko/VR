Shader "Custom/mySkybox"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Roughness("Roughness", Range(0,1)) = 0.5

        //Horizon
        _OffsetHorizon("Offset Horizon", Range(-1, 1)) = 0
        _HorizonIntensity("Horizon Intensity", Range(0, 10)) = 0.5
        
        _SunSet("太阳升起颜色", Color) = (1,1,1,1)
        _HorizonColor("Horizon Color", Color) = (1,1,1,1)
        
        //Sky
        _SkyTopColor("Sky Top Color", Color) = (1,1,1,1)
        _SkyMiddleColor("Sky Middle Color", Color) = (1,1,1,1)
        _SkyBottomColor("Sky Bottom Color", Color) = (1,1,1,1)

        //Sun
        _SunColor("Sun Color", Color) = (1,1,1,1)
        _SunRadius("Sun Radius", Range(0, 1)) = 0.5
        _SunFilling("Sun Filling", Range(0, 10)) = 0.5
        _SunIntensity("Sun Intensity", Range(0, 10)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipline" = "UniversalPipeline" "IgnoreProjector" = "True" "PreviewType" = "Skybox" }
        LOD 100
        Pass
        {
            Name "SkyBox"
            

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag   
            #pragma shader_feature FUZZY
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include "mySkybox.hlsl"
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
