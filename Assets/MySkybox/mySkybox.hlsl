#ifndef MY_SKYBOX_INCLUDE
#define MY_SKYBOX_INCLUDE



struct Attributes
{
    float4 positionOS : POSITION;
    float3 normal : NORMAL;
    float3 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float3 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
};

// Properties
CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor;
    float4 _MainTex_ST;
    float _Roughness;
    float _OffsetHorizon;
    float _HorizonIntensity;
    float3 _SunSet;
    float3 _HorizonColor;
    float3 _SkyTopColor;
    float3 _SkyBottomColor;
    float3 _SkyMiddleColor;
    float _SunRadius;
    float3 _SunColor;
    float _SunFilling;
    float _SunIntensity;
    
CBUFFER_END

TEXTURE2D (_MainTex);
SAMPLER(sampler_MainTex);

Varyings vert(Attributes input)
{
    Varyings output;
    output.positionHCS = TransformObjectToHClip(input.positionOS);
    output.positionWS = TransformObjectToWorld(input.positionOS).xyz;
    output.uv = input.uv;
    output.normalWS = TransformObjectToWorldNormal(input.normal);
    output.normalWS = normalize(output.normalWS);
    return output;
}

half4 frag(Varyings input) : SV_Target
{
    //前期数据
    input.normalWS = normalize(input.normalWS);
    Light mainLight = GetMainLight();

    float3 lightDirWS = mainLight.direction;
    float3 lightColor = mainLight.color;
    float3 viewDirWS = GetWorldSpaceViewDir(input.positionWS);
    float3 halfVector = normalize(lightDirWS + viewDirWS);//半角向量

    float roughness = _Roughness*_Roughness;
    float squareRoughness = roughness*roughness;

    

    //对每个数据做限制，防止除0
    float NdotL = max(saturate(dot(input.normalWS, lightDirWS)),0.000001);
    float NdotV = max(saturate(dot(input.normalWS, viewDirWS)),0.000001);
    float NdotH = max(saturate(dot(input.normalWS, halfVector)),0.000001);
    float VdotH = max(saturate(dot(viewDirWS, halfVector)),0.000001);
    float IdotH = max(saturate(dot(lightDirWS, halfVector)),0.000001);
    
    //Horizon
    float horizon = (input.uv.y*_HorizonIntensity - _OffsetHorizon);
    
    float3 lDir = (lightDirWS+1)*0.5;

    
    float2 skyUV = float2(input.uv.x / input.uv.y,input.uv.z/input.uv.y);

    float3 Albedo = _BaseColor.rgb*SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, skyUV).rgb;

    float3 skyColor = lerp(lerp(_SkyBottomColor,_SkyMiddleColor,horizon),_SkyTopColor,horizon);

    //sun
    float sun = distance(input.uv.xyz, float4(lightDirWS,0.0));
    float sunDisc = 1- (sun/_SunRadius);
    sunDisc = saturate(sunDisc*_SunFilling);
    
    float3 finalColor = skyColor + sunDisc*_SunColor*_SunIntensity*lightColor;
    
    

    return float4(finalColor,1);
}



#endif