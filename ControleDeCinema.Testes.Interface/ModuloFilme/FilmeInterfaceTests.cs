using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

[TestClass]
[TestCategory("Testes de Interface de Filme")]
public class FilmeInterfaceTests : TestFixture
{    
    [TestInitialize]
    public void TestInitialize() {
        RegistrarOuLogar();
    }

    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente() {

        // Arrange
        var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        // Act
        var filmeIndex = new FilmeIndexPageObjects(driver!)
            .IrPara(enderecoBase);

        filmeIndex
            .ClickCadastrar()
            .PreencherTitulo("Teste")
            .PreencherDuracao(100)
            .PreencherLancamento(true)
            .PreencherGenero("Suspense")
            .Confirmar();

        // Assert
        Assert.IsTrue(filmeIndex.ContemFilme("Teste"));
    }
}
