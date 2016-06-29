Shader "Sprites/Special Light Effects Hyper Xtreme"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		
		_AlternateTex ("Alternate texture file", 2D) = "white" {}
		_AlternateBlend ("Alternate texture blend", Range(0,1)) = 0.5
		_AlternateTexMask ("Alternate texture mask", 2D) = "white" {}
		
		_ChromaTex ("Chroma texture file", 2D) = "white" {}
		_ChromaColor ("Chroma Color", Color) = (1,1,1,1)

		_NormalMapTex ("Normal Map", 2D) = "white" {}				
		
		_BodyColor ("Body Color", Color) = (1,1,1,1)

        _TintBurn ("Tint Burn", Range(0,1)) = 0.2

        _BodyEmissionFactor ("Body Emission Factor", float) = 0.5
        _ChromaEmissionFactor("Chroma Emission Factor", float) = 0.8

        _ChromaLightPct ("Chroma Light Pct", range(0, 1)) = 1
        
        _BodyChromaLightMin ("Body Chroma Light Min", float) = 0.7
        _BodyChromaLightMax ("Body Chroma Light Max", float) = 1.0

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
		
		fixed4 _BodyColor;
		fixed4 _ChromaColor;

        float _TintBurn;
        float _BodyEmissionFactor;
        float _ChromaEmissionFactor;
        float _ChromaLightPct;
        
        float _BodyChromaLightMin;
        float _BodyChromaLightMax;

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
						
			o.Albedo = lerp( srcTex.rgb * _BodyColor.rgb * (1.0 - _TintBurn) * srcTex.a, altTex.rgb * altTex.a, altMaskInfluence )
				+ _BodyColor.rgb * _TintBurn * srcTex.a * _BodyColor.a;
			
			o.Alpha = lerp( srcTex.a, altTex.a, altMaskInfluence );
						            
            float bodyOpacity = _BodyChromaLightMin + (_BodyChromaLightMax - _BodyChromaLightMin) * _ChromaLightPct;

            o.Emission = _ChromaColor.rgb * chrTex.a * _ChromaEmissionFactor  * _ChromaLightPct
                + srcTex.rgb * _BodyColor.rgb * srcTex.a * _BodyEmissionFactor * bodyOpacity;
			
			//o.Normal = UnpackNormal ( lerp( neutralNormalMap, nmpTex, 1 ) );
			o.Normal = UnpackNormal ( neutralNormalMap );
			
		}
		ENDCG
	}

	//Fallback "Transparent/VertexLit"
	Fallback "Diffuse"
	
}
