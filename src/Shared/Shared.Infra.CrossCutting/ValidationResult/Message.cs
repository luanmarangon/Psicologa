

using System.ComponentModel;

namespace Shared.Infra.CrossCutting.ValidationResult
{
    public class Message
    {

        public enum TypeMessage
        {
            [Description("Success")]
            Success = 1,
            [Description("InvalidField")]
            InvalidField = 2,
            [Description("Error")]
            Error = 3,
            [Description("Information")]
            Information = 4,
            [Description("Notification")]
            Notification = 5,
            [Description("Alert")]
            Alert = 6
        }

        public string Id { get; set; }
        public TypeMessage Type { get; set; }
        public string Text { get; set; }

        public Message(TypeMessage type, string text, string id = "")
        {
            Id = id;
            Type = type;
            Text = text;
        }
    }
}
