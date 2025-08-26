using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Integracao.Compartilhado
{
    [TestClass]
    public abstract class TestFixture
    {
        protected ControleDeCinemaDbContext? dbContext;

        protected RepositorioFilmeEmOrm? repositorioFilme;
        protected RepositorioGeneroFilmeEmOrm? repositorioGenero;
        protected RepositorioSalaEmOrm? repositorioSala;
        protected RepositorioSessaoEmOrm? repositorioSessao;

        private static IDatabaseContainer? container;

        [AssemblyInitialize]
        public static async Task Setup(TestContext _)
        {
            container = new PostgreSqlBuilder()
                 .WithImage("postgres:16")
                 .WithName("ControleCinemaTestDB")
                 .WithDatabase("ControleCinemaDB")
                 .WithUsername("postgres")
                 .WithPassword("MyStrongPassword")
                 .WithCleanUp(true)
                 .Build();

            await InicializarBancoAsync(container);
        }

        [AssemblyCleanup]
        public static async Task TearDown()
        {
            await EncerrarBancoAsync();
        }

        [TestInitialize]
        public void ConfigurarTeste()
        {
            if (container is null)
                throw new ArgumentNullException("O banco de dados não foi inicializado.");

            dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(container.GetConnectionString());

            ConfigurarTabelas(dbContext);

            repositorioFilme = new RepositorioFilmeEmOrm(dbContext);
            repositorioGenero = new RepositorioGeneroFilmeEmOrm(dbContext);
            repositorioSala = new RepositorioSalaEmOrm(dbContext);
            repositorioSessao = new RepositorioSessaoEmOrm(dbContext);

            BuilderSetup.SetCreatePersistenceMethod<Filme>(repositorioFilme.Cadastrar);
            BuilderSetup.SetCreatePersistenceMethod<IList<Filme>>(repositorioFilme.CadastrarEntidades);

            BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(repositorioGenero.Cadastrar);
            BuilderSetup.SetCreatePersistenceMethod<IList<GeneroFilme>>(repositorioGenero.CadastrarEntidades);

            BuilderSetup.SetCreatePersistenceMethod<Sala>(repositorioSala.Cadastrar);
            BuilderSetup.SetCreatePersistenceMethod<IList<Sala>>(repositorioSala.CadastrarEntidades);

            BuilderSetup.SetCreatePersistenceMethod<Sessao>(repositorioSessao.Cadastrar);
            BuilderSetup.SetCreatePersistenceMethod<IList<Sessao>>(repositorioSessao.CadastrarEntidades);


        }

        private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            
            dbContext.Filmes.RemoveRange(dbContext.Filmes);
            dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
            dbContext.Salas.RemoveRange(dbContext.Salas);
            dbContext.Sessoes.RemoveRange(dbContext.Sessoes);

            dbContext.SaveChanges();
        }

        private static async Task InicializarBancoAsync(IDatabaseContainer container)
        {
            await container.StartAsync();
        }

        private static async Task EncerrarBancoAsync()
        {
            if (container is null)
                throw new ArgumentNullException("O Banco de dados não foi inicializado.");

            await container.StopAsync();
            await container.DisposeAsync();
        }
    }
}
