using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

public class GeneroFilmeIndexPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroFilmeIndexPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public GeneroFilmeIndexPageObjects IrPara(string enderecoBase) {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "generos"));

        return this;
    }

    public GeneroFilmeFormPageObjects ClickCadastrar() {
        var btn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-se='btnCadastrar']")));
            btn.Click();

        return new GeneroFilmeFormPageObjects(driver);
    }

    public GeneroFilmeFormPageObjects ClickEditar(string descricao) {
        var btn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".card a[title='Edição']")));
        btn.Click();

        return new GeneroFilmeFormPageObjects(driver);
    }

    public GeneroFilmeFormPageObjects ClickExcluir(string descricao) {
        var btn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".card a[title='Exclusão']")));
        btn.Click();

        return new GeneroFilmeFormPageObjects(driver);
    }

    public bool ContemGenero(string descricao) {        
        return driver.PageSource.Contains(descricao);
    }
}
