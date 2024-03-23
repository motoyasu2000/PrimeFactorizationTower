Shader "Custom/StripesShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _AnotherColor("AnotherColor", Color) = (1,1,1,1)
        _Space("Space", Float) = 0
        _Timer("Timer", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;
            fixed4 _AnotherColor;
            Float _Space;
            Float _Timer;

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
                float stripe = sin(i.uv.y * _Space + _Timer);
                return _Color * stripe + _AnotherColor * (1-stripe);
            }
            ENDCG
        }
    }
}
