using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
public abstract class TestFixture : IDisposable
{
    protected static IWebDriver? driver;
    protected static ControleDeCinemaDbContext? dbContext;
    protected static string? enderecoBase;

    private static IReadOnlyCollection<Cookie>? cookies;
    private static bool usuarioRegistrado = false;

    private static IDatabaseContainer? dbContainer;
    private readonly static int dbPort = 5432;

    private static IContainer? appContainer;
    private readonly static int appPort = 8080;

    private static IContainer? seleniumContainer;
    private readonly static int seleniumPort = 4444;

    private static IConfiguration? configuracao;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _) {

        configuracao = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<TestFixture>()
            .Build();

        var rede = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString())
            .WithCleanUp(true)
            .Build();

        await InicializarBancoAsync(rede);
        
        // inicia como container
        await InicializarAplicacaoAsync(rede);

        await InicializarWebDriverAsync(rede);
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes() {
        await EncerrarWebDriverAsync();

        await EncerrarAplicacaoAsync();

        await EncerrarBancoAsync();
    }

    [TestInitialize]
    public void InicializarTeste() {
        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(dbContainer!.GetConnectionString());

        ConfigurarTabelas(dbContext);
        
        if (cookies is not null && cookies.Any())
            ReaplicarCookies(driver!, cookies);
    }

    private static async Task InicializarWebDriverAsync(DotNet.Testcontainers.Networks.INetwork rede) {
        seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:latest")
            .WithPortBinding(seleniumPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-selenium-e2e")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithName("controle-de-cinema-selenium-e2e")          
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(seleniumPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)seleniumPort).ForPath("/wd/hub/status")))
            .WithCleanUp(true)
            .Build(); 
        
        await seleniumContainer.StartAsync();

        var enderecoSelenium = new Uri($"http://{seleniumContainer.Hostname}:{seleniumContainer.GetMappedPublicPort(seleniumPort)}/wd/hub");

        var options = new ChromeOptions();        
        options.AddArgument($"--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--window-size=1920,1080");

        driver = new RemoteWebDriver(enderecoSelenium, options.ToCapabilities(), TimeSpan.FromSeconds(120));
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
    }

    private static async Task InicializarBancoAsync(DotNet.Testcontainers.Networks.INetwork rede) {
        dbContainer = new PostgreSqlBuilder() // DOCKER
             .WithImage("postgres:16")
             .WithPortBinding(dbPort, true)
             .WithNetwork(rede)
             .WithNetworkAliases("controle-de-cinema-e2e-testdb")
             .WithName("controle-de-cinema-e2e-testdb")
             .WithDatabase("ControleCinemaDB")
             .WithUsername("postgres")
             .WithPassword("MyStrongPassword")
             .WithCleanUp(true)
             .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(dbPort))
             .Build();

        await dbContainer.StartAsync();
    }

    private static async Task InicializarAplicacaoAsync(DotNet.Testcontainers.Networks.INetwork rede) {
        
        // config imagem do Dockerfile
        var imagem = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Dockerfile")
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithName("controle-de-cinema-app-e2e:latest")
            .Build();

        await imagem.CreateAsync()
            .ConfigureAwait(false);

        // config connectionString
        // dbContainer.GetConnectionString() = $"Host=127.0.0.1;Port=5432;Database=ControleDeCinemaDb;Username=postgres;Password=MyStrongPassword";
        var connectionStringRede = dbContainer?.GetConnectionString()
            .Replace(dbContainer.Hostname, "controle-de-cinema-e2e-testdb")
            .Replace(dbContainer.GetMappedPublicPort(5432).ToString(), "5432");


        // config aplicacao
        appContainer = new ContainerBuilder()
            .WithImage(imagem)            
            .WithPortBinding(appPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-webapp")
            .WithName("controle-de-cinema-webapp")
            .WithEnvironment("SQL_CONNECTION_STRING", connectionStringRede)
            .WithEnvironment("NEWRELIC_LICENSE_KEY", configuracao?["NEWRELIC_LICENSE_KEY"])            
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(appPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)appPort).ForPath("/health")))
            .WithCleanUp(true)
            .Build(); 
        
        await appContainer.StartAsync();

        // config enderecoBase
        // http://controle-de-cinema-webapp:8080
        enderecoBase = $"http://{appContainer.Name}:{appPort}";
    }

    private static async Task EncerrarWebDriverAsync() {

        try {
            driver?.Quit();
            driver?.Dispose();

            if (seleniumContainer is not null) {
                await seleniumContainer.StopAsync();
                await seleniumContainer.DisposeAsync();
            }

        } catch (Exception ex) {
            // Log ou trate o erro
            Console.WriteLine($"Erro ao fechar o navegador: {ex.Message}");
        }
    }

    private static async Task EncerrarBancoAsync() {
        if (dbContainer is not null) {
            await dbContainer.StopAsync();
            await dbContainer.DisposeAsync();
        }
    }

    private static async Task EncerrarAplicacaoAsync() {
        if (appContainer is not null) {
            await appContainer.StopAsync();
            await appContainer.DisposeAsync();
        }
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext) {
        dbContext.Database.EnsureCreated();

        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
        dbContext.Filmes.RemoveRange(dbContext.Filmes);
        dbContext.Salas.RemoveRange(dbContext.Salas);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);

        dbContext.SaveChanges();
    }

    protected static void RegistrarOuLogar() {
        if (driver is null)
            throw new ArgumentNullException(nameof(driver));

        // já está logado neste driver?
        if (driver.Manage().Cookies.AllCookies.Any(c => c.Name == ".AspNetCore.Cookies"))
            return;

        // reaplica cookies de outro driver se existirem
        if (cookies != null && cookies.Any()) {
            ReaplicarCookies(driver, cookies);
            return;
        }

        try {
            // tenta login com usuário fixo
            driver.Navigate().GoToUrl($"{enderecoBase}/autenticacao/login");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var inputEmail = wait.Until(d => d.FindElement(By.Id("Email")));
            var inputSenha = wait.Until(d => d.FindElement(By.Id("Senha")));

            inputEmail.Clear();
            inputEmail.SendKeys("empresa@dominio.com");

            inputSenha.Clear();
            inputSenha.SendKeys("Teste@123");

            wait.Until(d => {
                var btn = d.FindElement(By.CssSelector("button[type='submit']"));
                if (!btn.Enabled || !btn.Displayed) return false;
                btn.Click();
                return true;
            });

            wait.Until(d => !d.Url.Contains("/autenticacao/login", StringComparison.OrdinalIgnoreCase));
        } catch {
            // se não encontrar campos de login, registra usuário
            if (!usuarioRegistrado) {
                usuarioRegistrado = true;
                RegistrarContaEmpresarial();
            }
        }

        // salva cookies para próximos testes
        cookies = driver.Manage().Cookies.AllCookies;
    }

    private static void ReaplicarCookies(IWebDriver driver, IReadOnlyCollection<OpenQA.Selenium.Cookie> cookies) {

        // precisa navegar primeiro
        driver.Navigate().GoToUrl(enderecoBase!); 
        driver.Manage().Cookies.DeleteAllCookies();

        foreach (var cookie in cookies)
            driver.Manage().Cookies.AddCookie(cookie);

        // aplica
        driver.Navigate().Refresh(); 
    }

    protected static void RegistrarContaEmpresarial() {

        if (driver is null)
            throw new ArgumentNullException(nameof(driver));

        driver.Manage().Cookies.DeleteAllCookies();

        driver.Navigate().GoToUrl($"{enderecoBase}/autenticacao/registro");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        var inputEmail = wait.Until(d => d.FindElement(By.Id("Email")));
        var inputSenha = wait.Until(d => d.FindElement(By.Id("Senha")));
        var inputConfirmarSenha = wait.Until(d => d.FindElement(By.Id("ConfirmarSenha")));
        var selectTipoUsuario = new SelectElement(wait.Until(d => d.FindElement(By.Id("Tipo"))));

        inputEmail.Clear();
        inputEmail.SendKeys("empresa@dominio.com");

        inputSenha.Clear();
        inputSenha.SendKeys("Teste@123");

        inputConfirmarSenha.Clear();
        inputConfirmarSenha.SendKeys("Teste@123");

        selectTipoUsuario.SelectByText("Empresa");

        wait.Until(d => {
            var btn = d.FindElement(By.CssSelector("button[type='submit']"));

            if (!btn.Enabled || !btn.Displayed) return false;

            btn.Click();

            return true;
        });

        wait.Until(d =>
                    !d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase) &&
                    d.FindElements(By.CssSelector("form[action='/autenticacao/registro']")).Count == 0
                );

        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
    }

    public void Dispose() {
        // Cleanup se necessário
    }
}
