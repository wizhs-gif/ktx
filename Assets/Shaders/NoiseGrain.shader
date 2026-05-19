Shader "UI/NoiseGrain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0
        _NoiseSpeed ("Noise Speed", Float) = 8
        _NoiseScale ("Noise Scale", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            float _NoiseIntensity;
            float _NoiseSpeed;
            float _NoiseScale;

            // 伪随机哈希函数
            float hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 基础纹理
                fixed4 col = tex2D(_MainTex, i.uv);

                // 生成动态噪点
                float2 noiseUV = i.uv * _NoiseScale;
                float time = _Time.y * _NoiseSpeed;

                // 两帧噪点混合，产生闪烁感
                float noise1 = hash(noiseUV + floor(time));
                float noise2 = hash(noiseUV + floor(time + 1));
                float noise = lerp(noise1, noise2, frac(time));

                // 混合：原始颜色 + 噪点
                col.rgb = lerp(col.rgb, noise, _NoiseIntensity);
                col.a = max(col.a, _NoiseIntensity * 0.8);

                return col;
            }
            ENDCG
        }
    }
}
