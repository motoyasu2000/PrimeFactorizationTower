Shader "Custom/StripesShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} //�Q�[���I�u�W�F�N�g�̃X�v���C�g�����_���[����擾�����e�N�X�`�����󂯎��B
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
            
            sampler2D _MainTex; 
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
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float stripePattern = sin(i.uv.y * _Space + _Timer) * 0.5 + 0.5;
                fixed4 stripeColor = lerp(_AnotherColor, _Color, stripePattern);
                stripeColor.a = texColor.a; //���̃e�N�X�`���̓����x��K�p
                if(texColor.a == 0) discard; //���̃e�N�X�`���œ����ȕ����͊��S�Ɍ����Ȃ��悤�ɂ���
                return stripeColor;
            }

            ENDCG
        }
    }
}
