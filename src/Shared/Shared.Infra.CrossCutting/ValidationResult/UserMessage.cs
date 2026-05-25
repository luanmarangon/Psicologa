
namespace Shared.Infra.CrossCutting.ValidationResult
{
    public class UserMessage : Message
    {
        public UserMessage(TypeMessage type, string text, string id = "") : base(type, text, id)
        {
        }

        
    }
}
