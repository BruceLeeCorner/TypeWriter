using Prism.Events;
using Prism.Mvvm;

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
    public class HideTypeBox : PubSubEvent
    {

    }
}