using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psicologa.Application;
using Psicologa.Application.Convenio.Services;
using Psicologa.Application.ServicoContato.Services;
using Psicologa.Application.ServicoContato.ViewsModel;
using Psicologa.Application.Usuario.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly UsuarioAutenticado _ua;
        private readonly RequisicaoAtual _req;
        private readonly IAppSettings _appSettings;
        private readonly ApplicationConvenioService _convenioService;
        private readonly ApplicationServicoContatoService _servicoContatoService;

        public HomeController(UsuarioAutenticado ua,
                              RequisicaoAtual req,
                              IAppSettings appSettings,
                              ApplicationConvenioService convenioService,
                              ApplicationServicoContatoService servicoContatoService
                             )
        {
            _ua = ua;
            _req = req;
            _appSettings = appSettings;
            _convenioService = convenioService;
            _servicoContatoService = servicoContatoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //public IActionResult EnviarEmailContato([FromBody] System.Text.Json.JsonElement dados)
        //{
        //    bool sucesso = false;
        //    ValidationResult vr = new ValidationResult();

        //    EmailContatoViewModel emailContato = null;
        //    try
        //    {
        //        emailContato = dados.Deserialize<EmailContatoViewModel>();

        //        Email email = new Email(_appSettings.Email.Email, _appSettings.Email.Email, _appSettings.Email.Senha, _appSettings.Email.Remetente, _appSettings.Email.SMTP, _appSettings.Email.Porta);

        //        string texto = $@"
        //                        <table width='100%' cellpadding='0' cellspacing='0'>

        //                          <!-- Dados do contato -->
        //                          <tr>
        //                            <td style='padding:8px 0;border-bottom:1px solid #e2e8f0;'>
        //                              <span style='font-size:12px;color:#94a3b8;'>Contato</span><br/>
        //                              <span style='font-size:15px;color:#0f172a;font-weight:bold;'>{emailContato.Nome}</span>
        //                            </td>
        //                          </tr>
        //                          <tr>
        //                            <td style='padding:8px 0;border-bottom:1px solid #e2e8f0;'>
        //                              <span style='font-size:12px;color:#94a3b8;'>E-mail</span><br/>
        //                              <a href='mailto:{emailContato.Email}'
        //                                 style='font-size:15px;color:#2563eb;font-weight:bold;text-decoration:none;'>
        //                                {emailContato.Email}
        //                              </a>
        //                            </td>
        //                          </tr>
        //                          <tr>
        //                            <td style='padding:8px 0;border-bottom:1px solid #e2e8f0;'>
        //                              <span style='font-size:12px;color:#94a3b8;'>Assunto</span><br/>
        //                              <span style='font-size:15px;color:#1e40af;font-weight:bold;'>{emailContato.Assunto}</span>
        //                            </td>
        //                          </tr>

        //                          <!-- Mensagem -->
        //                          <tr>
        //                            <td style='padding:16px 0 0;'>
        //                              <span style='font-size:12px;color:#94a3b8;'>Mensagem</span><br/>
        //                              <p style='margin:8px 0 0;font-size:15px;color:#334155;line-height:1.7;white-space:pre-line;'>
        //                                {emailContato.Mensagem}
        //                              </p>
        //                            </td>
        //                          </tr>

        //                          <!-- Origem -->
        //                          <tr>
        //                            <td style='padding:24px 0 0;'>
        //                              <p style='margin:0;font-size:12px;color:#94a3b8;border-top:1px solid #e2e8f0;padding-top:16px;'>
        //                                Enviado a partir do <strong style='color:#64748b;'>{_appSettings.Nome}</strong>
        //                              </p>
        //                            </td>
        //                          </tr>

        //                        </table>
        //                    ";

        //        string msgAux;
        //        (sucesso, msgAux) = email.EnviarEmail("luan.limarangon@hotmail.com", texto, "Psicologa - Contato Site");

        //        if (!sucesso)
        //            vr.Add(Message.TypeMessage.Error, msgAux);
        //        else
        //            vr.Add(Message.TypeMessage.Success, $"Mensagem enviada com sucesso!");

        //        sucesso = true;
        //        AddUserMessage(vr);
        //    }
        //    catch (Exception ex)
        //    {
        //        AddUserMessageError("Um erro ocorreu. Tente novamente");
        //    }

        //    return DefaultJSONResponse(sucesso);
        //}
        public IActionResult EnviarEmailContato([FromBody] System.Text.Json.JsonElement dados)
        {
            bool sucesso = false;
            ValidationResult vr = new ValidationResult();
            EmailContatoViewModel emailContato = null;

            try
            {
                emailContato = dados.Deserialize<EmailContatoViewModel>();

                string texto = $@"
            <table width='100%' cellpadding='0' cellspacing='0'>
              <!-- Dados do contato -->
              <tr>
                <td style='padding:8px 0;border-bottom:1px solid #e2e8f0;'>
                  <span style='font-size:12px;color:#94a3b8;'>Contato</span><br/>
                  <span style='font-size:15px;color:#0f172a;font-weight:bold;'>{emailContato.Nome}</span>
                </td>
              </tr>
              <tr>
                <td style='padding:8px 0;border-bottom:1px solid #e2e8f0;'>
                  <span style='font-size:12px;color:#94a3b8;'>E-mail</span><br/>
                  <a href='mailto:{emailContato.Email}'
                     style='font-size:15px;color:#2563eb;font-weight:bold;text-decoration:none;'>
                    {emailContato.Email}
                  </a>
                </td>
              </tr>
              <tr>
                <td style='padding:8px 0;border-bottom:1px solid #e2e8f0;'>
                  <span style='font-size:12px;color:#94a3b8;'>Assunto</span><br/>
                  <span style='font-size:15px;color:#1e40af;font-weight:bold;'>{emailContato.Assunto}</span>
                </td>
              </tr>
              <!-- Mensagem -->
              <tr>
                <td style='padding:16px 0 0;'>
                  <span style='font-size:12px;color:#94a3b8;'>Mensagem</span><br/>
                  <p style='margin:8px 0 0;font-size:15px;color:#334155;line-height:1.7;white-space:pre-line;'>
                    {emailContato.Mensagem}
                  </p>
                </td>
              </tr>
              <!-- Origem -->
              <tr>
                <td style='padding:24px 0 0;'>
                  <p style='margin:0;font-size:12px;color:#94a3b8;border-top:1px solid #e2e8f0;padding-top:16px;'>
                    Enviado a partir do <strong style='color:#64748b;'>{_appSettings.Nome}</strong>
                  </p>
                </td>
              </tr>
            </table>
        ";

                // Captura os valores necessários antes do Task.Run
                // (não pode usar _appSettings dentro do lambda diretamente
                //  pois o controller pode ser descartado antes da task terminar)
                var emailConfig = _appSettings.Email;
                var destinatario = "luan.limarangon@hotmail.com";
                var assunto = "Psicologa - Contato Site";
                var corpo = texto;

                // Dispara o envio em background e já retorna ao cliente
                _ = Task.Run(() =>
                {
                    try
                    {
                        Email email = new Email(
                            emailConfig.Email,
                            emailConfig.Email,
                            emailConfig.Senha,
                            emailConfig.Remetente,
                            emailConfig.SMTP,
                            emailConfig.Porta
                        );

                        email.EnviarEmail(destinatario, corpo, assunto);
                    }
                    catch (Exception ex)
                    {
                        // Aqui você pode logar o erro sem impactar o usuário
                        // Ex: _logger.LogError(ex, "Erro ao enviar e-mail de contato");
                        Console.WriteLine($"[Erro background e-mail] {ex.Message}");
                    }
                });

                // Responde imediatamente sem esperar o envio
                sucesso = true;
                vr.Add(Message.TypeMessage.Success, "Mensagem enviada com sucesso!");
                AddUserMessage(vr);
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            return DefaultJSONResponse(sucesso);
        }

        [HttpGet]
        public IActionResult ObterConvenioDestaques()
        {
            var convenios = _convenioService.ObterDestaquesHome();
            return DefaultJSONResponse(true, convenios);
        }

        [HttpPost]
        public IActionResult SaibaMais([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ServicoContatoViewModel servicoVM = null;
            try
            {
                servicoVM = dados.Deserialize<ServicoContatoViewModel>();
                (operacao, vr) = _servicoContatoService.SalvarSaibaMais(servicoVM, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }
            object retorno = null;
            if (operacao)
            {
                retorno = _servicoContatoService.Obter(servicoVM.Id);
                AddUserMessageSuccess("Contato enviado com Sucesso");
            }
            return DefaultJSONResponse(operacao, retorno);
        }
    }
}