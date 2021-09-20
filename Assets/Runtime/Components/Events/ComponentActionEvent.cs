using System;
using System.Collections.Generic;

namespace UIKit.Components.Events
{
    public class ComponentActionEvent<TEvent>
    {
        private readonly List<TEvent> _events = new List<TEvent>();

        public virtual bool AddEvent(TEvent action)
        {
            if (_events.Contains(action)) return false;
            _events.Add(action);
            return true;
        }

        public virtual void UnbindAll(Action<TEvent> eachElementAction)
        {
            for (int index = 0; index < _events.Count; index++)
            {
                eachElementAction.Invoke(_events[index]);
            }

            _events.Clear();
        }
    }
}
