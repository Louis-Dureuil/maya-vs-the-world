// Global variables
// This will use the texture bound to the object( like from the sprite batch ).
sampler ColorMapSampler : register(s0);

float2 SampleOffsets[21] =
{
    float2(-.001,0), 
    float2(-.009,0), 
    float2(-.008,0), 
    float2(-.007,0), 
    float2(-.006,0), 
    float2(-.005,0), 
    float2(-.004,0), 
    float2(-.003,0), 
    float2(-.002,0), 
    float2(-.001,0), 
    float2(0,0), 
    float2(.001,0), 
    float2(.002,0), 
    float2(.003,0), 
    float2(.004,0), 
    float2(.005,0), 
    float2(.006,0), 
    float2(.007,0), 
    float2(.008,0), 
    float2(.009,0), 
    float2(.01,0)
};
 
 float4 BlurPS(float2 texCoord : TEXCOORD0) : COLOR 
{
	
    float4 color = 0; 
    //combine the image filter 
    for( int i = 0; i < 20; i++ ) 
    { 
        color += tex2D(ColorMapSampler, texCoord + (SampleOffsets[i]) * 100) * .05; 
    } 
    return color; 
} 

// A timer to animate our desaturation.
float dTimer = 1.0F;

// A timer to animate our distorsion.
float fTimer;

// the amount of distortion
float fNoiseAmount;

// just a random starting number
int iSeed;


float4 PixelShaderFunction(float2 uv:TEXCOORD0, float4 color:COLOR0) : COLOR0
{
	float4 color2 = tex2D(ColorMapSampler, uv)*color;
    return color2;
}

float4 desature(float2 Tex:TEXCOORD0, float4 color2 : COLOR0) : COLOR
{
	float4 color = tex2D(ColorMapSampler, Tex)*color2;
	float4 moyenne = dot(color.rgb, float3(0.3, 0.59, 0.11));
	color = dot(color.rgb * dTimer, moyenne.rgb * (1 - dTimer)); 

	return color;
}

// Noise
float4 fog(float2 Tex: TEXCOORD0) : COLOR
{
 // Distortion factor
 float NoiseX = iSeed * fTimer * sin(Tex.x * Tex.y+fTimer);
 NoiseX=fmod(NoiseX,8) * fmod(NoiseX,4); 

 // Use our distortion factor to compute how much it will affect each
 // texture coordinate
 float DistortX = fmod(NoiseX,fNoiseAmount);
 float DistortY = fmod(NoiseX,fNoiseAmount+0.002);
 
 // Create our new texture coordinate based on our distortion factor
 float2 DistortTex = float2(DistortX,DistortY);
 
 // Use our new texture coordinate to look-up a pixel in ColorMapSampler.
 float4 Color=tex2D(ColorMapSampler, Tex+DistortTex);

Color.rgb = dot(Color.rgb, float3(0.3, 0.59, 0.11));  

    return Color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique Fog
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 fog();
	}
}

technique Desature
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 desature();
	}
}

technique Blur
{
pass Pass1
{
PixelShader = compile ps_2_0 BlurPS();
}
}