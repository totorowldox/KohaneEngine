Shader "Unlit/PatternGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PatternTex ("Pattern Texture", 2d) = "white" {}
        _Progress("Progress", Range (0, 1)) = 0
        _Gradient("Gradient", Range(0, 1)) = 0.1
        _Alpha("Alpha", Range(0,1)) = 1
        [Toggle(PURE_BLACK)] _UseBlack ("Use pure black?", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "IgnoreProjector"="True"
        }

        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        //Enable alpha blend
        Blend SrcAlpha OneMinusSrcAlpha


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

            sampler2D _MainTex;
            sampler2D _PatternTex;
            fixed _Progress;
            fixed _Alpha;
            fixed _Gradient;
            fixed _UseBlack;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float cast_value(float value, float oldmin, float oldmax, float newmin, float newmax)
            {
                value = clamp(oldmin, oldmax, value);
                return (value - oldmin) / (oldmax - oldmin) * (newmax - newmin) + newmin;
            }


            float4 frag(v2f i) : SV_Target
            {
                _Progress = cast_value(_Progress, 0.0, 1.0, -_Gradient, 1.0);

                float4 transit = tex2D(_PatternTex, i.uv);
                float4 main_tex = float4(0, 0, 0, 1);
                if (_UseBlack < 0.5)
                {
                    main_tex = tex2D(_MainTex, i.uv);
                    main_tex.a = _Alpha;
                }

                // 使用 float 提高计算中间值的精度
                float range_val = transit.r - _Progress;

                if (transit.r < _Progress)
                {
                    main_tex.a *= _Alpha;
                    return main_tex;
                }
                if (transit.r < _Progress + _Gradient)
                {
                    float alpha = 1.0 - saturate(range_val / _Gradient);
                    main_tex.a *= _Alpha * alpha;
                    return main_tex;
                }
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}