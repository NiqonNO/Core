using UnityEngine;

namespace NiqonNO
{
    public enum NOEase
    {
        Linear,
        InBack,
        InBounce,
        InCirc,
        InCubic,
        InElastic,
        InExpo,
        InQuad,
        InQuart,
        InQuint,
        InSine,
        OutBack,
        OutBounce,
        OutCirc,
        OutCubic,
        OutElastic,
        OutExpo,
        OutQuad,
        OutQuart,
        OutQuint,
        OutSine,
        InOutBack,
        InOutBounce,
        InOutCirc,
        InOutCubic,
        InOutElastic,
        InOutExpo,
        InOutQuad,
        InOutQuart,
        InOutQuint,
        InOutSine,
    }
    
    public static class NOEaseExtentions
    {
        public static float Ease(this NOEase ease, float time)
        {
            return ease switch
            {
                NOEase.Linear => Linear(time),
                NOEase.InBack => InBack(time),
                NOEase.InBounce => InBounce(time),
                NOEase.InCirc => InCirc(time),
                NOEase.InCubic => InCubic(time),
                NOEase.InElastic => InElastic(time),
                NOEase.InExpo => InExpo(time),
                NOEase.InQuad => InQuad(time),
                NOEase.InQuart => InQuart(time),
                NOEase.InQuint => InQuint(time),
                NOEase.InSine => InSine(time),
                NOEase.OutBack => OutBack(time),
                NOEase.OutBounce => OutBounce(time),
                NOEase.OutCirc => OutCirc(time),
                NOEase.OutCubic => OutCubic(time),
                NOEase.OutElastic => OutElastic(time),
                NOEase.OutExpo => OutExpo(time),
                NOEase.OutQuad => OutQuad(time),
                NOEase.OutQuart => OutQuart(time),
                NOEase.OutQuint => OutQuint(time),
                NOEase.OutSine => OutSine(time),
                NOEase.InOutBack => InOutBack(time),
                NOEase.InOutBounce => InOutBounce(time),
                NOEase.InOutCirc => InOutCirc(time),
                NOEase.InOutCubic => InOutCubic(time),
                NOEase.InOutElastic => InOutElastic(time),
                NOEase.InOutExpo => InOutExpo(time),
                NOEase.InOutQuad => InOutQuad(time),
                NOEase.InOutQuart => InOutQuart(time),
                NOEase.InOutQuint => InOutQuint(time),
                NOEase.InOutSine => InOutSine(time),
                _ => Linear(time)
            };
            
            float Linear(float t) => t;

            float InBack(float t) => t * t * t - t * Mathf.Sin(t * Mathf.PI);

            float OutBack(float t) => 1f - InBack(1f - t);

            float InOutBack(float t) =>
                t < 0.5f
                    ? 0.5f * InBack(2f * t)
                    : 0.5f * OutBack(2f * t - 1f) + 0.5f;

            float InBounce(float t) => 1f - OutBounce(1f - t);

            float OutBounce(float t) =>
                t < 4f / 11.0f ?
                    (121f * t * t) / 16.0f :
                t < 8f / 11.0f ?
                    (363f / 40.0f * t * t) - (99f / 10.0f * t) + 17f / 5.0f :
                t < 9f / 10.0f ?
                    (4356f / 361.0f * t * t) - (35442f / 1805.0f * t) + 16061f / 1805.0f :
                    (54f / 5.0f * t * t) - (513f / 25.0f * t) + 268f / 25.0f;

            float InOutBounce(float t) =>
                t < 0.5f
                    ? 0.5f * InBounce(2f * t)
                    : 0.5f * OutBounce(2f * t - 1f) + 0.5f;

            float InCirc(float t) => 1f - Mathf.Sqrt(1f - (t * t));

            float OutCirc(float t) => Mathf.Sqrt((2f - t) * t);

            float InOutCirc(float t) =>
                t < 0.5f
                    ? 0.5f * (1 - Mathf.Sqrt(1f - 4f * (t * t)))
                    : 0.5f * (Mathf.Sqrt(-((2f * t) - 3f) * ((2f * t) - 1f)) + 1f);

            float InCubic(float t) => t * t * t;

            float OutCubic(float t) => InCubic(t - 1f) + 1f;

            float InOutCubic(float t) =>
                t < 0.5f
                    ? 4f * t * t * t
                    : 0.5f * InCubic(2f * t - 2f) + 1f;

            float InElastic(float t) => Mathf.Sin(13f * (Mathf.PI * 0.5f) * t) * Mathf.Pow(2f, 10f * (t - 1f));

            float OutElastic(float t) => Mathf.Sin(-13f * (Mathf.PI * 0.5f) * (t + 1)) * Mathf.Pow(2f, -10f * t) + 1f;

            float InOutElastic(float t) =>
                t < 0.5f
                    ? 0.5f * Mathf.Sin(13f * (Mathf.PI * 0.5f) * (2f * t)) * Mathf.Pow(2f, 10f * ((2f * t) - 1f))
                    : 0.5f * (Mathf.Sin(-13f * (Mathf.PI * 0.5f) * ((2f * t - 1f) + 1f)) * Mathf.Pow(2f, -10f * (2f * t - 1f)) + 2f);

            float InExpo(float t) => Mathf.Approximately(0.0f, t) ? t : Mathf.Pow(2f, 10f * (t - 1f));

            float OutExpo(float t) => Mathf.Approximately(1.0f, t) ? t : 1f - Mathf.Pow(2f, -10f * t);

            float InOutExpo(float v) =>
                Mathf.Approximately(0.0f, v) || Mathf.Approximately(1.0f, v)
                    ? v
                    : v < 0.5f
                        ?  0.5f * Mathf.Pow(2f, (20f * v) - 10f)
                        : -0.5f * Mathf.Pow(2f, (-20f * v) + 10f) + 1f;

            float InQuad(float t) => t * t;

            float OutQuad(float t) => -t * (t - 2f);

            float InOutQuad(float t) =>
                t < 0.5f
                    ?  2f * t * t
                    : -2f * t * t + 4f * t - 1f;

            float InQuart(float t) => t * t * t * t;

            float OutQuart(float t)
            {
                var u = t - 1f;
                return u * u * u * (1f - t) + 1f;
            }

            float InOutQuart(float t) =>
                t < 0.5f
                    ? 8f * InQuart(t)
                    : -8f * InQuart(t - 1f) + 1f;

            float InQuint(float t) => t * t * t * t * t;

            float OutQuint(float t) => InQuint(t - 1f) + 1f;

            float InOutQuint(float t) =>
                t < 0.5f
                    ? 16f * InQuint(t)
                    : 0.5f * InQuint(2f * t - 2f) + 1f;

            float InSine(float t) => Mathf.Sin((t - 1f) * (Mathf.PI * 0.5f)) + 1f;

            float OutSine(float t) => Mathf.Sin(t * (Mathf.PI * 0.5f));

            float InOutSine(float t) => 0.5f * (1f - Mathf.Cos(t * Mathf.PI));
        }
    }
}