using Ecommerce_platforms.API.ModelsDTO;
using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Repository.Data.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_platforms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;
        private readonly SignInManager<AppUser> _signInManager;
        public ProfileController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IImageService imageService)
        {
            _userManager = userManager;
            _imageService = imageService;
            _signInManager = signInManager;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password has been changed successfully.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }

        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file provided.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("User not found.");

            try
            {
                var imageUrl = await _imageService.SaveImageAsync(file);
                user.ProfileImageUrl = imageUrl;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest(result.Errors);

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                // Log error (optional) and return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while uploading the image.");
            }
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO updateProfileDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(updateProfileDto.Email);
            if (user == null) return NotFound("User not found.");

            // Check if the password is correct
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, updateProfileDto.Password, lockoutOnFailure: false);
            if (!passwordCheck.Succeeded) return Unauthorized("Invalid password.");

            // Map DTO fields to the user entity for profile update
            user.FName = updateProfileDto.FName;
            user.LName = updateProfileDto.LName;
            user.Country = updateProfileDto.Country;
            user.City = updateProfileDto.City;
            user.Street = updateProfileDto.Street;
            user.PhoneNumber = updateProfileDto.PhoneNumber;

            // Update the user profile
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Profile updated successfully.");
        }

    }
}
