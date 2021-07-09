using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//UnityEvent<T>是抽象类，需要一个类来继承，否则无法实例化，string[] 用来传递marketList
public class UnityEventStrList : UnityEvent<string[]> { }

public class EventManager
{
    private Dictionary<string, UnityEvent> eventDic = new Dictionary<string, UnityEvent>();
    private Dictionary<string, UnityEventStrList> eventDicStrList = new Dictionary<string, UnityEventStrList>();
    private static EventManager eventManager = new EventManager();
    private EventManager() { }
    public static EventManager GetInstance
    {
        get
        {
            return eventManager;
        }
    }

    public void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventManager.eventDic.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventManager.eventDic.Add(eventName, thisEvent);
        }
    }
    public void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (eventManager.eventDic.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void StartListening(string eventName, UnityAction<string[]> listener)
    {
        UnityEventStrList thisEvent = null;
        if (eventManager.eventDicStrList.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEventStrList();
            thisEvent.AddListener(listener);
            eventManager.eventDicStrList.Add(eventName, thisEvent);
        }
    }
    public void StopListening(string eventName, UnityAction<string[]> listener)
    {
        if (eventManager == null) return;
        UnityEventStrList thisEvent = null;
        if (eventManager.eventDicStrList.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }



    public void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (eventManager.eventDic.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    public void TriggerEvent(string eventName, string[] par)
    {
        UnityEventStrList thisEvent = null;
        if (eventManager.eventDicStrList.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(par);
        }
    }

}
