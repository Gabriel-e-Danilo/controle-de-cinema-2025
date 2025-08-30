using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Interface de Gênero de Filme")]
public sealed class GeneroFilmeInterfaceTests : TestFixture
{
    [TestMethod]
    public void Test() {
        driver?.Navigate().GoToUrl("https://localhost:7131/generos");

        var elemento = driver?.FindElement(By.CssSelector("a[data-se='btnCadastrar']"));

        elemento?.Click();
    }
}
