using AuthServer.Domain.Roles;
using AuthServer.Domain.UserRoles;
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
        var roleRepository = scoped.ServiceProvider.GetRequiredService<IRoleRepository>();
        var userRoleRepository = scoped.ServiceProvider.GetRequiredService<IUserRoleRepository>();
        var unitOfWork = scoped.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var adminExists =
          await userRepository.ExistsByUserNameAsync(new UserName("admin"));

        if (adminExists)
        {
            return;
        }
        var adminRole= await roleRepository.GetByNameAsync(SystemRoles.Admin)??throw new Exception(
                "Admin role not found");
        var user = User.Create(
           new FirstName("Admin"),
           new LastName("Admin"),
           new UserName("admin"),
           new Email("Azizlee.t@gmail.com"),
           new Password("12345678"));
        
        
        await userRepository.AddAsync(user);

        var userRole = new UserRole(user.Id, adminRole.Id);

        await userRoleRepository.AddAsync(userRole);
        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception)
        {

          
        }
       

    }
}