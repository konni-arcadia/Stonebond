Shader "Sprites/Special Light Effects Xtreme"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		
		_AlternateTex ("Alternate texture file", 2D) = "white" {}
		_AlternateBlend ("Alternate texture blend", Range(0,1)) = 0.5
		_AlternateTexMask ("Alternate texture mask", 2D) = "white" {}
		
		_ChromaTex ("Chroma texture file", 2D) = "white" {}
		_ChromaTexColor ("Chroma texture tint", Color) = (1,1,1,1)

		_NormalMapTex ("Normal Map", 2D) = "white" {}
		
		//
		
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
		sampler2D _AlternateTexMask;
		sampler2D _ChromaTex;
		sampler2D _NormalMapTex;
		sampler2D _LightMap;
		
		float _AlternateBlend;
		float _LightMapBlend;
		
		fixed4 _Color;
		fixed4 _ChromaTexColor;
		fixed4 _Emission;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
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
			
			fixed4 altTex = tex2D(_AlternateTex, IN.uv_MainTex);
			fixed4 altMsk = tex2D(_AlternateTexMask, screenUV);
			
			fixed4 chrTex = tex2D(_ChromaTex, screenUV);
			
			fixed4 nmpTex = tex2D(_NormalMapTex, screenUV);
			
			fixed4 lmpTex = tex2D(_LightMap, screenUV);
			
			fixed4 srcTex = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			
			o.Albedo = srcTex.rgb * srcTex.a;
			//o.Albedo = lerp( srcTex.rgb * srcTex.a, altTex.rgb * altTex.a, altMsk.rgb * _AlternateBlend );
			
			o.Alpha = srcTex.a;
			//o.Alpha = lerp( srcTex.a, altTex.a, altMsk.rgb * _AlternateBlend );
			
			//o.Normal = UnpackNormal (tex2D (_NormalMapTex, IN.uv_BumpMap));
			
		}
		ENDCG
	}

Fallback "Transparent/VertexLit"
}
