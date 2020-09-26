#ifndef WINDSMOON_POST_PROCESSING_INCLUDED
#define WINDSMOON_POST_PROCESSING_INCLUDED

TEXTURE2D(_PostProcessingSource);
TEXTURE2D(_PostProcessingSource2);
SAMPLER(sampler_linear_clamp);
float4 _PostProcessingSource_TexelSize;

float4 _ProjectionParams; // x : if x is less than 0, the v is from top to bottom

struct Varyings
{
    float4 positionCS: SV_POSITION;
    float2 uv : VAR_UV;
};

Varyings PostProcessingVertex(uint vertexID : SV_VertexID) // the order is (-1, -1) (-1. 3) (3, -1)
{
    Varyings output;
    output.positionCS = float4(vertexID <= 1 ? -1.0 : 3.0, vertexID == 1 ? 3.0: -1.0, 0.0, 1.0);
    output.uv = float2(vertexID <= 1 ? 0.0 : 2.0, vertexID == 1 ? 2.0 : 0.0);

    if (_ProjectionParams.x < 0.0)
    {
        output.uv.y = 1.0 - output.uv.y;
    }
    
    return output;
}

float4 GetSourceTexelSize()
{
    return _PostProcessingSource_TexelSize;
}

float4 GetSource(float2 uv)
{
    return SAMPLE_TEXTURE2D(_PostProcessingSource, sampler_linear_clamp, uv);
}

float4 GetSource2(float2 uv)
{
    return SAMPLE_TEXTURE2D(_PostProcessingSource2, sampler_linear_clamp, uv);
}

float4 BloomHorizontalBlurFragment(Varyings input) : SV_TARGET
{
    float3 color = 0.0;
    float offsets[] = {-4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0};
    
    float weights[] = // the weights come form the Pascal's triangle, row 13 (remove the edge 2 columns) 
    {
        0.01621622, 0.05405405, 0.12162162, 0.19459459, 0.22702703,
        0.19459459, 0.12162162, 0.05405405, 0.01621622
    };

    for (int i = 0; i < 9; ++i)
    {
        float offset = offsets[i] * 2.0 * GetSourceTexelSize().x;
        color += GetSource(input.uv + float2(offset, 0.0)).rgb * weights[i];
    }

    return float4(color, 1.0);
}

float4 BloomVerticalBlurFragment(Varyings input) : SV_TARGET
{
    float3 color = 0.0;
    // float offsets[] = {-4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0};
    //
    // float weights[] = // the weights come form the Pascal's triangle, row 13 (remove the edge 2 columns) 
    // {
    //     0.01621622, 0.05405405, 0.12162162, 0.19459459, 0.22702703,
    //     0.19459459, 0.12162162, 0.05405405, 0.01621622
    // };

    // vertical can only sample 5 point use bilinear, but horizontal pass can not, because it has been used for  ??
    float offsets[] = {-3.23076923, -1.38461538, 0.0, 1.38461538, 3.23076923};
    float weights[] = {0.07027027, 0.31621622, 0.22702703, 0.31621622, 0.07027027};

    for (int i = 0; i < 5; ++i)
    {
        float offset = offsets[i] * GetSourceTexelSize().y; // the downsample has been did in the horizontal blur, so this time do not double the texel size
        color += GetSource(input.uv + float2(0.0, offset)).rgb * weights[i];
    }

    return float4(color, 1.0);
}

float4 BloomCombineFragment(Varyings input) : SV_TARGET
{
    float3 lowRes = GetSource(input.uv).rgb;
    float3 hightRes = GetSource2(input.uv).rgb;
    return float4(lowRes + hightRes, 1.0);
}

float4 CopyFragment(Varyings input) : SV_TARGET
{
    return GetSource(input.uv);
}
#endif