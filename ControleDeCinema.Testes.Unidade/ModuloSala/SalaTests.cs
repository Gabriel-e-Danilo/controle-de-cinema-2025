using ControleDeCinema.Dominio.ModuloSala;

namespace ControleDeCinema.Testes.Unidade.ModuloSala;

[TestClass]
[TestCategory("Testes de Unidade de Sala")]
public class SalaTests
{
    [TestMethod]
    public void AtualizarRegistro_DeveAtualizarDescricao() {

        // Arrange
        var salaOriginal = new Sala(1, 100);
        var salaEditada = new Sala(2, 150);

        // Act
        salaOriginal.AtualizarRegistro(salaEditada);

        // Assert
        Assert.AreEqual(2, salaOriginal.Numero);
        Assert.AreEqual(150, salaOriginal.Capacidade);
    }
}