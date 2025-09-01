using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ControleDeCinema.Testes.Interface.ModuloSala;
public class SalaIndexPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private static readonly By BtnCadastrar = By.CssSelector("a[data-se='btnCadastrar']");
    private static readonly By BtnEditar = By.CssSelector("a[data-se='btnEditar']");
    private static readonly By BtnExcluir = By.CssSelector("a[data-se='btnExcluir']");
    private static readonly By Cards = By.CssSelector(".card");
    private static readonly By ListaSalas = By.CssSelector("[data-se='lista-salas']");

    public SalaIndexPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public SalaIndexPageObjects IrPara(string enderecoBase) {
        var url = $"{enderecoBase.TrimEnd('/')}/salas";
        driver.Navigate().GoToUrl(url);

        wait.Until(d => ((IJavaScriptExecutor)d)
            .ExecuteScript("return document.readyState")?.ToString() == "complete");

        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(ListaSalas));

        return this;
    }

    public SalaFormPageObjects ClickCadastrar() {
        var btn = Waits.Clickable(driver, BtnCadastrar);

        Clicks.SafeClick(driver, btn);

        return new SalaFormPageObjects(driver);
    }

    public SalaFormPageObjects ClickEditar(int numero) {
        wait.Until(d => d.PageSource.Contains(numero.ToString()));

        var btn = Waits.Clickable(driver, BtnEditar);

        Clicks.SafeClick(driver, btn);

        return new SalaFormPageObjects(driver);
    }

    public SalaFormPageObjects ClickExcluir(int numero) {
        var card = wait.Until(d =>
            d.FindElements(Cards).FirstOrDefault(c =>
                c.FindElements(By.CssSelector("[data-se='numero-sala']"))
                 .Any(h => h.Text.Trim().Equals($"# {numero}", StringComparison.OrdinalIgnoreCase))
            )
        ) ?? throw new NoSuchElementException($"Card da sala {numero} não encontrado.");
        
        var btn = card.FindElement(By.CssSelector("a[data-se='btnExcluir']"));
        Clicks.SafeClick(driver, btn);
        
        return new SalaFormPageObjects(driver);
    }

    public bool ContemSala(string descricao) {
        // espere voltar para a tela de index
        wait.Until(d => d.FindElements(Cards).Count >= 0);

        return driver.PageSource.Contains(descricao);
    }

    public bool ContemSalaNumero(int numero) {
        wait.Until(ExpectedConditions.ElementExists(ListaSalas));

        var titulos = driver.FindElements(By.CssSelector(".card .card-title[data-se='numero-sala']"));

        return titulos.Any(h =>
            h.Text.Trim().Equals($"# {numero}", StringComparison.OrdinalIgnoreCase));
    }


}
