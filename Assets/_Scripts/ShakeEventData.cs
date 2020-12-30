// reference Mirza @ YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera Shake Event", menuName = "Custom/Camera Shake Event", order = 1)]
public class ShakeEventData : ScriptableObject
{
    public enum Target
    {
        Rotation,
        Position
    }

    public Target target = Target.Rotation;

    public float amplitude = 1.0f;  // force of shake
    public float frequency = 1.0f;  // speed of scroll through Perlin noise
    public float duration = 1.0f;

    public AnimationCurve blendOverLifetime = new AnimationCurve(
        new Keyframe( 0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * 720.0f ),
        new Keyframe( 0.2f, 1.0f ),
        new Keyframe( 1.0f, 0.0f ) );

    public void Init( float amplitude, float frequency, float duration, AnimationCurve blendOverLifetime, Target target)
    {
        this.target = target;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.duration = duration;
        this.blendOverLifetime = blendOverLifetime;
    }
}
