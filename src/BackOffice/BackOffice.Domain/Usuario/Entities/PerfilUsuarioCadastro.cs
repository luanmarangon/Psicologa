using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Psicologa.Domain.Usuario.Entities.PerfilUsuario;

namespace Psicologa.Domain.Usuario.Entities
{
    public class PerfilUsuarioCadastro : EntityBase
    {
        public int Id { get; set; }
        public string Nome { get; set; }


        public override bool Validar()
        {

            return base.ValidationResult.Count == 0;
        }
    }
}
