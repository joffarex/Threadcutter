 using System;
using Godot;

namespace Threadcutter.DataObjects;

public partial class JumpData : Resource
{
    [Export] public float JumpHeight { get; set; } = 300.0f;
    [Export] public float JumpTimeToPeak { get; set; } = 0.6f;
    [Export] public float JumpTimeToDescent { get; set; } = 0.5f;

    public float JumpVelocity => ((2.0f * JumpHeight) / JumpTimeToPeak) * -1.0f;
    public float JumpGravity => ((-2.0f * JumpHeight) / (float)Math.Pow(JumpTimeToPeak, 2.0f)) * -1.0f;
    public float FallGravity => ((-2.0f * JumpHeight) / (float)Math.Pow(JumpTimeToDescent, 2.0f)) * -1.0f;
}