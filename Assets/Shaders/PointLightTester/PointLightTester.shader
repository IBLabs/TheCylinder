Shader "Custom/URPPointLightEffectShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _LightPosition ("Light Position", Vector) = (0,1,0)
        _LightRadius ("Light Radius", Float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float3 _LightPosition;
                float _LightRadius;
            CBUFFER_END

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.worldPos = TransformObjectToWorld(input.positionOS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.screenPos = TransformWorldToHClip(output.worldPos);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 lightDir = input.worldPos - _LightPosition;
                float distance = length(lightDir);

                // Get depth value for the current pixel
                float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.screenPos.xy).r;
                float pixelDepth = input.screenPos.z / input.screenPos.w;

                // Adjust distance based on depth
                float depthDifference = abs(pixelDepth - sceneDepth);
                float adjustedDistance = distance + depthDifference * 0.1;

                // Check if the pixel is within the light radius
                float3 color = _BaseColor.rgb;
                if (adjustedDistance <= _LightRadius)
                {
                    color *= 2.0;
                }

                return half4(color, _BaseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
