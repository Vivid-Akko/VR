Shader "PostShader" {
    Properties {
        [HideInInspector]_BlitTexture ("Base (RGB)", 2D) = "white" {}


        _FogColor("FogColor",Color) = (0,0,0,0)
        _SkyFogColor("SkyFogColor",Color) = (0,0,0,0)
        _FogDensity("FogDensity",Range(0,10)) = 0.1
        _FogHeightRange("FogHeightRange",Vector) = (0,100,0,0)
        
        _PaperTex ("Paper (RGB)", 2D) = "white" {}
		_Num("Num", range(0,100)) = 3
    }
    SubShader {
        Pass {
            ZTest Off
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag_m
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            
            


            half _PixelSize;
            half4 _FogColor;
            half4 _SkyFogColor;
            

            sampler2D _CameraDepthTexture;
            
            float _FogDensity;
            float4x4 _InverseView;
            float4 _FogHeightRange;
            float _IsDepth;


            float4 _BlitTexture_TexelSize;
            half _Num;
			sampler2D _PaperTex;
            
			

            half4 frag_m(Varyings i):SV_TARGET {
                
                half4 col = half4(1, 1, 1, 1);
                
                float2 texelSize = _BlitTexture_TexelSize;

                float2 uv = i.texcoord;

                col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv); //采样的Blit图
                
                float depth = tex2D(_CameraDepthTexture, uv).r;
                depth = Linear01Depth(depth,_ZBufferParams);
                
                
                
        


                int x=0;
				int y=0;
				int numPow = _Num * _Num;
				//右上
				half3 rightUpOnes = 0;
				half3 rightUpSobel = 0;
				half3 rightUpCol = 0;

				for(x=0; x<_Num; x++){
					half2 righUp = uv.xy -half2(_BlitTexture_TexelSize.x, _BlitTexture_TexelSize.y);
					righUp += half2(_BlitTexture_TexelSize.x*(x+1),0);
					for(y=0; y<_Num; y++){
						righUp += half2(0,_BlitTexture_TexelSize.y);
						rightUpOnes += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, righUp).rgb;
						rightUpCol +=rightUpOnes;
						rightUpSobel += rightUpOnes * rightUpOnes;
					}
				}

				rightUpSobel =  rightUpSobel/numPow - pow(rightUpCol/numPow,2);//平方和的平均值-平均值的平方 = 方差
				half finalrightUpSobel = rightUpSobel.r+rightUpSobel.g+rightUpSobel.b;//将方差的rgb通道合并
				
				//左上
                half3 Left_upOnes = 0;
                half3 Left_upSobel=0;
                half3 Left_upCol=0;
                
                for(x=0;x<_Num;x++){
                    half2 Left_up = uv.xy + half2(_BlitTexture_TexelSize.x,-_BlitTexture_TexelSize.y);
                    Left_up -= half2(_BlitTexture_TexelSize.x*(x+1),0);
                    for(y=0;y<_Num;y++){
                        Left_up  += half2(0, _BlitTexture_TexelSize.y);
                        Left_upOnes = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, Left_up).rgb;
                        Left_upCol += Left_upOnes;
                        Left_upSobel += pow(Left_upOnes,2);
                    }
                }
                Left_upSobel =  Left_upSobel/numPow - pow(Left_upCol/numPow,2);
                half finalLeft_upSobel = Left_upSobel.r+Left_upSobel.g+Left_upSobel.b;

                //右下
                half3 right_downOnes = 0;
                half3 right_downSobel = 0;
                half3 right_downCol = 0;
                
                for(x=0;x<_Num;x++){
                    half2 right_down = uv.xy+half2(-_BlitTexture_TexelSize.x,_BlitTexture_TexelSize.y);
                    right_down +=half2(_BlitTexture_TexelSize.x*(x+1),0);
                    for(y=0;y<_Num;y++){
                        right_down  -= half2(0, _BlitTexture_TexelSize.y);
                        right_downOnes = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, right_down).rgb;
                        right_downCol += right_downOnes;
                        right_downSobel += pow(right_downOnes,2);
                    }
                }
                right_downSobel =  right_downSobel/numPow - pow(right_downCol/numPow,2);
                half finalright_downSobel = right_downSobel.r + right_downSobel.g + right_downSobel.b;
                //左下
                half3 Left_downOnes = 0;
                half3 Left_downSobel=0;
                half3 Left_downCol=0;
                
                for(x=0;x<_Num;x++){
                    half2 Left_down = uv.xy+half2(_BlitTexture_TexelSize.x,_BlitTexture_TexelSize.y);
                    Left_down += half2(_BlitTexture_TexelSize.x*(x+1),0);
                    for(y=0;y<_Num;y++){
                        Left_down  -= half2(0, _BlitTexture_TexelSize.y);
                        Left_downOnes = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, Left_down).rgb;
                        Left_downCol += Left_downOnes;
                        Left_downSobel += pow(Left_downCol,2);
                    }
                }

                Left_downSobel =  Left_downSobel/numPow - pow(Left_downCol/numPow,2);

                half finalLeft_downSobel = Left_downSobel.r+Left_downSobel.g+Left_downSobel.b;
				
				half minSobel = min(min(finalLeft_downSobel,finalright_downSobel),min(finalLeft_upSobel,finalrightUpSobel));

				if(minSobel==finalLeft_downSobel){
                    col.rgb = Left_downCol/numPow;
                }else if(minSobel==finalright_downSobel){
                    col.rgb = right_downCol/numPow;
                }else if(minSobel==finalLeft_upSobel){
                    col.rgb = Left_upCol/numPow;
                }else{
                    col.rgb = rightUpCol/numPow;
                }
                
                half3 paper = tex2D(_PaperTex, uv).rgb;
                col.rgb = col;


                //从深度重建世界空间位置
                float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
                float3 viewPos = float3((i.texcoord * 2 - 1) / p11_22, -1) * depth * _ProjectionParams.z;
                float4 worldPos = mul(_InverseView, float4(viewPos, 1)); //_ViewToWorld
                
                float fog =1;

                float4 FogColor;
                
                
                    fog = saturate(depth*pow(2,_FogDensity));
                    if (depth > 0.9) {
                        fog = fog/5;
                        FogColor = _SkyFogColor;
                    }
                    else{
                        fog = fog/2;
                        FogColor = _FogColor;
                    }
                    col = lerp (col, FogColor , fog);

                
               
                
                return col ;
            }
            ENDHLSL
        }
    }
}