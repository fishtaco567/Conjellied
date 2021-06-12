Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InsideColor ("Inside Color", Color) = (1, 1, 1, 1)
        _EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _InsideColor;
            fixed4 _EdgeColor;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - .1);
                fixed4 col10 = tex2D(_MainTex, i.uv + float2(1. / 480., 0));
                fixed4 coln10 = tex2D(_MainTex, i.uv + float2(-1. / 480., 0));
                fixed4 col01 = tex2D(_MainTex, i.uv + float2(0, 1. / 270.));
                fixed4 col0n1 = tex2D(_MainTex, i.uv + float2(0, -1. / 270.));
                float sob = abs(col10.a - coln10.a) + abs(col01.a - col0n1.a);
                sob = saturate(sob);

                col = lerp(_InsideColor, _EdgeColor, sob) * (col / fixed4(72./255., 152./255., 79./255., 1));

                return col;
            }
            ENDCG
        }
    }
}
