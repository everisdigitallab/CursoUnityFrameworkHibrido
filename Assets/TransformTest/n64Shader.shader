Shader "Unlit/n64Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TexSize("Texture Size", Int) = 1
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _TexSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			float4 n64BilinearFilter(float4 vtx_color, float2 texcoord_0, float Texture_X, float Texture_Y, sampler2D ColorSampler)
			{

				float2 tex_pix_a = float2(1 / Texture_X,0);
				float2 tex_pix_b = float2(0,1 / Texture_Y);
				float2 tex_pix_c = float2(tex_pix_a.x,tex_pix_b.y);
				float2 half_tex = float2(tex_pix_a.x*0.5,tex_pix_b.y*0.5);
				float2 UVCentered = texcoord_0 - half_tex;

				float4 diffuseColor = tex2D(ColorSampler,UVCentered);
				float4 sample_a = tex2D(ColorSampler,UVCentered + tex_pix_a);
				float4 sample_b = tex2D(ColorSampler,UVCentered + tex_pix_b);
				float4 sample_c = tex2D(ColorSampler,UVCentered + tex_pix_c);

				float interp_x = modf(UVCentered.x * Texture_X, Texture_X);
				float interp_y = modf(UVCentered.y * Texture_Y, Texture_Y);

				if (UVCentered.x < 0)
				{
					interp_x = 1 - interp_x * (-1);
				}
				if (UVCentered.y < 0)
				{
					interp_y = 1 - interp_y * (-1);
				}

				diffuseColor = (diffuseColor + interp_x * (sample_a - diffuseColor) + interp_y * (sample_b - diffuseColor))*(1 - step(1, interp_x + interp_y));
				diffuseColor += (sample_c + (1 - interp_x) * (sample_b - sample_c) + (1 - interp_y) * (sample_a - sample_c))*step(1, interp_x + interp_y);

				return diffuseColor * vtx_color;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 bfCol = n64BilinearFilter(float4(1,1,1,1), i.uv, _TexSize, _TexSize, _MainTex);
                return bfCol;
            }
            ENDCG
        }
    }
}
