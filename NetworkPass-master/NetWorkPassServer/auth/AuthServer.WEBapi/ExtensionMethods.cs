using AuthServer.Domain.Users;
using AuthServer.Domain.Users.ValueObjects;
using GenericRepository;

namespace AuthServer.WEBapi;

public static class ExtensionMethods
{
    public static async Task CreateFirstUser(this WebApplication application)
    {
        using var scoped = application.Services.CreateScope();

        var userRepository = scoped.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scoped.ServiceProvider.GetRequiredService<IUnitOfWork>();

        if (!await userRepository.AnyAsync(p => p.UserName != null && p.UserName.Value == "admin"))
        {
            FirstName firstName = new("Admin");
            LastName lastName = new("Admin");
            Email email = new("Azizlee.t@gmail.com");
            UserName userName = new("admin");
            Password password = new("12345678");
            IsAdmin isAdmin=new(true);

            var user = new User(firstName, lastName, email, isAdmin,  userName, password);
            userRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
        }
    }
}