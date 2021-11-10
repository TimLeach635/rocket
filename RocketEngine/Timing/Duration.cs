namespace RocketEngine.Timing
{
    public class Duration
    {
        // internally represented in seconds

        public Duration(double seconds)
        {
            Seconds = seconds;
        }

        public double Days => Seconds / 86400d;
        public double Hours => Seconds / 3600d;
        public double Minutes => Seconds / 60d;
        public double Seconds { get; }

        public double Milliseconds => Seconds * 1e3;

        public double Microseconds => Seconds * 1e6;

        // a "tick" in the context of C# is 100 nanoseconds (0.1 microseconds)
        public double Ticks => Seconds * 1e7;
        public double Nanoseconds => Seconds * 1e9;
    }
}