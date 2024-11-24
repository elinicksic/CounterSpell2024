Shader "Custom/LightMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass {
            // Stencil setup
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            // Render the light area normally (with the light cone)
            CGPROGRAM
            #pragma surface surf Lambert
            #include "UnityCG.cginc"

            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;

            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = float3(1, 1, 1); // Light color (or texture)
                o.Alpha = 1.0; // Full opacity for the light area
            }
            ENDCG
        }

        // Inverted effect (everything except the light cone is blocked)
        Pass {
            Stencil {
                Ref 1
                Comp Equal
                Pass Keep
            }

            // Render the background (everything that is blocked)
            CGPROGRAM
            #pragma surface surf Lambert
            #include "UnityCG.cginc"

            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;

            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = float3(0, 0, 0); // Background or blacked-out area
                o.Alpha = 1.0; // Full opacity for blocked area
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
