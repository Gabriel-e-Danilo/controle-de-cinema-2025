using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

[TestClass]
[TestCategory("Testes de Interface de Sala")]
public class SalaInterfaceTests : TestFixture
{
    [TestInitialize]
    public void TestInitialize() {
        RegistrarOuLogar();
    }

    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente() {

        // Arrange
        var salaIndex = new SalaIndexPageObjects(driver!);

        salaIndex
            .IrPara(enderecoBase!);

        // Act
        salaIndex
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();

        // Assert
        Assert.IsTrue(salaIndex.ContemSala(1.ToString()));
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente() {

        // Arrange
        var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

        var salaIndex = new SalaIndexPageObjects(driver!);

        salaIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();

        // Act
        salaIndex
            .ClickEditar(1)
            .PreencherNumero(2)
            .PreencherCapacidade(150)
            .Confirmar();

        // Assert
        Assert.IsTrue(salaIndex.ContemSala(2.ToString()));
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente() {

        // Arrange
        var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(30));

        var salaIndex = new SalaIndexPageObjects(driver!);

        salaIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();

        var cardDaSala1 = driver!.FindElements(By.CssSelector(".card"))
                .FirstOrDefault(c => c.Text.Contains("1"));

        // Act
        salaIndex
            .ClickExcluir(1)
            .ConfirmarExclusao();

        if (cardDaSala1 is not null) {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.StalenessOf(cardDaSala1));
        }

        // Assert
        Waits.Eventually(() =>
        {
            Assert.IsFalse(salaIndex.ContemSalaNumero(1));
        }, tentativas: 10, intervaloMs: 200);
    }
}
