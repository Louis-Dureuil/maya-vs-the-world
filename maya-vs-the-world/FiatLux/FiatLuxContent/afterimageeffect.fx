// Global variables
// This will use the texture bound to the object( like from the sprite batch ).
sampler ColorMapSampler : register(s0);
float alpha;


float4 afterImage(float2 uv:TEXCOORD0, float4 color:COLOR0) : COLOR0
{
	float4 color2 = tex2D(ColorMapSampler, uv);

	if (color2.a != 0) {
	color2.r = color.r * (alpha * 10);
	color2.b = color.b * (alpha * 10);
	color2.g = color.g * (alpha * 10);
	color2.a *= color.a * alpha;
	}
    return color2;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        PixelShader = compile ps_2_0 afterImage();
    }
}
