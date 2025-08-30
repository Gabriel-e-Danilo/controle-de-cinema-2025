using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDeCinema.Testes.Interface.ModuloSessao
{
    public class SessaoFormPageObject
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public SessaoFormPageObject(IWebDriver driver)
        {
            this.driver = driver;

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
        }

        public SessaoFormPageObject PreencherCampoDataHora(DateTime dataHora)
        {
            var campoDataHora = driver.FindElement(By.CssSelector("input[data-se='Inicio']"));
            
            campoDataHora.Clear();
            campoDataHora.SendKeys(dataHora.ToString("yyyy-MM-ddTHH:mm"));

            return this;
        }

        public SessaoFormPageObject PreencherCampoIngressos(int ingressos)
        {
            var campoIngressos = driver.FindElement(By.CssSelector("input[data-se='NumeroMaximoIngressos']"));
            
            campoIngressos.Clear();
            campoIngressos.SendKeys(ingressos.ToString());

            return this;
        }

        public SessaoFormPageObject SelecionarFilme(string filme)
        {
            wait.Until(d =>
            d.FindElement(By.Id("FilmeId")).Displayed &&
            d.FindElement(By.Id("FilmeId")).Enabled
        );

            var inputNome = driver.FindElement(By.Id("FilmeId"));
            inputNome.Clear();
            inputNome.SendKeys(filme);

            return this;
        }

        public SessaoFormPageObject SelecionarSala(string sala)
        {
            wait.Until(d =>
            d.FindElement(By.Id("SalaId")).Displayed &&
            d.FindElement(By.Id("SalaId")).Enabled
        );
            var inputNome = driver.FindElement(By.Id("SalaId"));
            inputNome.Clear();
            inputNome.SendKeys(sala);
            return this;
        }

        public SessaoIndexPageObject Confirmar()
        {
            wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

            wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

            return new SessaoIndexPageObject(driver!);
        }


    }
}
