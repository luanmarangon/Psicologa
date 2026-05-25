
using Shared.Infra.CrossCutting.ValidationResult;
using System.Text.Json.Serialization;

namespace Shared.Domain
{
    public abstract class EntityBase
    {
        public int Id { get; set; }

        public abstract bool Validar();

        [JsonIgnore]
        public ValidationResult ValidationResult { get; protected set; }

        public EntityBase()
        {
            ValidationResult = new ValidationResult();
        }

    }
}
