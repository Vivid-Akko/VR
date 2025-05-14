#ifndef LOOK_THROUGH_WATER_HLSL
#define LOOK_THROUGH_WATER_HLSL

sampler2D _CameraDepthTexture;

float ColorBelowWater(float2 screenUV, float4 screenPos)
{
    // 计算屏幕空间的深度值
    float2 uv = screenUV;
    float depth = tex2D(_CameraDepthTexture, uv).r; // 提取深度值 (r 通道)

    // 将深度值从非线性空间转换为线性空间
    float backgroundDepth = LinearEyeDepth(depth, _ZBufferParams);

    // 计算表面深度值
    float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);

    // 计算深度差异
    float depthDifference = backgroundDepth - surfaceDepth;

    // 返回标准化的深度差值
    return depthDifference / 20.0;
}


#endif