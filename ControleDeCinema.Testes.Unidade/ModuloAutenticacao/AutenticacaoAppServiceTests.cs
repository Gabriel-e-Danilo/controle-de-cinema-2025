using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
public sealed class AutenticacaoAppServiceTests
{
    private Mock<UserManager<Usuario>>? mockUserManager;
    private Mock<SignInManager<Usuario>>? mockSignInManager;
    private Mock<RoleManager<Cargo>>? mockRoleManager;

    private AutenticacaoAppService? autenticacaoAppService;

    [TestInitialize]
    public void Setup()
    {
        mockUserManager = new Mock<UserManager<Usuario>>();
        mockSignInManager = new Mock<SignInManager<Usuario>>();
        mockRoleManager = new Mock<RoleManager<Cargo>>();

        autenticacaoAppService = new AutenticacaoAppService(
            mockUserManager.Object,
            mockSignInManager.Object,
            mockRoleManager.Object
        );

    }

    [TestMethod]
    public void DeveCadastrarEmpresaComSucesso()
    {
        var empresa = new Usuario
        {
            UserName = "empresa_teste",
            Email = "teste@gmail.com"
        };

        autenticacaoAppService!.RegistrarAsync(empresa, "Senha@123", TipoUsuario.Empresa).Wait();

        Assert.IsTrue(empresa.UserName.Equals("empresa_teste"));
    }

    [TestMethod]
    public void DeveCadastrarClienteComSucesso()
    {
        var cliente = new Usuario
        {
            UserName = "cliente_teste",
            Email = "cliente@gmail.com"
        };

        autenticacaoAppService!.RegistrarAsync(cliente, "Senha@123", TipoUsuario.Cliente).Wait();
        
        Assert.IsTrue(cliente.UserName.Equals("cliente_teste"));
    }
}
