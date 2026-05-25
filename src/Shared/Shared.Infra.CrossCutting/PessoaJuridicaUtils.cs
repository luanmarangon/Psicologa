using System.Linq;

namespace Shared.Infra.CrossCutting
{
    public static class PessoaJuridicaUtils
    {
        public static bool ValidarCNPJ(string cnpj)
        {
            if (cnpj.Any(char.IsLetter))
            {
                return ValidarCNPJNovo(cnpj);
            }
            else
            {
                return ValidarCNPJAntigo(cnpj);
            }
        }

        private static bool ValidarCNPJNovo(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = new string(cnpj.Where(char.IsLetterOrDigit).ToArray()).ToUpper();

            if (cnpj.Length != 14)
                return false;

            string baseCnpj = cnpj.Substring(0, 12);
            int dv1Informado = cnpj[12] - '0';
            int dv2Informado = cnpj[13] - '0';

            int dv1Calculado = CalcularDV(baseCnpj);
            int dv2Calculado = CalcularDV(baseCnpj + dv1Calculado);

            return dv1Informado == dv1Calculado &&
                   dv2Informado == dv2Calculado;
        }

        private static int CalcularDV(string cnpj)
        {
            int peso = 2;
            int soma = 0;

            for (int i = cnpj.Length - 1; i >= 0; i--)
            {
                int valor = CharParaValor(cnpj[i]);
                soma += valor * peso;
                peso = peso == 9 ? 2 : peso + 1;
            }

            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static int CharParaValor(char c)
        {
            return (int)c - 48;
        }

        private static bool ValidarCNPJAntigo(string cnpj)
        {
            string CNPJ = cnpj.Replace(".", "");
            CNPJ = CNPJ.Replace("/", "");
            CNPJ = CNPJ.Replace("-", "");
            int[] digitos, soma, resultado;
            int nrDig;
            string ftmt;
            bool[] CNPJOk;

            ftmt = "6543298765432";
            digitos = new int[14];

            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            CNPJOk = new bool[2];
            CNPJOk[0] = false;
            CNPJOk[1] = false;
            try
            {
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(
                        CNPJ.Substring(nrDig, 1));
                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));

                    if (nrDig <= 12)
                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);
                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }
                return (CNPJOk[0] && CNPJOk[1]);
            }
            catch
            {
                return false;
            }
        }

        public static string FormatarCNPJ(string cnpj)
        {
            try
            {
                return string.Format("{0}.{1}.{2}/{3}-{4}", cnpj.Substring(0, 2), cnpj.Substring(2, 3), cnpj.Substring(5, 3), cnpj.Substring(8, 4), cnpj.Substring(12, 2));
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
