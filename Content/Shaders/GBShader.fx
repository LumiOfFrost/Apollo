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

int paletteHeight = 5;

int PaletteId = 0;

float brightness = 0;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;

};

sampler2D PaletteTextureSampler = sampler_state
{
	Texture = <PaletteTexture>;

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

	int palId = PaletteId;

	Texture2D palTex = PaletteTexture;
	

	if (color.r == 0) {


		if (0.1f + brightness >= 0.1f && 0.1f + brightness <= 0.9f) {

			return tex2D(PaletteTextureSampler, float2((0.1f + brightness), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}
		else {

			return tex2D(PaletteTextureSampler, float2((0.1f + brightness > 0.9f ? 0.9f : 0.1f), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}

	}
	else if (color.r <= 0.25f) {

		if (0.3f + brightness >= 0.1f && 0.3f + brightness <= 0.9f) {

			return tex2D(PaletteTextureSampler, float2((0.3f + brightness), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}
		else {

			return tex2D(PaletteTextureSampler, float2((0.3f + brightness > 0.9f ? 0.9f : 0.1f), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}

	}
	else if (color.r <= 0.5f) {

		if (0.5f + brightness >= 0.1f && 0.5f + brightness <= 0.9f) {

			return tex2D(PaletteTextureSampler, float2((0.5f + brightness), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}
		else {

			return tex2D(PaletteTextureSampler, float2((0.5f + brightness > 0.9f ? 0.9f : 0.1f), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}

	}
	else if (color.r <= 0.75f) {

		if (0.7f + brightness >= 0.1f && 0.7f + brightness <= 0.9f) {

			return tex2D(PaletteTextureSampler, float2((0.7f + brightness), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}
		else {

			return tex2D(PaletteTextureSampler, float2((0.7f + brightness > 0.9f ? 0.9f : 0.1f), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}

	}
	else {

		if (0.9f + brightness >= 0.1f && 0.9f + brightness <= 0.9f) {

			return tex2D(PaletteTextureSampler, float2((0.9f + brightness), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}
		else {

			return tex2D(PaletteTextureSampler, float2((0.9f + brightness > 0.9f ? 0.9f : 0.1f), 0.5f / paletteHeight + (1.0f / paletteHeight) * palId));

		}

	}

}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};