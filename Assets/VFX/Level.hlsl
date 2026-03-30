#ifndef MY_FUNCTIONS_INCLUDED
#define MY_FUNCTIONS_INCLUDED

bool PointInTriangle(float2 pnt, float2 a, float2 b, float2 c)
{
    float2 v0 = c - a;
    float2 v1 = b - a;
    float2 v2 = pnt - a;

    float dot00 = dot(v0, v0);
    float dot01 = dot(v0, v1);
    float dot02 = dot(v0, v2);
    float dot11 = dot(v1, v1);
    float dot12 = dot(v1, v2);

    float invDenom = 1.0 / (dot00 * dot11 - dot01 * dot01);

    float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
    float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

    return (u >= 0.0) && (v >= 0.0) && (u + v <= 1.0);
}

float rand(float2 co)
{
    return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
}

float2 rand2(float2 co)
{
    return float2(rand(co), rand(co + 1.0));
}

void Level_float(
    float _TriNum,
    float _SEED, 
    float _FadeSpeed,
    float _Size,
    float _TriangleSize,
    float2 _Offset,
    float _Time,
    float4 _BaseColor,
    float4 _BackgroundColor,
    float2 p,
    out float4 color)
{
    float2 uv = p / _Size + _Offset;

    float2 gridUV = uv;
    float2 tileID = floor(gridUV);

    float4 finalColor = _BackgroundColor;

    int triCount = (int)_TriNum;

    // Neighbor cells
    for(int tx = -1; tx <= 1; tx++)
    for(int ty = -1; ty <= 1; ty++)
    {
        float2 neighborID = tileID + float2(tx, ty);
        float2 baseSeed = neighborID + _SEED;

        for(int i = 0; i < triCount; i++)
        {
            float fi = (float)i;

            float2 center = neighborID + rand2(baseSeed + fi);

            float2 A = center + (rand2(baseSeed + fi * 3.0 + 0.0) - 0.5) * _TriangleSize;
            float2 B = center + (rand2(baseSeed + fi * 3.0 + 1.0) - 0.5) * _TriangleSize;
            float2 C = center + (rand2(baseSeed + fi * 3.0 + 2.0) - 0.5) * _TriangleSize;

            float fadeSpeed = 1.0 + rand(baseSeed + fi) * 2.0 * _FadeSpeed;
            float phase = rand(baseSeed + fi + 1.0) * 6.2831853;

            float alpha = (0.5 + 0.5 * sin(_Time * fadeSpeed + phase)) * _BaseColor.a;

            if(PointInTriangle(uv, A, B, C))
            {
                finalColor.rgb = lerp(finalColor.rgb, _BaseColor.rgb, alpha);
            }
        }
    }

    finalColor.a = 1.0;
    color = finalColor;
}

#endif