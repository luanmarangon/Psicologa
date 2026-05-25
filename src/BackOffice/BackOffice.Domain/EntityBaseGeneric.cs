
using Shared.Infra.CrossCutting.ValidationResult;
using System.Text.Json.Serialization;

namespace Psicologa.Domain
{
    public abstract class EntityBaseGeneric
    {

        [JsonIgnore]
        public ValidationResult ValidationResult { get; protected set; }

        public EntityBaseGeneric()
        {
            ValidationResult = new ValidationResult();
        }
    }
}
