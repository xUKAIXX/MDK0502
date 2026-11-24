using System.Security.Claims;
using BookList.Models;
using Microsoft.AspNetCore.Identity;

namespace BookList.Data;

/// <summary>
/// Заполняет базу ролями и демонстрационными учётными записями.
/// Вызывается один раз при старте, поэтому проверки «уже существует» обязательны.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Администратор: зарегистрирован три года назад — проходит политику OnlyAdults.
        await CreateUserAsync(
            userManager,
            email: "admin@booklist.local",
            password: "Admin123",
            fullName: "Администратор",
            role: "Admin",
            registeredAt: DateTime.Today.AddYears(-3));

        // Обычный пользователь: зарегистрирован сегодня — политика OnlyAdults его не пустит.
        await CreateUserAsync(
            userManager,
            email: "user@booklist.local",
            password: "User123",
            fullName: "Читатель",
            role: "User",
            registeredAt: DateTime.Today);
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role,
        DateTime registeredAt)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                RegisteredAt = registeredAt,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Не удалось создать пользователя " + email + ": "
                    + string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(user, role);
        }

        // Клейм с датой регистрации — по нему работает политика OnlyAdults.
        var hasClaim = (await userManager.GetClaimsAsync(user)).Any(c => c.Type == "RegisteredAt");

        if (!hasClaim)
        {
            await userManager.AddClaimAsync(
                user,
                new Claim("RegisteredAt", registeredAt.ToString("yyyy-MM-dd")));
        }
    }
}
