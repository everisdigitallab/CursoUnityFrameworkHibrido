// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/SunShader"
{
    Properties
    {
        _MidColor("Mid Color", Color) = (1,1,1,1)
		_EdgeColor("EdgeColor Color", Color) = (1,1,1,1)
		_Fresnel("Fresnel", Float) = 0.2
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
				float3 worldNormal : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
            };

			fixed4 _MidColor;
			fixed4 _EdgeColor;
			fixed _Fresnel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                return o;
            }

			void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
			{
				Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed fresnel;
				Unity_FresnelEffect_float(i.worldNormal, i.viewDir, _Fresnel, fresnel);
                fixed4 col = lerp(_MidColor, _EdgeColor, fresnel);
                return col;
            }
            ENDCG
        }
    }
}
