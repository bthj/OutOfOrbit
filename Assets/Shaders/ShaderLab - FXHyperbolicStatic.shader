// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'

//
// Shader: "FX/Hyperbolic Static"
// Version: v1.0
// Written by: Thomas Phillips
//
// Anyone is free to use this shader for non-commercial or commercial projects.
//
// Description:
// Generic force field effect.
// Play with color, opacity, and rate for different effects.
//

Shader "FX/Hyperbolic Static" {
	
Properties {
	_Rate ("Oscillation Rate", Range (1, 300)) = 300
	_MainTex ("Texture", 2D) = "white" { }
}

SubShader {
	
	ZWrite Off
	Tags { "Queue" = "Transparent" }
	Blend One One

	Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_fog_exp2
#include "UnityCG.cginc"

float _Rate;
sampler2D _MainTex;

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_base v)
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	return o;
}

half4 frag (v2f i) : COLOR
{
	half4 texcol = tex2D (_MainTex, i.uv);
	float3 color;
	float m;
	float t = frac(_Time[0]*_Time[0]*43758.5453);
	if(t>0.9)
	{
		return half4(0,0,0,0);
	}
	else
	{
		m = _Time[0]*_Rate + ((i.uv[0]+i.uv[1])*5000000*texcol.a);
		m = 0.1 + 0.9 * abs(sin(m));
		color = float3(m*texcol.r, m*texcol.g, m*texcol.b);
		return half4( color, 1 );
	}
}
ENDCG

    }
}
Fallback "Transparent/Diffuse"
}