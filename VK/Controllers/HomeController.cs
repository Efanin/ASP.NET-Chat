using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using VK.Models;

namespace VK.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SqlConnection connection = new("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\efani\\source\\repos\\VK\\database\\Database1.mdf;Integrated Security=True");
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(Request.Cookies["Login"]!=null)
                return RedirectToAction("Main");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult Register(string login, string password)
        {
            Console.WriteLine(login);
            Console.WriteLine(password);
            connection.Open();
            SqlCommand command = new(
                "insert into Users (Login, Password) values (@login, @password)",
                connection);
            command.Parameters.AddWithValue("login", login);
            command.Parameters.AddWithValue("password", password);
            command.ExecuteNonQuery();
            Response.Cookies.Append("Login", login, new CookieOptions());
            return RedirectToAction("Main");
        }
        public ActionResult Login(string login, string password)
        {
            Console.WriteLine(login);
            Console.WriteLine(password);
            connection.Open();
            SqlCommand command = new(
                "select * from Users where (Login = @login and Password = @password)",
                connection);
            command.Parameters.AddWithValue("login", login);
            command.Parameters.AddWithValue("password", password);
            SqlDataReader reader = command.ExecuteReader();
            bool isLogin = false;
            while(reader.Read())
            {
                Console.WriteLine(true);
                isLogin = true;
            }
            reader.Close();
            if(isLogin)
            {
                Response.Cookies.Append("Login", login, new CookieOptions());
            }
            return RedirectToAction("Main");
        }
        public IActionResult Main()
        {
            ViewData["User"] = Request.Cookies["Login"];
            return View();
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Login");
            return View("Index");
        }

        public IActionResult Chat()
        {
            connection.Open();
            SqlCommand command = new(
                "select * from Sms",
                connection);
            SqlDataReader reader = command.ExecuteReader();
            List<MessageModel> messages = new List<MessageModel>();
            while (reader.Read())
            {
                var user = Convert.ToString(reader["Users"]);
                messages.Add(new MessageModel(
                    user,
                    Convert.ToString(reader["Text"]),
                    user == Request.Cookies["Login"] ? true : false
                    ));
            }
            reader.Close();
            return View(messages);
        }
        public ActionResult Send(string sms)
        {
            connection.Open();
            SqlCommand command = new(
                "insert into Sms (Users, Text) values (@user, @text)",
                connection);
            command.Parameters.AddWithValue("user", Request.Cookies["Login"]);
            command.Parameters.AddWithValue("text", sms);
            command.ExecuteNonQuery();
            return RedirectToAction("Chat");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
