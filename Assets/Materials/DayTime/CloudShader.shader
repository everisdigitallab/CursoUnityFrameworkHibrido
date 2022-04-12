// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/CloudShader"
{
	Properties
	{
		_MinimumY("Minimum Y", Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_ShadowColor("Shadow Color", Color) = (0.6, 0.7, 1, 1)
		_ShadowDirection("Shadow Direction", Vector) = (-1,-1,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD0;
            };

			fixed _MinimumY;
			fixed4 _Color;
			fixed4 _ShadowColor;
			fixed3 _ShadowDirection;

            v2f vert (appdata v)
            {
                v2f o;
				fixed4 vertexPos = mul(unity_ObjectToWorld, v.vertex);
				vertexPos.y = max(_MinimumY, vertexPos.y);
				o.vertex = mul(UNITY_MATRIX_VP, vertexPos);
				o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed shadow = dot(i.normal, normalize(_ShadowDirection));
                fixed4 col = lerp(_Color, _ShadowColor, (shadow + 1) / 2);

                return col;
            }
            ENDCG
        }
    }
}
