using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

public class FilmeFormPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private static readonly By TituloInput = By.Id("Titulo");
    private static readonly By DuracaoInput = By.Id("Duracao");
    private static readonly By LancamentoCheck = By.Id("Lancamento");
    private static readonly By GeneroSelect = By.Id("GeneroId");
    private static readonly By BtnConfirmar = By.CssSelector("button[data-se='btnConfirmar']");
    private static readonly By BtnConfirmarExclusao = By.CssSelector("button[data-se='btnConfirmarExclusao']");
    private static readonly By Cards = By.CssSelector(".card");

    public FilmeFormPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    public FilmeFormPageObjects PreencherTitulo(string titulo) {
        var webE = wait.Until(ExpectedConditions.ElementIsVisible(TituloInput));

        webE.Clear();
        webE.SendKeys(titulo);

        return this;
    }

    public FilmeFormPageObjects PreencherDuracao(int duracao) {
        var webE = wait.Until(ExpectedConditions.ElementIsVisible(DuracaoInput));

        webE.Clear();
        webE.SendKeys(duracao.ToString());

        return this;
    }

    public FilmeFormPageObjects PreencherLancamento(bool lancamento) {
        var webE = wait.Until(ExpectedConditions.ElementToBeClickable(LancamentoCheck));

        if (webE.Selected != lancamento)
            Clicks.SafeClick(driver, webE);

        return this;
    }

    public FilmeFormPageObjects PreencherGenero(string generoTextoVisivel) {
        var webE = wait.Until(ExpectedConditions.ElementIsVisible(GeneroSelect));
        var select = new SelectElement(webE);

        wait.Until(_ => select.Options.Any());

        wait.Until(_ => select.Options.Any(o =>
            string.Equals(o.Text, generoTextoVisivel, StringComparison.OrdinalIgnoreCase)));

        select.SelectByText(generoTextoVisivel);
        return this;
    }

    public FilmeIndexPageObjects Confirmar() {
        var btn = Waits.Clickable(driver, BtnConfirmar, 20);

        Clicks.SafeClick(driver, btn);

        wait.Until(_ => driver.Url.Contains("/filmes", StringComparison.OrdinalIgnoreCase));
        wait.Until(_ => driver.FindElements(Cards).Count >= 0);

        return new FilmeIndexPageObjects(driver);
    }

    public FilmeIndexPageObjects ConfirmarExclusao() {
        var btn = Waits.Clickable(driver, BtnConfirmarExclusao, 20);
        Clicks.SafeClick(driver, btn);

        wait.Until(_ => driver.Url.Contains("/filmes", StringComparison.OrdinalIgnoreCase));
        wait.Until(_ => driver.FindElements(By.CssSelector(".card")).Count >= 0);

        return new FilmeIndexPageObjects(driver);
    }
}