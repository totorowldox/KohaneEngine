Shader "Unlit/AlphaBlend"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _TransitTex ("Transit Texture", 2D) = "white" {}
        _Progress ("Transit Progress", Range(0, 1)) = 0
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        
        Cull Off
        ZTest Always
        ZWrite Off
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _TransitTex;
            float4 _TransitTex_ST;
            
            fixed _Progress;
            fixed _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_TransitTex, i.uv);
                fixed4 ret = col * (1 - _Progress) + col2 * _Progress;
                ret *= i.color;
                ret.a *= _Alpha;
                return ret;
            }
            ENDCG
        }
    }
}
