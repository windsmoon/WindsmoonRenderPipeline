#ifndef WINDSMOON_COLOR_GRADING_INCLUDED
#define WINDSMOON_COLOR_GRADING_INCLUDED

float3 ColorGrading (float3 color)
{
    color = min(color, 60.0);
    return color;
}

#endif