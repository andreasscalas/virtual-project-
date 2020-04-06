Shader "Custom/Normal Mapping" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		//1
        _Bump ("Bump", 2D) = "bump" {}
	}
	SubShader 
	{

		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf CustomDiffuse
		
		sampler2D _MainTex;
		
		//2
        sampler2D _Bump;                

		struct Input 
		{
			float2 uv_MainTex;
			
			//3
            float2 uv_Bump;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			
			//4
            o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
            
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		inline float4 LightingCustomDiffuse (SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			float difLight = max(0, dot (s.Normal, lightDir));
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
			col.a = s.Alpha;
			return col;
		}

		ENDCG
	} 
	FallBack "Diffuse"
}