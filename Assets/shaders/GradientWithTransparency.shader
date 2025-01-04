Shader "Custom/GradientWithTransparency"
{
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}  
        _GradientStartColor ("Start Color", Color) = (1,1,1,1)
        _GradientEndColor ("End Color", Color) = (0,0,0,1)
        _GradientDirection ("Gradient Direction", Vector) = (1,0,0,0)
        _GradientStartUV ("Gradient Start UV", Vector) = (0,0.5,0,0)
        _TransparencyThreshold ("Transparency Threshold", Float) = 0.0  
        _TransparencyRange ("Transparency Range", Float) = 1.0  
    }  

    SubShader  
    {  
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }  
        LOD 100  

        Blend SrcAlpha OneMinusSrcAlpha // 使用alpha混合  

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
                float worldY : TEXCOORD1; // 用于传递世界坐标的Y值  
            };  

            sampler2D _MainTex;  
            float4 _MainTex_ST;  
            float4 _GradientStartColor;  
            float4 _GradientEndColor;  
            float4 _GradientDirection;  
            float4 _GradientStartUV;  
            float _TransparencyThreshold;  
            float _TransparencyRange;  

            v2f vert (appdata v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);  
                // 计算世界坐标的Y值并传递给片元着色器  
                o.worldY = mul(unity_ObjectToWorld, v.vertex).y;  
                return o;  
            }  

            fixed4 frag (v2f i) : SV_Target  
            {  
                // 计算渐变颜色  
                float2 gradDir = normalize(_GradientDirection.xy);  
                float2 uvOffset = i.uv - _GradientStartUV.xy;  
                float gradFactor = dot(uvOffset, gradDir) / length(gradDir * (i.uv.x - _GradientStartUV.x > 0 ? 1 : -1));  
                gradFactor = saturate(gradFactor);  
                fixed4 col = lerp(_GradientStartColor, _GradientEndColor, gradFactor);  

                // 根据世界Y坐标计算透明度  
                float alphaFactor = saturate((i.worldY - _TransparencyThreshold) / _TransparencyRange + 0.5);  
                col.a *= alphaFactor;  // 修改透明度，保持颜色不变

                // 还可以选择是否结合纹理颜色  
                // fixed4 texCol = tex2D(_MainTex, i.uv);  
                // col *= texCol;

                return col;  
            }  
            ENDCG  
        }  
    }  
    FallBack "Diffuse"  
}
