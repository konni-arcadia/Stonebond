Shader "Sprites/Special Light Effects"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_AlternateTex ("Alternate Sprite Texture", 2D) = "white" {}
		_AlternateBlend ("Alternate blend", Range(0,1)) = 0.5
		
		_MaskTex ("Alternate Mask", 2D) = "white" {}
		
		_LightMap ("LightMap Texture", 2D) = "grey" {}
		_LightMapBlend ("Light Blend", Range(0,1)) = 0
		
		_Color ("Tint", Color) = (1,1,1,1)
		_Emission ("Emission", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog keepalpha
		#pragma multi_compile _ PIXELSNAP_ON

		sampler2D _MainTex;
		sampler2D _AlternateTex;
		sampler2D _MaskTex;
		sampler2D _LightMap;
		
		float _LightMapBlend;
		float _AlternateBlend;
		
		fixed4 _Color;
		fixed4 _Emission;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
          	float4 screenPos;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			fixed4 lm = tex2D(_LightMap, screenUV);
			fixed4 am = tex2D(_MaskTex, screenUV);
			
			fixed4 at = tex2D(_AlternateTex, IN.uv_MainTex);
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			
			o.Albedo = lerp(c.rgb * c.a, at.rgb * at.a, am.rgb * _AlternateBlend);
			o.Alpha = lerp(c.a, at.a, am.rgb * _AlternateBlend);
			
		}
		ENDCG
	}

Fallback "Transparent/VertexLit"
}
