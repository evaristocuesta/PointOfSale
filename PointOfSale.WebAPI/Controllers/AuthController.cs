using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PointOfSale.WebAPI.Configuration;
using PointOfSale.WebAPI.ViewModels.Requests;
using PointOfSale.WebAPI.ViewModels.Responses;

namespace PointOfSale.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IOptions<JwtBearerTokenSettings> jwtTokenOptions, UserManager<IdentityUser> userManager)
        {
            _jwtBearerTokenSettings = jwtTokenOptions.Value;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Response response = new Response();
            if (!ModelState.IsValid || request == null)
            {
                response.Success = false;
                response.Message = "User Registration Failed";
                return new BadRequestObjectResult(response);
            }

            var identityUser = new IdentityUser() { UserName = request.UserName, Email = request.Email };
            var result = await _userManager.CreateAsync(identityUser, request.Password);
            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)
                {
                    dictionary.AddModelError(error.Code, error.Description);
                }
                response.Success = false;
                response.Message = "User Registration Failed";
                response.Data = dictionary;
                return new BadRequestObjectResult(response);
            }
            response.Success = true;
            response.Message = "User Registration Successful";
            return Ok(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Response response = new Response();
            IdentityUser identityUser;

            if (!ModelState.IsValid
                || request == null
                || (identityUser = await ValidateUser(request)) == null)
            {
                response.Success = false;
                response.Message = "Login failed";
                return new BadRequestObjectResult(response);
            }

            var token = GenerateToken(identityUser);
            response.Success = true;
            response.Message = "Login successful";
            response.Data = new LoginResponse 
            { 
                UserName = request.Username, 
                Token = token 
            };
            return Ok(response);
        }

        private async Task<IdentityUser> ValidateUser(LoginRequest request)
        {
            var identityUser = await _userManager.FindByNameAsync(request.Username);
            if (identityUser != null)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, request.Password);
                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }

            return null;
        }


        private string GenerateToken(IdentityUser identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtBearerTokenSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identityUser.UserName),
                    new Claim(ClaimTypes.Email, identityUser.Email)
                }),

                Expires = DateTime.UtcNow.AddSeconds(_jwtBearerTokenSettings.ExpiryTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _jwtBearerTokenSettings.Audience,
                Issuer = _jwtBearerTokenSettings.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}