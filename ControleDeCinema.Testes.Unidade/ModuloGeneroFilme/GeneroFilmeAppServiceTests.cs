using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Unidade de Gênero de Filme")]
public sealed class GeneroFilmeAppServiceTests
{
    private Mock<ITenantProvider>? tenantProviderMock;
    private Mock<IRepositorioGeneroFilme>? repositorioGeneroFilmeMock;
    private Mock<IUnitOfWork>? unitOfWorkMock;
    private Mock<ILogger<GeneroFilmeAppService>>? loggerMock;

    private GeneroFilmeAppService? generoAppService;

    [TestInitialize]
    public void Setup() {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioGeneroFilmeMock = new Mock<IRepositorioGeneroFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<GeneroFilmeAppService>>();

        generoAppService = new GeneroFilmeAppService(
            tenantProviderMock.Object,
            repositorioGeneroFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarOk_QuandoGeneroForValido() {

        // Arrange
        var genero = new GeneroFilme("Suspense");
        var generoTeste = new GeneroFilme("teste");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var result = generoAppService!.Cadastrar(genero);

        // Assert
        repositorioGeneroFilmeMock?.Verify(g => g.Cadastrar(genero), Times.Once());

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoGeneroForDuplicado() {

        // Arrange
        var genero = new GeneroFilme("Suspense");
        var generoTeste = new GeneroFilme("Suspense");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var result = generoAppService!.Cadastrar(genero);

        // Assert
        repositorioGeneroFilmeMock?.Verify(g => g.Cadastrar(It.IsAny<GeneroFilme>()), Times.Never());
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never());

        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoExcecaoForLancada() {
        
        // Arrange
        var genero = new GeneroFilme("Tes");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = generoAppService!.Cadastrar(genero);

        // Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(result);

        var mensagemErro = result.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarOk_QuandoGeneroForValido() {

        // Arrange
        var genero = new GeneroFilme("Suspense");
        var generoEditado = new GeneroFilme("Terror");
        var generoTeste = new GeneroFilme("teste");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var result = generoAppService!.Editar(genero.Id, generoEditado);

        // Assert
        repositorioGeneroFilmeMock?.Verify(g => g.Editar(genero.Id, generoEditado), Times.Once());

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoGeneroForDuplicado() {

        // Arrange
        var genero = new GeneroFilme("Suspense");
        var generoEditado = new GeneroFilme("Terror");
        var generoTeste = new GeneroFilme("Terror");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var result = generoAppService!.Editar(genero.Id, generoEditado);

        // Assert
        repositorioGeneroFilmeMock?.Verify(g => g.Editar(It.IsAny<Guid>(), It.IsAny<GeneroFilme>()), Times.Never());
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never());
        
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoExcecaoForLancada() {
        
        // Arrange
        var genero = new GeneroFilme("Tes");
        var generoEditado = new GeneroFilme("Teste Editado");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = generoAppService!.Editar(genero.Id, generoEditado);

        // Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(result);

        var mensagemErro = result.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Excluir_DeveRetornarOk_QuandoGeneroForValido() {

        // Arrange
        var genero = new GeneroFilme("Suspense");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        // Act
        var result = generoAppService!.Excluir(genero.Id);

        // Assert
        repositorioGeneroFilmeMock?.Verify(g => g.Excluir(genero.Id), Times.Once());
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Excluir_DeveRetornarFalha_QuandoExcecaoForLancada() {
        
        // Arrange
        var genero = new GeneroFilme("Teste");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = generoAppService!.Excluir(genero.Id);

        // Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(result);

        var mensagemErro = result.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarListaDeGeneros() {
        // Arrange
        var genero1 = new GeneroFilme("Suspense");
        var genero2 = new GeneroFilme("Terror");

        var generosSelecionados = new List<GeneroFilme>() { genero1, genero2 };

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(generosSelecionados);

        // Act
        var resultado = generoAppService!.SelecionarTodos();

        // Assert
        Assert.IsNotNull(resultado);
        CollectionAssert.AreEqual(generosSelecionados, resultado.Value);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        var genero1 = new GeneroFilme("Suspense");
        var genero2 = new GeneroFilme("Terror");

        var generosSelecionados = new List<GeneroFilme>() { genero1, genero2 };

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = generoAppService!.SelecionarTodos();

        // Assert
        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarGenero_QuandoIdForValido() {
        // Arrange
        var genero = new GeneroFilme("Suspense");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        // Act
        var resultado = generoAppService!.SelecionarPorId(genero.Id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(genero, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFalha_QuandoExcecaoForLancada() {
        
        // Arrange
        var genero = new GeneroFilme("Teste");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = generoAppService!.SelecionarPorId(genero.Id);

        // Assert
        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFalha_QuandoIdForInvalido() {

        // Arrange
        var generoId = Guid.NewGuid();

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(generoId))
            .Returns((GeneroFilme?)null);

        // Act
        var resultado = generoAppService!.SelecionarPorId(generoId);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }
}
