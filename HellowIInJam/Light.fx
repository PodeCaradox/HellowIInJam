

	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0




sampler TextureSampler : register(s0);

Texture2D LightMask;
sampler LightMaskSampler = sampler_state { Texture = <LightMask>; };

struct VertexShaderInput
{
	float4 position	: POSITION0;
	float4 color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput SpriteVertexShader(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.position = input.position;
	output.color = input.color;
	output.texCoord = input.texCoord;
	return output;
}

float4 SpritePixelShader(VertexShaderOutput input) : COLOR0
{
float4 lightColour = tex2D(LightMaskSampler, input.texCoord) ;
  float4 tex = tex2D(TextureSampler, input.texCoord)* input.color;

  return lightColour * tex;
}


technique BasicColorDrawing
{
    pass Pass0
    {
		VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}