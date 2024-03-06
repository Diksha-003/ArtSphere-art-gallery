using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using newUser.Data;
using Project.DTO;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    // Display all users
    public IActionResult GetAllUsers()
    {
        var usersWithRoles = _context.userViews
            .FromSqlRaw("SELECT u.Id, u.FirstName, u.LastName, u.Email, u.PhoneNumber, r.Name AS Role FROM aspnetusers u " +
                        "JOIN aspnetuserroles ur ON u.Id = ur.UserId " +
                        "JOIN aspnetroles r ON ur.RoleId = r.Id")
            .Select(u => new UserView
            {
                Id = (u.Id as string) ?? null,
                FirstName = (u.FirstName as string) ?? null,
                LastName = (u.LastName as string) ?? null,
                Email = (u.Email as string) ?? null,
                PhoneNumber = (u.PhoneNumber as string) ?? null,
                Role = (u.Role as string) ?? null
            })
            .ToList();

        return View(usersWithRoles);
    }


    // Display user details
    public IActionResult UserDetails(string id)
    {
        var user = _context.userViews
            .FromSqlRaw("SELECT Id, FirstName, LastName, Email, PhoneNumber FROM aspnetusers WHERE Id = {0}", id)
            .Select(u => new UserView
            {
                Id = (u.Id as string) ?? null,
                FirstName = (u.FirstName as string) ?? null,
                LastName = (u.LastName as string) ?? null,
                Email = (u.Email as string) ?? null,
                PhoneNumber = (u.PhoneNumber as string) ?? null
            })
            .FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

/*    // Create user
    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateUser(UserView user)
    {
        if (ModelState.IsValid)
        {
            // Perform necessary validations and save to the database
            // For simplicity, assume userView corresponds to the User entity
            _context.userViews.Add(user);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAllUsers));
        }

        return View(user);
    }*/
/*
    // Update user
    [HttpGet]
    public IActionResult UpdateUser(string id)
    {
        var user = _context.userViews
            .FromSqlRaw("SELECT Id, FirstName, LastName, Email, PhoneNumber FROM aspnetusers WHERE Id = {0}", id)
            .Select(u => new UserView
            {
                Id = (u.Id as string) ?? null,
                FirstName = (u.FirstName as string) ?? null,
                LastName = (u.LastName as string) ?? null,
                Email = (u.Email as string) ?? null,
                PhoneNumber = (u.PhoneNumber as string) ?? null
            })
            .FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public IActionResult UpdateUser(UserView user)
    {
        if (ModelState.IsValid)
        {
            // Fetch existing user details for validation and display purposes
            var existingUser = _context.userViews
                .FromSqlRaw("SELECT Id, FirstName, LastName, Email, PhoneNumber FROM aspnetusers WHERE Id = {0}", user.Id)
                .Select(u => new UserView
                {
                    Id = (u.Id as string) ?? null,
                    FirstName = (u.FirstName as string) ?? null,
                    LastName = (u.LastName as string) ?? null,
                    Email = (u.Email as string) ?? null,
                    PhoneNumber = (u.PhoneNumber as string) ?? null
                })
                .FirstOrDefault();

            if (existingUser == null)
            {
                return NotFound();
            }

            // Perform necessary validations (if any)

            // Parameterized SQL query to update the user in the database
            var updateSql = "UPDATE aspnetusers SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber WHERE Id = @Id";

            _context.Database.ExecuteSqlRaw(updateSql,
                new SqlParameter("@FirstName", user.FirstName),
                new SqlParameter("@LastName", user.LastName),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PhoneNumber", user.PhoneNumber),
                new SqlParameter("@Id", user.Id));

            return RedirectToAction(nameof(GetAllUsers));
        }

        return View(user);
    }

*/


    // Delete user
    [HttpGet]
    public IActionResult DeleteUser(string id)
    {
        var user = _context.userViews
            .FromSqlRaw("SELECT Id, FirstName, LastName, Email, PhoneNumber FROM aspnetusers WHERE Id = {0}", id)
            .Select(u => new UserView
            {
                Id = (u.Id as string) ?? null,
                FirstName = (u.FirstName as string) ?? null,
                LastName = (u.LastName as string) ?? null,
                Email = (u.Email as string) ?? null,
                PhoneNumber = (u.PhoneNumber as string) ?? null
            })
            .FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost, ActionName("DeleteUser")]
    public IActionResult ConfirmDeleteUser(string id)
    {
        // Fetch user details for display purposes (similar to DeleteUser action)
        var userToDelete = _context.userViews
            .FromSqlRaw("SELECT Id, FirstName, LastName, Email, PhoneNumber FROM aspnetusers WHERE Id = {0}", id)
            .Select(u => new UserView
            {
                Id = (u.Id as string) ?? null,
                FirstName = (u.FirstName as string) ?? null,
                LastName = (u.LastName as string) ?? null,
                Email = (u.Email as string) ?? null,
                PhoneNumber = (u.PhoneNumber as string) ?? null
            })
            .FirstOrDefault();

        if (userToDelete == null)
        {
            return NotFound();
        }

        // Perform necessary validations (if any)

        // Raw SQL query to delete the user from the database
        var deleteSql = "DELETE FROM aspnetusers WHERE Id = {0}";
        _context.Database.ExecuteSqlRaw(deleteSql, id);

        return RedirectToAction(nameof(GetAllUsers));
    }
}
