using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloFilme;

[TestClass]
[TestCategory("Testes de Unidade de Filme")]
public sealed class FilmeAppServiceTests
{
    private Mock<ITenantProvider>? tenantProviderMock;
    private Mock<IRepositorioFilme>? repositorioFilmeMock;
    private Mock<IUnitOfWork>? unitOfWorkMock;
    private Mock<ILogger<FilmeAppService>>? loggerMock;

    private FilmeAppService? filmeAppService;

    [TestInitialize]
    public void Setup() {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioFilmeMock = new Mock<IRepositorioFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<FilmeAppService>>();

        filmeAppService = new FilmeAppService(
            tenantProviderMock.Object,
            repositorioFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_Deve_RetornarOk_QuandoFilmeForValido() {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);
        var filmeTeste = new Filme("B", 90, false, genero);

        repositorioFilmeMock!.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeTeste });

        // Act
        var resultado = filmeAppService!.Cadastrar(filme);

        // Assert
        repositorioFilmeMock!.Verify(f => f.Cadastrar(filme), Times.Once());

        unitOfWorkMock!.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Deve_RetornarFalha_QuandoFilmeForDuplicado() {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);
        var filmeTeste = new Filme("A", 90, false, genero);

        repositorioFilmeMock!.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeTeste });

        // Act
        var resultado = filmeAppService!.Cadastrar(filme);

        // Assert
        repositorioFilmeMock!.Verify(f => f.Cadastrar(It.IsAny<Filme>()), Times.Never());

        unitOfWorkMock!.Verify(u => u.Commit(), Times.Never());

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoExcecaoForLancada() {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);
        var filmeTeste = new Filme("B", 90, false, genero);

        repositorioFilmeMock!.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeTeste });

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = filmeAppService!.Cadastrar(filme);

        // Assert        
        unitOfWorkMock!.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarOk_QuandoFilmeForValido() {

        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);
        var filmeEditado = new Filme("B", 150, true, genero);
        var filmeTeste = new Filme("C", 90, false, genero);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeTeste });

        // Act
        var resultado = filmeAppService!.Editar(filme.Id, filmeEditado);

        // Assert
        repositorioFilmeMock!.Verify(f => f.Editar(filme.Id, filmeEditado), Times.Once());

        unitOfWorkMock!.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoFilmeForDuplicado() {

        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);
        var filmeEditado = new Filme("B", 150, true, genero);
        var filmeTeste = new Filme("B", 90, false, genero);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeTeste });

        // Act
        var resultado = filmeAppService!.Editar(filme.Id, filmeEditado);

        // Assert
        repositorioFilmeMock!.Verify(f => f.Editar(It.IsAny<Guid>(), It.IsAny<Filme>()), Times.Never());

        unitOfWorkMock!.Verify(u => u.Commit(), Times.Never());

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);
        var filmeEditado = new Filme("B", 150, true, genero);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = filmeAppService!.Editar(filme.Id, filmeEditado);

        // Assert        
        unitOfWorkMock!.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_DeveRetornarOk_QuandoFilmeForValido() {

        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        // Act
        var resultado = filmeAppService!.Excluir(filme.Id);

        // Assert
        repositorioFilmeMock!.Verify(f => f.Excluir(filme.Id), Times.Once());

        unitOfWorkMock!.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = filmeAppService!.Excluir(filme.Id);

        // Assert        
        unitOfWorkMock!.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarListaDeFilmes() {

        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme1 = new Filme("A", 120, true, genero);
        var filme2 = new Filme("B", 150, true, genero);

        var filmesSelecionados = new List<Filme> { filme1, filme2 };

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistros())
            .Returns(filmesSelecionados);

        // Act
        var resultado = filmeAppService!.SelecionarTodos();

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(filmesSelecionados, resultado.Value);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = filmeAppService!.SelecionarTodos();

        // Assert
        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }   

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFilme_QuandoIdForValido() {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("A", 120, true, genero);

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        // Act
        var resultado = filmeAppService!.SelecionarPorId(filme.Id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(filme, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFalha_QuandoExcecaoForLancada() {
        // Arrange
        var filmeId = Guid.NewGuid();

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filmeId))
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = filmeAppService!.SelecionarPorId(filmeId);

        // Assert
        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFalha_QuandoIdForInvalido() {

        // Arrange
        var filmeId = Guid.NewGuid();

        repositorioFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(filmeId))
            .Returns((Filme?)null);

        // Act
        var resultado = filmeAppService!.SelecionarPorId(filmeId);

        // Assert
        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }
}


