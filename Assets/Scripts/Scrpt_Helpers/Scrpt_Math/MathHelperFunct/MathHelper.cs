using UnityEngine;


//https://stackoverflow.com/questions/13462001/ease-in-and-ease-out-animation-formula
//Shout out to them for the Bezier in out blend

//https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
//Shot out for the damp function!
public class MathHelper
{
    public static int ToIntPercent(float val)
    {
        return Mathf.RoundToInt(val * 100);
    }

    public static float FloatToAudioLog(float val)
    {
        return val == 0f ? -80f : Mathf.Log10(val)*20;
    }

    public static float BezierBlend(float t)
    {
        return t * t * (3.0f - 2.0f * t);
    }

    //Input an angle value. Returns a [-180,180) Value useful for comparisons
    public static float RotationTo180Scale(float t)
    {
        float num = Mathf.Repeat(t, 360f);
        return num > 180f ? num - 360f : num;
    }


    //Input a [0,1] at 0.5 will give you a value of 9 at the rest will smooth out to a value of 9
    public static float BezierBuffClamped(float t)
    {
        return t <= 0.5 ? 32f * t * (1f - t) + 1f : 9;
    }

    public static float BezierBuff(float t, float m, float n = 1)
    {
        return -4 * (m-n) * t * (1f - t) + n;
    }

    public static float CubicBuff(float t, float m, float n = 1)
    {
        return (n-m) *t*t*t + m;
    }

    public static float DampAngle(float source, float target, float smoothing, float dt)
    {
        return Mathf.LerpAngle(source, target, 1 - Mathf.Pow(smoothing, dt));
    }

    public static float Damp(float source, float target, float smoothing, float dt)
    {
        return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }

    public static Vector3 DampVec3(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        return Vector3.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }

}
