using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using UserApi.Data.Models;
using UserApi.Models;
using UserApi.Services;


namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMemoryCache cache;
        private readonly EmailService emailService;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,IMemoryCache cache,EmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.cache = cache;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] dtoNewUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FullName = model.Name,
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpperInvariant(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpperInvariant(),
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate,
                SelectGender = model.SelectGender,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            try
            {
                if (roleManager == null)
                {
                    return StatusCode(500, new { message = "RoleManager service is not available." });
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole("User"));
                    if (!roleResult.Succeeded)
                    {
                        return StatusCode(500, new { message = "Failed to create User role." });
                    }
                }

                await userManager.AddToRoleAsync(user, "User");

                int verificationCode = Random.Shared.Next(100000, 999999);
                string cacheKey = $"VerificationCode_{user.Email.ToLowerInvariant()}";
                cache.Set(cacheKey, verificationCode, TimeSpan.FromMinutes(10));

                await emailService.SendVerificationEmail(model.Email, verificationCode);

                return Ok(new { message = "User registered successfully. Verification email sent." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestdto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found!" });
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to change password.", errors = changePasswordResult.Errors });
            }

            return Ok(new { message = "Password changed successfully!" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Logindto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> EmailVerification([FromBody] EmailVerificationdto model)
        {
            string cacheKey = $"VerificationCode_{model.Email.ToLowerInvariant()}";

            if (!cache.TryGetValue(cacheKey, out int storedCode))
            {
                return BadRequest(new { message = "Verification code expired or invalid." });
            }

            if (model.VerificationCode != storedCode)
            {
                return BadRequest(new { message = "Invalid verification code." });
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);

            // Remove the verification code from cache after successful verification
            cache.Remove(cacheKey);

            return Ok(new { message = "Email verified successfully." });
        }
    }
}



