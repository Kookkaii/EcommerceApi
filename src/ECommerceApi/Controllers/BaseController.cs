using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }
}