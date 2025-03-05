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
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha // 启用透明度混合
            ZWrite Off // 关闭深度写入
            ZTest LEqual // 深度测试
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

            // 顶点着色器
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // 正确转换纹理坐标
                return o;
            }

            // 片段着色器
            fixed4 frag(v2f i) : SV_Target
            {
                // 计算渐变方向和偏移
                float2 gradDir = normalize(_GradientDirection.xy);
                float2 uvOffset = i.uv - _GradientStartUV.xy;

                // 计算渐变因子
                float gradFactor = dot(uvOffset, gradDir) / length(gradDir);
                gradFactor = saturate(gradFactor); // 确保在 [0, 1] 范围内

                // 计算渐变颜色
                fixed4 col = lerp(_GradientStartColor, _GradientEndColor, gradFactor);

                // 获取主纹理的颜色（处理透明度）
                fixed4 texCol = tex2D(_MainTex, i.uv);

                // 将渐变的透明度和纹理的透明度结合
                col.a *= texCol.a;

                return col;
            }
            ENDCG
        }
    }
}
