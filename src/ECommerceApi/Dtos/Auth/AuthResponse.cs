using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Dtos.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }

    public class LoginSuccessExample : IExamplesProvider<AuthResponse>
    {
        public AuthResponse GetExamples()
        {
            return new AuthResponse
            {
                AccessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlcyI6WyJDb21wbGlhbmNlIiwiQXBwT3duZXIiLCJBZG1pbiIsIkJyYW5jaCIsIkNERyIsIk1ha2VyIl0sInNjb3BlIjpbInJlYWQiLCJ3cml0ZSIsInRydXN0Il0sImF1dGhvcml0aWVzIjpbIlVTRVIiXSwic3ViIjoiNTBUMDEwMDkiLCJqdGkiOiJkMjUyOGRjMy1hOWU4LTQ3MWUtODM2NS02ZjMzZGFkNDZiNjEiLCJuYmYiOjE3MjQwMzExNDUsImV4cCI6MTcyNDAzMjk0NSwiaWF0IjoxNzI0MDMxMTQ1LCJpc3MiOiJpcHJvLWFjY291bnQiLCJhdWQiOiJpcHJvLXdlYiJ9.PA0MdyP0pLpa_GPPfhhe4RBoGhH6p8n8ZcnnNPBcumTqNBrrCvA_iglskuuBO2SYpRDNAZ22uX5AHQ6ogrsX4zbq5dgEhQ1Z-sPfrDXbH6pDfRcSGPvHiPiAVpDKOkJWfY9HEviiJD3gNALEky6uk_D-RBq_Jb1eX6SK-bV-j7kr0TCQpb0WCOC-z6vqL6h2BX__zkef_k7DMJZnQouZnHh1ysBDosBLwkoy_oSl3PeKH-6TdYuCf0EotqMXEv8DeoP7W5d5elNVj_sv8bqBGE93HID1-2QMXiZ5Sfh051qSZflp0y7ieyT5AWnq7C0htPmwNcQ61tqH0NWUcdwtmw",
                ExpiresInMinutes = 15
            };
        }
    }
    public class LoginUnauthorizedExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                status = 401,
                error = "Unauthorized",
                message = "Invalid email or password."
            };
        }
    }
    
}