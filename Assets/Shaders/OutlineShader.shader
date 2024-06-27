Shader "Custom/OutlineShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "LightMode"="Universal2D" }

        Pass {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float alpha = tex.a;

                //周囲のアルファ値の取得
                float2 offsets = float2(0.03, 0.03);
                float alphaRight = tex2D(_MainTex, i.uv + float2(offsets.x, 0)).a;
                float alphaUp = tex2D(_MainTex, i.uv + float2(0, offsets.y)).a;
                float alphaLeft = tex2D(_MainTex, i.uv - float2(offsets.x, 0)).a;
                float alphaDown = tex2D(_MainTex, i.uv - float2(0, offsets.y)).a;

                //色のある部分で、かつ周囲にアルファ値が存在するなら赤で表示。そうでなければ描画しない
                if (alpha > 0 && (alphaRight == 0 || alphaUp == 0 || alphaLeft == 0 || alphaDown == 0)) {
                    return fixed4(1, 0, 0, 1);
                }
                else{
                    discard;
                }

                return tex;
            }
            ENDCG
        }
    }
}