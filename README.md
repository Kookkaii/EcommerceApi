# ECommerceApi
E-Commerce API built with .NET 8, PostgreSQL, and Docker.

### Project Setup
- Clone the repository:
```
git clone https://github.com/Kookkaii/EcommerceApi.git
cd ECommerceApi
```

- Build and run the containers:
```
docker compose up --build 
```

- Swagger UI to explore endpoints:
http://localhost:8080/swagger/index.html

## Testing
- Run unit tests:
```
dotnet test
```

### Library reference
lib in project: 
- [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net) 
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwtbearer)
- [Microsoft.AspNetCore.OpenApi](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle)
- [Microsoft.EntityFrameworkCore.Design](https://github.com/dotnet/efcore)
- [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.npgsql.org/efcore/)
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Swashbuckle.AspNetCore.Filters](https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters)

lib in test project: 
- [AutoFixture](https://github.com/AutoFixture/AutoFixture) (v4.18.1) – Auto-generate test data
- [AutoFixture.AutoMoq](https://github.com/AutoFixture/AutoFixture) (v4.18.1) – Integration with Moq
- [coverlet.collector](https://github.com/tonerdo/coverlet) (v6.0.0) – Code coverage
- [Microsoft.NET.Test.Sdk](https://github.com/microsoft/testfx) (v18.0.0) – Test SDK
- [Moq](https://github.com/moq/moq) (v4.20.72) – Mocking framework
- [Shouldly](https://github.com/shouldly/shouldly) (v4.3.0) – Assertion library
- [xunit](https://github.com/xunit/xunit) (v2.5.3) – Test framework
- [xunit.runner.visualstudio](https://github.com/xunit/xunit) (v2.5.3) – Visual Studio test runner



