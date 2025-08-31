using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

public class GeneroFilmeFormPageObjects {

    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroFilmeFormPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public GeneroFilmeFormPageObjects PreencherDescricao(string descricao) {
        var inputNome = driver.FindElement(By.Id("Descricao"));

        inputNome.Clear();
        inputNome.SendKeys(descricao);

        return this;
    }

    public GeneroFilmeIndexPageObjects Confirmar() {
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        wait.Until(d => d.FindElement(By.CssSelector(".card]")).Displayed);

        return new GeneroFilmeIndexPageObjects(driver);
    }

    public GeneroFilmeIndexPageObjects ConfirmarExclusao() {
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        wait.Until(d => d.FindElement(By.CssSelector(".card]")).Displayed);

        return new GeneroFilmeIndexPageObjects(driver);
    }
}
