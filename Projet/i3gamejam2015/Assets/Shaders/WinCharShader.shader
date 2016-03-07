Shader "UI/WinChar"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		

		
		_ChromaTex ("Chroma texture file", 2D) = "black" {}
		_ChromaTexColor ("Chroma texture tint", Color) = (1,1,1,1)

		_Color ("Tint", Color) = (1,1,1,1)
		
		_Emission ("Emission", Color) = (1,1,1,1)
		
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 1
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
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alpha
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma shader_feature REDIFY_ON
		//#pragma fragment frag

		sampler2D _MainTex;
		sampler2D _ChromaTex;
		
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
			o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			
		
			
			fixed4 chrTex = tex2D(_ChromaTex, IN.uv_MainTex);
			

			
			fixed4 white = (1,1,1,1);

			

			

			
			fixed4 srcTex = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			

			
			float tintBurn = 0.2;
			
			//o.Albedo = srcTex.rgb * IN.color * srcTex.a+ ( _ChromaTexColor * chrTex.a* srcTex.a * (1-tintBurn));
			//o.Albedo = ( srcTex.rgb * IN.color * srcTex.a );
			//	+ ( _ChromaTexColor * chrTex.a * (1-tintBurn));
			o.Albedo = ( srcTex.rgb * _Color.rgb * (1-tintBurn) * srcTex.a)
			+ (_Color.rgb * tintBurn * srcTex.a * _Color.a)
				;
				//;
			
			o.Alpha = srcTex.a
			 + chrTex.a;
			//o.Alpha = srcTex.a;
			
			
			o.Emission = _ChromaTexColor.rgb * chrTex.a * 0.75
			 + srcTex.rgb * _Color.rgb * (1-tintBurn) * srcTex.a * .5;
			

			
		}
		ENDCG
	}

	//Fallback "Transparent/VertexLit"
	Fallback "Diffuse"
	
}