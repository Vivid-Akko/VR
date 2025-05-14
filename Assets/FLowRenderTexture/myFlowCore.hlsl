#ifndef MY_FLOW_INCLUDE
#define MY_FLOW_INCLUDE

#ifndef unity_ColorSpaceDielectricSpec
#define unity_ColorSpaceDielectricSpec float4(0.04,0.04,0.04,0.75)
#endif

#pragma surface surf Standard fullforwardshadows
#pragma shader_feature _ADD_LIGHT_ON_ADD_LIGHT_OFF

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float4 screenPos : TEXCOORD3;
};

// Properties
CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor;
    float4 _MainTex_ST;
    float _Metallic;
    float _Roughness;
    float _FlowSpeed;
    float _UJump;
    float _VJump;
    float _Tiling;
    float _FlowStrength;
    float _FlowOffset;
    float _HeightScale,_HeightScaleModulate;
    float4 _WaveA,_WaveB,_WaveC;   
    float _WaveSpeed;
    float _WaveNormal;
    float _Blend;
    float _Distortion;
    float4 _SunColor;
    float _FoamScale;
CBUFFER_END

TEXTURE2D (_MainTex);
SAMPLER(sampler_MainTex);

sampler2D _FlowMap;
sampler2D _DerivHeightMap;
sampler2D _ReflectionTex;
sampler2D _FoamTex;


Varyings vert(Attributes input)
{
    Varyings output;
    //Wave
    float3 gridPoint = input.positionOS.xyz;
    float3 tangent = 0;
    float3 binormal = 0;
    float3 p = gridPoint;
    p += GerstnerWave(_WaveA,gridPoint,tangent,binormal,_WaveSpeed);
    p += GerstnerWave(_WaveB, gridPoint, tangent, binormal,_WaveSpeed);
    p += GerstnerWave(_WaveC, gridPoint, tangent, binormal,_WaveSpeed);
    //
    float3 normalWave = normalize(cross(binormal,tangent));
    input.positionOS.xyz = p; 
    output.positionHCS = TransformObjectToHClip(input.positionOS);
    output.positionWS = TransformObjectToWorld(input.positionOS).xyz;
    
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    output.normalWS = TransformObjectToWorldNormal(input.normal);
    output.normalWS = normalize(output.normalWS);
 
    output.normalWS = output.normalWS+normalWave*_WaveNormal ;
    output.screenPos = ComputeScreenPos(output.positionHCS);

    return output;
}

//光照函数

//正态分布函数D
float Distribution(float roughness , float NdotH)
{
    float lerpSquareRoughness = pow(lerp(0.002,1,roughness),2);
    float D = lerpSquareRoughness/(PI*pow(pow(NdotH,2)*(lerpSquareRoughness-1)+1,2));
    return D;
}

//几何函数G
float Geometry(float roughness , float NdotL , float NdotV)
{
    float k = pow(roughness+1,2)/8;//直接光照
    float GLeft = NdotL/lerp(NdotL,1,k);
    float GRight = NdotV/lerp(NdotV,1,k);
    float G = GLeft*GRight;
    return G;
}

//菲涅尔函数F
float3 Fresnel(float3 F0 , float VdotH)
{
    float3 F = F0 + (1-F0)*exp((-5.55473*VdotH-6.98316)*VdotH);
    return F;
}

//CubeMap采样等级Mip
float CubeMapMipLevel(float _Roughness)
{
    float mip_roughness = _Roughness*(1.7-0.7*_Roughness); 
    half mip = mip_roughness*UNITY_SPECCUBE_LOD_STEPS;
    return mip;
}

//间接光菲涅尔函数
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(float3(1,1,1)*(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

//多光源
real3 SahderSingleLight(Light light,half normalWS,half3 viewDirWS,bool isAdditionalLight)
{
    half NdotL = saturate(dot(normalWS, light.direction))*0.5+0.5;
    half3 diffsueCol = light.color * NdotL*light.distanceAttenuation*_BaseColor.rgb;
    half3 halfDir = normalize(light.direction + viewDirWS);
    half specColor = light.color * pow(saturate(dot(normalWS, halfDir)), _Roughness);
    half3 finalColor = diffsueCol + specColor;
}

half4 frag(Varyings input) : SV_Target
{
    
    
    //水体流动
    float2 jump = float2(_UJump,_VJump);
    float4 flow = tex2Dlod(_FlowMap,float4(input.uv,0,0)).rgba;
    float noise = flow.a; 
    
    flow.xy = flow.rg*2-1;
    flow *= _FlowStrength;
    float time = _Time.y*_FlowSpeed+noise;
    float3 uvwA = FlowUVW(input.uv,flow.xy,jump,_Tiling,_FlowOffset, time, true);
    float3 uvwB = FlowUVW(input.uv,flow.xy,jump,_Tiling,_FlowOffset, time, false);

    //DerivHeightMap
    float finalHeightScale = flow.z*_HeightScaleModulate+_HeightScale;

    float3 dhA = UnpackDerivativeHeight(tex2Dlod(_DerivHeightMap,float4(uvwA.xy,0,0))).rgb*uvwA.z* finalHeightScale ;
    float3 dhB = UnpackDerivativeHeight(tex2Dlod(_DerivHeightMap,float4(uvwB.xy,0,0))).rgb*uvwB.z* finalHeightScale ;
    float3 normal = normalize(float3(-(dhA.xy+dhB.xy),1));
    input.normalWS = normalize(input.normalWS+float3(normal.x,0,normal.y));

    

    //前期数据
    //input.normalWS = normalize(input.normalWS);
    Light mainLight = GetMainLight();
    float3 lightDirWS = normalize(mainLight.direction);
    float3 lightColor = mainLight.color;

    
    uint lightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0; lightIndex < lightCount; ++lightIndex) {
        Light light = GetAdditionalLight(lightIndex, input.positionWS);

        half additionalNdotL = saturate(dot(input.normalWS, light.direction))*0.5+0.5;
        half3 diffsueCol = light.color * additionalNdotL*light.distanceAttenuation;

        //float3 attenuatedLightColor = light.color * (light.distanceAttenuation) * saturate(dot(light.direction, input.normalWS));
        lightColor += lightColor*diffsueCol;
    }

    
    float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.positionWS));
    float3 halfVector = normalize(lightDirWS + viewDirWS);//半角向量

    float roughness = _Roughness*_Roughness;
    float squareRoughness = roughness*roughness;

    float2 screenUV = input.positionHCS.xy * (_ScreenParams.zw - 1);


    //基础颜色
    float3 texA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvwA.xy).rgb* uvwA.z;
    float3 texB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvwB.xy).rgb* uvwB.z;
    //float3 Albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb*_BaseColor;
    float3 Albedo = _BaseColor.rgb*(texA+texB);
    //float3 Albedo = ColorBelowWater(screenUV,input.screenPos);
    //Albedo = float3(1,1,1);
    


    //对每个数据做限制，防止除0
    float NdotL = max(saturate(dot(input.normalWS, lightDirWS)),0.000001);
    float NdotV = max(saturate(dot(input.normalWS, viewDirWS)),0.000001);
    float NdotH = max(saturate(dot(input.normalWS, halfVector)),0.000001);
    float VdotH = max(saturate(dot(viewDirWS, halfVector)),0.000001);
    float IdotH = max(saturate(dot(lightDirWS, halfVector)),0.000001);
    
    //直接光照_镜面反射项
    float D = Distribution(roughness,NdotH);
    float G = Geometry(roughness,NdotL,NdotV);
    float3 F0 = lerp(unity_ColorSpaceDielectricSpec.rgb,Albedo,_Metallic);
    float3 F = Fresnel(F0,VdotH);

    float3 specularBRDF = (D*G*F)/(4*NdotL*NdotV);
    float3 specColor = _SunColor*specularBRDF*NdotL*PI;
    specColor = saturate(specColor);

    //直接光照_漫反射项
    float3 kd = (1-F)*(1-_Metallic);
    float3 diffuseBRDF = kd/PI;
    float3 diffuseColor = diffuseBRDF*Albedo*_SunColor*NdotL*PI;

    float3 directColor = specColor+diffuseColor;

    //间接光照_镜面反射项
    float mip = CubeMapMipLevel(_Roughness);
    float3 reflectVec = reflect(-viewDirWS,input.normalWS);
    half4 rgmb = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0,samplerunity_SpecCube0,reflectVec,mip);//采样CubeMap
    float3 iblSpecular = DecodeHDREnvironment(rgmb,unity_SpecCube0_HDR).rgb; //HDR解码

    half surfaceReducation = 1.0/(roughness*roughness+1.0);//非金属反射减少

    float oneMinusReflectivity = unity_ColorSpaceDielectricSpec.a*(1-_Metallic);
    half3 grazingTerm = saturate((1.0 - _Roughness) + (1 -oneMinusReflectivity));
    float NdotVPow5 = pow((1-NdotV),5);
    float3 FresnelLerp = lerp(F0,grazingTerm,NdotVPow5);
    
    float3 iblSpecularColor = surfaceReducation*FresnelLerp*iblSpecular;

    //间接光照_漫反射项
    half3 iblDiffuse = SampleSH(input.normalWS);//球谐光照
    float3 Flast = fresnelSchlickRoughness(max(NdotV,0.0),F0,roughness); 
    float kdLast = (1-Flast)*(1-_Metallic);//边缘反射减少

    float3 iblDiffuseColor = iblDiffuse*kdLast*Albedo;

    float3 indirectColor = iblSpecularColor+iblDiffuseColor;

    //最终颜色
    float3 finalColor = directColor+indirectColor;
    //float2 FlowVector = tex2Dlod(_FlowMap,float4(input.uv,0,0)).rg*2-1;
    //finalColor = float3(FlowVector,0);
    

    //反射
    float2 offset = normal.xy*_Distortion;

    float3 ReflectionColor = tex2Dlod(_ReflectionTex,float4(( input.screenPos.x / input.screenPos.w + offset.x/input.screenPos.w ),(input.screenPos.y / input.screenPos.w - offset.y/input.screenPos.w),0,0)).rgb;
    
    finalColor = lerp(finalColor,ReflectionColor,_Blend);


    //白沫
    float foamAtten = GetFoamAtten(input.positionWS,screenUV + normal.xz * 0.1,_FoamScale,input.screenPos);
    //为白沫贴图增加UV扰动
    float2 foamUV = (input.uv + time * float2(0.01,0.01) + normal.xz * 0.005) * 30;
    float foamDiffuse = tex2Dlod(_FoamTex,float4(foamUV,0,0)).r;
    half3 foamTerm = foamDiffuse;
    
    finalColor = lerp(finalColor,foamTerm,foamAtten*foamDiffuse);
    return float4(finalColor,1.0);
}




#endif