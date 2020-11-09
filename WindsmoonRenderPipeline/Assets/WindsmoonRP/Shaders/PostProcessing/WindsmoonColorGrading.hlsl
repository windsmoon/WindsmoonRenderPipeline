#ifndef WINDSMOON_COLOR_GRADING_INCLUDED
#define WINDSMOON_COLOR_GRADING_INCLUDED

// x : Pow(2f, PostExposure)
// y : Contrast * 0.01f + 1f, contrast si -100 ~ 100, so the y is 0 ~ 2 
float4 _ColorGradingData;
float4 _ColorFilter;

float3 ColorGradingPostExposure(float3 color)
{
    return color * _ColorGradingData.x;
}

float3 ColorGradingContrast(float3 color)
{
    // convert to logc space can get better result
    color = LinearToLogC(color);
    color = (color - ACEScc_MIDGRAY) * _ColorGradingData.y + ACEScc_MIDGRAY;
    return LogCToLinear(color);
}

float3 ColorGrading (float3 color)
{
    color = min(color, 60.0);
    color = ColorGradingPostExposure(color);
    color = ColorGradingContrast(color);
    // contrast can make negative color so we should eliminate the negative value
    color = max(color, 0.0);
    return color;
}

#endif