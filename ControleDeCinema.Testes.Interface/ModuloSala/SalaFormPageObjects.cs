using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ControleDeCinema.Testes.Interface.ModuloSala;
public class SalaFormPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private static readonly By InputNumero = By.Id("Numero");
    private static readonly By InputCapacidade = By.Id("Capacidade");
    private static readonly By BtnConfirmar = By.CssSelector("button[data-se='btnConfirmar']");
    private static readonly By BtnConfirmarExclusao = By.CssSelector("button[data-se='btnConfirmarExclusao']");
    private static readonly By Cards = By.CssSelector(".card");
    private static readonly By ListaSalas = By.CssSelector("[data-se='lista-salas']");

    public SalaFormPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public SalaFormPageObjects PreencherNumero(int numero) {
        var input = wait.Until(ExpectedConditions.ElementIsVisible(InputNumero));

        input.Clear();
        input.SendKeys(numero.ToString());

        return this;
    }

    public SalaFormPageObjects PreencherCapacidade(int capacidade) {
        var input = wait.Until(ExpectedConditions.ElementIsVisible(InputCapacidade));

        input.Clear();
        input.SendKeys(capacidade.ToString());

        return this;
    }

    public SalaIndexPageObjects Confirmar() {
        var js = (IJavaScriptExecutor)driver;

        // estado antes do clique
        var urlAntes = driver.Url;

        var btn = Waits.Clickable(driver, BtnConfirmar, 20);
        Clicks.SafeClick(driver, btn);

        // aguarda o browser terminar navegação/render
        wait.Until(_ => js.ExecuteScript("return document.readyState")?.ToString() == "complete");

        // espere por um dos cenários: sucesso no index, validação no form, ou login
        bool foiSucesso = wait.Until(d =>
        {
            var u = d.Url.ToLowerInvariant();

            // SUCESSO: voltou para /salas e o container da lista existe
            if (u.Contains("/salas")) {
                var listaOk = d.FindElements(By.CssSelector("[data-se='lista-salas']")).Count > 0;
                if (listaOk) return true;
            }

            // FALHA DE VALIDAÇÃO: permaneceu na mesma URL (form) e o campo "Número" existe
            var aindaNoForm = d.FindElements(By.Id("Numero")).Count > 0;
            if (aindaNoForm) {
                
                var temErroCampo = d.FindElements(By.CssSelector(".field-validation-error")).Any();
                var temResumo = d.FindElements(By.CssSelector(".validation-summary-errors,.text-danger.validation-summary-valid")).Any();
                if (temErroCampo || temResumo)
                    throw new Exception("Falha de validação ao cadastrar sala (permaneceu no formulário).");
            }

            // LOGIN / NÃO AUTORIZADO
            if (u.Contains("/login") || u.Contains("/account"))
                throw new Exception("Redirecionado para login ao confirmar. Verifique autenticação/perfis no teste.");

            return false; // continue esperando
        });

        if (!foiSucesso)
            throw new WebDriverTimeoutException("Não conseguiu confirmar o cadastro da sala.");

        wait.Until(d =>
            d.FindElements(By.CssSelector(".card .card-title[data-se='numero-sala']"))
             .Any(h => !string.IsNullOrWhiteSpace(h.Text))
        );

        return new SalaIndexPageObjects(driver);
    }

    public SalaIndexPageObjects ConfirmarExclusao() {
        var qtdAntes = driver.FindElements(Cards).Count;

        var btn = Waits.Clickable(driver, BtnConfirmarExclusao, 20);
        Clicks.SafeClick(driver, btn);

        wait.Until(_ => driver.Url.Contains("/salas", StringComparison.OrdinalIgnoreCase));
        wait.Until(ExpectedConditions.ElementExists(ListaSalas));
        wait.Until(d => d.FindElements(Cards).Count == Math.Max(0, qtdAntes - 1));

        return new SalaIndexPageObjects(driver);
    }
}
