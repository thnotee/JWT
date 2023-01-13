using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace OAuth2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Kakao() {

     

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Privacy(string code)
    {
        using (var httpClient = new HttpClient()) {

            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("client_id", "0def9ac5ef45f949547210255bd662b2");
            parameters.Add("redirect_uri", "https://localhost:7004/Home/Privacy");
            parameters.Add("code", code);
            var encodedContent = new FormUrlEncodedContent(parameters);

            using (var response = await httpClient.PostAsync("https://kauth.kakao.com/oauth/token", encodedContent))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result  = JsonConvert.DeserializeObject<kakaoTest>(apiResponse);
                ViewData["value"] = result;
            }
        }
            return View();
    }

    public async Task<IActionResult> KakaoLogout(string ACCESS_TOKEN) {

        using (var httpClient = new HttpClient())
        {

            var haderValue = $"Bearer {ACCESS_TOKEN}";
            httpClient.DefaultRequestHeaders.Add("Authorization", haderValue);

            //using (var response = await httpClient.PostAsync("https://kapi.kakao.com/v1/user/logout", null))
            using (var response = await httpClient.PostAsync("https://kapi.kakao.com//v1/user/unlink", null))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                /*
                var result = JsonConvert.DeserializeObject<kakaoTest>(apiResponse);
                ViewData["value"] = result;
                */
            }
        }

        return RedirectToAction("index");
    }
    /*
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }*/
}

public class kakaoTest
{

    public string? token_type { get; set; }
    public string? id_token { get; set; }
    public string? access_token { get; set; }
    public string? expires_in { get; set; }
    public string? refresh_token { get; set; }
    public string? refresh_token_expires_in { get; set; }
    public string? scope { get; set; }

}