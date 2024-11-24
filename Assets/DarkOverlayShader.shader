Shader "Custom/FogOfWar"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _ViewRadius ("View Radius", Float) = 5
        _WallPeekRadius ("Wall Peek Radius", Float) = 1
        _FogSmoothing ("Fog Smoothing", Float) = 1
        _ShadowSoftness ("Shadow Softness", Float) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
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
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _PlayerPos;
            float _ViewRadius;
            float _WallPeekRadius;
            float _FogSmoothing;
            float4 _ShadowPoints[2880];
            float _ShadowSoftness;
            int _ShadowCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }
            
            float GetShadowFactor(float2 worldPos, float2 playerPos)
            {
                float2 toPoint = worldPos - playerPos;
                float distToPoint = length(toPoint);
                float shadowFactor = 0;
                
                float baseVisibility = 1;
                if (distToPoint > _ViewRadius)
                {
                    baseVisibility = saturate((_ViewRadius + _WallPeekRadius - distToPoint) / _WallPeekRadius);
                }
                
                float angleToPoint = atan2(toPoint.y, toPoint.x);
                if (angleToPoint < 0) angleToPoint += 2 * UNITY_PI;
                
                // Find nearest shadow points
                int nearestIdx = -1;
                float nearestDist = 1000000;
                
                for (int i = 0; i < _ShadowCount; i++)
                {
                    float2 shadowVec = _ShadowPoints[i].xy - playerPos;
                    float shadowAngle = atan2(shadowVec.y, shadowVec.x);
                    if (shadowAngle < 0) shadowAngle += 2 * UNITY_PI;
                    
                    float angleDiff = abs(shadowAngle - angleToPoint);
                    if (angleDiff > UNITY_PI) angleDiff = 2 * UNITY_PI - angleDiff;
                    
                    if (angleDiff < nearestDist)
                    {
                        nearestDist = angleDiff;
                        nearestIdx = i;
                    }
                }
                
                if (nearestIdx >= 0)
                {
                    float2 shadowPoint = _ShadowPoints[nearestIdx].xy;
                    float2 shadowNormal = _ShadowPoints[nearestIdx].zw;
                    float2 toShadow = shadowPoint - playerPos;
                    float distToShadow = length(toShadow);
                    
                    if (distToPoint > distToShadow)
                    {
                        // Reveal a small area around the wall
                        float wallReveal = saturate(1 - (distToPoint - distToShadow) / _WallPeekRadius);
                        float transition = saturate((distToPoint - distToShadow) / _ShadowSoftness);
                        shadowFactor = lerp(wallReveal * 0.5, 1, transition);
                        
                        float2 normalizedToPoint = normalize(toPoint);
                        float dotProduct = dot(shadowNormal, normalizedToPoint);
                        shadowFactor = max(shadowFactor * baseVisibility, saturate(1 - dotProduct));
                    }
                }
                
                return shadowFactor;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _PlayerPos);
                float fogAmount = saturate((dist - _ViewRadius) / _FogSmoothing);
                
                float shadowFactor = GetShadowFactor(i.worldPos, _PlayerPos);
                fogAmount = max(fogAmount, shadowFactor);
                
                // Reduce fog density for wall peek areas
                if (dist > _ViewRadius)
                {
                    float wallPeekFactor = saturate((_ViewRadius + _WallPeekRadius - dist) / _WallPeekRadius);
                    fogAmount = lerp(1, fogAmount, wallPeekFactor * 0.7); // 0.7 controls wall visibility
                }
                
                // Add subtle ambient light
                fogAmount = lerp(0.15, 1, fogAmount);
                
                return fixed4(0, 0, 0, fogAmount);
            }
            ENDCG
        }
    }
}