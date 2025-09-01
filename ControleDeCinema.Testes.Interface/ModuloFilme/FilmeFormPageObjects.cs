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
    private static readonly By BtnSubmit = By.Id("botaoConfirmar");
    private static readonly By Cards = By.CssSelector(".card");

    public FilmeFormPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        wait.Until(ExpectedConditions.ElementIsVisible(TituloInput));
    }

    public FilmeFormPageObjects PreencherTitulo(string titulo) {
        var el = wait.Until(ExpectedConditions.ElementIsVisible(TituloInput));

        el.Clear();
        el.SendKeys(titulo);

        return this;
    }

    public FilmeFormPageObjects PreencherDuracao(int duracao) {
        var el = wait.Until(ExpectedConditions.ElementIsVisible(DuracaoInput));

        el.Clear();
        el.SendKeys(duracao.ToString());

        return this;
    }

    public FilmeFormPageObjects PreencherLancamento(bool lancamento) {
        var el = wait.Until(ExpectedConditions.ElementToBeClickable(LancamentoCheck));

        if (el.Selected != lancamento)
            Clicks.SafeClick(driver, el);

        return this;
    }

    public FilmeFormPageObjects PreencherGenero(string generoTextoVisivel) {
        var el = wait.Until(ExpectedConditions.ElementIsVisible(GeneroSelect));
        var select = new SelectElement(el);

        wait.Until(_ => select.Options.Any());

        wait.Until(_ => select.Options.Any(o =>
            string.Equals(o.Text, generoTextoVisivel, StringComparison.OrdinalIgnoreCase)));

        select.SelectByText(generoTextoVisivel);
        return this;
    }

    public FilmeIndexPageObjects Confirmar() {
        var btn = Waits.Clickable(driver, BtnSubmit, 20);

        Clicks.SafeClick(driver, btn);

        wait.Until(_ => driver.Url.Contains("/filmes", StringComparison.OrdinalIgnoreCase));
        wait.Until(_ => driver.FindElements(Cards).Count >= 0);

        return new FilmeIndexPageObjects(driver);
    }

    public FilmeIndexPageObjects ConfirmarExclusao() {
        var btn = Waits.Clickable(driver, By.Id("botaoConfirmar"), 20);
        Clicks.SafeClick(driver, btn);

        wait.Until(_ => driver.Url.Contains("/filmes", StringComparison.OrdinalIgnoreCase));
        wait.Until(_ => driver.FindElements(By.CssSelector(".card")).Count >= 0);

        return new FilmeIndexPageObjects(driver);
    }
}