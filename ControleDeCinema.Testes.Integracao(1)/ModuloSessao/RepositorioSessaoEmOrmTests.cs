using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao_1_;

[TestClass]
[TestCategory("Testes de Integração de Sessão")]
public sealed class RepositorioSessaoEmOrmTest : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
    {
        var filme = Builder<Filme>.CreateNew().Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.Now.AddHours(1), 50, filme, sala);

        repositorioSessao?.Cadastrar(sessao);
        dbContext?.SaveChanges();

        var registroSelecionado = repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Sessao_Corretamente()
    {
        var filme = Builder<Filme>.CreateNew().Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.Now.AddHours(1), 50, filme, sala);

        var filme2 = Builder<Filme>.CreateNew().Persist();

        var sessaoEditada = new Sessao(DateTime.Now.AddHours(2), 30, filme2, sala);

        var conseguiuEditar = repositorioSessao!.Editar(sessao.Id, sessaoEditada);
        dbContext!.SaveChanges();

        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(sessao, registroSelecionado);

    }

    [TestMethod]
    public void Deve_Excluir_Sessao_Corretamente()
    {
        var filme = Builder<Filme>.CreateNew().Persist();
        var sala = Builder<Sala>.CreateNew().Persist();
        var sessao = new Sessao(DateTime.Now.AddHours(1), 50, filme, sala);

        repositorioSessao!.Cadastrar(sessao);
        dbContext!.SaveChanges();

        var conseguiuExcluir = repositorioSessao.Excluir(sessao.Id);
        dbContext.SaveChanges();

        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);
        
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_Sessoes_Corretamente()
    {
        var filme = Builder<Filme>.CreateNew().Persist();
        var sala = Builder<Sala>.CreateNew().Persist();
        var sessao = new Sessao(DateTime.Now.AddHours(1), 50, filme, sala);
        var sessao2 = new Sessao(DateTime.Now.AddHours(10), 50, filme, sala);
        var sessao3 = new Sessao(DateTime.Now.AddHours(5), 50, filme, sala);

        List<Sessao> sessoes = [sessao, sessao2, sessao3];

        repositorioSessao!.CadastrarEntidades(sessoes);
        dbContext!.SaveChanges();

        var sessoesSelecionadas = sessoes.OrderBy(x => x.Inicio).ToList();

        var sessoesRecebidas = repositorioSessao.SelecionarRegistros();

        CollectionAssert.AreEqual(sessoesSelecionadas, sessoesRecebidas);
    }
}
