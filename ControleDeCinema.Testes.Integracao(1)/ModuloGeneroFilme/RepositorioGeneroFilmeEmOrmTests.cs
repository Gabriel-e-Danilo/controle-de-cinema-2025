using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Integração de Gênero de Filme")]
public sealed class RepositorioGeneroFilmeEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Genero_Corretamente() {

        // Arrange
        var genero = new GeneroFilme("Suspense");

        // Act
        repositorioGenero!.Cadastrar(genero);
        dbContext!.SaveChanges();

        // Assert
        var registroSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

        Assert.AreEqual(genero, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Genero_Corretamente() {

        // Arrange
        var genero = new GeneroFilme("Suspense");
        repositorioGenero!.Cadastrar(genero);
        dbContext!.SaveChanges();

        var generoEditado = new GeneroFilme("Terror");

        // Act
        var conseguiuEditar = repositorioGenero.Editar(genero.Id, generoEditado);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(genero, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Genero_Corretamente() {

        // Arrange
        var genero = new GeneroFilme("Matemática");
        repositorioGenero!.Cadastrar(genero);
        dbContext!.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioGenero.Excluir(genero.Id);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Generos_Corretamente() {

        // Arrange - Arranjo
        var genero1 = new GeneroFilme("Suspense");
        var genero2 = new GeneroFilme("Terror");
        var genero3 = new GeneroFilme("Fantasia");

        repositorioGenero!.Cadastrar(genero1);
        repositorioGenero!.Cadastrar(genero2);
        repositorioGenero!.Cadastrar(genero3);

        dbContext!.SaveChanges();

        List<GeneroFilme> generosEsperados = [genero1, genero2, genero3];

        var generosEsperadosOrdenados = generosEsperados
            .OrderBy(d => d.Descricao)
            .ToList();

        // Act - Ação
        var generosRecebidos = repositorioGenero.SelecionarRegistros();

        // Assert - Asseção
        CollectionAssert.AreEqual(generosEsperadosOrdenados, generosRecebidos);
    }

}
