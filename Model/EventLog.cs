using System;
using System.Reflection;
using System.Text.Json;

namespace HAS.Profile.Model
{
    public class EventLog : IEntity
    {
        public string Id { get; private set; }
        public DateTime CaptureDate { get; private set; }
        public string Assembly { get; private set; }
        public string Event { get; private set; }
        public string ProfileId { get; private set; }
        public object Message { get; private set; }
        public object Result { get; private set; }

        private EventLog() { }

        private EventLog(object command)
        {
            CaptureDate = DateTime.UtcNow;
            Assembly = command.GetType().Assembly.GetName().Name;
            Event = command.GetType().Name;

            PropertyInfo profileInfo = command.GetType().GetProperty("ProfileId") ?? command.GetType().GetProperty("InstructorId");
            ProfileId = (string)profileInfo.GetValue(command, null);

            Message = command;
        }

        public static EventLog Create(object item) => new EventLog(item);

        public void AddResult(object result) => Result = result;
    }
}
