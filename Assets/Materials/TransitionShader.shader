Shader "Custom/TransitionShader" {
		Properties{
		_MainTex("Base", 2D) = "white" {}
		_Guide("Transition GrayscaleTexture", 2D) = "white" {}
		_Threshold("Threshold",Range(0,1)) = 0
		}
		SubShader{
			Cull Off ZWrite Off ZTest Always
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				float4 _MainTex_TexelSize;

				v2f simplevert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.uv1 = v.uv;

					#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv1.y = 1 - o.uv1.y;
					#endif

					return o;
				}

				sampler2D _Guide;
				sampler2D _MainTex;
				float _Threshold;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 transition_current_pixelColour = tex2D(_Guide, i.uv1);

					if (transition_current_pixelColour.b < _Threshold)
						return float4(0,0,0,1);

					return tex2D(_MainTex, i.uv1);
				}
				ENDCG
			}
		}
}
