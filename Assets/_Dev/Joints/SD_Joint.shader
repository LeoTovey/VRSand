Shader "Custom/NewSurfaceShader"
{
    Properties
    {
       _Color("Color",Color) = (1,1,0,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert 
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertexPos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertexPos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertexPos = UnityObjectToClipPos(v.vertexPos);
                o.uv = v.uv;
                return o;
            }


            float4 _Color;

            half4 frag(v2f i) : SV_Target
            {
                return _Color;
            }

            ENDCG



        }

    }
}
