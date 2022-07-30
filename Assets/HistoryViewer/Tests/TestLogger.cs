using UnityEngine;
public class TestLogger : Negi0109.HistoryViewer.Interfaces.ILogger
{
    public void Log(string text)
    {
        Debug.Log(text);
    }
}
