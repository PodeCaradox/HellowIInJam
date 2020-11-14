#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

Texture2D LightMask;
sampler LightMaskSampler = sampler_state { Texture = <LightMask>; };

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
float4 lightColour = tex2D(LightMaskSampler, input.TextureCoordinates) ;
  float4 tex = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
  float4 color;
	if(lightColour.a >= 0.4 && lightColour.a <= 0.6){
	color.r = 0.156862745;
	color.g = 0.129411764;
	color.b = 0.086274509;
	color.a = 1;
	}else{
	color = lightColour * tex;
	}
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};