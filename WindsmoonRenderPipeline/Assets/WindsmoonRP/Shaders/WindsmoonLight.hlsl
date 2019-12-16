#ifndef WINDSMOON_LIGHT_INCLUDED
#define WINDSMOON_LIGHT_INCLUDED

struct Light
{
    float3 direction;
    float3 color;
};

Light GetDirectionalLight()
{
    Light light;
    light.direction = float3(0, 1, 0);
    light.color = 1;
    return light;
}

#endif