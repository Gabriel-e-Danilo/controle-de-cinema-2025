using Castle.Core.Logging;
using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
public sealed class SessaoAppServiceTests
{
    private Mock<IRepositorioFilme>? mockRepositorioFilme;
    private Mock<IRepositorioGeneroFilme>? mockRepositorioGenero;
    private Mock<IRepositorioSala>? mockRepositorioSala;
    private Mock<IRepositorioSessao>? mockRepositorioSessao;
    private Mock<IUnitOfWork>? mockUnitOfWork;
    private Mock<ILogger<SessaoAppService>>? mockLogger;  
    private Mock<ITenantProvider>? mockTenantProvider;

    private SessaoAppService? sessaoAppService;

    [TestInitialize]
    public void Setup()
    {
        mockRepositorioFilme = new Mock<IRepositorioFilme>();
        mockRepositorioGenero = new Mock<IRepositorioGeneroFilme>();
        mockRepositorioSala = new Mock<IRepositorioSala>();
        mockRepositorioSessao = new Mock<IRepositorioSessao>();
        mockUnitOfWork = new Mock<IUnitOfWork>();
        mockLogger = new Mock<ILogger<SessaoAppService>>();
      
        sessaoAppService = new SessaoAppService(
            mockTenantProvider!.Object,
            mockRepositorioSessao!.Object,
            mockUnitOfWork!.Object,
            mockLogger.Object
        );
    }

    [TestMethod]
    public void Deve_Cadastrar_Sessao_Valida()
    {
        var sessao = new Sessao(DateTime.Now.AddHours(1), 50, new Filme("Filme 2, Eletric Bogaloo", 120, true, new GeneroFilme("Ação")), new Sala(1, 100));

        var sessaoTeste = new Sessao(DateTime.Now.AddHours(1), 50, new Filme("Filme Teste", 120, true, new GeneroFilme("Ação")), new Sala(1, 100));


        mockRepositorioSessao?
            .Setup(s => s.SelecionarRegistros())
            .Returns(new List<Sessao>() { sessaoTeste })
            ;

        var resultado = sessaoAppService?.Cadastrar(sessao);

        mockRepositorioSessao?.Verify(r => r.Cadastrar(sessao), Times.Once);

        mockUnitOfWork?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

}
