Shader "Custom/TimeSwapTransition"
{
    Properties
    {
        _ModernTex ("Modern Texture", 2D) = "white" {}
        _AncientTex ("Ancient Texture", 2D) = "white" {}
        _SwapDirection ("Swap Direction", Float) = 0

        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0
        _EdgeWidth ("Edge Width", Float) = 0.05

        _Distortion ("Distortion", Float) = 0
        _GlowStrength ("Glow Strength", Float) = 1
        _ChromaticAmount ("Chromatic Amount", Float) = 0
        _RuneStrength ("Rune Strength", Float) = 1
        _TransitionTime ("Transition Time", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Overlay"
        }

        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            sampler2D _ModernTex;
            sampler2D _AncientTex;
            float _SwapDirection;

            float4 _Center;
            float _Radius;
            float _EdgeWidth;

            float _Distortion;
            float _GlowStrength;
            float _ChromaticAmount;
            float _RuneStrength;
            float _TransitionTime;


            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;


                fixed4 modern =
                    tex2D(
                        _ModernTex,
                        uv
                    );


                fixed4 ancient =
                    tex2D(
                        _AncientTex,
                        uv
                    );


                float distanceFromPlayer =
                    distance(
                        uv,
                        _Center.xy
                    );


                float mask =
                    smoothstep(
                        _Radius - _EdgeWidth,
                        _Radius,
                        distanceFromPlayer
                    );


                // Outside circle = modern
                // Inside circle = ancient

                fixed4 modernToAncient = lerp(ancient, modern, mask);
                fixed4 ancientToModern = lerp(modern, ancient, mask);

                fixed4 col = lerp(
                    modernToAncient,
                    ancientToModern,
                    _SwapDirection
                );

                // DEBUG BLACK OUTLINE
                //

                float outlineWidth = 0.003;

                float outline =
                    step(abs(distanceFromPlayer - _Radius), 0.01);

                col.rgb = lerp(col.rgb, float3(0,0,0), outline);

                return col;
            }

            ENDCG
        }
    }
}