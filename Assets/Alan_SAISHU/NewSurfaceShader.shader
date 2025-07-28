Shader "Custom/URP/DepthFadeTransparent"
{
    Properties
    {
        _BaseColor ("Base Color (Plane Tint)", Color) = (1,1,1,1)
        _DepthMin  ("Fade Start (m)",          Float) = 0.1    // 背景距平面多少米开始淡出
        _DepthMax  ("Fade End   (m)",          Float) = 3.0    // 完全看不到背景的距离
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"     ="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 200

        // 透明物体一般不写深度，避免遮挡其他透明
        Blend SrcAlpha OneMinusSrcAlpha
        Cull  Off
        ZWrite Off

        Pass
        {
            Name "Depth‐Fade­Transparent"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float4 screenPos  : TEXCOORD1;   // xy/w 用于取深度
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _DepthMin;
                float  _DepthMax;
            CBUFFER_END

            // 顶点着色器 ---------------------------------------------------
            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv         = IN.uv;
                OUT.screenPos  = ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            // 片元着色器 ---------------------------------------------------
            half4 Frag(Varyings IN) : SV_Target
            {
                // 1) 取得本像素在相机空间的「自身深度」
                float  planeDepth = LinearEyeDepth(IN.positionCS.z, _ZBufferParams);

                // 2) 从摄像机深度纹理中采样，得到「背景最近的不透明物体深度」
                float  rawSceneDepth = SampleSceneDepth(IN.screenPos.xy / IN.screenPos.w);
                float  sceneDepth    = LinearEyeDepth(rawSceneDepth, _ZBufferParams);

                // 3) 计算背景距本平面的距离（<=0 说明背景更近或被遮挡）
                float  diff = max(0, sceneDepth - planeDepth);

                // 4) 将距离映射到 0~1 Alpha（越远 → Alpha 越大 → 越看不到背景）
                float  alpha = saturate( (diff - _DepthMin) / (_DepthMax - _DepthMin) );

                return float4(_BaseColor.rgb, alpha * _BaseColor.a);
            }
            ENDHLSL
        }
    }
    Fallback Off
}