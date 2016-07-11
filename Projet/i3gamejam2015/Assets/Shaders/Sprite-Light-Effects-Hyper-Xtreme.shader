Shader "Sprites/Special Light Effects Hyper Xtreme"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
				
		_ChromaTex ("Chroma texture file", 2D) = "white" {}
		_ChromaColor ("Chroma Color", Color) = (1,1,1,1)

		_NormalMapTex ("Normal Map", 2D) = "white" {}				
		
        _BodyColor("Body Color", Color) = (1,1,1,1)

        _BodyEmissionFactor("Body Emission Factor", float) = 0.5
        _ChromaEmissionFactor("Chroma Emission Factor", float) = 0.8

        _ChromaLightPct("Chroma Light Pct", range(0, 1)) = 1

        _BodyChromaLightMin("Body Chroma Light Min", float) = 0.7
        _BodyChromaLightMax("Body Chroma Light Max", float) = 1.0

        _NormalYModifier("Normal Y Modifier (hacky fix)", float) = 1.0

		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Opaque" 
			//"RenderType"="Transparent"
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
		sampler2D _ChromaTex;
		sampler2D _NormalMapTex;			
		
		fixed4 _BodyColor;
		fixed4 _ChromaColor;

        float _BodyEmissionFactor;
        float _ChromaEmissionFactor;
        float _ChromaLightPct;
        
        float _BodyChromaLightMin;
        float _BodyChromaLightMax;

        float _NormalYModifier;

		struct Input
		{
			float2 uv_MainTex;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{						
			fixed4 chromaTex = tex2D(_ChromaTex, IN.uv_MainTex);						
						
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = mainTex.rgb * _BodyColor.rgb * mainTex.a;			
            o.Alpha = mainTex.a;
						            
            float bodyOpacity = lerp(_BodyChromaLightMin, _BodyChromaLightMax, _ChromaLightPct);
            o.Emission = chromaTex.rgb * _ChromaColor.rgb * chromaTex.a * _ChromaEmissionFactor  * _ChromaLightPct
                + mainTex.rgb * _BodyColor.rgb * mainTex.a * _BodyEmissionFactor * bodyOpacity;

            fixed4 nmpTex = tex2D(_NormalMapTex, IN.uv_MainTex);
            o.Normal = UnpackNormal(nmpTex);
            o.Normal.y = o.Normal.y * _NormalYModifier;
		}
		ENDCG
	}

	Fallback "Diffuse"	
}
