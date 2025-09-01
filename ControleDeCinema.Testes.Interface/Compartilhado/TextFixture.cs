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

    private static IReadOnlyCollection<Cookie>? cookies;
    private static bool usuarioRegistrado = false;

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
        
        if (cookies != null && cookies.Any())
            ReaplicarCookies(driver!, cookies);
    }

    private static void InicializarWebDriver() {
        var options = new ChromeOptions();

        //options.AddArgument("--headless");

        driver = new ChromeDriver(options);
    }

    private static void EncerrarWebDriver() {
        try {
            driver?.Quit();
            driver?.Dispose();
        } catch (Exception ex) {
            // Log ou trate o erro
            Console.WriteLine($"Erro ao fechar o navegador: {ex.Message}");
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
        driver.Navigate().GoToUrl(enderecoBase); 
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

    public void Dispose()
    {
        // Cleanup se necessário
    }
}
