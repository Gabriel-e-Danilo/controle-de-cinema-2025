using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Interface de Gênero de Filme")]
public sealed class GeneroFilmeInterfaceTests : TestFixture
{
    [TestInitialize]
    public void TestInitialize() {
        RegistrarOuLogar();
    }

    [TestMethod]
    public void Deve_Cadastrar_Genero_Corretamente() {

        // Arrange
        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase!);

        // Act
        generoIndex
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        // Assert
        Assert.IsTrue(generoIndex.ContemGenero("Suspense"));
    }

    [TestMethod]
    public void Deve_Editar_Genero_Corretamente() {

        // Arrange
        var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        // Act
        generoIndex
            .ClickEditar("Suspense")
            .PreencherDescricao("Suspense Editado")
            .Confirmar();

        // Assert
        Assert.IsTrue(generoIndex.ContemGenero("Suspense Editado"));
    }

    [TestMethod]
    public void Deve_Excluir_Genero_Corretamente() {

        // Arrange
        var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        // Act
        generoIndex
            .ClickExcluir("Suspense")
            .ConfirmarExclusao();

        // Assert
        Assert.IsFalse(generoIndex.ContemGenero("Suspense"));
    }
}