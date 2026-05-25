using System.Collections.Generic;
using System.Linq;

namespace Shared.Infra.CrossCutting.ValidationResult
{
    public class ValidationResult
    {
        List<Message> _messages = new List<Message>();

        public ValidationResult()
        {

        }

        public Message[] Messages { get => _messages.ToArray(); }

        public int Count { get => _messages.Count; }


        public void Add(Message.TypeMessage type, string text)
        {
            _messages.Add(new Message(type, text));
        }

        public void Add(Message message)
        {
            _messages.Add(message);
        }

        public void Add(Message[] messages)
        {
            foreach (var m in messages)
            {
                _messages.Add(m);
            }
        }

        public void AddUserMessageSuccess(string text, string id = "")
        {
            _messages.Add(new UserMessage(Message.TypeMessage.Success, text, id));
        }

        public void AddUserMessageError(string text, string id = "")
        {
            _messages.Add(new UserMessage(Message.TypeMessage.Error, text, id));
        }

        public void AddUserMessageInvalidField(string text, string id = "")
        {
            _messages.Add(new UserMessage(Message.TypeMessage.InvalidField, text, id));
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public bool IsValid()
        {
            return
            _messages.Where(m =>
               m.Type == Message.TypeMessage.Error ||
               m.Type == Message.TypeMessage.InvalidField).Count() == 0;
        }

    }
}
