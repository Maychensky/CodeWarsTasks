namespace CodeWarsTasks
{
    public class TCP
    {
        private enum Event { APP_PASSIVE_OPEN, APP_ACTIVE_OPEN, APP_SEND, APP_CLOSE, APP_TIMEOUT, RCV_SYN, RCV_ACK, RCV_SYN_ACK, RCV_FIN, RCV_FIN_ACK }
        private enum State { CLOSED, LISTEN, SYN_SENT, SYN_RCVD, ESTABLISHED, CLOSE_WAIT, LAST_ACK, FIN_WAIT_1, FIN_WAIT_2, CLOSING, TIME_WAIT, ERROR }

        private static Dictionary<State, Dictionary<Event, State>> _dictionary = new Dictionary<State, Dictionary<Event, State>>
        {
            { State.CLOSED, new Dictionary<Event, State>() { { Event.APP_PASSIVE_OPEN, State.LISTEN}, { Event.APP_ACTIVE_OPEN, State.SYN_SENT},}},
            { State.LISTEN, new Dictionary<Event, State>() { { Event.RCV_SYN, State.SYN_RCVD}, { Event.APP_SEND, State.SYN_SENT}, { Event.APP_CLOSE, State.CLOSED},}},
            { State.SYN_RCVD, new Dictionary<Event, State>() { { Event.APP_CLOSE, State.FIN_WAIT_1}, { Event.RCV_ACK, State.ESTABLISHED},}},
            { State.SYN_SENT, new Dictionary<Event, State>() { { Event.RCV_SYN, State.SYN_RCVD}, { Event.RCV_SYN_ACK, State.ESTABLISHED}, { Event.APP_CLOSE, State.CLOSED}, }},
            { State.ESTABLISHED, new Dictionary<Event, State>() { { Event.APP_CLOSE, State.FIN_WAIT_1}, { Event.RCV_FIN, State.CLOSE_WAIT},}},
            { State.FIN_WAIT_1, new Dictionary<Event, State>() { { Event.RCV_FIN, State.CLOSING}, { Event.RCV_FIN_ACK, State.TIME_WAIT}, { Event.RCV_ACK, State.FIN_WAIT_2}, }},
            { State.CLOSING, new Dictionary<Event, State>() { { Event.RCV_ACK, State.TIME_WAIT}, } },
            { State.FIN_WAIT_2, new Dictionary<Event, State>() { { Event.RCV_FIN, State.TIME_WAIT}, } },
            { State.TIME_WAIT, new Dictionary<Event, State>() { { Event.APP_TIMEOUT, State.CLOSED}, } },
            { State.CLOSE_WAIT, new Dictionary<Event, State>() { { Event.APP_CLOSE, State.LAST_ACK}, } },
            { State.LAST_ACK, new Dictionary<Event, State>() { { Event.RCV_ACK, State.CLOSED} } },
        };

        private static State _currentState;

        private static Event[] _events;
        public static string TraverseStates(string[] events)
        {
            _currentState = State.CLOSED;
            _events = GetArrayEvents(events);
            return RunAutomaton().ToString();
        }

        private static State RunAutomaton()
        {
            for (int currentIndexEvent = 0; currentIndexEvent < _events.Length; currentIndexEvent++)
                if (_dictionary[_currentState].ContainsKey(_events[currentIndexEvent]))
                    _currentState = _dictionary[_currentState][_events[currentIndexEvent]];
                else return State.ERROR;
            return _currentState;
        }

        private static Event[] GetArrayEvents(string[] eventsString)
        {
            Event[] events = new Event[eventsString.Length];
            for (int i = 0; i < eventsString.Length; i++)
                events[i] = GetEvent(eventsString[i]);
            return events;
        }

        private static Event GetEvent(string eventName)
        {
            Event choice;
            if (Enum.TryParse(eventName, out choice)) return choice;
            else throw new Exception();
        }
    }
}