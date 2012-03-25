sampler s;

float timer;
float state;

float4 PixelShaderFunction(float2 texcoord: TEXCOORD0, float4 color: COLOR0) : COLOR0
{
    float4 c = tex2D(s, texcoord);
	c *= color;
	if (texcoord.x >= timer - 0.05 && texcoord.x <= timer + 0.05)
	{
		if (state > 0.80)
		 { c.r = 0;
		  c.g = 1;
		  c.b = 0;
		  }else if (state > 0.60)
		  {c.r = 0;
		  c.g = 0;
		  c.b = 1;
		 }else if (state > 0.40)
		  {c.r = 1;
		  c.g = 1;
		  c.b = 0;
		  }else
		  {c.r = 1;
		  c.g = 0;
		  c.b = 0;
		  }
	} 
    return c;
}

technique Technique1
{
    pass Pass1
    {
        
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
