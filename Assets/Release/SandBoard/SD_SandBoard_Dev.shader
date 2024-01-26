
Shader"Custom/S_SandBoardShader"
{
    Properties
    {
        _HeightMap("Height Map", 2D) = "black" {}
        _InitHeight("Init Height",float) = 1.0
        _BackGround("Back Ground",Color) = (1,1,1,1)
        _SandColor("Sand Color",Color) = (1,1,1,1)
        _GlowColor("Glow Color",Color) = (1,1,0,1)
        _HeightMap_TexelSize_X("HeightMap TexelSize X", float) = 1.0
        _HeightMap_TexelSize_Y("HeightMap TexelSize Y", float) = 1.0
        _GranularMap02("GranularMap Level02", 2D) = "black" {}
        _GranularMap04("GranularMap Level04", 2D) = "black" {}
        _GranularMap06("GranularMap Level06", 2D) = "black" {}
        _GranularMap08("GranularMap Level08", 2D) = "black" {}
        _ParallaxScale("Parallax Scale", float) = 1
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
            sampler2D _DisplacementHeightMap;
            sampler2D _CollisionMap;
            sampler2D _GranularMap08;
            sampler2D _GranularMap06;
            sampler2D _GranularMap04;
            sampler2D _GranularMap02;
            float _InitHeight;
            float4 _BackGround;
            float4 _GlowColor;
            float4 _SandColor;
            float _HeightMap_TexelSize_X;
            float _HeightMap_TexelSize_Y;
            float _ParallaxScale;

            float3 InitializeFragmentNormal(float2 uv) 
            {
	            float2 du = float2(0.5 * _HeightMap_TexelSize_X, 0);
	            float u1 = tex2D(_HeightMap, uv - du).a;
	            float u2 = tex2D(_HeightMap, uv + du).a;
	            float2 dv = float2(0, 0.5 * _HeightMap_TexelSize_Y);
	            float v1 = tex2D(_HeightMap, uv - dv).a;
	            float v2 = tex2D(_HeightMap, uv + dv).a;
	            return normalize(float3(u1 - u2, 1.0, v1 - v2));
            }

            float2 ParallaxMapping(float2 texCoords, float3 viewDir)
            { 
                // number of depth layers
                const float minLayers = 10;
                const float maxLayers = 20;
                float numLayers = lerp(maxLayers, minLayers, abs(dot(float3(0.0, 0.0, 1.0), viewDir))); 

                // calculate the size of each layer
                float layerDepth = 1.0 / numLayers;
                // depth of current layer
                float currentLayerDepth = 0.0;
                // the amount to shift the texture coordinates per layer (from vector P)
                float2 P = viewDir.xy / viewDir.z * _ParallaxScale; 
                float2 deltaTexCoords = P / numLayers;
  
                // get initial values
                float2  currentTexCoords     = texCoords;
                float currentDepthMapValue = tex2D(_HeightMap, currentTexCoords).a;

                for (int i = 1; i < maxLayers && currentLayerDepth < currentDepthMapValue; i++) 
                {
                    // shift texture coordinates along direction of P
                    currentTexCoords -= deltaTexCoords;
                    // get depthmap value at current texture coordinates
                    currentDepthMapValue = tex2D(_HeightMap, currentTexCoords).a;  
                    // get depth of next layer
                    currentLayerDepth += layerDepth;  
                }
                
      
                
    
                // -- parallax occlusion mapping interpolation from here on
                // get texture coordinates before collision (reverse operations)
                float2 prevTexCoords = currentTexCoords + deltaTexCoords;

                // get depth after and before collision for linear interpolation
                float afterDepth  = currentDepthMapValue - currentLayerDepth;
                float beforeDepth = tex2D(_HeightMap, prevTexCoords).a - currentLayerDepth + layerDepth;
 
                // interpolation of texture coordinates
                float weight = afterDepth / (afterDepth - beforeDepth);
                float2 finalTexCoords = prevTexCoords * weight + currentTexCoords * (1.0 - weight);

                return finalTexCoords;
            }


            half4 frag(v2f i) : SV_Target
            {
                //float2 uv = float2(i.uv.x, i.uv.y);
                float3 viewDir = normalize(i.tangentViewPos);
                float2 texCoords = i.uv;
                //float2 texCoords = ParallaxMapping(i.uv,  viewDir);
                float2 uv = texCoords;
                //float intensity = abs(sin(_Time.y)) * 0.5 + 0.5;
                //half3 backColor = lerp(_BackGround.rgb, _GlowColor.rgb, intensity);
                half4 height = tex2D(_HeightMap, uv).rgba;
                half3 sandColor = half3(0.0, 0.0, 0.0);

                if (height.a > 0.0)
                {
                    sandColor = height.rgb / height.a;
                }



                if(texCoords.x > 1.0 || texCoords.y > 1.0 || texCoords.x < 0.0 || texCoords.y < 0.0)
                    discard;


     
                half sandHeight = clamp(height.a, 0.0, 1.0);
                

                //half3 noise = tex2D(_GranularMap02, uv).rgb;

                half3 noise = half3(0, 0, 0);

                if (sandHeight > 0.8)
                {
                    noise += tex2D(_GranularMap08, uv).rgb;
                }
                else if(sandHeight > 0.6)
                {
                    noise += tex2D(_GranularMap06, uv).rgb;
                }
                else if(sandHeight > 0.4)
                {
                    noise += tex2D(_GranularMap04, uv).rgb;
                }

                if (sandHeight > 0.0)
                {
                    noise += tex2D(_GranularMap08, uv).rgb;
                }
    
                noise = clamp(noise, 0.0, 1.0);
                
                half3 finalColor = lerp(_BackGround, sandColor, sandHeight * noise.r);
                //half3 finalColor = lerp(_BackGround, _SandColor, sandHeight * noise.r);

                half4 tags = tex2D( _DisplacementHeightMap, uv).rgba;
                half4 color = half4(finalColor, 1.0) + tags;

                // Lighting



                //half3 normal = tex2D(_NormalMap, texCoords).rgb;
                //normal = normalize(normal * 2.0 - 1.0);   
                half3 normal = InitializeFragmentNormal(texCoords);
    
                //half3 color = tex2D(_MainTex, texCoords).rgb;
                half3 ambient = finalColor;

                half3 lightDir = normalize(i.tangentLightPos);
                float diff = max(dot(lightDir, normal), 0.0);
                half3 diffuse = diff * finalColor;
 
                half3 reflectDir = reflect(-lightDir, normal);
                half3 halfwayDir = normalize(lightDir + viewDir);  
                half specular = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

                return half4(ambient, 1.0);
            }

            ENDCG



        }

    }
}
