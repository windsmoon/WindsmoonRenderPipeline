#ifndef WINDSMOON_SURFACE_INCLUDED
#define WINDSMOON_SURFACE_INCLUDED

struct Surface
{
    float3 normal;
    float3 color;
    float alpha;
    float metallic;
	float smoothness;
};

#endif