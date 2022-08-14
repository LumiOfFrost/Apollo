#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

Texture2D PaletteTexture;

int PaletteId;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 baseColor = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;

	float4 color = baseColor;
	color.rgb = max(max(color.r, color.g), color.b);

	if (color.r == 0) {

		return tex2D(paletteTexture, float2(0.5f, 0.5f + paletteId))

	}
	else {
		return color;
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