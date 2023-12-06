Shader "Unlit/BlocksShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _GlowColor("Glow Color", Color) = (1,0,0,1)
        _GlowStrength("Glow Strength", Range(0,1)) = 0.5
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float4 _GlowColor;
            float _GlowStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color;
                col.rgb += _GlowColor.rgb * _GlowStrength; // Simple glow effect
                return col;
            }
            ENDCG
        }
    }
}
