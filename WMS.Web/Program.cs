using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using WMS.Utility;
using WMS.DataAccess.Repository.IRepository;
using Core.DataAccess.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

var Conn = new Connection_UAT();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    Conn.Data_Source +
    Conn.Password +
    Conn.Persist_Security_Info +
    Conn.User_ID +
    Conn.Initial_Catalog +
    Conn.Connect_Timeout +
    Conn.Max_Pool_Size +
    Conn.TrustServerCertificate
));

//var Conn_UAT = new Connection_UAT();
//builder.Services.AddDbContext<AppDbContext_2>(options => options.UseSqlServer(
//    Conn_UAT.Data_Source +
//    Conn_UAT.Password +
//    Conn_UAT.Persist_Security_Info +
//    Conn_UAT.User_ID +
//    Conn_UAT.Initial_Catalog +
//    Conn_UAT.Connect_Timeout +
//    Conn_UAT.Max_Pool_Size +
//    Conn_UAT.TrustServerCertificate
//    ));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MapperConfig));

var Jwt = new Jwt();

builder.Services.AddAuthentication(
    )
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Error/NotAllowed";
        options.LogoutPath = "/Home/Login";
        options.SlidingExpiration = true;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Jwt.Issuer,
            ValidAudience = Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme,
        CookieAuthenticationDefaults.AuthenticationScheme);

    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

    var onlySecondJwtSchemePolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    options.AddPolicy("Bearer", onlySecondJwtSchemePolicyBuilder
        .RequireAuthenticatedUser()
        .Build());
    var onlyCookieSchemePolicyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
    options.AddPolicy("Cookie", onlyCookieSchemePolicyBuilder
        .RequireAuthenticatedUser()
        .Build());

    options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("ProfileId", "1"));
    options.AddPolicy("Tenant", policy => policy.RequireClaim("ProfileId", "1", "2"));
    options.AddPolicy("AdminWarehouse", policy => policy.RequireClaim("ProfileId", "1", "3"));
});

// Add ToastNotification
//builder.Services.AddNotyf(config =>
//{
//    config.DurationInSeconds = 3;
//    config.IsDismissable = true;
//    config.Position = NotyfPosition.BottomLeft;
//});

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//Add Data Seed
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    //DbInitializer.Initialize(service);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/InternalServer");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

var Mimes = new FileExtensionContentTypeProvider();
Mimes.Mappings[".apk"] = "application/vnd.android.package-archive";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = Mimes
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseNotyf();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");
    endpoints.MapRazorPages();
});

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run(context =>
{
    context.Response.StatusCode = 404;
    return Task.FromResult(0);
});