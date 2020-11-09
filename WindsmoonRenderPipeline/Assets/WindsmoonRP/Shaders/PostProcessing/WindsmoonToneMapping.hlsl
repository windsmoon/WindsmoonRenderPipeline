#ifndef WINDSMOON_TONE_MAPPING_INCLUDED
#define WINDSMOON_TONE_MAPPING_INCLUDED

// float4 ToneMappingNonePassFragment (Varyings input) : SV_TARGET {
//     float4 color = GetSource(input.fxUV);
//     color.rgb = ColorGrade(color.rgb);
//     return color;
// }

// ACES adds a hue shift to very bright colors, pushing them toward white.
// This also happens when cameras or eyes get overwhelmed by too much light. (from catlike)
float4 ToneMappingACESFragment(Varyings input) : SV_Target
{
    float4 color = GetSource(input.uv);
    // because the precision limition, the very large values end up at 1 much earlier than infinity
    // It can become a problem for some functions when half values are used.
    // Due to a bug in the shader compiler this happens in some cases with the Metal API, even when float is used explicitly.
    // This also affects some MacBooks, not only mobiles. (from catlike)
    // 60 is a good limition
    color.rgb = min(color.rgb, 60.0); 
    color.rgb = AcesTonemap(unity_to_ACES(color.rgb)); // todo : the function impl
    return color;
}

float4 ToneMappingNeutralFragment(Varyings input) : SV_Target
{
    float4 color = GetSource(input.uv);
    // because the precision limition, the very large values end up at 1 much earlier than infinity
    // It can become a problem for some functions when half values are used.
    // Due to a bug in the shader compiler this happens in some cases with the Metal API, even when float is used explicitly.
    // This also affects some MacBooks, not only mobiles. (from catlike)
    // 60 is a good limition
    color.rgb = min(color.rgb, 60.0); 
    color.rgb = NeutralTonemap(color.rgb); // todo : the function impl
    return color;
}

float4 ToneMappingReinhardFragment(Varyings input) : SV_Target
{
    float4 color = GetSource(input.uv);
    // because the precision limition, the very large values end up at 1 much earlier than infinity
    // It can become a problem for some functions when half values are used.
    // Due to a bug in the shader compiler this happens in some cases with the Metal API, even when float is used explicitly.
    // This also affects some MacBooks, not only mobiles. (from catlike)
    // 60 is a good limition
    color.rgb = min(color.rgb, 60.0); 
    color.rgb /= (1 + color.rgb);
    return color;
}

#endif