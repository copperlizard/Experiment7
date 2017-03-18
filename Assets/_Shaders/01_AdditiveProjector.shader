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
			//#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f 
			{
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				half2 uvTexcoord : TEXCOORD2;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			//UNITY_INSTANCING_CBUFFER_START(MyProperties)
			//UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			//UNITY_INSTANCING_CBUFFER_END

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			//v2f vert(float4 vertex : POSITION)
			v2f vert(appdata_base v)
			{
				v2f o;
				//UNITY_SETUP_INSTANCE_ID(v);
				//UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvShadow = mul(unity_Projector, v.vertex);
				o.uvFalloff = mul(unity_ProjectorClip, v.vertex);
				o.uvTexcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.pos);
				
				return o;
			}			
			
			float4 _Color;

			sampler2D _ShadowTex;
			sampler2D _FalloffTex;

			fixed4 frag(v2f i) : SV_Target
			{	
				//UNITY_SETUP_INSTANCE_ID(i);
				fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 col = (tex2D(_MainTex, i.uvTexcoord) * UNITY_ACCESS_INSTANCED_PROP(_Color).a) * (texS * texF.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1, 1, 1, 1));				
				return col;
			}

			ENDCG
		}
	}
}