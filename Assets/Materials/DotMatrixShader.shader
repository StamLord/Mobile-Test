Shader "Unlit/DotMatrixShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
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

            fixed4 frag (v2f i) : COLOR
            {
                half2 pUV = (_MainTex_TexelSize.x / i.uv.x, _MainTex_TexelSize.y / i.uv.y);
                half4 c = tex2D(_MainTex, i.uv) ;
                c.rgb *= c.a;

                return c;
            }
            ENDCG
        }
    }
}
