using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Event_Management_API_Light.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Event_Management_API_Light.Controllers;

[Route("api/[controller]")] 
[ApiController]

public class AuthController : ControllerBase
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly SignInManager<IdentityUser> _signInManager;
  private readonly IConfiguration _configuration;

  public AuthController(UserManager<IdentityUser> userManager, 
    SignInManager<IdentityUser> signInManager,
    IConfiguration configuration)
  {
    _configuration = configuration;
    _userManager = userManager;
    _signInManager = signInManager;
  }
  
  [HttpPost("register")]
  public async Task<IActionResult>Register([FromBody] RegisterDto registerDto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var user = new IdentityUser { UserName = registerDto.Email, Email = registerDto.Email };
    var result = await _userManager.CreateAsync(user, registerDto.Password);
    if (result.Succeeded)
    {
      return Ok(new { Message = "User registered successfully." });
    }
    return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
  }
  
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }
    var user = await _userManager.FindByEmailAsync(loginDto.Email);
    if (user == null)
    {
      return Unauthorized(new { Message = "Invalid email or password." });
    }

    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
    if(!result.Succeeded)
    {
      return Unauthorized(new { Message = "Invalid email or password." });
    }

    var authClaims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.Email, user.Email),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim(ClaimTypes.NameIdentifier, user.Id)
    };
    
    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
    
    var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"],     
      audience: _configuration["Jwt:Audience"], 
      expires: DateTime.Now.AddHours(3),       
      claims: authClaims,                      
      signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256) // Signatur
    );
    
    return Ok(new
    {
      Token = new JwtSecurityTokenHandler().WriteToken(token),
      Expiration = token.ValidTo 
    });

  }

  
}