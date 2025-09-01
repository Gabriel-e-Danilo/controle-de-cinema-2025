using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.WebApp.Orm;

public static class DatabaseOperations
{
    public static void ApplyMigrations(this IHost app)
    {
        var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ControleDeCinemaDbContext>();

        // Passos para corrigir erro de migração:
        // 1. Verifique a mensagem de erro detalhada no output do Visual Studio ou logs do container Docker.
        // 2. Certifique-se de que a string de conexão do banco de dados está correta e acessível pelo container.
        // 3. Se você alterou o modelo de dados, gere uma nova migration com o comando:
        //    dotnet ef migrations add <NomeDaMigration> --project ControleDeCinema.Infraestrutura.Orm
        // 4. Aplique as migrations localmente para testar: dotnet ef database update --project ControleDeCinema.Infraestrutura.Orm
        // 5. Se estiver usando Docker, crie uma nova imagem após atualizar as migrations:
        //    docker build -t controledecinema:latest .
        // 6. Substitua o container antigo pelo novo usando a imagem atualizada.
        // 7. Certifique-se de que o banco de dados está acessível a partir do container (verifique variáveis de ambiente e rede Docker).

        // O código abaixo está correto para aplicar migrations em tempo de execução.
        // Não é necessário alterar este trecho, apenas garantir que o ambiente e as migrations estejam corretos.
        dbContext.Database.Migrate();
    }
}
