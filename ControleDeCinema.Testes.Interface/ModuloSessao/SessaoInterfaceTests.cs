using ControleDeCinema.Testes.Interface.ModuloSessao;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
public sealed class SessaoInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sessao()
    {
        // logica de criação do filme e da sala

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase);
            
        sessaoIndex
            .ClickCadastrar()
            .PreencherCampoIngressos(100)
            .PreencherCampoDataHora(DateTime.Now.AddHours(1))
            .SelecionarFilme("Filme 1")
            .SelecionarSala("Sala 1")
            .Confirmar();

        Assert.IsTrue(sessaoIndex.ContemSessao("Filme 1"));

    }


    [TestMethod]
    public void Deve_Editar_Sessao()
    {
        // logica de criação do filme e da sala

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase);

        sessaoIndex
            .ClickCadastrar()
            .PreencherCampoIngressos(100)
            .PreencherCampoDataHora(DateTime.Now.AddHours(1))
            .SelecionarFilme("Filme 1")
            .SelecionarSala("Sala 1")
            .Confirmar();

        sessaoIndex
            .ClickEditar()
            .PreencherCampoIngressos(150)
            .PreencherCampoDataHora(DateTime.Now.AddHours(2))
            .SelecionarFilme("Filme 2")
            .SelecionarSala("Sala 1")
            .Confirmar();

        Assert.IsTrue(sessaoIndex.ContemSessao("Filme 2"));

    }

    [TestMethod]
    public void Deve_Excluir_Sessao()
    {
        // logica de criação do filme e da sala
        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase);
        
        
        sessaoIndex
            .ClickCadastrar()
            .PreencherCampoIngressos(100)
            .PreencherCampoDataHora(DateTime.Now.AddHours(1))
            .SelecionarFilme("Filme 1")
            .SelecionarSala("Sala 1")
            .Confirmar();
        
        sessaoIndex
            .ClickExcluir()
            .Confirmar();

        Assert.IsFalse(sessaoIndex.ContemSessao("Filme 1"));
    }
}
