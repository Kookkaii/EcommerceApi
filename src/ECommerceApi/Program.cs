using ECommerceApi.Data;
using ECommerceApi.Extensions;
using ECommerceApi.Helpers.Extensions;
using ECommerceApi.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddAppConfigurations(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwaggerConfig();

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(dbContext);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
