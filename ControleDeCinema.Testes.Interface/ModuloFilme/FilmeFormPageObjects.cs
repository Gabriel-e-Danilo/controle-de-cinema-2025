using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

public class FilmeFormPageObjects
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public FilmeFormPageObjects(IWebDriver driver) {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public FilmeFormPageObjects PreencherTitulo(string titulo) {
        wait.Until(d => 
            d.FindElement(By.Id("Titulo")).Displayed &&
            d.FindElement(By.Id("Titulo")).Enabled
        );

        var nomeInput = driver.FindElement(By.Id("Titulo"));
        nomeInput.Clear();
        nomeInput.SendKeys(titulo);

        return this;
    }

    public FilmeFormPageObjects PreencherDuracao(int duracao) {
        wait.Until(d =>
            d.FindElement(By.Id("Duracao")).Displayed &&
            d.FindElement(By.Id("Duracao")).Enabled
        );

        var duracaoInput = driver.FindElement(By.Id("Duracao"));
        duracaoInput.Clear();
        duracaoInput.SendKeys(duracao.ToString());

        return this;
    }

    public FilmeFormPageObjects PreencherLancamento(bool lancamento) {
        wait.Until(d =>
            d.FindElement(By.Id("Lancamento")).Displayed &&
            d.FindElement(By.Id("Lancamento")).Enabled
        );

        var lancamentoInput = driver.FindElement(By.Id("Lancamento"));
        
        if (lancamentoInput.Selected != lancamento) {
            lancamentoInput.Click();
        }

        return this;
    }

    public FilmeFormPageObjects PreencherGenero(string genero) {
        wait.Until(d =>
            d.FindElement(By.Id("GeneroId")).Displayed &&
            d.FindElement(By.Id("GeneroId")).Enabled
        );

        var generoInput = driver.FindElement(By.Id("GeneroId"));
        generoInput.Clear();
        generoInput.SendKeys(genero);

        return this;
    }

    public FilmeIndexPageObjects Confirmar() {
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        wait.Until(d => d.FindElement(By.CssSelector(".card")).Displayed);
        return new FilmeIndexPageObjects(driver);
    }

}
