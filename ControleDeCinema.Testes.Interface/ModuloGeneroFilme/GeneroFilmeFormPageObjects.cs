using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

public class GeneroFilmeFormPageObjects {

    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private static readonly By InputDescricao = By.Id("Descricao");
    private static readonly By BtnConfirmar = By.Id("botaoConfirmar");
    private static readonly By Cards = By.CssSelector(".card");

    public GeneroFilmeFormPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public GeneroFilmeFormPageObjects PreencherDescricao(string descricao) {
        var input = wait.Until(ExpectedConditions.ElementIsVisible(InputDescricao));

        input.Clear();
        input.SendKeys(descricao);

        return this;
    }

    public GeneroFilmeIndexPageObjects Confirmar() {
        var btn = Waits.Clickable(driver, BtnConfirmar);
        Clicks.SafeClick(driver, btn);

        // espere voltar para a tela de index
        wait.Until(d => d.FindElements(Cards).Count >= 0);

        return new GeneroFilmeIndexPageObjects(driver);
    }

    public GeneroFilmeIndexPageObjects ConfirmarExclusao() {
        var btn = Waits.Clickable(driver, BtnConfirmar);
        Clicks.SafeClick(driver, btn);

        wait.Until(d => d.FindElements(Cards).Count >= 0);
        return new GeneroFilmeIndexPageObjects(driver);
    }
}
