Shader "Custom/GradientShader"
{
   Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GradientStartColor ("Start Color", Color) = (1,1,1,1)
        _GradientEndColor ("End Color", Color) = (0,0,0,1)
        _GradientDirection ("Gradient Direction", Vector) = (1,0,0,0)
        _GradientStartUV ("Gradient Start UV", Vector) = (0,0.5,0,0) 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GradientStartColor;
            float4 _GradientEndColor;
            float4 _GradientDirection;
            float4 _GradientStartUV;
 
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {

                float2 gradDir = normalize(_GradientDirection.xy);
 
                float2 uvOffset = i.uv - _GradientStartUV.xy;
 
                float gradFactor = dot(uvOffset, gradDir) / length(gradDir * (i.uv.x - _GradientStartUV.x > 0 ? 1 : -1)); 
 
                gradFactor = saturate(gradFactor);
 
                fixed4 col = lerp(_GradientStartColor, _GradientEndColor, gradFactor);
 
                //（可选）如果要与基础纹理合并，则乘以纹理样本
                // fixed4 texCol = tex2D(_MainTex, i.uv);
                // col *= texCol;
 
                return col;
            }
            ENDCG
        }
    }
}