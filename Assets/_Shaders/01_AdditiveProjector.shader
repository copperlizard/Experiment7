// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/01_AdditiveProjector"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "gray" {}
		_ShadowTex("Cookie", 2D) = "gray" {}
		_FalloffTex("FallOff", 2D) = "white" {}
	}

	Subshader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
		{			
			ZWrite Off
			ColorMask RGB			
			//Blend One One
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend OneMinusDstColor One
			Offset -1, -1

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				half2 uvTexcoord : TEXCOORD2;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			//v2f vert(float4 vertex : POSITION)
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvShadow = mul(unity_Projector, v.vertex);
				o.uvFalloff = mul(unity_ProjectorClip, v.vertex);
				o.uvTexcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 _Color;
			
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;

			fixed4 frag(v2f i) : SV_Target
			{	
				fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 col = (tex2D(_MainTex, i.uvTexcoord) * _Color.a) * (texS * texF.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1, 1, 1, 1));				
				return col;
			}

			ENDCG
		}
	}
}