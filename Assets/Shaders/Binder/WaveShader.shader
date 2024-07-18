Shader "Custom/WaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _AnotherColor("AnotherColor", Color) = (1,1,1,1)
        _WaveAmplitude ("Wave Amplitude", Float) = 10
        _WaveFrequency ("Wave Frequency", Float) = 1.0
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveHeight ("Wave Height", Float) = 1.0
    }

    SubShader
    {
        Tags{ "LightMode"="Universal2D" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _AnotherColor;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _WaveHeight;

            //スライダーのvalueが取りうる上限値
            static float maxSliderValue  = 50.0;

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

                //周波数や速さ、振幅を定義することで、移動するsin波を定義
                float localWave = sin(i.uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude / maxSliderValue ;

                //上で定義したsin波に高さも考慮(テクスチャは0~1で表されるが、sin波の基線が0であることに注意)
                float resultWave = localWave +_WaveHeight/maxSliderValue ;

                float2 wavedUV = i.uv;
                fixed4 color = _AnotherColor;
                if(i.uv.y - resultWave < 0)color = _Color;

                color.a = texColor.a;
                if(color.a == 0) discard;
                return color;
            }

            ENDCG
        }
    }
}