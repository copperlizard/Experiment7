Shader "Hidden/01_SpeedBlurShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float _BlurStrength;

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f IN) : SV_Target
			{	
				float samples[10];
				samples[0] = -0.08;
				samples[1] = -0.05;
				samples[2] = -0.03;
				samples[3] = -0.02;
				samples[4] = -0.01;
				samples[5] = 0.01;
				samples[6] = 0.02;
				samples[7] = 0.03;
				samples[8] = 0.05;
				samples[9] = 0.08;

				float2 dir = 0.5 - IN.uv;
				float dist = sqrt(dir.x * dir.x + dir.y * dir.y);
				dir = dir / dist;
				
				float4 color = tex2D(_MainTex, IN.uv);
				float4 sum = float4(0.0, 0.0, 0.0, 0.0);

				for (int i = 0; i < 10; i++)
				{					;
					sum += tex2D(_MainTex, IN.uv + dir * samples[i] * 0.5); 
				}
				
				sum *= 1.0 / 11.0;

				float t = dist * 2.2;
				t = clamp(t, 0.0, 1.0);

				t = smoothstep(0.15, 0.5, t);

				return lerp(color, sum, t * _BlurStrength);
			}
			ENDCG
		}
	}
}
