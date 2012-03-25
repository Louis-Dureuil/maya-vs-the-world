sampler s;
sampler s2;

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
 

float4 PixelShaderFunction(float2 uv:TEXCOORD0, float4 color:COLOR0) : COLOR0
{
	float4 color2 = 0;
	color2 = tex2D(s, uv+(16,16))*color;
    
	return color2;
}

float4 BlurPS(float2 texCoord : TEXCOORD0) : COLOR 
{
	
    float4 color = 0; 
    //combine the image filter 
    for( int i = 0; i < 20; i++ ) 
    { 
        color += tex2D(s, texCoord + (SampleOffsets[i]) * 0.1) * .05; 
    } 
    return color; 
} 

float4 fog(float2 uv:TEXCOORD0, float4 color:COLOR0) : COLOR
{
	float4 c = tex2D(s2, uv);
	float4 color2 = 0;
	color2 += tex2D(s, uv ) * color;
	
	color2.x -= 0.4;
	color2.y -= 0.4;
	color2.z -= 0.4;

	color2.rgb -= c.rgb / 4;

    return color2;
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

technique Blur
{
pass Pass1
{
PixelShader = compile ps_2_0 BlurPS();
}
}