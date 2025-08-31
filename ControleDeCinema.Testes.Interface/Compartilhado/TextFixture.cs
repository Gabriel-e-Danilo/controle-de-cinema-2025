using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
public abstract class TestFixture : IDisposable
{
    protected static IWebDriver? driver;
    protected static ControleDeCinemaDbContext? dbContext;

    protected static string enderecoBase = "https://localhost:7131";
    private static string connectionString = "Host=localhost;Port=5432;Database=ControleDeCinemaDb;Username=postgres;Password=YourStrongPassword";

    [AssemblyInitialize]
    public static void ConfigurarTestes(TestContext _) {
        InicializarWebDriver();
    }

    [AssemblyCleanup]
    public static void EncerrarTestes() {
        EncerrarWebDriver();
    }

    [TestInitialize]
    public void ConfigurarTestes() {
        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(connectionString);

        ConfigurarTabelas(dbContext);

        InicializarWebDriver();
    }

    private static void InicializarWebDriver() {
        var options = new ChromeOptions();

        // options.AddArgument("--headless");

        driver = new ChromeDriver(options);
    }

    private static void EncerrarWebDriver() {
        driver?.Quit();
        driver?.Dispose();
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext) {
        dbContext.Database.EnsureCreated();

        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
        dbContext.Filmes.RemoveRange(dbContext.Filmes);
        dbContext.Salas.RemoveRange(dbContext.Salas);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);

        dbContext.SaveChanges();
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

    public void Dispose()
    {
        // Cleanup se necessário
    }
}
