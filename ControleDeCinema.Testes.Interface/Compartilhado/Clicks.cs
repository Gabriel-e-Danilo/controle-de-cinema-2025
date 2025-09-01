using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace ControleDeCinema.Testes.Interface.Compartilhado;

public static class Clicks
{
    public static void SafeClick(IWebDriver driver, IWebElement webE, int tentativas = 3) {
        var actions = new Actions(driver);

        for (int i = 0; i < tentativas; i++) {

            try { 
                actions
                    .MoveToElement(webE)
                    .Pause(TimeSpan.FromMilliseconds(100))
                    .Click()
                    .Perform(); 
                return; 

            } catch (ElementClickInterceptedException) { 
                Thread.Sleep(250);
            }
        }

        webE.Click();
    }
}