//  VolFx © NullTale - https://x.com/NullTale
Shader "Hidden/VolFx/Warp"
{
    SubShader
    {
        LOD 0

        ZTest Always
        Cull Off
        ZWrite Off
        ZClip false

        Pass
        {
            CGPROGRAM

            #pragma vertex vert_img_custom
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "UnityShaderVariables.cginc"

            struct appdata_img_custom
            {
                float4 vertex : POSITION;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f_img_custom
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                half2 stereoUV : TEXCOORD2;
#if UNITY_UV_STARTS_AT_TOP
                half4 uv2 : TEXCOORD1;
                half4 stereoUV2 : TEXCOORD3;
#endif
            };

            uniform sampler2D _MainTex;
            uniform half4 _MainTex_TexelSize;
            uniform half4 _MainTex_ST;

            uniform float _RadialScale;
            uniform float _Tiling;
            uniform float _Animation;
            uniform float _Power;
            uniform float _Remap;
            uniform float _MaskScale;
            uniform float _MaskHardness;
            uniform float _MaskPower;
            uniform float4 _Color;

            // Permutation helpers for noise
            float3 mod289(float3 x) { return x - floor(x / 289.0) * 289.0; }
            float3 permute(float3 x) { return mod289(((x * 34.0) + 1.0) * x); }

            // 2D simplex noise
            float snoise(float2 v)
            {
                const float4 C = float4(0.211324865, 0.366025403, -0.577350269, 0.0243902439);
                float2 i = floor(v + dot(v, C.yy));
                float2 x0 = v - i + dot(i, C.xx);
                float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;
                i = mod289(i.xyy).xy;

                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
                float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
                m = m * m;
                m = m * m;

                float3 x = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;

                m *= 1.7928429 - 0.8537347 * (a0 * a0 + h * h);

                float3 g;
                g.x = a0.x * x0.x + h.x * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;

                return 130.0 * dot(m, g);
            }

            v2f_img_custom vert_img_custom(appdata_img_custom v)
            {
                v2f_img_custom o;
                o.pos = v.vertex;
                o.uv = float4(v.texcoord.xy, 1, 1);

#if UNITY_UV_STARTS_AT_TOP
                o.uv2 = float4(v.texcoord.xy, 1, 1);
                o.stereoUV2 = UnityStereoScreenSpaceUVAdjust(o.uv2, _MainTex_ST);
                if (_MainTex_TexelSize.y < 0.0)
                    o.uv.y = 1.0 - o.uv.y;
#endif
                o.stereoUV = UnityStereoScreenSpaceUVAdjust(o.uv, _MainTex_ST);
                return o;
            }

            half4 frag(v2f_img_custom i) : SV_Target
            {
#if UNITY_UV_STARTS_AT_TOP
                half2 uv = i.uv2;
                half2 stereoUV = i.stereoUV2;
#else
                half2 uv = i.uv;
                half2 stereoUV = i.stereoUV;
#endif

                float2 uvMainTex = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                half4 sceneColor = tex2D(_MainTex, uvMainTex);

                float2 centeredUV = i.uv.xy - 0.5;

                // Polar coordinates with radial scale and tiling
                float2 polarUV;
                polarUV.x = length(centeredUV) * _RadialScale * 2.0;
                polarUV.y = atan2(centeredUV.x, centeredUV.y) / 6.28318548 * _Tiling;

                // Animate noise input
                float2 noiseInput = polarUV + float2(-_Animation * _Time.y, 0);

                // Simplex noise remapped to [0,1]
                float noise = snoise(noiseInput);
                noise = noise * 0.5 + 0.5;

                // Speed lines calculation with power and remap
                float threshold = _Remap;
                float speedLines = saturate((pow(noise, _Power) - threshold) / (1.0 - threshold));

                // Mask calculations
                float2 maskUV = i.uv.xy * 2.0 - 1.0;
                float hardnessLerp = lerp(0.0, _MaskScale, _MaskHardness);
                float mask = pow(1.0 - saturate((length(maskUV) - _MaskScale) / (hardnessLerp - _MaskScale - 0.001)), _MaskPower);

                float maskedSpeedLines = speedLines * mask;

                float3 colorRGB = _Color.rgb;
                float alpha = _Color.a;

                half4 finalColor = lerp(sceneColor, half4(maskedSpeedLines * colorRGB, 0.0), maskedSpeedLines * alpha);

                return half4(finalColor.rgb, sceneColor.a);
            }

            ENDCG
        }
    }
}
