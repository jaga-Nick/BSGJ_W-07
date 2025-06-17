Shader "Hidden/CameraEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 機能のON/OFFをこれで切り替える
            #pragma multi_compile __ FOCUS_ON
            #pragma multi_compile __ COLORBLIND_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // Texelのサイズ (1/width, 1/height)

            // C#から受け取るパラメータ
            float4 _FocusPoint; // x, y (0-1)
            int _ColorblindType;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            // --- 色覚補正ロジック ---
            // Daltonize (色補正) のための行列
            // これらの値は研究に基づいて調整します
            static const float3x3 ProtanopiaCorrection = float3x3(
                0.0, 2.02344, -2.52581,
                0.0, 1.0, 0.0,
                0.0, 0.0, 1.0
            );
            static const float3x3 DeuteranopiaCorrection = float3x3(
                1.0, 0.0, 0.0,
                0.494207, 0.0, 1.24827,
                0.0, 0.0, 1.0
            );

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // --- 視線注視（ぼかし） ---
                #if FOCUS_ON
                float dist = distance(i.uv, _FocusPoint.xy);
                float blurAmount = smoothstep(0.1, 0.4, dist) * 4.0; // 注目点から離れるほど強くぼかす
                
                if (blurAmount > 0)
                {
                    float4 blurredCol = float4(0,0,0,0);
                    // 簡単なボックスブラー
                    for(int x = -2; x <= 2; x++)
                    {
                        for(int y = -2; y <= 2; y++)
                        {
                            float2 offset = float2(x, y) * _MainTex_TexelSize.xy * blurAmount;
                            blurredCol += tex2D(_MainTex, i.uv + offset);
                        }
                    }
                    col = blurredCol / 25.0;
                }
                #endif

                // --- 色覚異常対応 ---
                #if COLORBLIND_ON
                if (_ColorblindType == 0) // 1型
                {
                    col.rgb = mul(ProtanopiaCorrection, col.rgb);
                }
                else if (_ColorblindType == 1) // 2型
                {
                    col.rgb = mul(DeuteranopiaCorrection, col.rgb);
                }
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}