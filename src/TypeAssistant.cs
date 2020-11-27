
using System;
using System.Threading;

public class TypeAssistant
{
    public event Action<string> Idled;
    string Text;
    Timer WaitingTimer;

    public TypeAssistant()
    {
        WaitingTimer = new Timer(p => Idled(Text));
    }

    public void TextChanged(string text)
    {
        Text = text;
        WaitingTimer.Change(500, Timeout.Infinite);
    }
}
