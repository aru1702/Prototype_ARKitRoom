Shader "Custom/ZBufferBlockingShaderwithColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
            {
                "RenderType"="Transparent"
                "Queue" = "Geometry-1" //
            }
        LOD 100
 
        GrabPass{
            "_BackgroundTexture"
        }
 
        Pass
        {
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
                float4 grabPos : TEXCOORD1;
 
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            sampler2D _BackgroundTexture;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Calculates screen grab position
                o.grabPos = ComputeGrabScreenPos(o.vertex);
 
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculates screen uv
                float2 screenUV = i.grabPos.xy / i.grabPos.w;
                return tex2D(_BackgroundTexture, screenUV);
            }
            ENDCG
        }
    }
}
