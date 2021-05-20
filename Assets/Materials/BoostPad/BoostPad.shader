Shader "Custom/BoostPad"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 objectPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.objectPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {

                float bandCurveStrength = 0.1;
                float bandWavyness = 10;

                float bandSideOffset = cos(i.objectPosition.x * bandWavyness) * bandCurveStrength;

                float bandSize = 0.08;
                float bandSpeed = 0.8;

                float bandForwardOffset = (i.objectPosition.y - (_Time.y * bandSpeed)) + bandSideOffset;

                float band = sin(bandForwardOffset / bandSize);

                band = ceil(band);

                return float4(band, 1 - band, band, 1);
            }
            ENDCG
        }
    }
}
