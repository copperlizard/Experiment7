Shader "Custom/01_WorldShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		//#pragma surface surf Lambert vertex:vert
		
		struct Input 
		{
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
		};

		float mod289(float x)
		{
			return x - floor(x * (1.0 / 289.0)) * 289.0;
		}

		float4 mod289(float4 x)
		{
			return x - floor(x * (1.0 / 289.0)) * 289.0;
		}

		float4 perm(float4 x)
		{
			return mod289(((x * 34.0) + 1.0) * x);
		}

		float noise(float3 p)
		{
			float3 a = floor(p);
			float3 d = p - a;
			d = d * d * (3.0 - 2.0 * d);

			float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
			float4 k1 = perm(b.xyxy);
			float4 k2 = perm(k1.xyxy + b.zzww);

			float4 c = k2 + a.zzzz;
			float4 k3 = perm(c);
			float4 k4 = perm(c + 1.0);

			float4 o1 = frac(k3 * (1.0 / 41.0));
			float4 o2 = frac(k4 * (1.0 / 41.0));

			float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
			float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

			return o4.y * d.y + o4.x * (1.0 - d.y);
		}

		void vert (inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.worldNormal = v.normal;
			o.worldPos = v.vertex.xyz;
		}

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 t = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//o.Albedo = c.rgb;
			
			//Use worldNormal plus worldPos to paint world
			//o.Albedo *= IN.worldNormal;			
			//o.Albedo *= noise(IN.worldPos/10.0);
			
			fixed4 a = fixed4(0.5, 0.5, 0.8, 1.0);
			fixed4 b = fixed4(0.5, 0.0, 0.8, 1.0);
			fixed4 c = fixed4(0.8, 0.0, 1.0, 1.0);
			fixed4 d = fixed4(0.0, 0.0, 0.8, 1.0);


			fixed4 g = lerp(fixed4(0.8f, 0.0f, 1.0f, 1.0f), a + b * cos(2.0*3.14*(c * _Time.x + d)), noise(IN.worldPos*2.0) * sin(_Time.x*4.0));

			o.Albedo = lerp(t, g, smoothstep(0.6, 0.65, noise(IN.worldPos/10.0)));
			
			o.Metallic = lerp(0.0, 1.0, smoothstep(0.6, 0.65, noise(IN.worldPos / 10.0)));
			o.Smoothness = o.Metallic;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = t.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
