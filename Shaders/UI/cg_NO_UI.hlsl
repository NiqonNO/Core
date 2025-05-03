#ifndef NIQONNO_SDF_HLSL_INCLUDED
#define NIQONNO_SDF_HLSL_INCLUDED

float dot2( float2 v ) { return dot(v,v); }

float sdCircle( float2 p, float2 a)
{
    return length(p-a);
}
float sdSegment( float2 p, float2 a, float2 b )
{
    float2 pa = p-a, ba = b-a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h );
}
float sdBezier( float2 pos, float2 A, float2 B, float2 C )
{    
    float2 b = A - 2.0*B + C;
    if(length(b) < 0.1)
    {
        return sdSegment(pos, A, C);
    }
    float2 a = B - A;
    float2 c = a * 2.0;
    float2 d = A - pos;
    float kk = 1.0/dot(b,b);
    float kx = kk * dot(a,b);
    float ky = kk * (2.0*dot(a,a)+dot(d,b)) / 3.0;
    float kz = kk * dot(d,a);      
    float res = 0.0;
    float p = ky - kx*kx;
    float p3 = p*p*p;
    float q = kx*(2.0*kx*kx-3.0*ky) + kz;
    float h = q*q + 4.0*p3;
    if( h >= 0.0) 
    { 
        h = sqrt(h);
        float2 x = (float2(h,-h)-q)/2.0;
        float2 uv = sign(x)*pow(abs(x), (float2(1.0/3.0,1.0/3.0)));
        float t = clamp( uv.x+uv.y-kx, 0.0, 1.0 );
        res = dot2(d + (c + b*t)*t);
    }
    else
    {
        float z = sqrt(-p);
        float v = acos( q/(p*z*2.0) ) / 3.0;
        float m = cos(v);
        float n = sin(v)*1.732050808;
        float2  t = clamp(float3(m+m,-n-m,n-m)*z-kx,0.0,1.0);
        res = min( dot2(d+(c+b*t.x)*t.x),
                   dot2(d+(c+b*t.y)*t.y) );
        // the third root cannot be the closest
        // res = min(res,dot2(d+(c+b*t.z)*t.z));
    }
    return sqrt( res );
}

float opSmoothUnion( float d1, float d2, float k )
{
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) - k*h*(1.0-h);
}

float opSmoothSubtraction( float d1, float d2, float k )
{
    float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
    return lerp( d2, -d1, h ) + k*h*(1.0-h);
}

void SliderSlit_float(float2 position, float4 sliderCorners, float2 handlePositions, out float alpha)
{
    alpha = opSmoothUnion(sdBezier(position, sliderCorners.xy, handlePositions, sliderCorners.zw) - 3, sdCircle(position, handlePositions) - 13, 30);
    alpha = saturate(alpha);
}

void TernarySlit_float(float2 position, float2 topCorner, float4 bottomCorners, float2 handlePositions, out float alpha)
{
    alpha = min(min(sdSegment(position, bottomCorners.xy, handlePositions), sdSegment(position, bottomCorners.zw, handlePositions)), sdSegment(position, topCorner.xy, handlePositions)) - 3;
    alpha = opSmoothUnion(alpha, sdCircle(position, handlePositions) - 13, 30);
    alpha = saturate(alpha);
}

#endif
