using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDeCinema.Testes.Interface.ModuloSessao
{
    public class SessaoIndexPageObject
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public SessaoIndexPageObject(IWebDriver driver)
        {
            this.driver = driver;


            wait.Until(d => d.FindElement(By.CssSelector("table")).Displayed);
        }

        public SessaoIndexPageObject IrPara(string enderecoBase)
        {
            driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "sessoes"));

            return this;
        }

        public SessaoFormPageObject ClickCadastrar()
        {
            wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

            return new SessaoFormPageObject(driver!);
        }

        public SessaoFormPageObject ClickEditar()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Edição']"))).Click();

            return new SessaoFormPageObject(driver!);
        }

        public SessaoFormPageObject ClickExcluir()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Exclusão']"))).Click();

            return new SessaoFormPageObject(driver!);
        }

        public bool ContemSessao(string nome)
        {
            wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

            return driver.PageSource.Contains(nome);
        }
    }
    
}