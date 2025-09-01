using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

public class GeneroFilmeIndexPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private static readonly By BtnCadastrar = By.CssSelector("a[data-se='btnCadastrar']");
    private static readonly By Cards = By.CssSelector(".card");
    private static readonly By BtnEditar = By.CssSelector(".card a[title='Edição']");
    private static readonly By BtnExcluir = By.CssSelector(".card a[title='Exclusão']");

    public GeneroFilmeIndexPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public GeneroFilmeIndexPageObjects IrPara(string enderecoBase) {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "generos"));

        wait.Until(ExpectedConditions.ElementExists(BtnCadastrar));
        wait.Until(d => d.FindElements(Cards).Count >= 0);

        return this;
    }

    public GeneroFilmeFormPageObjects ClickCadastrar() {
        var btn = Waits.Clickable(driver, BtnCadastrar);

        Clicks.SafeClick(driver, btn);

        return new GeneroFilmeFormPageObjects(driver);
    }

    public GeneroFilmeFormPageObjects ClickEditar(string descricao) {
        wait.Until(d => d.PageSource.Contains(descricao));

        var btn = Waits.Clickable(driver, BtnEditar);

        Clicks.SafeClick(driver, btn);

        return new GeneroFilmeFormPageObjects(driver);
    }

    public GeneroFilmeFormPageObjects ClickExcluir(string descricao) {
        wait.Until(d => d.PageSource.Contains(descricao));

        var btn = Waits.Clickable(driver, BtnExcluir);

        Clicks.SafeClick(driver, btn);

        return new GeneroFilmeFormPageObjects(driver);
    }

    public bool ContemGenero(string descricao) {
        // espere voltar para a tela de index
        wait.Until(d => d.FindElements(Cards).Count >= 0);

        return driver.PageSource.Contains(descricao);
    }
}
