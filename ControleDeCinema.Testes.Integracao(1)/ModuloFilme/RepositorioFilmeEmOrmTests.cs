using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao_1_.ModuloFilme;

[TestClass]
[TestCategory("Testes de Integração de Filme")]
public sealed class RepositorioFilmeEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente() {

        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("As longas tranças de um careca", 80, true, genero);

        // Act
        repositorioFilme!.Cadastrar(filme);
        dbContext!.SaveChanges();

        // Assert
        var registroSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.AreEqual(filme, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente() {

        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("Poeira em alto-mar", 60, false, genero);
        repositorioFilme!.Cadastrar(filme);
        dbContext!.SaveChanges();

        var filmeEditado = new Filme("Chuva em chamas", 70, false, genero);

        // Act
        var conseguiuEditar = repositorioFilme.Editar(filme.Id, filmeEditado);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(filme, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente() {

        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("O silêncio barulhento da madrugada", 90, true, genero);
        repositorioFilme!.Cadastrar(filme);
        dbContext!.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioFilme.Excluir(filme.Id);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Generos_Corretamente() {

        // Arrange - Arranjo
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme1 = new Filme("As pegadas do vento", 10, true, genero);
        var filme2 = new Filme("Sombras de luz", 20, false, genero);
        var filme3 = new Filme("O assovio mudo do surdo", 30, true, genero);

        repositorioFilme!.Cadastrar(filme1);
        repositorioFilme!.Cadastrar(filme2);
        repositorioFilme!.Cadastrar(filme3);

        dbContext!.SaveChanges();

        List<Filme> filmesEsperados = [filme1, filme2, filme3];

        var filmesEsperadosOrdenados = filmesEsperados
            .OrderBy(f => f.Titulo)
            .ToList();

        // Act - Ação
        var filmesRecebidos = repositorioFilme.SelecionarRegistros();

        // Assert - Asseção
        CollectionAssert.AreEqual(filmesEsperadosOrdenados, filmesRecebidos);
    }

}
