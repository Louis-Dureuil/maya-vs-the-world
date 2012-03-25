// Global variables
// This will use the texture bound to the object( like from the sprite batch ).
sampler ColorMapSampler : register(s0);
float alpha;

float2 sides[4] = 
{
float2(-0.005, 0),
float2(0.005, 0),
float2(0, -0.005),
float2(0, 0.005)
};

float4 OutlineFont(float2 uv:TEXCOORD0, float4 color:COLOR0) : COLOR0
{
	float4 color2 = tex2D(ColorMapSampler, uv);
	bool jouxted = false;
	float4 color3;

	// Get Left color info
	color3 = tex2D(ColorMapSampler, sides[0] + uv);
	jouxted = jouxted || color3.a != 0;

	// Get Right color info
	color3 = tex2D(ColorMapSampler, sides[1] + uv);
	jouxted = jouxted || color3.a != 0;


	// Get Up color info
	color3 = tex2D(ColorMapSampler, sides[2] + uv);
	jouxted = jouxted || color3.a != 0;


	// Get Down color info
	color3 = tex2D(ColorMapSampler, sides[3] + uv);
	jouxted = jouxted || color3.a != 0;


	if (color2.a == 0 && jouxted) {
	color2.r = 0;
	color2.b = 0;
	color2.g = 0;
	color2.a = 255;
	}
    return color2;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        PixelShader = compile ps_2_0 OutlineFont();
    }
}
