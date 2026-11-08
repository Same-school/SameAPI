using FluentValidation;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SameApi.Business;
using SameApi.Business.User.Command;
using SameApi.Db;

var builder = WebApplication.CreateBuilder(args);


var projectId = builder.Configuration["Firestore:ProjectId"]!;
var DatabaseId = builder.Configuration["Firestore:DatabaseId"]!;
// -------------------- Controllers --------------------
builder.Services.AddControllers();

// -------------------- Swagger --------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Test", Version = "v1" });

    c.AddServer(new OpenApiServer
    {
        Url = "https://localhost:7171",
        Description = "Local dev server"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Colle ton ID token Firebase"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// -------------------- Services --------------------

builder.Services.RegisterFireStore(projectId, DatabaseId);
builder.Services.RegisterFirebaseAdmin(projectId);
builder.Services.RegisterSameApiDbContainer();
builder.Services.AddAutoMapper(cfg => { }, typeof(SameApiProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommand>(); 
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateUserCommand).Assembly)
);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FirestoreDb>();
    await DbRegisterExtension.SeedGendersAsync(db);
    await DbRegisterExtension.SeedSchoolsAsync(db);
}

if (app.Environment.IsDevelopment())
{
    // Swagger uniquement en développement
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Test v1");
        c.RoutePrefix = "swagger"; // Accessible via /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { ok = true, dotnet = Environment.Version.ToString() }));


app.Run();