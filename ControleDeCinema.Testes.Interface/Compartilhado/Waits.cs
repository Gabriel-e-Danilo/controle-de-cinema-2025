using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.Compartilhado;
public static class Waits
{
    public static IWebElement Clickable(IWebDriver driver, By by, int segundos = 10) =>
        new WebDriverWait(driver, TimeSpan.FromSeconds(segundos))
            .Until(SeleniumExtras.WaitHelpers.ExpectedConditions
            .ElementToBeClickable(by));

    public static void Eventually(Action assert, int tentativas = 10, int intervaloMs = 200) {
        Exception? last = null;
        for (int i = 0; i < tentativas; i++) {

            try { 
                assert(); 
                
                return; 

            } catch (Exception ex) { 
                last = ex; 
                Thread.Sleep(intervaloMs);
            }
        }
        throw last!;
    }
}
