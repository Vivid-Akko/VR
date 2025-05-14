#ifndef FOAM_HLSL
#define FOAM_HLSL

float4x4 MatrixInvVP;
float4x4 MatrixVP;

float SampleDepth(float2 uv){
    return tex2Dlod(_CameraDepthTexture,float4(uv,0,0)).r;
}



float GetFoamAtten(float3 positionWS,float2 screenUV,float foamScale,float4 screenPos)

{
    screenUV = screenPos.xy / screenPos.w;
    float safeW = max(screenPos.w, 1e-4);
    float depth = SampleDepth(screenUV);
    float backgroundDepth = LinearEyeDepth(depth, _ZBufferParams);

    float dis = saturate(backgroundDepth-safeW);
    return pow(max(0,1 - dis / lerp(0.1,1,foamScale)),3);
}



#endif