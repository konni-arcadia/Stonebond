Shader "Sprites/Special Light Effects Hyper Xtreme"
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
		
		_Color ("Tint", Color) = (1,1,1,1)	

        _TintBurn ("TintBurn", Range(0,1)) = 0.2

        _TintEmissionFactor ("TintEmissionFactor", float) = 1.0
        _ChromaEmissionFactor("ChromaEmissionFactor", float) = 1.0
		
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			//"RenderType"="Opaque" 
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off // On
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
		
		float _AlternateBlend;
		
		fixed4 _Color;
		fixed4 _ChromaTexColor;

        float _TintBurn;
        float _TintEmissionFactor;
        float _ChromaEmissionFactor;

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
			o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			
			fixed4 altTex = tex2D(_AlternateTex, IN.uv_MainTex);
			fixed4 altMsk = tex2D(_AlternateTexMask, screenUV);
			
			fixed4 chrTex = tex2D(_ChromaTex, IN.uv_MainTex);
			
			fixed4 nmpTex = tex2D(_NormalMapTex, IN.uv_BumpMap);			
            fixed4 neutralNormalMap = (0, 0, 0, 0);
			
			fixed4 srcTex = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			
			float altMaskInfluence = altMsk.rgb * _AlternateBlend;		
						
			o.Albedo = lerp( srcTex.rgb * _Color.rgb * (1.0 - _TintBurn) * srcTex.a, altTex.rgb * altTex.a, altMaskInfluence )
				+ _Color.rgb * _TintBurn * srcTex.a * _Color.a;			
			
			o.Alpha = lerp( srcTex.a, altTex.a, altMaskInfluence );
						
			o.Emission = _ChromaTexColor.rgb * chrTex.a * _ChromaEmissionFactor
			 + srcTex.rgb * _Color.rgb * srcTex.a * _TintEmissionFactor;
			
			//o.Normal = UnpackNormal ( lerp( neutralNormalMap, nmpTex, 1 ) );
			o.Normal = UnpackNormal ( neutralNormalMap );
			
		}
		ENDCG
	}

	//Fallback "Transparent/VertexLit"
	Fallback "Diffuse"
	
}
