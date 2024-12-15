using Prism.Events;

namespace TypeWriter
{
    public class AppConfigChangedEvent : PubSubEvent
    {
    }

    public class NewFileLoadedEvent : PubSubEvent
    {
    }

    public class ShowTypeBoxEvent : PubSubEvent
    {
    }

    public class HideTypeBoxEvent : PubSubEvent
    {
    }

    public class AudioControlTypeChangedEvent : PubSubEvent<object>
    {
    }

    public class AudioPlayModeChangedEvent : PubSubEvent<object>
    {
    }

    public class AudioSelected : PubSubEvent<string>
    {
    }

    public enum AudioControlType
    {
        Next,
        Previous,
        Forward,
        Back,
        ResetSpeedRatio,
        IncrementSpeedRatio,
        DecrementSpeedRatio
    }

    public enum AudioPlayMode
    {
        SingleLoop,
        ListLoop,
        OrderPlay,
        RandomPlay
    }
}