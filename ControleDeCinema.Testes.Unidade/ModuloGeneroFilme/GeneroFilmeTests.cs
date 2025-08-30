using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Unidade.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Unidade de Gênero de Filme")]
public class GeneroFilmeTests
{
    private GeneroFilme? generoFilme;

    [TestMethod]
    public void Deve_AdicionarFilme_AoGenero_Corretamente() {

        // Arrange
        generoFilme = new GeneroFilme("Suspense");

        var filme = new Filme("Varmengo o Filme", 90, true, generoFilme);

        // Act
        generoFilme.AdicionarFilme(filme);

        // Assert
        var generoContemFilme = generoFilme.Filmes.Contains(filme);

        Assert.IsTrue(generoContemFilme);
    }

    [TestMethod]
    public void Deve_RemoverFilme_DoGenero_Corretamente() {

        // Arrange
        generoFilme = new GeneroFilme("Suspense");

        var filme = new Filme("SPFC Tricampeão Mundial", 90, true, generoFilme);

        // Act
        generoFilme.RemoverFilme(filme);

        // Assert
        var generoNaoContemFilme = generoFilme.Filmes.Contains(filme);

        Assert.IsFalse(generoNaoContemFilme);
    }

    [TestMethod]
    public void AtualizarRegistro_DeveAtualizarDescricao() {

        // Arrange
        var generoOriginal = new GeneroFilme("Suspense");
        var generoEditado = new GeneroFilme("Terror");

        // Act
        generoOriginal.AtualizarRegistro(generoEditado);

        // Assert
        Assert.AreEqual("Terror", generoOriginal.Descricao);
    }
}
