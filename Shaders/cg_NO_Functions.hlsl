#ifndef NIQONNO_Functions_HLSL_INCLUDED
#define NIQONNO_Functions_HLSL_INCLUDED

//#include "cg_NO_SDF.hlsl"

void MultiStepInclusive_float(float value, int steps, out float result)
{
    result = saturate(floor(saturate(value) * steps) / (steps-1));
}

void MultiStepExclusive_float(float value, int steps, out float result)
{
    result = saturate(floor(saturate(value) * steps) / steps);
}

#endif
