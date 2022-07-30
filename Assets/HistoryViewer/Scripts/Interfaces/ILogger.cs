public interface ILogger
{
    public void Log(string text);

    class NoLogger : ILogger
    {
        public void Log(string text) { }
    }
}
