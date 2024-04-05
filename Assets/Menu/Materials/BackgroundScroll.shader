Shader "Unlit/BackgroundScroll"
{
    Properties
    {
        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        _Direction("Direction", Vector) = (0.1, 0.1, 0, 0)
        _Tiling("Tiling", Vector) = (12.8, 7.2, 0, 0)
        _Offset("Offset", Vector) = (0, 0, 0, 0)
        _BackgroundColour("BackgroundColour", Color) = (0, 0.8076223, 0.9849057, 1)
        _ForegroundColour("ForegroundColour", Color) = (0.5690778, 0.614522, 0.9698113, 1)
        _TimeSinceStart("TimeSinceStart", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        Pass
        {
            Name "Sprite Unlit"
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
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
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
             float3 positionWS;
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
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
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
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
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
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
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
        float4 _MainTex_TexelSize;
        float2 _Direction;
        float2 _Tiling;
        float2 _Offset;
        float4 _BackgroundColour;
        float4 _ForegroundColour;
        float _TimeSinceStart;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Ellipse_float(float2 UV, float Width, float Height, out float Out)
        {
        #if defined(SHADER_STAGE_RAY_TRACING)
            Out = saturate((1.0 - length((UV * 2 - 1) / float2(Width, Height))) * 1e7);
        #else
            float d = length((UV * 2 - 1) / float2(Width, Height));
            Out = saturate((1 - d) / fwidth(d));
        #endif
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
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
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4 = _BackgroundColour;
            float4 _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4 = _ForegroundColour;
            UnityTexture2D _Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _Property_12e077eecc034146802873ff225748f1_Out_0_Vector2 = _Tiling;
            float _Property_c8df71bfd861464a94447bfaa2646540_Out_0_Float = _TimeSinceStart;
            float2 _Property_f519d62b90394e61b73ead4e57de576a_Out_0_Vector2 = _Direction;
            float2 _Multiply_9e8eb72fa027415e8c84a9fcc805c1c6_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_c8df71bfd861464a94447bfaa2646540_Out_0_Float.xx), _Property_f519d62b90394e61b73ead4e57de576a_Out_0_Vector2, _Multiply_9e8eb72fa027415e8c84a9fcc805c1c6_Out_2_Vector2);
            float2 _Property_cda020dc55354ec5906a6f1887de2b84_Out_0_Vector2 = _Offset;
            float2 _Add_dbe2480cca2d496a9dac082dfff8127c_Out_2_Vector2;
            Unity_Add_float2(_Multiply_9e8eb72fa027415e8c84a9fcc805c1c6_Out_2_Vector2, _Property_cda020dc55354ec5906a6f1887de2b84_Out_0_Vector2, _Add_dbe2480cca2d496a9dac082dfff8127c_Out_2_Vector2);
            float2 _TilingAndOffset_14a94eab58e44c889551dfc77d3e74cb_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_12e077eecc034146802873ff225748f1_Out_0_Vector2, _Add_dbe2480cca2d496a9dac082dfff8127c_Out_2_Vector2, _TilingAndOffset_14a94eab58e44c889551dfc77d3e74cb_Out_3_Vector2);
            float4 _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D.tex, _Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D.samplerstate, _Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_14a94eab58e44c889551dfc77d3e74cb_Out_3_Vector2) );
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_R_4_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.r;
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_G_5_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.g;
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_B_6_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.b;
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_A_7_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.a;
            float4 _Multiply_2b3428d39ef542b4a49fd072744d8f1a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4, (_SampleTexture2D_358a20cb02d2421aa1821941167f2023_A_7_Float.xxxx), _Multiply_2b3428d39ef542b4a49fd072744d8f1a_Out_2_Vector4);
            float4 _Add_b826e477fa4a4d2b918f619a353ba81a_Out_2_Vector4;
            Unity_Add_float4(_Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4, _Multiply_2b3428d39ef542b4a49fd072744d8f1a_Out_2_Vector4, _Add_b826e477fa4a4d2b918f619a353ba81a_Out_2_Vector4);
            float _Split_171bb1abda604a9389f8725256a1486e_R_1_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[0];
            float _Split_171bb1abda604a9389f8725256a1486e_G_2_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[1];
            float _Split_171bb1abda604a9389f8725256a1486e_B_3_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[2];
            float _Split_171bb1abda604a9389f8725256a1486e_A_4_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[3];
            float2 _Property_c5df797a198248b9bb6409590ad4a5a1_Out_0_Vector2 = _Tiling;
            float2 _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2;
            Unity_Divide_float2(_Property_c5df797a198248b9bb6409590ad4a5a1_Out_0_Vector2, float2(10, 10), _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2);
            float _Split_211e6a10e9d14756a197745e039659fc_R_1_Float = _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2[0];
            float _Split_211e6a10e9d14756a197745e039659fc_G_2_Float = _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2[1];
            float _Split_211e6a10e9d14756a197745e039659fc_B_3_Float = 0;
            float _Split_211e6a10e9d14756a197745e039659fc_A_4_Float = 0;
            float _Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float;
            Unity_Ellipse_float(IN.uv0.xy, _Split_211e6a10e9d14756a197745e039659fc_G_2_Float, _Split_211e6a10e9d14756a197745e039659fc_R_1_Float, _Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float);
            float _Multiply_0834d9be551e42579588f15a04b8754d_Out_2_Float;
            Unity_Multiply_float_float(_SampleTexture2D_358a20cb02d2421aa1821941167f2023_A_7_Float, _Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float, _Multiply_0834d9be551e42579588f15a04b8754d_Out_2_Float);
            float _Multiply_d02dfac3f1534d3ea640053202c36fd4_Out_2_Float;
            Unity_Multiply_float_float(_Split_171bb1abda604a9389f8725256a1486e_A_4_Float, _Multiply_0834d9be551e42579588f15a04b8754d_Out_2_Float, _Multiply_d02dfac3f1534d3ea640053202c36fd4_Out_2_Float);
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_R_1_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[0];
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_G_2_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[1];
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_B_3_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[2];
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_A_4_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[3];
            float _Multiply_5275c5914b9d490198a7906f94088ece_Out_2_Float;
            Unity_Multiply_float_float(_Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float, _Split_25a78d9b84e04ba9b1347166d7a3b14e_A_4_Float, _Multiply_5275c5914b9d490198a7906f94088ece_Out_2_Float);
            float _Add_8b761b0fef624925991e0a16a50e4d6e_Out_2_Float;
            Unity_Add_float(_Multiply_d02dfac3f1534d3ea640053202c36fd4_Out_2_Float, _Multiply_5275c5914b9d490198a7906f94088ece_Out_2_Float, _Add_8b761b0fef624925991e0a16a50e4d6e_Out_2_Float);
            surface.BaseColor = (_Add_b826e477fa4a4d2b918f619a353ba81a_Out_2_Vector4.xyz);
            surface.Alpha = _Add_8b761b0fef624925991e0a16a50e4d6e_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
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
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
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
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
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
             float3 positionWS;
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
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
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
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
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
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
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
        float4 _MainTex_TexelSize;
        float2 _Direction;
        float2 _Tiling;
        float2 _Offset;
        float4 _BackgroundColour;
        float4 _ForegroundColour;
        float _TimeSinceStart;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Ellipse_float(float2 UV, float Width, float Height, out float Out)
        {
        #if defined(SHADER_STAGE_RAY_TRACING)
            Out = saturate((1.0 - length((UV * 2 - 1) / float2(Width, Height))) * 1e7);
        #else
            float d = length((UV * 2 - 1) / float2(Width, Height));
            Out = saturate((1 - d) / fwidth(d));
        #endif
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
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
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4 = _BackgroundColour;
            float4 _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4 = _ForegroundColour;
            UnityTexture2D _Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _Property_12e077eecc034146802873ff225748f1_Out_0_Vector2 = _Tiling;
            float _Property_c8df71bfd861464a94447bfaa2646540_Out_0_Float = _TimeSinceStart;
            float2 _Property_f519d62b90394e61b73ead4e57de576a_Out_0_Vector2 = _Direction;
            float2 _Multiply_9e8eb72fa027415e8c84a9fcc805c1c6_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_c8df71bfd861464a94447bfaa2646540_Out_0_Float.xx), _Property_f519d62b90394e61b73ead4e57de576a_Out_0_Vector2, _Multiply_9e8eb72fa027415e8c84a9fcc805c1c6_Out_2_Vector2);
            float2 _Property_cda020dc55354ec5906a6f1887de2b84_Out_0_Vector2 = _Offset;
            float2 _Add_dbe2480cca2d496a9dac082dfff8127c_Out_2_Vector2;
            Unity_Add_float2(_Multiply_9e8eb72fa027415e8c84a9fcc805c1c6_Out_2_Vector2, _Property_cda020dc55354ec5906a6f1887de2b84_Out_0_Vector2, _Add_dbe2480cca2d496a9dac082dfff8127c_Out_2_Vector2);
            float2 _TilingAndOffset_14a94eab58e44c889551dfc77d3e74cb_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_12e077eecc034146802873ff225748f1_Out_0_Vector2, _Add_dbe2480cca2d496a9dac082dfff8127c_Out_2_Vector2, _TilingAndOffset_14a94eab58e44c889551dfc77d3e74cb_Out_3_Vector2);
            float4 _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D.tex, _Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D.samplerstate, _Property_ef5606304faf426c8b03afb249ce37a1_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_14a94eab58e44c889551dfc77d3e74cb_Out_3_Vector2) );
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_R_4_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.r;
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_G_5_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.g;
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_B_6_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.b;
            float _SampleTexture2D_358a20cb02d2421aa1821941167f2023_A_7_Float = _SampleTexture2D_358a20cb02d2421aa1821941167f2023_RGBA_0_Vector4.a;
            float4 _Multiply_2b3428d39ef542b4a49fd072744d8f1a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4, (_SampleTexture2D_358a20cb02d2421aa1821941167f2023_A_7_Float.xxxx), _Multiply_2b3428d39ef542b4a49fd072744d8f1a_Out_2_Vector4);
            float4 _Add_b826e477fa4a4d2b918f619a353ba81a_Out_2_Vector4;
            Unity_Add_float4(_Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4, _Multiply_2b3428d39ef542b4a49fd072744d8f1a_Out_2_Vector4, _Add_b826e477fa4a4d2b918f619a353ba81a_Out_2_Vector4);
            float _Split_171bb1abda604a9389f8725256a1486e_R_1_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[0];
            float _Split_171bb1abda604a9389f8725256a1486e_G_2_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[1];
            float _Split_171bb1abda604a9389f8725256a1486e_B_3_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[2];
            float _Split_171bb1abda604a9389f8725256a1486e_A_4_Float = _Property_2f59e4ff974c44ff98abcaf975510507_Out_0_Vector4[3];
            float2 _Property_c5df797a198248b9bb6409590ad4a5a1_Out_0_Vector2 = _Tiling;
            float2 _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2;
            Unity_Divide_float2(_Property_c5df797a198248b9bb6409590ad4a5a1_Out_0_Vector2, float2(10, 10), _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2);
            float _Split_211e6a10e9d14756a197745e039659fc_R_1_Float = _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2[0];
            float _Split_211e6a10e9d14756a197745e039659fc_G_2_Float = _Divide_2c49ec7b91eb4c24b08e4cc679bc3629_Out_2_Vector2[1];
            float _Split_211e6a10e9d14756a197745e039659fc_B_3_Float = 0;
            float _Split_211e6a10e9d14756a197745e039659fc_A_4_Float = 0;
            float _Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float;
            Unity_Ellipse_float(IN.uv0.xy, _Split_211e6a10e9d14756a197745e039659fc_G_2_Float, _Split_211e6a10e9d14756a197745e039659fc_R_1_Float, _Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float);
            float _Multiply_0834d9be551e42579588f15a04b8754d_Out_2_Float;
            Unity_Multiply_float_float(_SampleTexture2D_358a20cb02d2421aa1821941167f2023_A_7_Float, _Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float, _Multiply_0834d9be551e42579588f15a04b8754d_Out_2_Float);
            float _Multiply_d02dfac3f1534d3ea640053202c36fd4_Out_2_Float;
            Unity_Multiply_float_float(_Split_171bb1abda604a9389f8725256a1486e_A_4_Float, _Multiply_0834d9be551e42579588f15a04b8754d_Out_2_Float, _Multiply_d02dfac3f1534d3ea640053202c36fd4_Out_2_Float);
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_R_1_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[0];
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_G_2_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[1];
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_B_3_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[2];
            float _Split_25a78d9b84e04ba9b1347166d7a3b14e_A_4_Float = _Property_3900467ddbcd43beb3358a92e18a4922_Out_0_Vector4[3];
            float _Multiply_5275c5914b9d490198a7906f94088ece_Out_2_Float;
            Unity_Multiply_float_float(_Ellipse_00295639f878485b8b3b1c06177d672e_Out_4_Float, _Split_25a78d9b84e04ba9b1347166d7a3b14e_A_4_Float, _Multiply_5275c5914b9d490198a7906f94088ece_Out_2_Float);
            float _Add_8b761b0fef624925991e0a16a50e4d6e_Out_2_Float;
            Unity_Add_float(_Multiply_d02dfac3f1534d3ea640053202c36fd4_Out_2_Float, _Multiply_5275c5914b9d490198a7906f94088ece_Out_2_Float, _Add_8b761b0fef624925991e0a16a50e4d6e_Out_2_Float);
            surface.BaseColor = (_Add_b826e477fa4a4d2b918f619a353ba81a_Out_2_Vector4.xyz);
            surface.Alpha = _Add_8b761b0fef624925991e0a16a50e4d6e_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
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
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}