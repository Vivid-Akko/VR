Shader "Custom/FlowShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _ReflectionTex("Reflection", 2D) = "white" {}
        _Distortion("Distortion", Range(0,1)) = 0.5
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Roughness("Roughness", Range(0,1)) = 0.5
        _SunColor("Sun Color", Color) = (1,1,1,1)
        //Gamma矫正
        [Gamma]_Metallic("Metallic", Range(0,1)) = 0.5
        _Blend("Blend", Range(0,1)) = 0.5

        [Space(10)]
        _FlowMap("Flow Map", 2D) = "white" {}
        
        _UJump("U Jump", Range(-0.25, 0.25)) = 0.25
        _VJump("V Jump", Range(-0.25, 0.25)) = 0.25
        _Tiling("Tiling", Range(0, 100)) = 1
        _FlowSpeed("Flow Speed", Range(0, 1)) = 0.5
        _FlowStrength("Flow Strength", Range(0, 2)) = 0.5
        _FLowOffest("Flow Offset", Range(-1, 1)) = 0.5

        [Space(10)]
        _DerivHeightMap("Deriv(AG) Height(B) Map", 2D) = "black" {}
        _HeightScale("Height Scale, Constant", Range(0, 1)) = 0.5
        _HeightScaleModulate("Height Scale, Modulate", Range(0, 10)) = 5

        [Space(10)]
        _WaveSpeed("Wave Speed", Range(0, 1)) = 0.5
        _WaveNormal("Wave Normal", Range(0, 1)) = 0.5
        _WaveA("Wave A(dir, steepness, wvaelength)", Vector) = (1, 0, 0.5, 10)
        _WaveB ("Wave B", Vector) = (0,1,0.25,20)
        _WaveC ("Wave C", Vector) = (1,1,0.15,10)

        [Space(10)]
        _FoamTex("Foam Texture", 2D) = "white" {}
        _FoamScale("Foam Scale", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag   
            #pragma multi_compile
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            

            
             
            #include "LookThroughWater.hlsl"    
            #include "FlowRenderCore.hlsl"
            #include "Foaming.hlsl"
            #include "myFlowCore.hlsl"
            

            
            ENDHLSL
        }
    }
    //FallBack "Diffuse"
}
