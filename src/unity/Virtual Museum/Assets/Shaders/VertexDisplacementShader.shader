Shader "Custom/HeightMapDeformSurface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightMap ("Height Map", 2D) = "black" {}
        _HeightScale ("Height Scale", Float) = 1.0
        _MaskTex ("Mask texture", 2D) = "transparent" {}
        _BlendFactor ("Blend Factor", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert

        sampler2D _MainTex;
        sampler2D _HeightMap;
        sampler2D _MaskTex;
        float _HeightScale;
        float _BlendFactor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
        };

        void vert(inout appdata_full v)
        {
            // Sample the height map
            float height = tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0)).r;
            
            // Adjust the vertex position by the height value
            v.vertex.z += height * _HeightScale;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from the base texture
            fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            
            fixed4 maskColor = tex2D(_MaskTex, IN.uv_MaskTex);

            fixed4 finalColor = lerp(baseColor, maskColor, _BlendFactor * maskColor.a);
            
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
