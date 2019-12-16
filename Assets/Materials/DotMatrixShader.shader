Shader "Unlit/pixelDots"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Spacing ("Spacing", Range(0,1)) = 0.25
        _ShadowColor("Shadow Color", Color) = (0,0,0,1)
        _BlurStep("Blur Step", Float) = 0.1
        _BlurDistance("Blur Distance", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _ShadowColor;
            float _BlurStep;
            float _BlurDistance;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 sum = float4(0,0,0,0);
                float blurStepY = _BlurStep * _MainTex_TexelSize.y / _MainTex_TexelSize.x;
                float2 tc = i.uv;
                
                sum += tex2D(_MainTex, float2(tc.x - 4.0*_BlurStep, tc.y - 4.0*blurStepY)) * 0.0162162162;
                sum += tex2D(_MainTex, float2(tc.x - 3.0*_BlurStep, tc.y - 3.0*blurStepY)) * 0.0540540541;
                sum += tex2D(_MainTex, float2(tc.x - 2.0*_BlurStep, tc.y - 2.0*blurStepY)) * 0.1216216216;
                sum += tex2D(_MainTex, float2(tc.x - 1.0*_BlurStep, tc.y - 1.0*blurStepY)) * 0.1945945946;

                sum += tex2D(_MainTex, float2(tc.x, tc.y)) * 0.2270270270;

                sum += tex2D(_MainTex, float2(tc.x + 1.0*_BlurStep, tc.y + 1.0*blurStepY)) * 0.1945945946;
                sum += tex2D(_MainTex, float2(tc.x + 2.0*_BlurStep, tc.y + 2.0*blurStepY)) * 0.1216216216;
                sum += tex2D(_MainTex, float2(tc.x + 3.0*_BlurStep, tc.y + 3.0*blurStepY)) * 0.0540540541;
                sum += tex2D(_MainTex, float2(tc.x + 4.0*_BlurStep, tc.y + 4.0*blurStepY)) * 0.0162162162;

                fixed4 col = sum;
                return col * _ShadowColor;
            }
            ENDCG
        }
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float _Spacing;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                // uvs * texture resolution
                float2 texelUV = i.uv * _MainTex_TexelSize.zw;
 
                // set texture to point filtering, sample normally
                fixed4 col = tex2D(_MainTex, i.uv);
 
                // optional force texture to be sampled as if it's using point filtering
                // float2 pointUVs = (floor(texelUV) + 0.5) * _MainTex_TexelSize.xy;
                // fixed4 col = tex2D(_MainTex, pointUVs);
 
                // rescale each texel to a -1 to 1 range, then get the absolute of that.
                // this gets a distance from the center of the "pixel", where the edges are 1
                // and the center is 0.
                float2 pixelUV = frac(texelUV) * 2.0 - 1.0;
 
                // get the max distance for a square dot
                float pixelCenterDist = max(abs(pixelUV.x), abs(pixelUV.y));
 
                // alternative if you want round dots, scaled so a spacing of 0 has no holes
                //float pixelCenterDist = length(pixelUV) * 0.70716;
 
                // clip the pixel distance value
                clip(1 - _Spacing - pixelCenterDist);
 
                return col * _Color;
            }
            ENDCG
        }
    }
}
