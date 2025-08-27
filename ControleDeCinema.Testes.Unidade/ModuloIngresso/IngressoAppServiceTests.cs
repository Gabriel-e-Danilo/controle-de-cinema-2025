using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.Compartilhado;
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
public class IngressoAppServiceTests
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
    public void Deve_Comprar_Ingresso_Corretamente()
    {
        var sessao = new Sessao(DateTime.Now.AddHours(1), 50, new Filme("Filme 2, Eletric Bogaloo", 120, true, new GeneroFilme("Ação")), new Sala(1, 100));
        
        var ingresso = sessao.GerarIngresso(10, true);

        Assert.IsNotNull(ingresso);
        Assert.AreEqual(10, ingresso.NumeroAssento);
        Assert.IsTrue(ingresso.MeiaEntrada);
        Assert.AreEqual(sessao, ingresso.Sessao);
    }

    [TestMethod]
    public void Nao_Deve_Comprar_Ingresso_Para_Sessao_Lotada()
    {
        var sessao = new Sessao(DateTime.Now.AddHours(1), 1, new Filme("Filme 2, Eletric Bogaloo", 120, true, new GeneroFilme("Ação")), new Sala(1, 100));

        var ingresso = sessao.GerarIngresso(10, true);
        Assert.IsNotNull(ingresso);

        
        var ingresso2 = sessao.GerarIngresso(11, true);

        Assert.IsNull(ingresso2);
    }
}
