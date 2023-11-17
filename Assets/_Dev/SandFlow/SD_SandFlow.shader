Shader "Custom/SD_SandFlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed", Float) = 1.0
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _FlowSpeed;
            half _Transparency;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 flow = float2(0.1, 0.1);
                float offset = sin(_Time.y * _FlowSpeed) * 0.1;
                float2 newUV = i.texcoord + flow * offset;
                half4 c = tex2D(_MainTex, newUV);
                c.a = _Transparency; // …Ë÷√Õ∏√˜∂»
                return c;
            }
            ENDCG
        }
    }
}
