using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateAgency.Domain;
using RealEstateAgency.Infrastructure.Identity;

namespace RealEstateAgency.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await EnsureRolesAsync(roleManager);
        await EnsureUsersAsync(userManager);
        await SeedDictionariesAsync(context);
        await SeedAgentsAsync(context);
        await SeedPropertiesAsync(context);
        await SeedDealsAsync(context);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { "Admin", "Agent", "Client" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task EnsureUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new[]
        {
            new
            {
                User = new ApplicationUser
                {
                    UserName = "admin@agency.local",
                    Email = "admin@agency.local",
                    FullName = "Администратор",
                    Phone = "+7 (900) 000-00-01",
                    RegisteredAt = DateTime.Now.AddYears(-3),
                    EmailConfirmed = true
                },
                Password = "Admin123!",
                Role = "Admin"
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "agent@agency.local",
                    Email = "agent@agency.local",
                    FullName = "Анна Петрова",
                    Phone = "+7 (900) 222-22-22",
                    RegisteredAt = DateTime.Now.AddYears(-1),
                    EmailConfirmed = true
                },
                Password = "Agent123!",
                Role = "Agent"
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "client@agency.local",
                    Email = "client@agency.local",
                    FullName = "Игорь Смирнов",
                    Phone = "+7 (911) 111-22-33",
                    RegisteredAt = DateTime.Now,
                    EmailConfirmed = true
                },
                Password = "Client123!",
                Role = "Client"
            }
        };

        foreach (var item in users)
        {
            if (await userManager.FindByEmailAsync(item.User.Email!) is not null)
            {
                continue;
            }

            var result = await userManager.CreateAsync(item.User, item.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать пользователя {item.User.Email}: {string.Join(", ", result.Errors.Select(error => error.Description))}");
            }

            await userManager.AddToRoleAsync(item.User, item.Role);
            await userManager.AddClaimAsync(item.User, new Claim("RegisteredAt", item.User.RegisteredAt.ToString("O")));
        }
    }

    private static async Task SeedDictionariesAsync(ApplicationDbContext context)
    {
        if (await context.PropertyTypes.AnyAsync())
        {
            return;
        }

        context.PropertyTypes.AddRange(
            PropertyType.Create("Квартира"),
            PropertyType.Create("Дом"),
            PropertyType.Create("Коммерческая недвижимость"),
            PropertyType.Create("Земельный участок"));

        context.Districts.AddRange(
            District.Create("Центральный", "Санкт-Петербург"),
            District.Create("Невский", "Санкт-Петербург"),
            District.Create("Приморский", "Санкт-Петербург"),
            District.Create("Василеостровский", "Санкт-Петербург"),
            District.Create("Петроградский", "Санкт-Петербург"),
            District.Create("Калининский", "Санкт-Петербург"),
            District.Create("Московский", "Санкт-Петербург"),
            District.Create("Курортный", "Санкт-Петербург"));

        await context.SaveChangesAsync();
    }

    private static async Task SeedAgentsAsync(ApplicationDbContext context)
    {
        if (await context.Agents.AnyAsync())
        {
            return;
        }

        context.Agents.AddRange(
            Agent.Create("Иван Иванов", "+7 (900) 111-11-11", "ivanov@agency.local", 8),
            Agent.Create("Анна Петрова", "+7 (900) 222-22-22", "petrova@agency.local", 5),
            Agent.Create("Алексей Сидоров", "+7 (900) 333-33-33", "sidorov@agency.local", 3),
            Agent.Create("Мария Кузнецова", "+7 (900) 444-44-44", "kuznetsova@agency.local", 6));

        await context.SaveChangesAsync();
    }

    private static async Task SeedPropertiesAsync(ApplicationDbContext context)
    {
        if (await context.Properties.AnyAsync())
        {
            return;
        }

        var flat = await context.PropertyTypes.FirstAsync(type => type.Name == "Квартира");
        var house = await context.PropertyTypes.FirstAsync(type => type.Name == "Дом");
        var commercial = await context.PropertyTypes.FirstAsync(type => type.Name == "Коммерческая недвижимость");

        var central = await context.Districts.FirstAsync(district => district.Name == "Центральный");
        var nevsky = await context.Districts.FirstAsync(district => district.Name == "Невский");
        var primorsky = await context.Districts.FirstAsync(district => district.Name == "Приморский");
        var vasilievsky = await context.Districts.FirstAsync(district => district.Name == "Василеостровский");
        var petrogradsky = await context.Districts.FirstAsync(district => district.Name == "Петроградский");
        var kalininsky = await context.Districts.FirstAsync(district => district.Name == "Калининский");
        var moskovsky = await context.Districts.FirstAsync(district => district.Name == "Московский");
        var kurortny = await context.Districts.FirstAsync(district => district.Name == "Курортный");

        var ivanov = await context.Agents.FirstAsync(agent => agent.FullName == "Иван Иванов");
        var petrova = await context.Agents.FirstAsync(agent => agent.FullName == "Анна Петрова");
        var sidorov = await context.Agents.FirstAsync(agent => agent.FullName == "Алексей Сидоров");
        var kuznetsova = await context.Agents.FirstAsync(agent => agent.FullName == "Мария Кузнецова");

        var now = DateTime.UtcNow;

        var properties = new[]
        {
            Property.Create(
                "2-комнатная квартира на Невском проспекте",
                "Невский проспект, 100",
                12_500_000m,
                65.4,
                2,
                5,
                9,
                2012,
                "Светлая квартира с окнами на Невский проспект, развитая инфраструктура.",
                flat,
                central,
                ivanov,
                null,
                null,
                now.AddDays(-30)),
            Property.Create(
                "Студия у метро «Горьковская»",
                "Петроградская набережная, 20",
                6_800_000m,
                28.5,
                1,
                3,
                6,
                2018,
                "Уютная студия с качественным ремонтом, 5 минут до метро.",
                flat,
                petrogradsky,
                petrova,
                null,
                null,
                now.AddDays(-25)),
            Property.Create(
                "3-комнатная квартира в Приморском районе",
                "улица Савушкина, 45",
                14_200_000m,
                82.0,
                3,
                8,
                22,
                2015,
                "Просторная квартира с лоджией и видом на залив.",
                flat,
                primorsky,
                sidorov,
                null,
                null,
                now.AddDays(-20)),
            Property.Create(
                "Коттедж во Всеволожске",
                "Всеволожск, улица Центральная, 7",
                28_000_000m,
                180.0,
                5,
                2,
                2,
                2019,
                "Двухэтажный коттедж с участком 12 соток и гаражом.",
                house,
                vasilievsky,
                kuznetsova,
                null,
                null,
                now.AddDays(-18)),
            Property.Create(
                "Офис в бизнес-центре «Петровский»",
                "Петровская коса, 5",
                32_000_000m,
                120.0,
                0,
                4,
                7,
                2008,
                "Офисное помещение с мебелью и парковкой.",
                commercial,
                petrogradsky,
                ivanov,
                null,
                null,
                now.AddDays(-15)),
            Property.Create(
                "1-комнатная квартира на Васильевском острове",
                "Большой проспект В.О., 55",
                7_900_000m,
                38.0,
                1,
                4,
                9,
                2005,
                "Комната с балконом, рядом парк и набережная.",
                flat,
                vasilievsky,
                petrova,
                null,
                null,
                now.AddDays(-12)),
            Property.Create(
                "2-комнатная квартира в Московском районе",
                "улица Типанова, 25",
                10_500_000m,
                58.0,
                2,
                7,
                12,
                1998,
                "Квартира с хорошей транспортной доступностью.",
                flat,
                moskovsky,
                sidorov,
                null,
                null,
                now.AddDays(-10)),
            Property.Create(
                "4-комнатная квартира на Крестовском острове",
                "Крестовский проспект, 10",
                45_000_000m,
                150.0,
                4,
                6,
                8,
                2020,
                "Видовая квартира в новом жилом комплексе.",
                flat,
                petrogradsky,
                kuznetsova,
                null,
                null,
                now.AddDays(-8)),
            Property.Create(
                "2-комнатная квартира в Калининском районе",
                "улица Дачная, 12",
                9_300_000m,
                60.0,
                2,
                9,
                14,
                2007,
                "Продажа от собственника, документы готовы.",
                flat,
                kalininsky,
                ivanov,
                null,
                null,
                now.AddDays(-6)),
            Property.Create(
                "1-комнатная квартира в Невском районе",
                "улица Коллонтай, 8",
                6_200_000m,
                35.0,
                1,
                2,
                9,
                1985,
                "Комната в панельном доме, требует косметического ремонта.",
                flat,
                nevsky,
                petrova,
                null,
                null,
                now.AddDays(-4)),
            Property.Create(
                "Дом на Каменном острове",
                "Каменный остров, 3",
                95_000_000m,
                420.0,
                8,
                2,
                3,
                2016,
                "Авторский дом с панорамными окнами и участком 20 соток.",
                house,
                petrogradsky,
                kuznetsova,
                null,
                null,
                now.AddDays(-3)),
            Property.Create(
                "Коммерческое помещение на Лиговском",
                "Лиговский проспект, 50",
                22_000_000m,
                180.0,
                0,
                1,
                5,
                2003,
                "Помещение свободного назначения с отдельным входом.",
                commercial,
                central,
                sidorov,
                null,
                null,
                now.AddDays(-2))
        };

        context.Properties.AddRange(properties);
        await context.SaveChangesAsync();

        properties[4].ChangeStatus(PropertyStatus.ForRent);
        properties[6].ChangeStatus(PropertyStatus.ForRent);
        properties[8].ChangeStatus(PropertyStatus.Sold);
        properties[9].ChangeStatus(PropertyStatus.ForRent);
        properties[9].ChangeStatus(PropertyStatus.Rented);
        properties[11].ChangeStatus(PropertyStatus.ForRent);

        await context.SaveChangesAsync();
    }

    private static async Task SeedDealsAsync(ApplicationDbContext context)
    {
        if (await context.Deals.AnyAsync())
        {
            return;
        }

        var property1 = await context.Properties.FirstAsync(property => property.Id == 1);
        var property5 = await context.Properties.FirstAsync(property => property.Id == 5);
        var property6 = await context.Properties.FirstAsync(property => property.Id == 6);
        var property7 = await context.Properties.FirstAsync(property => property.Id == 7);

        var now = DateTime.UtcNow;

        var purchase = Deal.Create(
            property1,
            "Ольга Смирнова",
            "+7 (911) 111-22-33",
            DealType.Purchase,
            12_500_000m,
            now.AddDays(-5),
            "Ипотека одобрена, сделка завершена.");

        var rent = Deal.Create(
            property5,
            "ООО «Вектор»",
            "+7 (921) 222-33-44",
            DealType.Rent,
            180_000m,
            now.AddDays(-4),
            "Аренда офиса на 11 месяцев.");

        var purchase2 = Deal.Create(
            property6,
            "Дмитрий Кузнецов",
            "+7 (931) 333-44-55",
            DealType.Purchase,
            7_900_000m,
            now.AddDays(-2),
            "Назначена встреча для просмотра.");

        var rent2 = Deal.Create(
            property7,
            "Мария Андреева",
            "+7 (941) 444-55-66",
            DealType.Rent,
            95_000m,
            now.AddDays(-1),
            "Новая заявка на аренду.");

        context.Deals.AddRange(purchase, rent, purchase2, rent2);
        await context.SaveChangesAsync();

        purchase.Complete();
        property1.MarkAsSold();

        rent.Complete();
        property5.MarkAsRented();

        await context.SaveChangesAsync();
    }
}
