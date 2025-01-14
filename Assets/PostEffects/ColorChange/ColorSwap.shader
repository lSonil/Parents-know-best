Shader "Unlit/ExactColorSwap"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OriginalColor("Original Color", Color) = (1,1,1,1)
        _TargetColor("Target Color", Color) = (1,1,1,1)
        _OriginalColor1("Original Color", Color) = (1,1,1,1)
        _TargetColor1("Target Color", Color) = (1,1,1,1)
        _Tolerance("Tolerance", Range(0, 0.01)) = 0.001
    }

    SubShader
    {
        Tags {"Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        ZTest Off

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
                float4 color : COLOR;  // Vertex color

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;  // Pass vertex color to fragment shader
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OriginalColor;
            float4 _TargetColor;
            float4 _OriginalColor1;
            float4 _TargetColor1;
            float _Tolerance;
            float _Alpha;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;  // Pass vertex color to the fragment shader
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                
                // Color replacement logic
                if (length(col - _OriginalColor) < _Tolerance)
                {
                    return half4(_TargetColor.rgb, i.color.a); // Preserve original alpha
                }

                if (length(col - _OriginalColor1) < _Tolerance)
                {
                    return half4(_TargetColor1.rgb, i.color.a); // Preserve original alpha
                }
                return col; // Return unmodified color and alpha
            }
            ENDCG
        }
    }
}
