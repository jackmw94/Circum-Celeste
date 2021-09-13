// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "StarClusterPack/StarCluster_URP"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[Toggle]_LinearWorkflow("LinearWorkflow", Range( 0 , 1)) = 1
		[Toggle(_PERPARTICLEORIENTATION_ON)] _PerParticleOrientation("PerParticleOrientation", Float) = 0
		_Shape("Shape", 2D) = "white" {}
		_ColorTex("Color", 2D) = "white" {}
		_Size("Size", 2D) = "white" {}
		[Toggle]_Use_ProceduralShape("Use_ProceduralShape", Float) = 0
		_ProceduralShape_Iterations("ProceduralShape_Iterations", Float) = 10
		_ProceduralShape_OutExp("ProceduralShape_OutExp", Float) = 1
		_ProceduralShape_IterExp("ProceduralShape_IterExp", Float) = 2
		_Color_Tint("Color_Tint", Color) = (1,1,1,1)
		_Color_Multiplier("Color_Multiplier", Float) = 1
		_Color_Gamma("Color_Gamma", Color) = (1,1,1,1)
		_Color_Saturation("Color_Saturation", Float) = 1
		_Size_Multiplier("Size_Multiplier", Float) = 1
		_Variation_Shift("Variation_Shift", Float) = 0
		[Toggle(_USE_CAMERA_POSITION_ON)] _Use_Camera_Position("Use_Camera_Position", Float) = 0
		_Camera_Position("Camera_Position", Vector) = (0,0,0,0)
		[Toggle(_USE_ATTENUATION_ON)] _Use_Attenuation("Use_Attenuation", Float) = 0
		_Attenuation_Strength("Attenuation_Strength", Float) = 1
		_Attenuation_Exponent("Attenuation_Exponent", Float) = 0.8
		[Toggle]_Use_LensEffect("Use_LensEffect", Float) = 0
		_LensEffect("LensEffect", 2D) = "white" {}
		_LensEffect_Distance("LensEffect_Distance", Float) = 1
		_LensEffect_DistanceExponent("LensEffect_DistanceExponent", Float) = 3
		[Toggle(_USE_TWINKLE_ON)] _Use_Twinkle("Use_Twinkle", Float) = 0
		_Twinkle_Ramp("Twinkle_Ramp", 2D) = "white" {}
		_Twinkle_Speed("Twinkle_Speed", Float) = 1
		_Twinkle_Strength("Twinkle_Strength", Float) = 1
		_Minimum_Screen_Size("Minimum_Screen_Size", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 2.0

		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One One , One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature _PERPARTICLEORIENTATION_ON
			#pragma shader_feature _USE_CAMERA_POSITION_ON
			#pragma shader_feature _USE_ATTENUATION_ON
			#pragma shader_feature _USE_TWINKLE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color_Tint;
			float4 _Shape_ST;
			float4 _Color_Gamma;
			float3 _Camera_Position;
			float _Color_Multiplier;
			float _Twinkle_Strength;
			float _Twinkle_Speed;
			float _Minimum_Screen_Size;
			float _Use_ProceduralShape;
			float _ProceduralShape_OutExp;
			float _ProceduralShape_Iterations;
			float _ProceduralShape_IterExp;
			float _Variation_Shift;
			float _Size_Multiplier;
			float _Use_LensEffect;
			float _LensEffect_DistanceExponent;
			float _LensEffect_Distance;
			float _Attenuation_Exponent;
			float _Attenuation_Strength;
			float _LinearWorkflow;
			float _Color_Saturation;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Size;
			sampler2D _Shape;
			sampler2D _Twinkle_Ramp;
			sampler2D _ColorTex;
			sampler2D _LensEffect;


			float Exponentglow212_g11( float ramp , float size_exp , float mult_exp , int MaxIter )
			{
				int i = 1;
				float value = 0;
				float v01 = 0;
				float v02 = 0;
				float v03 = 0;
				while (i < MaxIter)
				{
				    v01 = MaxIter/(i+1);
				    v02 = pow(ramp, pow(size_exp, v01));
				    v03 = v02/pow((i+1), mult_exp);
				    value += v03;
				    i+=1;
				}
				return value;
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv062_g11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break63_g11 = ( uv062_g11 - float2( 0.5,0.5 ) );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 normalizeResult12_g11 = normalize( ( ase_worldPos - _Camera_Position ) );
				#ifdef _USE_CAMERA_POSITION_ON
				float3 staticSwitch14_g11 = normalizeResult12_g11;
				#else
				float3 staticSwitch14_g11 = ase_worldViewDir;
				#endif
				float3 temp_output_17_0_g11 = mul( float4( staticSwitch14_g11 , 0.0 ), GetObjectToWorldMatrix() ).xyz;
				float3 temp_output_21_0_g11 = cross( float3(0,1,0) , temp_output_17_0_g11 );
				float3 normalizeResult23_g11 = normalize( cross( temp_output_21_0_g11 , temp_output_17_0_g11 ) );
				float2 uv05_g11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break18_g11 = ( uv05_g11 - float2( 0.5,0.5 ) );
				float3 normalizeResult24_g11 = normalize( temp_output_21_0_g11 );
				#ifdef _PERPARTICLEORIENTATION_ON
				float3 staticSwitch38_g11 = ( ( normalizeResult23_g11 * break18_g11.y ) + ( normalizeResult24_g11 * break18_g11.x * -1.0 ) );
				#else
				float3 staticSwitch38_g11 = ( ( mul( float4( float3(1,0,0) , 0.0 ), mul(GetWorldToHClipMatrix(),GetObjectToWorldMatrix()) ).xyz * break63_g11.x ) + ( mul( float4( float3(0,1,0) , 0.0 ), mul(GetWorldToHClipMatrix(),GetObjectToWorldMatrix()) ).xyz * break63_g11.y ) );
				#endif
				float3 normalizeResult29_g11 = normalize( staticSwitch38_g11 );
				float clampResult89_g11 = clamp( _Attenuation_Strength , 0.0 , 100.0 );
				float clampResult254_g11 = clamp( ( length( ( ase_worldPos - _WorldSpaceCameraPos ) ) * 50.0 * clampResult89_g11 ) , 0.0 , 1.0 );
				float clampResult90_g11 = clamp( _Attenuation_Exponent , 0.0 , 100.0 );
				#ifdef _USE_ATTENUATION_ON
				float staticSwitch43_g11 = ( 1.0 * ( 1.0 / pow( clampResult254_g11 , clampResult90_g11 ) ) );
				#else
				float staticSwitch43_g11 = 1.0;
				#endif
				float clampResult87_g11 = clamp( _LensEffect_Distance , 0.0 , 100.0 );
				float clampResult83_g11 = clamp( ( clampResult87_g11 - length( ( ase_worldPos - _WorldSpaceCameraPos ) ) ) , 0.0 , clampResult87_g11 );
				float clampResult88_g11 = clamp( _LensEffect_DistanceExponent , 0.0 , 100.0 );
				float temp_output_85_0_g11 = pow( clampResult83_g11 , clampResult88_g11 );
				float lerpResult72_g11 = lerp( staticSwitch43_g11 , ( staticSwitch43_g11 * ( temp_output_85_0_g11 + 1.0 ) ) , _Use_LensEffect);
				float temp_output_104_0_g11 = ( _Variation_Shift / 100.0 );
				float4 appendResult98_g11 = (float4(( v.ase_color.g + temp_output_104_0_g11 ) , 0.0 , 0.0 , 0.0));
				float4 tex2DNode97_g11 = tex2Dlod( _Size, float4( appendResult98_g11.xy, 0, 0.0) );
				float3 linearToGamma267_g11 = FastLinearToSRGB( tex2DNode97_g11.rgb );
				float4 lerpResult268_g11 = lerp( tex2DNode97_g11 , float4( linearToGamma267_g11 , 0.0 ) , _LinearWorkflow);
				float2 uv_Shape = v.ase_texcoord.xy * _Shape_ST.xy + _Shape_ST.zw;
				float4 tex2DNode204_g11 = tex2Dlod( _Shape, float4( uv_Shape, 0, 0.0) );
				float3 linearToGamma266_g11 = FastLinearToSRGB( tex2DNode204_g11.rgb );
				float4 lerpResult265_g11 = lerp( tex2DNode204_g11 , float4( linearToGamma266_g11 , 0.0 ) , _LinearWorkflow);
				float2 uv0213_g11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_16 = (0.5).xx;
				float ramp212_g11 = ( 1.0 - ( length( ( uv0213_g11 - temp_cast_16 ) ) * 2.0 ) );
				float size_exp212_g11 = _ProceduralShape_IterExp;
				float mult_exp212_g11 = 1.0;
				int MaxIter212_g11 = (int)round( _ProceduralShape_Iterations );
				float localExponentglow212_g11 = Exponentglow212_g11( ramp212_g11 , size_exp212_g11 , mult_exp212_g11 , MaxIter212_g11 );
				float clampResult255_g11 = clamp( localExponentglow212_g11 , 0.0 , 1.0 );
				float4 appendResult207_g11 = (float4(pow( clampResult255_g11 , _ProceduralShape_OutExp ) , 0.0 , 1.0 , 0.0));
				float4 lerpResult198_g11 = lerp( lerpResult265_g11 , appendResult207_g11 , _Use_ProceduralShape);
				float4 break152_g11 = lerpResult198_g11;
				float4 appendResult123_g11 = (float4(( v.ase_color.g + ( _TimeParameters.x * _Twinkle_Speed * 0.1 ) ) , 0.5 , 0.0 , 0.0));
				float4 tex2DNode125_g11 = tex2Dlod( _Twinkle_Ramp, float4( appendResult123_g11.xy, 0, 0.0) );
				#ifdef _USE_TWINKLE_ON
				float staticSwitch132_g11 = ( ( ( tex2DNode125_g11.r - 0.5 ) * _Twinkle_Strength ) + 1.0 );
				#else
				float staticSwitch132_g11 = 1.0;
				#endif
				
				o.ase_color = v.ase_color;
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( ( normalizeResult29_g11 * max( ( lerpResult72_g11 * 0.1 * _Size_Multiplier * lerpResult268_g11.r * ( break152_g11.z * 2.5 ) ) , ( length( ( ase_worldPos - _WorldSpaceCameraPos ) ) * _Minimum_Screen_Size * 0.002 ) ) ) * staticSwitch132_g11 );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float temp_output_104_0_g11 = ( _Variation_Shift / 100.0 );
				float4 appendResult182_g11 = (float4(( IN.ase_color.g + temp_output_104_0_g11 ) , 0.0 , 0.0 , 0.0));
				float4 tex2DNode175_g11 = tex2D( _ColorTex, appendResult182_g11.xy );
				float3 linearToGamma270_g11 = FastLinearToSRGB( tex2DNode175_g11.rgb );
				float4 lerpResult271_g11 = lerp( tex2DNode175_g11 , float4( linearToGamma270_g11 , 0.0 ) , _LinearWorkflow);
				float3 linearToGamma278_g11 = FastLinearToSRGB( _Color_Tint.rgb );
				float4 lerpResult277_g11 = lerp( _Color_Tint , float4( linearToGamma278_g11 , 0.0 ) , _LinearWorkflow);
				float2 uv_Shape = IN.ase_texcoord3.xy * _Shape_ST.xy + _Shape_ST.zw;
				float4 tex2DNode204_g11 = tex2D( _Shape, uv_Shape );
				float lerpResult201_g11 = lerp( tex2DNode204_g11.a , 0.2 , _Use_ProceduralShape);
				float3 linearToGamma266_g11 = FastLinearToSRGB( tex2DNode204_g11.rgb );
				float4 lerpResult265_g11 = lerp( tex2DNode204_g11 , float4( linearToGamma266_g11 , 0.0 ) , _LinearWorkflow);
				float2 uv0213_g11 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_11 = (0.5).xx;
				float ramp212_g11 = ( 1.0 - ( length( ( uv0213_g11 - temp_cast_11 ) ) * 2.0 ) );
				float size_exp212_g11 = _ProceduralShape_IterExp;
				float mult_exp212_g11 = 1.0;
				int MaxIter212_g11 = (int)round( _ProceduralShape_Iterations );
				float localExponentglow212_g11 = Exponentglow212_g11( ramp212_g11 , size_exp212_g11 , mult_exp212_g11 , MaxIter212_g11 );
				float clampResult255_g11 = clamp( localExponentglow212_g11 , 0.0 , 1.0 );
				float4 appendResult207_g11 = (float4(pow( clampResult255_g11 , _ProceduralShape_OutExp ) , 0.0 , 1.0 , 0.0));
				float4 lerpResult198_g11 = lerp( lerpResult265_g11 , appendResult207_g11 , _Use_ProceduralShape);
				float4 break152_g11 = lerpResult198_g11;
				float4 temp_output_141_0_g11 = ( ( lerpResult271_g11 * lerpResult277_g11 * _Color_Tint.a * ( 5.0 * lerpResult201_g11 ) * _Color_Multiplier ) * ( break152_g11.x + ( break152_g11.y * 10.0 ) ) );
				float2 uv0193_g11 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 tex2DNode190_g11 = tex2D( _LensEffect, uv0193_g11 );
				float3 linearToGamma272_g11 = FastLinearToSRGB( tex2DNode190_g11.rgb );
				float4 lerpResult273_g11 = lerp( tex2DNode190_g11 , float4( linearToGamma272_g11 , 0.0 ) , _LinearWorkflow);
				float4 break274_g11 = lerpResult273_g11;
				float clampResult87_g11 = clamp( _LensEffect_Distance , 0.0 , 100.0 );
				float clampResult83_g11 = clamp( ( clampResult87_g11 - length( ( WorldPosition - _WorldSpaceCameraPos ) ) ) , 0.0 , clampResult87_g11 );
				float clampResult88_g11 = clamp( _LensEffect_DistanceExponent , 0.0 , 100.0 );
				float temp_output_85_0_g11 = pow( clampResult83_g11 , clampResult88_g11 );
				float clampResult143_g11 = clamp( temp_output_85_0_g11 , 0.0 , 1.0 );
				float4 lerpResult139_g11 = lerp( temp_output_141_0_g11 , ( ( lerpResult271_g11 * lerpResult277_g11 * _Color_Tint.a * ( 5.0 * lerpResult201_g11 ) * _Color_Multiplier ) * ( break274_g11.r + ( break274_g11.g * 0.5 ) + ( break274_g11.b * 3.0 ) ) ) , clampResult143_g11);
				float4 lerpResult137_g11 = lerp( temp_output_141_0_g11 , lerpResult139_g11 , _Use_LensEffect);
				float4 appendResult123_g11 = (float4(( IN.ase_color.g + ( _TimeParameters.x * _Twinkle_Speed * 0.1 ) ) , 0.5 , 0.0 , 0.0));
				float4 tex2DNode125_g11 = tex2D( _Twinkle_Ramp, appendResult123_g11.xy );
				#ifdef _USE_TWINKLE_ON
				float staticSwitch132_g11 = ( ( ( tex2DNode125_g11.r - 0.5 ) * _Twinkle_Strength ) + 1.0 );
				#else
				float staticSwitch132_g11 = 1.0;
				#endif
				float4 temp_cast_17 = (0.0).xxxx;
				float4 temp_cast_18 = (100.0).xxxx;
				float4 clampResult109_g11 = clamp( ( lerpResult137_g11 * staticSwitch132_g11 ) , temp_cast_17 , temp_cast_18 );
				float4 break231_g11 = clampResult109_g11;
				float clampResult256_g11 = clamp( break231_g11.r , 0.0 , 1.0 );
				float clampResult257_g11 = clamp( break231_g11.g , 0.0 , 1.0 );
				float clampResult258_g11 = clamp( break231_g11.b , 0.0 , 1.0 );
				float4 appendResult240_g11 = (float4(pow( clampResult256_g11 , ( 1.0 / _Color_Gamma.r ) ) , pow( clampResult257_g11 , ( 1.0 / _Color_Gamma.g ) ) , pow( clampResult258_g11 , ( 1.0 / _Color_Gamma.b ) ) , 0.0));
				float3 hsvTorgb250_g11 = RGBToHSV( appendResult240_g11.xyz );
				float3 hsvTorgb251_g11 = HSVToRGB( float3(hsvTorgb250_g11.x,( hsvTorgb250_g11.y * _Color_Saturation ),hsvTorgb250_g11.z) );
				float3 gammaToLinear283_g11 = FastSRGBToLinear( hsvTorgb251_g11 );
				float3 lerpResult286_g11 = lerp( hsvTorgb251_g11 , ( gammaToLinear283_g11 * 4.0 ) , _LinearWorkflow);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = lerpResult286_g11;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#pragma shader_feature _PERPARTICLEORIENTATION_ON
			#pragma shader_feature _USE_CAMERA_POSITION_ON
			#pragma shader_feature _USE_ATTENUATION_ON
			#pragma shader_feature _USE_TWINKLE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color_Tint;
			float4 _Shape_ST;
			float4 _Color_Gamma;
			float3 _Camera_Position;
			float _Color_Multiplier;
			float _Twinkle_Strength;
			float _Twinkle_Speed;
			float _Minimum_Screen_Size;
			float _Use_ProceduralShape;
			float _ProceduralShape_OutExp;
			float _ProceduralShape_Iterations;
			float _ProceduralShape_IterExp;
			float _Variation_Shift;
			float _Size_Multiplier;
			float _Use_LensEffect;
			float _LensEffect_DistanceExponent;
			float _LensEffect_Distance;
			float _Attenuation_Exponent;
			float _Attenuation_Strength;
			float _LinearWorkflow;
			float _Color_Saturation;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Size;
			sampler2D _Shape;
			sampler2D _Twinkle_Ramp;


			float Exponentglow212_g11( float ramp , float size_exp , float mult_exp , int MaxIter )
			{
				int i = 1;
				float value = 0;
				float v01 = 0;
				float v02 = 0;
				float v03 = 0;
				while (i < MaxIter)
				{
				    v01 = MaxIter/(i+1);
				    v02 = pow(ramp, pow(size_exp, v01));
				    v03 = v02/pow((i+1), mult_exp);
				    value += v03;
				    i+=1;
				}
				return value;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv062_g11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break63_g11 = ( uv062_g11 - float2( 0.5,0.5 ) );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 normalizeResult12_g11 = normalize( ( ase_worldPos - _Camera_Position ) );
				#ifdef _USE_CAMERA_POSITION_ON
				float3 staticSwitch14_g11 = normalizeResult12_g11;
				#else
				float3 staticSwitch14_g11 = ase_worldViewDir;
				#endif
				float3 temp_output_17_0_g11 = mul( float4( staticSwitch14_g11 , 0.0 ), GetObjectToWorldMatrix() ).xyz;
				float3 temp_output_21_0_g11 = cross( float3(0,1,0) , temp_output_17_0_g11 );
				float3 normalizeResult23_g11 = normalize( cross( temp_output_21_0_g11 , temp_output_17_0_g11 ) );
				float2 uv05_g11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break18_g11 = ( uv05_g11 - float2( 0.5,0.5 ) );
				float3 normalizeResult24_g11 = normalize( temp_output_21_0_g11 );
				#ifdef _PERPARTICLEORIENTATION_ON
				float3 staticSwitch38_g11 = ( ( normalizeResult23_g11 * break18_g11.y ) + ( normalizeResult24_g11 * break18_g11.x * -1.0 ) );
				#else
				float3 staticSwitch38_g11 = ( ( mul( float4( float3(1,0,0) , 0.0 ), mul(GetWorldToHClipMatrix(),GetObjectToWorldMatrix()) ).xyz * break63_g11.x ) + ( mul( float4( float3(0,1,0) , 0.0 ), mul(GetWorldToHClipMatrix(),GetObjectToWorldMatrix()) ).xyz * break63_g11.y ) );
				#endif
				float3 normalizeResult29_g11 = normalize( staticSwitch38_g11 );
				float clampResult89_g11 = clamp( _Attenuation_Strength , 0.0 , 100.0 );
				float clampResult254_g11 = clamp( ( length( ( ase_worldPos - _WorldSpaceCameraPos ) ) * 50.0 * clampResult89_g11 ) , 0.0 , 1.0 );
				float clampResult90_g11 = clamp( _Attenuation_Exponent , 0.0 , 100.0 );
				#ifdef _USE_ATTENUATION_ON
				float staticSwitch43_g11 = ( 1.0 * ( 1.0 / pow( clampResult254_g11 , clampResult90_g11 ) ) );
				#else
				float staticSwitch43_g11 = 1.0;
				#endif
				float clampResult87_g11 = clamp( _LensEffect_Distance , 0.0 , 100.0 );
				float clampResult83_g11 = clamp( ( clampResult87_g11 - length( ( ase_worldPos - _WorldSpaceCameraPos ) ) ) , 0.0 , clampResult87_g11 );
				float clampResult88_g11 = clamp( _LensEffect_DistanceExponent , 0.0 , 100.0 );
				float temp_output_85_0_g11 = pow( clampResult83_g11 , clampResult88_g11 );
				float lerpResult72_g11 = lerp( staticSwitch43_g11 , ( staticSwitch43_g11 * ( temp_output_85_0_g11 + 1.0 ) ) , _Use_LensEffect);
				float temp_output_104_0_g11 = ( _Variation_Shift / 100.0 );
				float4 appendResult98_g11 = (float4(( v.ase_color.g + temp_output_104_0_g11 ) , 0.0 , 0.0 , 0.0));
				float4 tex2DNode97_g11 = tex2Dlod( _Size, float4( appendResult98_g11.xy, 0, 0.0) );
				float3 linearToGamma267_g11 = FastLinearToSRGB( tex2DNode97_g11.rgb );
				float4 lerpResult268_g11 = lerp( tex2DNode97_g11 , float4( linearToGamma267_g11 , 0.0 ) , _LinearWorkflow);
				float2 uv_Shape = v.ase_texcoord.xy * _Shape_ST.xy + _Shape_ST.zw;
				float4 tex2DNode204_g11 = tex2Dlod( _Shape, float4( uv_Shape, 0, 0.0) );
				float3 linearToGamma266_g11 = FastLinearToSRGB( tex2DNode204_g11.rgb );
				float4 lerpResult265_g11 = lerp( tex2DNode204_g11 , float4( linearToGamma266_g11 , 0.0 ) , _LinearWorkflow);
				float2 uv0213_g11 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_16 = (0.5).xx;
				float ramp212_g11 = ( 1.0 - ( length( ( uv0213_g11 - temp_cast_16 ) ) * 2.0 ) );
				float size_exp212_g11 = _ProceduralShape_IterExp;
				float mult_exp212_g11 = 1.0;
				int MaxIter212_g11 = (int)round( _ProceduralShape_Iterations );
				float localExponentglow212_g11 = Exponentglow212_g11( ramp212_g11 , size_exp212_g11 , mult_exp212_g11 , MaxIter212_g11 );
				float clampResult255_g11 = clamp( localExponentglow212_g11 , 0.0 , 1.0 );
				float4 appendResult207_g11 = (float4(pow( clampResult255_g11 , _ProceduralShape_OutExp ) , 0.0 , 1.0 , 0.0));
				float4 lerpResult198_g11 = lerp( lerpResult265_g11 , appendResult207_g11 , _Use_ProceduralShape);
				float4 break152_g11 = lerpResult198_g11;
				float4 appendResult123_g11 = (float4(( v.ase_color.g + ( _TimeParameters.x * _Twinkle_Speed * 0.1 ) ) , 0.5 , 0.0 , 0.0));
				float4 tex2DNode125_g11 = tex2Dlod( _Twinkle_Ramp, float4( appendResult123_g11.xy, 0, 0.0) );
				#ifdef _USE_TWINKLE_ON
				float staticSwitch132_g11 = ( ( ( tex2DNode125_g11.r - 0.5 ) * _Twinkle_Strength ) + 1.0 );
				#else
				float staticSwitch132_g11 = 1.0;
				#endif
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( ( normalizeResult29_g11 * max( ( lerpResult72_g11 * 0.1 * _Size_Multiplier * lerpResult268_g11.r * ( break152_g11.z * 2.5 ) ) , ( length( ( ase_worldPos - _WorldSpaceCameraPos ) ) * _Minimum_Screen_Size * 0.002 ) ) ) * staticSwitch132_g11 );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18100
2560;245;1919;1005;1195.5;501;1;True;True
Node;AmplifyShaderEditor.FunctionNode;27;-389,-15.5;Inherit;False;StarCluster_AmplifyShaderFunction;0;;11;7ac8d45e9b71a274ebee5f7461282171;0;0;2;FLOAT3;0;FLOAT3;1
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;20;25,-10;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;True;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;21;25,-10;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;StarClusterPack/StarCluster_URP;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;True;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;True;1;1;False;-1;1;False;-1;1;1;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;1;  Blend;2;Two Sided;1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;DOTS Instancing;0;Meta Pass;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;22;25,-10;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;True;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;True;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;23;25,-10;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;True;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;True;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;24;25,-10;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;True;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;21;2;27;0
WireConnection;21;5;27;1
ASEEND*/
//CHKSM=C77ACC7416C242A01DFC7AEA9A80E0CC0C3E8A0C