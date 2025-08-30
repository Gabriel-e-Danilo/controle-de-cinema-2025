using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao_1_.ModuloSala;

[TestClass]
[TestCategory("Testes de Integração de Sala")]
public sealed class RepositorioSalaEmOrmTests : TestFixture
{
    [TestMethod]
    public sealed void Deve_Cadastrar_Sala_Corretamente() {

        // Arrange
        var sala = new Sala(1, 45);

        // Act
        repositorioSala!.Cadastrar(sala);
        dbContext!.SaveChanges();

        // Assert
        var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

        Assert.AreEqual(sala, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente() {

        // Arrange
        var sala = new Sala(1, 45);
        repositorioSala!.Cadastrar(sala);
        dbContext!.SaveChanges();

        var salaEditada = new Sala(2, 30);

        // Act
        var conseguiuEditar = repositorioSala.Editar(sala.Id, salaEditada);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(sala, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente() {

        // Arrange
        var sala = new Sala(1, 45);
        repositorioSala!.Cadastrar(sala);
        dbContext!.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioSala.Excluir(sala.Id);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Sala_Corretamente() {

        // Arrange - Arranjo
        var sala1 = new Sala(1, 45);
        var sala2 = new Sala(2, 30);
        var sala3 = new Sala(3, 15);

        repositorioSala!.Cadastrar(sala1);
        repositorioSala!.Cadastrar(sala2);
        repositorioSala!.Cadastrar(sala3);

        dbContext!.SaveChanges();

        List<Sala> salasEsperadas = [sala1, sala2, sala3];

        var salasEsperadasOrdenadas = salasEsperadas
            .OrderBy(s => s.Numero)
            .ToList();

        // Act - Ação
        var salasRecebidas = repositorioSala.SelecionarRegistros().OrderBy(s => s.Numero).ToList();

        // Assert - Asseção
        CollectionAssert.AreEqual(salasEsperadasOrdenadas, salasRecebidas);
    }
}
