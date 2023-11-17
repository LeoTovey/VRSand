Shader "Custom/SD_DisplacementShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _ParallaxMap("Parallax Map", 2D) = "black" {}
        _ParallaxScale("Parallax Scale", float) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert 
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertexPos : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
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


            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _ParallaxMap;
            float _ParallaxScale;

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
                float currentDepthMapValue = tex2D(_ParallaxMap, currentTexCoords).r;

                for (int i = 1; i < maxLayers && currentLayerDepth < currentDepthMapValue; i++) 
                {
                    // shift texture coordinates along direction of P
                    currentTexCoords -= deltaTexCoords;
                    // get depthmap value at current texture coordinates
                    currentDepthMapValue = tex2D(_ParallaxMap, currentTexCoords).r;  
                    // get depth of next layer
                    currentLayerDepth += layerDepth;  
                }
                
      
                
    
                // -- parallax occlusion mapping interpolation from here on
                // get texture coordinates before collision (reverse operations)
                float2 prevTexCoords = currentTexCoords + deltaTexCoords;

                // get depth after and before collision for linear interpolation
                float afterDepth  = currentDepthMapValue - currentLayerDepth;
                float beforeDepth = tex2D(_ParallaxMap, prevTexCoords).r - currentLayerDepth + layerDepth;
 
                // interpolation of texture coordinates
                float weight = afterDepth / (afterDepth - beforeDepth);
                float2 finalTexCoords = prevTexCoords * weight + currentTexCoords * (1.0 - weight);

                return finalTexCoords;
            }

            float3 InitializeFragmentNormal(float2 uv) 
            {
	            float2 du = float2(0.01, 0);
	            float u1 = tex2D(_ParallaxMap, uv - du);
	            float u2 = tex2D(_ParallaxMap, uv + du);
	            float2 dv = float2(0, 0.01);
	            float v1 = tex2D(_ParallaxMap, uv - dv);
	            float v2 = tex2D(_ParallaxMap, uv + dv);
	            return normalize(float3(u1 - u2, 1.0, v1 - v2));
            }


            half4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.tangentViewPos);
                float2 texCoords = i.uv;
                texCoords = ParallaxMapping(texCoords,  viewDir);

                //if(texCoords.x > 1.0 || texCoords.y > 1.0 || texCoords.x < 0.0 || texCoords.y < 0.0)
                    //discard;

                half3 normal = tex2D(_NormalMap, texCoords).rgb;
                //normal = normalize(normal * 2.0 - 1.0);   
                normal = InitializeFragmentNormal(texCoords);
    
                half3 color = tex2D(_MainTex, texCoords).rgb;
                half3 ambient = 0.0 * color;

                half3 lightDir = normalize(i.tangentLightPos);
                float diff = max(dot(lightDir, normal), 0.0);
                half3 diffuse = diff * color;
 
                half3 reflectDir = reflect(-lightDir, normal);
                half3 halfwayDir = normalize(lightDir + viewDir);  
                half spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

                half3 specular = half3(0.2, 0.2, 0.2) * spec;

                return half4(ambient + diffuse + specular, 1.0f);
            }

            ENDCG



        }

    }
    FallBack "Diffuse"
}

