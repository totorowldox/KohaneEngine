Shader "Unlit/AlphaBlend"
{
    Properties
    {
        [PerRendererData] _MainTex ("Old Texture (Main)", 2D) = "white" {}
        _SecondTex ("New Texture", 2D) = "white" {}
        _Lerp ("Transition Progress", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        
        Cull Off Lighting Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;
            float _Lerp;

            v2f vert(appdata_t v) {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col1 = (tex2D(_MainTex, i.texcoord));
                fixed4 col2 = (tex2D(_SecondTex, i.texcoord));

                fixed4 finalCol;
                finalCol.rgb = lerp(col1.rgb * col1.a, col2.rgb * col2.a, _Lerp);
                finalCol.a   = lerp(col1.a, col2.a, _Lerp);

                finalCol *= i.color;

                return finalCol;
            }
            ENDCG
        }
    }
}