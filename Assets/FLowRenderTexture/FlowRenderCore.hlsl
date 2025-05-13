#ifndef FLOW_RENDER_CORE_INCLUDED
#define FLOW_RENDER_CORE_INCLUDED



float3 FlowUVW(float2 uv,float2 flowVector ,float2 jump,float tiling,float flowOffest, float time,bool flowB)
{
	float phaseOffest = flowB ? 0.5 : 0;
    float progress = frac(time + phaseOffest);
    float3 uvw;
	uvw.xy = uv - flowVector * (progress+flowOffest);
	uvw.xy *= tiling;
	uvw.xy +=  phaseOffest;
	uvw.xy += (time -progress) * jump;
	uvw.z = 1 - abs(1 - 2 * progress);
	return uvw;
    
}

float3 UnpackDerivativeHeight (float4 textureData)
{
	float3 derivativeHeight = textureData.agb;
	derivativeHeight.xy = derivativeHeight.xy * 2 - 1;
	return derivativeHeight;
}

//Wave
float3 GerstnerWave(float4 wave ,float3 p ,inout float3 tangent ,inout float3 binormal,float waveSpeed)
{
	float steepness = wave.z;
	float wavelength = wave.w;
	float k = 2 * PI / wavelength;
	float c = sqrt(9.8 / k)*waveSpeed;
	float2 d = normalize(wave.xy);
	float f = k * (dot(d, p.xz) - c * _Time.y);
	float a = steepness / k;

	tangent += float3(
		-d.x * d.x * (steepness * sin(f)),
		d.x*(steepness*cos(f)), 
		-d.x * d.y * (steepness * sin(f))
		);
	binormal += float3(
		-d.x * d.y * (steepness * sin(f)),
		d.y*(steepness*cos(f)), 
		-d.y * d.y * (steepness * sin(f))
		);

	return float3(d.x * a * cos(f), a * sin(f), d.y * a * cos(f));
}


#endif