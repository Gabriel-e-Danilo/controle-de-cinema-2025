using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;
public class FilmeIndexPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private static readonly By BtnCadastrar = By.CssSelector("a[data-se='btnCadastrar']");
    private static readonly By Cards = By.CssSelector(".card");
    private static readonly By FormAnchor = By.Id("Titulo");

    public FilmeIndexPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public FilmeIndexPageObjects IrPara(string enderecoBase) {
        driver.Navigate().GoToUrl($"{enderecoBase}/filmes");

        wait.Until(ExpectedConditions.ElementExists(BtnCadastrar));
        wait.Until(_ => driver.FindElements(Cards).Count >= 0);

        return this;
    }

    public FilmeFormPageObjects ClickCadastrar() {
        var btn = Waits.Clickable(driver, BtnCadastrar, 20);

        Clicks.SafeClick(driver, btn);

        wait.Until(ExpectedConditions.ElementIsVisible(FormAnchor));

        return new FilmeFormPageObjects(driver);
    }

    public FilmeFormPageObjects ClickEditar(string titulo) {
        wait.Until(_ => driver.PageSource.Contains(titulo));

        var btn = Waits.Clickable(driver, By.CssSelector(".card a[title='Edição']"), 20);
        Clicks.SafeClick(driver, btn);

        wait.Until(ExpectedConditions.ElementIsVisible(FormAnchor));

        return new FilmeFormPageObjects(driver);
    }

    public FilmeFormPageObjects ClickExcluir(string titulo) {
        wait.Until(_ => driver.PageSource.Contains(titulo));

        var btn = Waits.Clickable(driver, By.CssSelector(".card a[title='Exclusão']"), 20);

        Clicks.SafeClick(driver, btn);

        return new FilmeFormPageObjects(driver);
    }

    public bool ContemFilme(string titulo) {
        wait.Until(_ => driver.FindElements(Cards).Count >= 0);

        return driver.PageSource.Contains(titulo);
    }
}
