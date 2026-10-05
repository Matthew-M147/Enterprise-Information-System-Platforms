using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly TravelAgencyContext _db;

    public HomeController(TravelAgencyContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ClientsCount = await _db.Clients.CountAsync();
        ViewBag.ToursCount = await _db.Tours.CountAsync();
        ViewBag.OrdersCount = await _db.Orders.CountAsync();
        return View();
    }
}
