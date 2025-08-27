using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;

namespace ControleDeCinema.Testes.Unidade_1_;

[TestClass]
[TestCategory("Testes de Unidade de Sessão")]
public sealed class SessaoTests
{
    private Filme? filme;
    
    private Sala? sala;

    private Sessao? sessao;

    private Ingresso? ingresso;

    [TestMethod]
    public void Deve_Gerar_Ingresso_Corretamente()
    {
        sessao = new Sessao(DateTime.Now.AddHours(1), 50, filme!, sala!);

        ingresso = sessao.GerarIngresso(10, true);

        Ingresso ingressoEsperado = new Ingresso(10, true, sessao);

        Assert.AreEqual(ingressoEsperado, ingresso);
    }

    [TestMethod]
    public void Deve_Obter_Assentos_Disponiveis_Corretamente()
    {
        sessao = new Sessao(DateTime.Now.AddHours(1), 5, filme!, sala!);
        
        sessao.GerarIngresso(1, false);
        sessao.GerarIngresso(2, true);
        sessao.GerarIngresso(3, false);

        int[] assentosDisponiveis = sessao.ObterAssentosDisponiveis();
        int[] assentosEsperados = new int[] { 4, 5 };

        CollectionAssert.AreEqual(assentosEsperados, assentosDisponiveis);
    }

    [TestMethod]
    public void Deve_Obter_Quantidade_Ingressos_Disponiveis_Corretamente()
    {
        sessao = new Sessao(DateTime.Now.AddHours(1), 5, filme!, sala!);

        sessao.GerarIngresso(1, false);
        sessao.GerarIngresso(2, true);
        sessao.GerarIngresso(3, false);

        int quantidadeIngressosDisponiveis = sessao.ObterQuantidadeIngressosDisponiveis();
        int quantidadeEsperada = 2;

        Assert.AreEqual(quantidadeEsperada, quantidadeIngressosDisponiveis);
    }
}
