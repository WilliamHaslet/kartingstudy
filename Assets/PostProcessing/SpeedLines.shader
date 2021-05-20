Shader "Custom/SpeedLines"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int triangleCount;
            float2 triangesPoints[1000];
            float opacity;

            float TriangleArea(float2 p1, float2 p2, float2 p3)
            {

                return abs(((p1.x * (p2.y - p3.y)) + (p2.x * (p3.y - p1.y)) + (p3.x * (p1.y - p2.y))) / 2.0);

            }

            float InTriangle(float2 p1, float2 p2, float2 p3, float2 checkPoint) 
            {

                float area = TriangleArea(p1, p2, p3);
                float area1 = TriangleArea(checkPoint, p2, p3);
                float area2 = TriangleArea(p1, checkPoint, p3);
                float area3 = TriangleArea(p1, p2, checkPoint);

                return (area1 + area2 + area3) - area;

            }

            float3 frag (v2f i) : SV_Target
            {
                float3 baseColor = tex2D(_MainTex, i.uv);

                float3 triangleColor = baseColor;
                
                for (int j = 0; j < triangleCount * 3; j += 3)
                {
                
                    float inTriangle = InTriangle(triangesPoints[j], triangesPoints[j + 1], triangesPoints[j + 2], i.uv);

                    if (inTriangle <= 0.00001) {

                        triangleColor = float3(1, 1, 1);

                        break;

                    }

                }

                return ((1 - opacity) * baseColor) + (opacity * triangleColor);
            }
            ENDCG
        }
    }
}
