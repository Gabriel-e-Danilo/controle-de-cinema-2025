using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;
public class FilmeIndexPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public FilmeIndexPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public FilmeIndexPageObjects IrPara(string enderecoBase) {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "filmes"));

        return this;
    }

    public FilmeFormPageObjects ClickCadastrar() {
        driver.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Click();

        return new FilmeFormPageObjects(driver);
    }

    public FilmeFormPageObjects ClickEditar(string descricao) {
        driver.FindElement(By.CssSelector(".card a[title='Edição']")).Click();

        return new FilmeFormPageObjects(driver);
    }

    public FilmeFormPageObjects ClickExcluir(string descricao) {
        driver.FindElement(By.CssSelector(".card a[title='Exclusão']")).Click();

        return new FilmeFormPageObjects(driver);
    }

    public bool ContemFilme(string descricao) {
        return driver.PageSource.Contains(descricao);
    }
}
