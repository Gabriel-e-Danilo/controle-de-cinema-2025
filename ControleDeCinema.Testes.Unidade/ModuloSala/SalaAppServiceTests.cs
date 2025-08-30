using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSala;

[TestClass]
[TestCategory("Testes de Unidade de Sala")]
public sealed class SalaAppServiceTests
{
    private Mock<ITenantProvider>? tenantProviderMock;
    private Mock<IRepositorioSala>? repositorioSalaMock;
    private Mock<IUnitOfWork>? unitOfWorkMock;
    private Mock<ILogger<SalaAppService>>? loggerMock;

    private SalaAppService? salaAppService;

    [TestInitialize]
    public void Setup() {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSalaMock = new Mock<IRepositorioSala>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SalaAppService>>();

        salaAppService = new SalaAppService(
            tenantProviderMock.Object,
            repositorioSalaMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarOk_QuandoSalaForValida() {

        // Arrange
        var sala = new Sala(1, 100);
        var salaTeste = new Sala(2, 200);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { salaTeste });

        // Act
        var result = salaAppService!.Cadastrar(sala);

        // Assert
        repositorioSalaMock?.Verify(g => g.Cadastrar(sala), Times.Once());

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoNumeroDuplicado() {

        // Arrange
        var sala = new Sala(1, 100);
        var salaTeste = new Sala(1, 200);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { salaTeste });

        // Act
        var result = salaAppService!.Cadastrar(sala);

        // Assert
        repositorioSalaMock?.Verify(g => g.Cadastrar(It.IsAny<Sala>()), Times.Never());
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never());

        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        var sala = new Sala(1, 100);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = salaAppService!.Cadastrar(sala);

        // Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(result);

        var mensagemErro = result.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarOk_QuandoSalaForValida() {

        // Arrange
        var sala = new Sala(1, 100);
        var salaEditada = new Sala(2, 200);
        var salaTeste = new Sala(3, 300);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { salaTeste });

        // Act
        var result = salaAppService!.Editar(sala.Id, salaEditada);

        // Assert
        repositorioSalaMock?.Verify(g => g.Editar(sala.Id, salaEditada), Times.Once());

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoNumeroDuplicado() {

        // Arrange
        var sala = new Sala(1, 100);
        var salaEditada = new Sala(2, 200);
        var salaTeste = new Sala(2, 200);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { salaTeste });

        // Act
        var result = salaAppService!.Editar(sala.Id, salaEditada);

        // Assert
        repositorioSalaMock?.Verify(g => g.Editar(It.IsAny<Guid>(), It.IsAny<Sala>()), Times.Never());
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never());

        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        var sala = new Sala(1, 100);
        var salaEditada = new Sala(2, 200);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = salaAppService!.Editar(sala.Id, salaEditada);

        // Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(result);

        var mensagemErro = result.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void Excluir_DeveRetornarOk_QuandoSalaForValida() {

        // Arrange
        var sala = new Sala(1, 100);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        // Act
        var result = salaAppService!.Excluir(sala.Id);

        // Assert
        repositorioSalaMock?.Verify(g => g.Excluir(sala.Id), Times.Once());
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once());

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Excluir_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        var sala = new Sala(1, 100);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = salaAppService!.Excluir(sala.Id);

        // Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once());

        Assert.IsNotNull(result);

        var mensagemErro = result.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarListaDeSalas() {
        // Arrange
        var sala1 = new Sala(1, 100);
        var sala2 = new Sala(2, 200);

        var salasSelecionadas = new List<Sala> { sala1, sala2 };

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(salasSelecionadas);

        // Act
        var resultado = salaAppService!.SelecionarTodos();

        // Assert
        Assert.IsNotNull(resultado);
        CollectionAssert.AreEqual(salasSelecionadas, resultado.Value);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarFalha_QuandoExcecaoForLancada() {

        // Arrange
        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = salaAppService!.SelecionarTodos();

        // Assert
        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarSala_QuandoIdForValido() {
        // Arrange
        var sala = new Sala(1, 100);

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        // Act
        var resultado = salaAppService!.SelecionarPorId(sala.Id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(sala, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFalha_QuandoIdForInvalido() {

        // Arrange
        var salaId = Guid.NewGuid();

        repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(salaId))
            .Returns((Sala?)null);

        // Act
        var resultado = salaAppService!.SelecionarPorId(salaId);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }
}
