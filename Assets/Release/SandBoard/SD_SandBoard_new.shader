
Shader"Custom/S_SandBoardShader"
{
    Properties
    {
        _HeightMap("Height Map", 2D) = "black" {}
        _BackGround("Back Ground",Color) = (1,1,1,1)
        _GlowColor("Glow Color",Color) = (1,1,0,1)

        _SandColorTexture ("Sand Color Texture", 2D) = "white" {}
        _SandImpurityTexture ("Sand Impurity Texture", 2D) = "white" {}
        _SandNormalTexture ("Sand Normal Texture", 2D) = "bump" {}
        _SandNoiseHighTexture ("Sand Noise High Texture", 2D) = "white" {}
        _SandNoiseMediumTexture ("Sand Noise Medium Texture", 2D) = "white" {}
        _SandNoiseLowTexture ("Sand Noise Low Texture", 2D) = "white" {}
        _SandThicknessScale ("Thickness Scale", Float) = 1.0
        
        _Noise1StartMixThickness ("Noise 1 Start Mix Thickness", Float) = 0.2
        _Noise2StartMixThickness ("Noise 2 Start Mix Thickness", Float) = 0.5
        _Noise3StartMixThickness ("Noise 3 Start Mix Thickness", Float) = 0.8

        _SandNoiseLowScale ("Sand Noise Low Scale", Float) = 1.0
        _SandNoiseMediumScale ("Sand Noise Medium Scale", Float) = 1.0
        _SandNoiseHighScale ("Sand Noise High Scale", Float) = 1.0
        
        _SandNoiseLowUVScale ("Sand Noise Low UV Scale", Float) = 1.0
        _SandNoiseMediumUVScale ("Sand Noise Medium UV Scale", Float) = 1.0
        _SandNoiseHighUVScale ("Sand Noise High UV Scale", Float) = 1.0
        
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
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertexPos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 tangentLightPos : TEXCOORD1;
                float3 tangentViewPos : TEXCOORD2;
                float3 tangentFragPos : TEXCOORD3;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertexPos = UnityObjectToClipPos(v.vertexPos);
                o.uv = v.uv;
                float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w;
                float3x3 objectToTangent = float3x3(v.tangent.xyz, binormal, v.normal);
                o.tangentLightPos = mul(objectToTangent, ObjSpaceLightDir(v.vertexPos)).xyz;
                o.tangentViewPos  = mul(objectToTangent, ObjSpaceViewDir(v.vertexPos)).xyz;
                o.tangentFragPos  = mul(objectToTangent, o.vertexPos).xyz;

                return o;

            }

            sampler2D _HeightMap;
            float4 _BackGround;
            float4 _GlowColor;
            
            sampler2D _SandColorTexture;
            sampler2D _SandImpurityTexture;
            sampler2D _SandNormalTexture;
            
            sampler2D _SandNoiseHighTexture;
            sampler2D _SandNoiseMediumTexture;
            sampler2D _SandNoiseLowTexture;

            float _SandThicknessScale;
            float _Noise1StartMixThickness;
            float _Noise2StartMixThickness;
            float _Noise3StartMixThickness;

            float _SandNoiseLowScale;
            float _SandNoiseMediumScale;
            float _SandNoiseHighScale;

            float _SandNoiseLowUVScale;
            float _SandNoiseMediumUVScale;
            float _SandNoiseHighUVScale;
            

            half4 frag(v2f i) : SV_Target
            {
                float2 texCoords = i.uv;
                float2 uv = texCoords;

                half4 sand = tex2D(_HeightMap, uv).rgba;

                half4 sandTextureColor = tex2D(_SandColorTexture, uv * 3.0) ;
                half3 sandColor = sand.rgb * sandTextureColor.rgb;
                float sandHeight = sand.a;
                
                float mixRatio1 = saturate(sandHeight  / _Noise1StartMixThickness);
                float mixRatio2 = saturate((sandHeight - _Noise1StartMixThickness) / (_Noise2StartMixThickness - _Noise1StartMixThickness));
                float mixRatio3 = saturate((sandHeight - _Noise2StartMixThickness) / (_Noise3StartMixThickness - _Noise2StartMixThickness));
                float mixRatio4 = saturate((sandHeight - _Noise3StartMixThickness) / (1.0 - _Noise3StartMixThickness));

                float2 noiseUV = uv;
                fixed4 noise1 = tex2D (_SandNoiseLowTexture, noiseUV * _SandNoiseLowUVScale) * _SandNoiseLowScale;
                fixed4 noise2 = tex2D (_SandNoiseMediumTexture, noiseUV * _SandNoiseMediumUVScale) * _SandNoiseMediumScale;
                fixed4 noise3 = tex2D (_SandNoiseHighTexture, noiseUV * _SandNoiseHighUVScale) * _SandNoiseHighScale;
                
                float noise = mixRatio1 * noise1 + mixRatio2 * noise2 + mixRatio3 * noise3 + mixRatio4 * 1.0;

                sandHeight = saturate(noise);
                
                half3 finalColor = _BackGround.rgb * (1.0 - sandHeight) + sandHeight * sandColor;
                return half4(finalColor, 1.0);
            }

            ENDCG



        }

    }
}
