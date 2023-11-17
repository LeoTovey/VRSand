
Shader"Custom/S_SandBoardShader"
{
    Properties
    {
        _HeightMap("Height Map", 2D) = "black" {}
        _CollisionMap("Collision Map", 2D) = "black" {}
        _DisplacementHeightMap("Sand Art Map", 2D) = "black" {}
        _InitHeight("Init Height",float) = 1.0
        _BackGround("Back Ground",Color) = (1,1,1,1)
        _GlowColor("Glow Color",Color) = (1,1,0,1)
        _GranularMap02("GranularMap Level02", 2D) = "black" {}
        _GranularMap04("GranularMap Level04", 2D) = "black" {}
        _GranularMap06("GranularMap Level06", 2D) = "black" {}
        _GranularMap08("GranularMap Level08", 2D) = "black" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" }
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
            sampler2D _DisplacementHeightMap;
            sampler2D _CollisionMap;
            sampler2D _GranularMap08;
            sampler2D _GranularMap06;
            sampler2D _GranularMap04;
            sampler2D _GranularMap02;
            float _InitHeight;
            float4 _BackGround;
            float4 _GlowColor;

            float3 InitializeFragmentNormal(float2 uv) 
            {
	            float2 du = float2(0.01, 0);
	            float u1 = tex2D(_HeightMap, uv - du).a;
	            float u2 = tex2D(_HeightMap, uv + du).a;
	            float2 dv = float2(0, 0.01);
	            float v1 = tex2D(_HeightMap, uv - dv).a;
	            float v2 = tex2D(_HeightMap, uv + dv).a;
	            return normalize(float3(u1 - u2, 1.0, v1 - v2));
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = float2(i.uv.x, i.uv.y);
                float intensity = abs(sin(_Time.y)) * 0.5 + 0.5;
                half3 backColor = lerp(_BackGround.rgb, _GlowColor.rgb, intensity);
                half4 height = tex2D(_HeightMap, uv).rgba;
                half3 sandColor = 0;
                
                if (height.a > 0)
                {
                    sandColor = height.rgb / height.a;
                }
                half sandHeight = height.a / _InitHeight;
                
                half3 noise = tex2D(_GranularMap02, uv).rgb;

                if (sandHeight > 0.8)
                {
                    noise = tex2D(_GranularMap08, uv).rgb;
                }
                else if (sandHeight > 0.6)
                {
                    noise = tex2D(_GranularMap06, uv).rgb;
                }
                else if (sandHeight > 0.4)
                {
                    noise = tex2D(_GranularMap04, uv).rgb;
                }

                half3 finalColor = backColor * (1.0 - sandHeight * sandColor * noise.r);
                half4 tags = tex2D( _DisplacementHeightMap, uv).rgba;
                half4 color = half4(finalColor, 1.0) + tags;

                // Lighting
                float3 viewDir = normalize(i.tangentViewPos);
                float2 texCoords = i.uv;

                //half3 normal = tex2D(_NormalMap, texCoords).rgb;
                //normal = normalize(normal * 2.0 - 1.0);   
                half3 normal = InitializeFragmentNormal(texCoords);
    
                //half3 color = tex2D(_MainTex, texCoords).rgb;
                //half3 ambient = 0.0 * color;

                half3 lightDir = normalize(i.tangentLightPos);
                float diff = max(dot(lightDir, normal), 0.0);
                half3 diffuse = diff * color;
 
                half3 reflectDir = reflect(-lightDir, normal);
                half3 halfwayDir = normalize(lightDir + viewDir);  
                half specular = pow(max(dot(normal, halfwayDir), 0.0), 32.0);
                return half4(finalColor, 1.0);
            }

            ENDCG



        }

    }
}
