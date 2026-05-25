using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Controllers
{
    [Authorize("CookieAuth")]
    public class BaseController: Controller
    {
        public BaseController()
        {
            UserMessages = new List<UserMessage>();
        }

        public List<UserMessage> UserMessages { get; set; }

        public void AddUserMessage(UserMessage.TypeMessage type, string text)
        {
            UserMessages.Add(new UserMessage(type, text));
        }

        public void AddUserMessageSuccess(string text)
        {
            UserMessages.Add(new UserMessage(Message.TypeMessage.Success, text));
        }

        public void AddUserMessageError(string text)
        {
            UserMessages.Add(new UserMessage(Message.TypeMessage.Error, text));
        }

        public void AddUserMessageInvalidField(string text)
        {
            UserMessages.Add(new UserMessage(Message.TypeMessage.InvalidField, text));
        }

        public void AddUserMessage(ValidationResult vr)
        {
            foreach (var item in vr.Messages)
            {
                UserMessages.Add(new UserMessage(item.Type, item.Text));
            }
        }

        public IActionResult DefaultJSONResponse(bool success, object data = null, UserMessage msg = null)
        {
            if (msg != null)
            {
                UserMessages.Add(msg);
            }

            return Json(new
            {
                success,
                data,
                messages = UserMessages
            });
        }
    }
}
