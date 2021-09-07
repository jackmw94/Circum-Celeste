Shader "ShimmerMaskable"
    {
        Properties
        {
            ShimmerPosition("ShimmerPosition", Float) = -0.3
            Vector1_2faa617f2f02433ca0bc7592c5c611e6("ShimmerWidth", Float) = 0.05
            Color_1f42b50a332d4845a92cf3bc710ac724("BehindColour", Color) = (1, 1, 1, 1)
            Color_65f6165abced4cb986c6244544f3c5cc("ShimmerColour", Color) = (0, 0.3096058, 1, 1)
            Color_9d92da585efe4621b683115dc43ff927("AheadColour", Color) = (0.9514793, 0.5509434, 1, 1)
            [NoScaleOffset]Texture2D_3a58e9e937e94cf598dc09e567577ad6("Texture2D", 2D) = "white" {}
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
			
			_StencilComp ("Stencil Comparison", Float) = 8
			_Stencil ("Stencil ID", Float) = 0
			_StencilOp ("Stencil Operation", Float) = 0
			_StencilWriteMask ("Stencil Write Mask", Float) = 255
			_StencilReadMask ("Stencil Read Mask", Float) = 255
			_ColorMask ("Color Mask", Float) = 15
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Transparent"
                "UniversalMaterialType" = "Lit"
                "Queue"="Transparent"
            }
			Stencil
			{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask 255
				WriteMask 255
			}
			
			ColorMask [_ColorMask]
			
            Pass
            {
                Name "Sprite Lit"
                Tags
                {
                    "LightMode" = "Universal2D"
                }
    
                // Render State
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite Off
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
                #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
                #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
                #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3
                // GraphKeywords: <None>
    
                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_COLOR
                #define VARYINGS_NEED_SCREENPOSITION
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITELIT
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float4 texCoord0;
                    float4 color;
                    float4 screenPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float4 interp0 : TEXCOORD0;
                    float4 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyzw =  input.texCoord0;
                    output.interp1.xyzw =  input.color;
                    output.interp2.xyzw =  input.screenPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.interp0.xyzw;
                    output.color = input.interp1.xyzw;
                    output.screenPosition = input.interp2.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float ShimmerPosition;
                float Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                float4 Color_1f42b50a332d4845a92cf3bc710ac724;
                float4 Color_65f6165abced4cb986c6244544f3c5cc;
                float4 Color_9d92da585efe4621b683115dc43ff927;
                float4 Texture2D_3a58e9e937e94cf598dc09e567577ad6_TexelSize;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_3a58e9e937e94cf598dc09e567577ad6);
                SAMPLER(samplerTexture2D_3a58e9e937e94cf598dc09e567577ad6);
                SAMPLER(_SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_Sampler_3_Linear_Repeat);
    
                // Graph Functions
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_InverseLerp_float(float A, float B, float T, out float Out)
                {
                    Out = (T - A)/(B - A);
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                {
                    Out = lerp(A, B, T);
                }
                
                void Unity_Ceiling_float(float In, out float Out)
                {
                    Out = ceil(In);
                }
                
                void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_OneMinus_float(float In, out float Out)
                {
                    Out = 1 - In;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A + B;
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float4 SpriteMask;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_3a58e9e937e94cf598dc09e567577ad6, samplerTexture2D_3a58e9e937e94cf598dc09e567577ad6, IN.uv0.xy);
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_R_4 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.r;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_G_5 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.g;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_B_6 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.b;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_A_7 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.a;
                    float4 _Property_5de05dc9e3cb4362878d1d025cd7e8e3_Out_0 = Color_1f42b50a332d4845a92cf3bc710ac724;
                    float4 _Property_559c4c36c16e44f09f49fe6f64f31523_Out_0 = Color_65f6165abced4cb986c6244544f3c5cc;
                    float _Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0 = ShimmerPosition;
                    float _Property_0ca46824c6ec4c688875571edb454b42_Out_0 = Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                    float _Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2;
                    Unity_Subtract_float(_Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0, _Property_0ca46824c6ec4c688875571edb454b42_Out_0, _Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2);
                    float4 _UV_50a3b843d7f148daacfafae1d53198ab_Out_0 = IN.uv0;
                    float _Split_761292a7cee6442d9f4caecc82be37e2_R_1 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[0];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_G_2 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[1];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_B_3 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[2];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_A_4 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[3];
                    float _InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3;
                    Unity_InverseLerp_float(_Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2, _Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3);
                    float _Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3;
                    Unity_Clamp_float(_InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3, 0, 1, _Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3);
                    float4 _Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3;
                    Unity_Lerp_float4(_Property_5de05dc9e3cb4362878d1d025cd7e8e3_Out_0, _Property_559c4c36c16e44f09f49fe6f64f31523_Out_0, (_Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3.xxxx), _Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3);
                    float _Property_8fca4eb5e7ef431ea40584fc170ba7cb_Out_0 = ShimmerPosition;
                    float _Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2;
                    Unity_Subtract_float(_Property_8fca4eb5e7ef431ea40584fc170ba7cb_Out_0, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2);
                    float _Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1;
                    Unity_Ceiling_float(_Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2, _Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1);
                    float _Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3;
                    Unity_Clamp_float(_Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1, 0, 1, _Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3);
                    float4 _Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2;
                    Unity_Multiply_float(_Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3, (_Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3.xxxx), _Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2);
                    float _OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1;
                    Unity_OneMinus_float(_Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3, _OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1);
                    float4 _Property_2aa149177f404f2482a22697aad9fb57_Out_0 = Color_65f6165abced4cb986c6244544f3c5cc;
                    float4 _Property_afc3e50528c14c8cbf5d62214c47f73a_Out_0 = Color_9d92da585efe4621b683115dc43ff927;
                    float _Property_672074400f124ef1871d02f14f134eae_Out_0 = ShimmerPosition;
                    float _Property_40de8dd5b07d4f56bfee703e439a8ced_Out_0 = Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                    float _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2;
                    Unity_Add_float(_Property_672074400f124ef1871d02f14f134eae_Out_0, _Property_40de8dd5b07d4f56bfee703e439a8ced_Out_0, _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2);
                    float _InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3;
                    Unity_InverseLerp_float(_Property_672074400f124ef1871d02f14f134eae_Out_0, _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3);
                    float _Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3;
                    Unity_Clamp_float(_InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3, 0, 1, _Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3);
                    float4 _Lerp_995906d17c8348abbb8129137e88b457_Out_3;
                    Unity_Lerp_float4(_Property_2aa149177f404f2482a22697aad9fb57_Out_0, _Property_afc3e50528c14c8cbf5d62214c47f73a_Out_0, (_Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3.xxxx), _Lerp_995906d17c8348abbb8129137e88b457_Out_3);
                    float4 _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2;
                    Unity_Multiply_float((_OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1.xxxx), _Lerp_995906d17c8348abbb8129137e88b457_Out_3, _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2);
                    float4 _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2;
                    Unity_Add_float4(_Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2, _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2, _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2);
                    float4 _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2;
                    Unity_Multiply_float(_SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0, _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2, _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2);
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_R_1 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[0];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_G_2 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[1];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_B_3 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[2];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_A_4 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[3];
                    surface.BaseColor = (_Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2.xyz);
                    surface.Alpha = _Split_3ee56aa51e404c1aa72b915d906deb9e_A_4;
                    surface.SpriteMask = IsGammaSpace() ? float4(1, 1, 1, 1) : float4 (SRGBToLinear(float3(1, 1, 1)), 1);
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.uv0 =                         input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteLitPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "Sprite Normal"
                Tags
                {
                    "LightMode" = "NormalsRendering"
                }
    
                // Render State
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite Off
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITENORMAL
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 normalWS;
                    float4 tangentWS;
                    float4 texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float4 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.normalWS;
                    output.interp1.xyzw =  input.tangentWS;
                    output.interp2.xyzw =  input.texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.normalWS = input.interp0.xyz;
                    output.tangentWS = input.interp1.xyzw;
                    output.texCoord0 = input.interp2.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float ShimmerPosition;
                float Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                float4 Color_1f42b50a332d4845a92cf3bc710ac724;
                float4 Color_65f6165abced4cb986c6244544f3c5cc;
                float4 Color_9d92da585efe4621b683115dc43ff927;
                float4 Texture2D_3a58e9e937e94cf598dc09e567577ad6_TexelSize;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_3a58e9e937e94cf598dc09e567577ad6);
                SAMPLER(samplerTexture2D_3a58e9e937e94cf598dc09e567577ad6);
                SAMPLER(_SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_Sampler_3_Linear_Repeat);
    
                // Graph Functions
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_InverseLerp_float(float A, float B, float T, out float Out)
                {
                    Out = (T - A)/(B - A);
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                {
                    Out = lerp(A, B, T);
                }
                
                void Unity_Ceiling_float(float In, out float Out)
                {
                    Out = ceil(In);
                }
                
                void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_OneMinus_float(float In, out float Out)
                {
                    Out = 1 - In;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A + B;
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float3 NormalTS;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_3a58e9e937e94cf598dc09e567577ad6, samplerTexture2D_3a58e9e937e94cf598dc09e567577ad6, IN.uv0.xy);
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_R_4 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.r;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_G_5 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.g;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_B_6 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.b;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_A_7 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.a;
                    float4 _Property_5de05dc9e3cb4362878d1d025cd7e8e3_Out_0 = Color_1f42b50a332d4845a92cf3bc710ac724;
                    float4 _Property_559c4c36c16e44f09f49fe6f64f31523_Out_0 = Color_65f6165abced4cb986c6244544f3c5cc;
                    float _Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0 = ShimmerPosition;
                    float _Property_0ca46824c6ec4c688875571edb454b42_Out_0 = Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                    float _Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2;
                    Unity_Subtract_float(_Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0, _Property_0ca46824c6ec4c688875571edb454b42_Out_0, _Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2);
                    float4 _UV_50a3b843d7f148daacfafae1d53198ab_Out_0 = IN.uv0;
                    float _Split_761292a7cee6442d9f4caecc82be37e2_R_1 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[0];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_G_2 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[1];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_B_3 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[2];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_A_4 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[3];
                    float _InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3;
                    Unity_InverseLerp_float(_Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2, _Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3);
                    float _Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3;
                    Unity_Clamp_float(_InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3, 0, 1, _Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3);
                    float4 _Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3;
                    Unity_Lerp_float4(_Property_5de05dc9e3cb4362878d1d025cd7e8e3_Out_0, _Property_559c4c36c16e44f09f49fe6f64f31523_Out_0, (_Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3.xxxx), _Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3);
                    float _Property_8fca4eb5e7ef431ea40584fc170ba7cb_Out_0 = ShimmerPosition;
                    float _Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2;
                    Unity_Subtract_float(_Property_8fca4eb5e7ef431ea40584fc170ba7cb_Out_0, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2);
                    float _Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1;
                    Unity_Ceiling_float(_Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2, _Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1);
                    float _Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3;
                    Unity_Clamp_float(_Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1, 0, 1, _Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3);
                    float4 _Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2;
                    Unity_Multiply_float(_Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3, (_Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3.xxxx), _Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2);
                    float _OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1;
                    Unity_OneMinus_float(_Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3, _OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1);
                    float4 _Property_2aa149177f404f2482a22697aad9fb57_Out_0 = Color_65f6165abced4cb986c6244544f3c5cc;
                    float4 _Property_afc3e50528c14c8cbf5d62214c47f73a_Out_0 = Color_9d92da585efe4621b683115dc43ff927;
                    float _Property_672074400f124ef1871d02f14f134eae_Out_0 = ShimmerPosition;
                    float _Property_40de8dd5b07d4f56bfee703e439a8ced_Out_0 = Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                    float _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2;
                    Unity_Add_float(_Property_672074400f124ef1871d02f14f134eae_Out_0, _Property_40de8dd5b07d4f56bfee703e439a8ced_Out_0, _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2);
                    float _InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3;
                    Unity_InverseLerp_float(_Property_672074400f124ef1871d02f14f134eae_Out_0, _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3);
                    float _Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3;
                    Unity_Clamp_float(_InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3, 0, 1, _Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3);
                    float4 _Lerp_995906d17c8348abbb8129137e88b457_Out_3;
                    Unity_Lerp_float4(_Property_2aa149177f404f2482a22697aad9fb57_Out_0, _Property_afc3e50528c14c8cbf5d62214c47f73a_Out_0, (_Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3.xxxx), _Lerp_995906d17c8348abbb8129137e88b457_Out_3);
                    float4 _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2;
                    Unity_Multiply_float((_OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1.xxxx), _Lerp_995906d17c8348abbb8129137e88b457_Out_3, _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2);
                    float4 _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2;
                    Unity_Add_float4(_Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2, _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2, _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2);
                    float4 _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2;
                    Unity_Multiply_float(_SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0, _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2, _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2);
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_R_1 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[0];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_G_2 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[1];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_B_3 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[2];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_A_4 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[3];
                    surface.BaseColor = (_Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2.xyz);
                    surface.Alpha = _Split_3ee56aa51e404c1aa72b915d906deb9e_A_4;
                    surface.NormalTS = IN.TangentSpaceNormal;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.uv0 =                         input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteNormalPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "Sprite Forward"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }
    
                // Render State
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite Off
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_COLOR
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITEFORWARD
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float4 texCoord0;
                    float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float4 interp0 : TEXCOORD0;
                    float4 interp1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyzw =  input.texCoord0;
                    output.interp1.xyzw =  input.color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.interp0.xyzw;
                    output.color = input.interp1.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float ShimmerPosition;
                float Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                float4 Color_1f42b50a332d4845a92cf3bc710ac724;
                float4 Color_65f6165abced4cb986c6244544f3c5cc;
                float4 Color_9d92da585efe4621b683115dc43ff927;
                float4 Texture2D_3a58e9e937e94cf598dc09e567577ad6_TexelSize;
                CBUFFER_END
                
                // Object and Global properties
                TEXTURE2D(Texture2D_3a58e9e937e94cf598dc09e567577ad6);
                SAMPLER(samplerTexture2D_3a58e9e937e94cf598dc09e567577ad6);
                SAMPLER(_SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_Sampler_3_Linear_Repeat);
    
                // Graph Functions
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_InverseLerp_float(float A, float B, float T, out float Out)
                {
                    Out = (T - A)/(B - A);
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                {
                    Out = lerp(A, B, T);
                }
                
                void Unity_Ceiling_float(float In, out float Out)
                {
                    Out = ceil(In);
                }
                
                void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_OneMinus_float(float In, out float Out)
                {
                    Out = 1 - In;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A + B;
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float3 NormalTS;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_3a58e9e937e94cf598dc09e567577ad6, samplerTexture2D_3a58e9e937e94cf598dc09e567577ad6, IN.uv0.xy);
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_R_4 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.r;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_G_5 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.g;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_B_6 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.b;
                    float _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_A_7 = _SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0.a;
                    float4 _Property_5de05dc9e3cb4362878d1d025cd7e8e3_Out_0 = Color_1f42b50a332d4845a92cf3bc710ac724;
                    float4 _Property_559c4c36c16e44f09f49fe6f64f31523_Out_0 = Color_65f6165abced4cb986c6244544f3c5cc;
                    float _Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0 = ShimmerPosition;
                    float _Property_0ca46824c6ec4c688875571edb454b42_Out_0 = Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                    float _Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2;
                    Unity_Subtract_float(_Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0, _Property_0ca46824c6ec4c688875571edb454b42_Out_0, _Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2);
                    float4 _UV_50a3b843d7f148daacfafae1d53198ab_Out_0 = IN.uv0;
                    float _Split_761292a7cee6442d9f4caecc82be37e2_R_1 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[0];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_G_2 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[1];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_B_3 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[2];
                    float _Split_761292a7cee6442d9f4caecc82be37e2_A_4 = _UV_50a3b843d7f148daacfafae1d53198ab_Out_0[3];
                    float _InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3;
                    Unity_InverseLerp_float(_Subtract_cbb8039b26bf40e99bcf864998b68004_Out_2, _Property_00630c67b8bf47358e7d3f90e7c0f3c1_Out_0, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3);
                    float _Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3;
                    Unity_Clamp_float(_InverseLerp_a892871eff6d44068046d702e26db6f7_Out_3, 0, 1, _Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3);
                    float4 _Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3;
                    Unity_Lerp_float4(_Property_5de05dc9e3cb4362878d1d025cd7e8e3_Out_0, _Property_559c4c36c16e44f09f49fe6f64f31523_Out_0, (_Clamp_65a6c71b206c477e895e83e4db274a8d_Out_3.xxxx), _Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3);
                    float _Property_8fca4eb5e7ef431ea40584fc170ba7cb_Out_0 = ShimmerPosition;
                    float _Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2;
                    Unity_Subtract_float(_Property_8fca4eb5e7ef431ea40584fc170ba7cb_Out_0, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2);
                    float _Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1;
                    Unity_Ceiling_float(_Subtract_08c41acf03274423b5fd4ddad35ae95c_Out_2, _Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1);
                    float _Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3;
                    Unity_Clamp_float(_Ceiling_be1b591e1669481b8e92fac2559d17b0_Out_1, 0, 1, _Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3);
                    float4 _Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2;
                    Unity_Multiply_float(_Lerp_1530db8596384dcdaf1b31204e3f040b_Out_3, (_Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3.xxxx), _Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2);
                    float _OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1;
                    Unity_OneMinus_float(_Clamp_3ed0965ce7f043f4a0c7b2044b12c420_Out_3, _OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1);
                    float4 _Property_2aa149177f404f2482a22697aad9fb57_Out_0 = Color_65f6165abced4cb986c6244544f3c5cc;
                    float4 _Property_afc3e50528c14c8cbf5d62214c47f73a_Out_0 = Color_9d92da585efe4621b683115dc43ff927;
                    float _Property_672074400f124ef1871d02f14f134eae_Out_0 = ShimmerPosition;
                    float _Property_40de8dd5b07d4f56bfee703e439a8ced_Out_0 = Vector1_2faa617f2f02433ca0bc7592c5c611e6;
                    float _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2;
                    Unity_Add_float(_Property_672074400f124ef1871d02f14f134eae_Out_0, _Property_40de8dd5b07d4f56bfee703e439a8ced_Out_0, _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2);
                    float _InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3;
                    Unity_InverseLerp_float(_Property_672074400f124ef1871d02f14f134eae_Out_0, _Add_0646a68bc8b94054bbbea0f4fa7a0039_Out_2, _Split_761292a7cee6442d9f4caecc82be37e2_R_1, _InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3);
                    float _Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3;
                    Unity_Clamp_float(_InverseLerp_e439780c153c4ac28c46b941c8e0bb42_Out_3, 0, 1, _Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3);
                    float4 _Lerp_995906d17c8348abbb8129137e88b457_Out_3;
                    Unity_Lerp_float4(_Property_2aa149177f404f2482a22697aad9fb57_Out_0, _Property_afc3e50528c14c8cbf5d62214c47f73a_Out_0, (_Clamp_a8189244cdd6448b8ab15338b19fd4a0_Out_3.xxxx), _Lerp_995906d17c8348abbb8129137e88b457_Out_3);
                    float4 _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2;
                    Unity_Multiply_float((_OneMinus_d71611e3063d490b95a4fb85ef88c41f_Out_1.xxxx), _Lerp_995906d17c8348abbb8129137e88b457_Out_3, _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2);
                    float4 _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2;
                    Unity_Add_float4(_Multiply_c126479e89db42fbbaa7d9fa3bfd63a6_Out_2, _Multiply_e43564b4ecea44849aef5fb217457fe8_Out_2, _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2);
                    float4 _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2;
                    Unity_Multiply_float(_SampleTexture2D_ce97c4c3fb3745acbc4363cdb3495318_RGBA_0, _Add_e7a027577d1d40918ac0ef41322f18ff_Out_2, _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2);
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_R_1 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[0];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_G_2 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[1];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_B_3 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[2];
                    float _Split_3ee56aa51e404c1aa72b915d906deb9e_A_4 = _Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2[3];
                    surface.BaseColor = (_Multiply_f01a56738fa3471f923ee9857c33cdb3_Out_2.xyz);
                    surface.Alpha = _Split_3ee56aa51e404c1aa72b915d906deb9e_A_4;
                    surface.NormalTS = IN.TangentSpaceNormal;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.uv0 =                         input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteForwardPass.hlsl"
    
                ENDHLSL
            }
        }
        FallBack "Hidden/Shader Graph/FallbackError"
    }
