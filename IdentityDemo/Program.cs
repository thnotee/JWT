using IdentityDemo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using IdentityDemo.Kakao;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication()

.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
/*
.AddKakaoTalk1("kakao", opt => {
    opt.ClientId = "0def9ac5ef45f949547210255bd662b2";
    opt.ClientSecret = "B07GrE7EqtJTex8zjh62M88vs1B6W9SA";
})

.AddKakaoTalk(opt =>
{
    opt.ClientId = "0def9ac5ef45f949547210255bd662b2";
    opt.ClientSecret = "B07GrE7EqtJTex8zjh62M88vs1B6W9SA";

})*/

.AddOAuth("testkakao", opt =>
{
    opt.ClientId = "0def9ac5ef45f949547210255bd662b2";
    opt.ClientSecret = "B07GrE7EqtJTex8zjh62M88vs1B6W9SA";
    opt.CallbackPath = "/signin-kakaotalk";
    opt.AuthorizationEndpoint = "https://kauth.kakao.com/oauth/authorize";
    opt.TokenEndpoint = "https://kauth.kakao.com/oauth/token";
    opt.UserInformationEndpoint = "https://kapi.kakao.com/v2/user/me";

    opt.SaveTokens = true;
    opt.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    opt.ClaimActions.MapCustomJson(ClaimTypes.Name, user =>
    {
        JsonElement property = user;
        bool hasProperty = property.TryGetProperty("kakao_account", out property)
                        && property.TryGetProperty("profile", out property)
                        && property.TryGetProperty("nickname", out property)
                        && property.ValueKind == JsonValueKind.String;
        return hasProperty
        ? property.GetString()
            : null;
    });
    opt.ClaimActions.MapJsonSubKey(ClaimTypes.Email, "kakao_account", "email");
    opt.ClaimActions.MapJsonSubKey(ClaimTypes.DateOfBirth, "kakao_account", "birthday");
    opt.ClaimActions.MapJsonSubKey(ClaimTypes.Gender, "kakao_account", "gender");
    opt.ClaimActions.MapJsonSubKey(ClaimTypes.MobilePhone, "kakao_account", "phone_number");
    opt.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            context.RunClaimActions(json.RootElement);
        }
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
