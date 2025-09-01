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
        var inputNome = wait.Until(d => {
            var el = d.FindElement(By.Id("Descricao"));
            return (el != null && el.Displayed) ? el : null;
        });

        inputNome.Clear();
        inputNome.SendKeys(descricao);

        return this;
    }

    public GeneroFilmeIndexPageObjects Confirmar() {
        // Aguarda até o botão estar presente e clicável
        var botao = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("botaoConfirmar")));
        botao.Click(); 

        return new GeneroFilmeIndexPageObjects(driver);
    }

    public GeneroFilmeIndexPageObjects ConfirmarExclusao() {
        // Aguarda até o botão estar presente e clicável
        var botao = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("botaoConfirmar")));
        botao.Click();

        return new GeneroFilmeIndexPageObjects(driver);
    }
}
