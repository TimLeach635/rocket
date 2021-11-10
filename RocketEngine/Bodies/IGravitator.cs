namespace RocketEngine.Bodies
{
    public interface IGravitator : IBody
    {
        float Mass { get; }
        float StandardGravitationalParameter { get; }
    }
}