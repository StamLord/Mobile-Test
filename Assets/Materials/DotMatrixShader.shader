Shader "Unlit/DotMatrixShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Spacing("Spacing", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            fixed4 _Color;
            float4 _MainTex_TexelSize;
            float _Spacing;
            float xPuv;
            float yPuv;

            fixed4 frag (v2f i) : COLOR
            {
                i.uv += i.uv;

                // Calculate uv units to 1 texture unit
                xPuv = 1 / _MainTex_TexelSize.z;
                yPuv = 1 / _MainTex_TexelSize.w;

                // Calculate which pixel in the texture is it
                int xPixel = floor(i.uv.x / xPuv);
                int yPixel = floor(i.uv.y / yPuv);

                half4 c;

                // Even pixels are rendered
                half even = ceil(xPixel % 2 + yPixel % 2);
                //if(xPixel % 2 == 0 && yPixel % 2 == 0)
                //{
                    // Calculates which texture pixel to use
                    fixed xOffset = xPixel / 2 * xPuv;
                    fixed yOffset = yPixel / 2 * yPuv;
                    c = tex2D(_MainTex, i.uv - float2(xOffset, yOffset));
                    c.rgb *= c.a;
                //}
                //else // Blank
                //{
                //    c = float4(0,0,0,0);
                //}
                c = lerp(c, float4(0,0,0,0), even);
                return c;
            }
            ENDCG
        }
    }
}
