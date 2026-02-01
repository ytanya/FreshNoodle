using FreshNoodle.UI.Models;

namespace FreshNoodle.UI.Services;

public class MenuService
{
    private List<MenuItem> _menuItems = new()
    {
        new MenuItem { Title = "Home", Href = "", Icon = "bi-house-door-fill-nav-menu", RequireAuth = false },
        new MenuItem { Title = "Products", Href = "products", Icon = "bi-box-seam-fill", Roles = "Admin,Manager,User" },
        new MenuItem { Title = "Dashboard", Href = "admin", Icon = "bi-speedometer2", Roles = "Admin" },
        new MenuItem { Title = "Financials", Href = "admin/financials", Icon = "bi-graph-up-arrow", Roles = "Admin,Accounting" },
        new MenuItem { Title = "Operations", Href = "admin/operations", Icon = "bi-truck", Roles = "Admin" },
        new MenuItem { Title = "User Management", Href = "admin/users", Icon = "bi-people-fill", Roles = "Admin" },



        new MenuItem { Title = "Role Management", Href = "admin/roles", Icon = "bi-shield-lock-fill", Roles = "Admin" },
        new MenuItem { Title = "Customer Management", Href = "admin/customers", Icon = "bi-person-badge-fill", Roles = "Admin" },
        new MenuItem { Title = "Customer Types", Href = "admin/customer-types", Icon = "bi-tags-fill", Roles = "Admin" },

        new MenuItem { Title = "Price Types", Href = "admin/price-types", Icon = "bi-currency-dollar", Roles = "Admin" },
        new MenuItem { Title = "Payment Types", Href = "admin/payment-types", Icon = "bi-credit-card-fill", Roles = "Admin" }
    };






    public IEnumerable<MenuItem> GetMenuItems() => _menuItems;
}
