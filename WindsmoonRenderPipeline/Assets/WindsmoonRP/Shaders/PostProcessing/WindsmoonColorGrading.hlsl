#ifndef WINDSMOON_COLOR_GRADING_INCLUDED
#define WINDSMOON_COLOR_GRADING_INCLUDED

// x : Pow(2f, PostExposure)
// y : Contrast * 0.01f + 1f, contrast si -100 ~ 100, so the y is 0 ~ 2
// z : HueShift * (1f / 360f), HueShift is -180 ~ 180
// w : saturation is the same as contrast
float4 _ColorAdjustmentData;
float4 _ColorFilter;
float4 _WhiteBalanceData;

float3 ColorGradingPostExposure(float3 color)
{
    return color * _ColorAdjustmentData.x;
}

float3 ColorGradingContrast(float3 color)
{
    // convert to logc space can get better result
    color = LinearToLogC(color);
    color = (color - ACEScc_MIDGRAY) * _ColorAdjustmentData.y + ACEScc_MIDGRAY;
    return LogCToLinear(color);
}

float3 ColorGradingColorFilter(float3 color)
{
    return color * _ColorFilter.rgb;
}

float3 ColorGradingHueShift(float3 color)
{
    color = RgbToHsv(color);
    float hue = color.x + _ColorAdjustmentData.z;
    color.x = RotateHue(hue, 0.0, 1.0);
    return HsvToRgb(color);
}

float3 ColorGradingSaturation (float3 color)
{
    float luminance = Luminance(color);
    return (color - luminance) * _ColorAdjustmentData.w + luminance;
}

float3 ColorGrading (float3 color)
{
    color = min(color, 60.0);
    color = ColorGradingPostExposure(color);
    color = ColorGradingContrast(color);     // contrast can make negative color
    color = ColorGradingColorFilter(color); // filter can work with negative color so we can apply it before eliminating negative valuses
    color = max(color, 0.0);
    color = ColorGradingHueShift(color); // hue shift muse wokr with positive values
    color = ColorGradingSaturation(color); // saturation is the last work, and it can make negative color
    color = max(color, 0.0);
    return color;
}

#endif