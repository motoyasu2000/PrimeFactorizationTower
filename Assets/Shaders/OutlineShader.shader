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

                //���͂̃A���t�@�l�̎擾
                float offset = _Offset; //�㉺���E�����̋���
                float diagonalOffset = offset / sqrt(2); //�΂ߕ����̋���

                float    alphaUp = tex2D(_MainTex, i.uv + float2(0, offset)).a;
                float  alphaDown = tex2D(_MainTex, i.uv - float2(0, offset)).a;
                float alphaRight = tex2D(_MainTex, i.uv + float2(offset, 0)).a;
                float  alphaLeft = tex2D(_MainTex, i.uv - float2(offset, 0)).a;
                float alphaUpperRight = tex2D(_MainTex, i.uv + float2(diagonalOffset,diagonalOffset)).a;
                float alphaUpperLeft  = tex2D(_MainTex, i.uv + float2(-diagonalOffset,diagonalOffset)).a;
                float alphaLowerRight = tex2D(_MainTex, i.uv + float2(diagonalOffset,-diagonalOffset)).a;
                float alphaLowerLeft  = tex2D(_MainTex, i.uv + float2(-diagonalOffset,-diagonalOffset)).a;

                //--------------------�F�̂��镔���Ɠ��������̗֊s���̎��͂ɐF��h��--------------------------
                //�F�̂��镔���ŁA�����͂ɓ������������݂���Ȃ�_MiniBlockColor�ŕ\���B
                if (alpha > 0 && (alphaUp == 0 || alphaDown == 0 || alphaRight == 0 || alphaLeft == 0 || alphaUpperRight == 0 || alphaUpperLeft == 0 || alphaLowerRight == 0 || alphaLowerLeft == 0)) {
                    return _OutLineColor;
                }
                //�����ȕ����ŁA�����͂ɐF�̂��镔�������݂���Ȃ�_MiniBlockColor�ŕ\���B
                if(alpha == 0 && (alphaRight > 0 || alphaLeft > 0 || alphaUp > 0 || alphaDown > 0 || alphaUpperRight > 0 || alphaUpperLeft > 0 || alphaLowerRight > 0 || alphaLowerLeft > 0)){
                    return _OutLineColor;
                }
                discard;

                //�Ă΂�Ȃ��͂�
                return tex;
            }
            ENDCG
        }
    }
}