Shader "windsmoon Tools/VisualNormal"
{
	Properties
	{
		_Length("Length", Float) = 1
		_Color("Color", Color) = (1, 0, 0 ,0)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2g
			{
				float4 pos : POSITION;
				float4 worldPos : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
			};

			float _Length;
			float4 _Color;

			v2g vert (appdata v)
			{
				v2g o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				return o;
			}
			
			[maxvertexcount(6)]
			void geom(triangle v2g p[3], inout LineStream<g2f> stream)
			{
				for (int i = 0; i < 3; ++i)
      			{
          			g2f o;
          			o.pos = p[i].pos;
					stream.Append(o);
					o.pos.w = 1;
          			o.pos.xyz = p[i].worldPos.xyz + p[i].worldNormal * _Length;
          			o.pos = mul(UNITY_MATRIX_VP, o.pos);
      				stream.Append(o);
      				stream.RestartStrip();
      			}
			}

			fixed4 frag (g2f i) : SV_Target
			{
				return _Color;
			}

			ENDCG
		}
	}
}
