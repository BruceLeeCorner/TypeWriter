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
}