float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
texture Atlas;
sampler2D textureSampler = sampler_state {
	Texture = (Atlas);
	MagFilter = None;
	MinFilter = None;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 AmbientColor = float4(0.5, 0.5, 0.5, 1);
float AmbientIntensity = 0.6;
float3 LightDirection = float3(1, -1, 0.5);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	//float4 pos : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);
	//output.pos = worldPos;

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float intensity = -dot(normal, LightDirection);
	output.Color = saturate(AmbientColor * (AmbientIntensity + intensity / 4));
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//float depth = sqrt(input.pos.x * input.pos.x + input.pos.y * input.pos.y + input.pos.z * input.pos.z);
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	//textureColor.a = textureColor.r;
	if (textureColor.a > 0.1f) 
	{
		AmbientIntensity = 0;
	}
	clip(textureColor.a < 0.1f ? -1 : 1);

	return input.Color * textureColor;
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
	}
}