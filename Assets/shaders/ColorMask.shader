Shader "Custom/ColorMask"
{
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}  
        _TransparencyThreshold ("Transparency Threshold", Float) = 0.0  
        _TransparencyRange ("Transparency Range", Float) = 1.0  
    }  
    SubShader  
    {  
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }  
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
                fixed4 col = tex2D(_MainTex, i.uv);  
                // 根据世界Y坐标计算透明度  
                // 当Y坐标在Threshold附近时，透明度最低，随着Y值远离Threshold，透明度增加  
                float alphaFactor = saturate((i.worldY - _TransparencyThreshold) / _TransparencyRange + 0.5);  
                
                // 只改变透明度，不改变颜色  
                col.a *= alphaFactor;  
                return col;  
            }  
            ENDCG  
        }  
    }  
    FallBack "Diffuse"  
}