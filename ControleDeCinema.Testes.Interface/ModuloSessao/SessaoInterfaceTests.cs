using ControleDeCinema.Testes.Interface.ModuloFilme;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using ControleDeCinema.Testes.Interface.ModuloSala;
using ControleDeCinema.Testes.Interface.ModuloSessao;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
public sealed class SessaoInterfaceTests : TestFixture
{
    [TestInitialize]
    public void TestInitialize()
    {
        RegistrarOuLogar();
    }


    [TestMethod]
    public void Deve_Cadastrar_Sessao()
    {
        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        
        var filmeIndex = new FilmeIndexPageObjects(driver!)
            .IrPara(enderecoBase!);

        filmeIndex
            .ClickCadastrar()
            .PreencherTitulo("Teste")
            .PreencherDuracao(100)
            .PreencherLancamento(true)
            .PreencherGenero("Suspense")
            .Confirmar();


        var salaIndex = new SalaIndexPageObjects(driver!);

        salaIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();

       
        
        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase);
            
        sessaoIndex
            .ClickCadastrar()
            .PreencherCampoIngressos(100)
            .PreencherCampoDataHora(DateTime.Now.AddHours(1))
            .SelecionarFilme("Teste")
            .SelecionarSala("1")
            .Confirmar();

        Assert.IsTrue(sessaoIndex.ContemSessao("Filme 1"));

    }


    [TestMethod]
    public void Deve_Editar_Sessao()
    {

        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();


        var filmeIndex = new FilmeIndexPageObjects(driver!)
            .IrPara(enderecoBase!);

        filmeIndex
            .ClickCadastrar()
            .PreencherTitulo("Teste")
            .PreencherDuracao(100)
            .PreencherLancamento(true)
            .PreencherGenero("Suspense")
            .Confirmar();

        filmeIndex
            .ClickCadastrar()
            .PreencherTitulo("Teste 2")
            .PreencherDuracao(120)
            .PreencherLancamento(false)
            .PreencherGenero("Suspense")
            .Confirmar();


        var salaIndex = new SalaIndexPageObjects(driver!);

        salaIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();


        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase);

        sessaoIndex
            .ClickCadastrar()
            .PreencherCampoIngressos(100)
            .PreencherCampoDataHora(DateTime.Now.AddHours(1))
            .SelecionarFilme("Teste")
            .SelecionarSala("1")
            .Confirmar();

        sessaoIndex
            .ClickEditar()
            .PreencherCampoIngressos(150)
            .PreencherCampoDataHora(DateTime.Now.AddHours(2))
            .SelecionarFilme("Teste 2")
            .SelecionarSala("1")
            .Confirmar();

        Assert.IsTrue(sessaoIndex.ContemSessao("Teste 2"));

    }

    [TestMethod]
    public void Deve_Excluir_Sessao()
    {
        var generoIndex = new GeneroFilmeIndexPageObjects(driver!);

        generoIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();


        var filmeIndex = new FilmeIndexPageObjects(driver!)
            .IrPara(enderecoBase!);

        filmeIndex
            .ClickCadastrar()
            .PreencherTitulo("Teste")
            .PreencherDuracao(100)
            .PreencherLancamento(true)
            .PreencherGenero("Suspense")
            .Confirmar();


        var salaIndex = new SalaIndexPageObjects(driver!);

        salaIndex
            .IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();



        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase);

        sessaoIndex
            .ClickCadastrar()
            .PreencherCampoIngressos(100)
            .PreencherCampoDataHora(DateTime.Now.AddHours(1))
            .SelecionarFilme("Teste")
            .SelecionarSala("1")
            .Confirmar();

        sessaoIndex
            .ClickExcluir();
            

        Assert.IsFalse(sessaoIndex.ContemSessao("Teste"));
    }
}
