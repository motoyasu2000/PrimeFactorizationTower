Shader "Custom/DefaultBlocksShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} //ゲームオブジェクトのスプライトレンダラーからテクスチャを受け取る。
        _Color ("Main Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D _MainTex; 
            fixed4 _Color;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 color = _Color;
                if(texColor.a == 0) discard; //元のテクスチャで透明な部分は完全に見えないようにする
                return color;
            }

            ENDCG
        }
    }
}
