
using System;
using System.Threading;

public class TypeAssistant
{
    public event Action<string> Idled;
    string Text;
    Timer Timer;

    public TypeAssistant()
    {
        Timer = new Timer(p => Idled(Text));
    }

    public void TextChanged(string text)
    {
        Text = text;
        Timer.Change(500, Timeout.Infinite);
    }
}
