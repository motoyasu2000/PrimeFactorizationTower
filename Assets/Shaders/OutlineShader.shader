Shader "Custom/OutlineShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _OutLineColor ("OutLineColor", Color) = (1,1,1,1)
        _Offset ("Offset", float) = 0.02
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
            float4 _OutLineColor;
            float _Offset;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 tex = tex2D(_MainTex, i.uv);
                float alpha = tex.a;

                //周囲のアルファ値の取得
                float offset = _Offset; //上下左右方向の距離
                float diagonalOffset = offset / sqrt(2); //斜め方向の距離

                float    alphaUp = tex2D(_MainTex, i.uv + float2(0, offset)).a;
                float  alphaDown = tex2D(_MainTex, i.uv - float2(0, offset)).a;
                float alphaRight = tex2D(_MainTex, i.uv + float2(offset, 0)).a;
                float  alphaLeft = tex2D(_MainTex, i.uv - float2(offset, 0)).a;
                float alphaUpperRight = tex2D(_MainTex, i.uv + float2(diagonalOffset,diagonalOffset)).a;
                float alphaUpperLeft  = tex2D(_MainTex, i.uv + float2(-diagonalOffset,diagonalOffset)).a;
                float alphaLowerRight = tex2D(_MainTex, i.uv + float2(diagonalOffset,-diagonalOffset)).a;
                float alphaLowerLeft  = tex2D(_MainTex, i.uv + float2(-diagonalOffset,-diagonalOffset)).a;

                //--------------------色のある部分と透明部分の輪郭線の周囲に色を塗る--------------------------
                //色のある部分で、かつ周囲に透明部分が存在するなら_MiniBlockColorで表示。
                if (alpha > 0 && (alphaUp == 0 || alphaDown == 0 || alphaRight == 0 || alphaLeft == 0 || alphaUpperRight == 0 || alphaUpperLeft == 0 || alphaLowerRight == 0 || alphaLowerLeft == 0)) {
                    return _OutLineColor;
                }
                //透明な部分で、かつ周囲に色のある部分が存在するなら_MiniBlockColorで表示。
                if(alpha == 0 && (alphaRight > 0 || alphaLeft > 0 || alphaUp > 0 || alphaDown > 0 || alphaUpperRight > 0 || alphaUpperLeft > 0 || alphaLowerRight > 0 || alphaLowerLeft > 0)){
                    return _OutLineColor;
                }
                discard;

                //呼ばれないはず
                return tex;
            }
            ENDCG
        }
    }
}