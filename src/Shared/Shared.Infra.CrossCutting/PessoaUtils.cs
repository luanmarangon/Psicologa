using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Infra.CrossCutting
{
    public static class PessoaUtils
    {
        public static string FormatarCPFCPNJ(string valor)
        {
            if (valor.Length == 11)
            {
                return PessoaFisicaUtils.FormatarCPF(valor);
            }
            else if (valor.Length == 14)
            {
                return PessoaJuridicaUtils.FormatarCNPJ(valor);
            }
            else return valor;
        }

    }

}
