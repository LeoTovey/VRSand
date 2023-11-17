
Shader"Custom/S_SandErosionShader"
{
    Properties
    {
        _HeightMap("Height Map", 2D) = "black" {}
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

            sampler2D _HeightMap;
            half4 frag(v2f i) : SV_Target
            {
                float2 texCoords = i.uv;
                float2 uv = texCoords;
                half4 height = tex2D(_HeightMap, uv).rgba;
                return half4(height.xyz, 1.0);
            }

            ENDCG



        }

    }
}
