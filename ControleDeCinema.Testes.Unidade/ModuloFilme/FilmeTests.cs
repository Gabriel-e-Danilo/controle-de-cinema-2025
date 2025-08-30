using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;

namespace ControleDeCinema.Testes.Unidade.ModuloFilme;

[TestClass]
[TestCategory("Testes de Unidade de Filme")]
public class FilmeTests
{
    private Filme? filme;
    private Sala? sala;

    [TestMethod]
    public void Deve_AdicionarSessao_AoFilme_Corretamente() {

        // Arrange
        var genero = new GeneroFilme("Suspense");

        filme = new Filme("Varmengo o Filme", 90, true, genero);

        var sessao = new Sessao(DateTime.Now, 10, filme, sala!);

        // Act
        filme.AdicionarSessao(sessao);

        // Assert
        var filmeContemSessao = filme.Sessoes.Contains(sessao);

        Assert.IsTrue(filmeContemSessao);
    }

    [TestMethod]
    public void Deve_RemoverSessao_DoFilme_Corretamente() {

        // Arrange
        var genero = new GeneroFilme("Suspense");

        filme = new Filme("SPFC Tricampeão Mundial", 90, true, genero);

        var sessao = new Sessao(DateTime.Now, 10, filme, sala!);

        // Act
        filme.RemoverSessao(sessao);

        // Assert
        var filmeNaoContemSessao = filme.Sessoes.Contains(sessao);

        Assert.IsFalse(filmeNaoContemSessao);
    }

    [TestMethod]
    public void AtualizarRegistro_DeveAtualizarTodosOsCampos() {

        // Arrange
        var generoOriginal = new GeneroFilme("Suspense");
        var filmeOriginal = new Filme("Varmengo o Filme", 90, true, generoOriginal);

        var generoEditado = new GeneroFilme("Terror");
        var filmeEditado = new Filme("SPFC Tricampeão Mundial", 100, false, generoEditado);

        // Act
        filmeOriginal.AtualizarRegistro(filmeEditado);

        // Assert
        Assert.AreEqual("SPFC Tricampeão Mundial", filmeOriginal.Titulo);
        Assert.AreEqual(100, filmeOriginal.Duracao);
        Assert.AreEqual(false, filmeOriginal.Lancamento);
        Assert.AreEqual(generoEditado, filmeOriginal.Genero);
    }
}