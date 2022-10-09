using FBus.Business.Authorization.Interfaces;
using FBus.Business.Authorization.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.Authorization.Implements
{
  public class AuthorizationService : BaseService, IAuthorizationService
  {
    private readonly IConfiguration _configuration;
    private readonly FirebaseApp _firebaseApp;
    private readonly FirebaseAuth _firebaseAuth;
    public AuthorizationService(IUnitOfWork unitOfWork, IConfiguration configuration, FirebaseApp firebaseApp) : base(unitOfWork)
    {
      _configuration = configuration;
      _firebaseApp = firebaseApp;
      _firebaseAuth = FirebaseAuth.GetAuth(_firebaseApp);
    }

    private async Task<Response> LoginUserNamePassword<T>(string userName, string password, T result, Login loginType) where T : class
    {
      string message = null;
      dynamic user = null;

      switch ((int)loginType)
      {
        case 0:
          {
            user = await _unitOfWork.AdminRepository.Query()
            .Where(x => x.UserName == userName && x.Password != null)
            .FirstOrDefaultAsync();
            break;
          }
        case 1:
          {
            user = await _unitOfWork.DriverRepository.Query()
            .Where(x => x.Phone == userName && x.Password != null)
            .FirstOrDefaultAsync();
            break;
          }

      }



      if (user != null && VerifyPassword(password, user.Password, user.Salt))
      {
        if (((int)loginType == 1 && user.Status != 0) || (int)loginType == 0)
        {
          foreach (var x in result.GetType().GetProperties())
          {
            foreach (var y in user.GetType().GetProperties())
            {
              if (x.Name.Equals(y.Name))
              {
                x.SetValue(result, y.GetValue(user));
              }
            }
          }
        }
        else
        {
          return new()
          {
            StatusCode = (int)StatusCode.BadRequest,
            Message = Message.CustomContent("The user doesn't have permission to access this resource")
          };
        }
      }
      else
      {
        return new()
        {
          StatusCode = (int)StatusCode.BadRequest,
          Message = "Invalid user name or password"
        };

      }

      return new Response(200, result, message);
    }

    private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
      for (int i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != passwordHash[i]) return false;
      }

      return true;
    }

    public async Task<Response> Login(LoginSearchModel model, Login loginType)
    {
      Response resultModel = null;
      string result = null;

      if (!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password))
      {
        switch ((int)loginType)
        {
          case 0:
            {
              resultModel = await LoginUserNamePassword<AdminViewModel>(model.UserName, model.Password, new AdminViewModel(), loginType);
              break;
            }
          case 1:
            {
              resultModel = await LoginUserNamePassword<DriverViewModel>(model.UserName, model.Password, new DriverViewModel(), loginType);
              break;
            }
        }

      }

      if (resultModel.Data != null)
      {
        switch ((int)loginType)
        {
          case 0:
            {
              result = GetToken((AdminViewModel)resultModel.Data, 0);
              break;
            }
          case 1:
            {
              result = GetToken((DriverViewModel)resultModel.Data, 1);
              break;
            }
        }

      }
      else
      {
        return new()
        {
          StatusCode = (int)StatusCode.BadRequest,
          Message = resultModel.Message
        };
      }

      return new()
      {
        StatusCode = (int)StatusCode.Ok,
        Data = result,
        Message = Message.CustomContent("Đăng nhập thành công"),
      };
    }

    private string GetToken<T>(T model, int loginType) where T : class
    {
      var authClaims = new List<Claim>();
      foreach (var x in model.GetType().GetProperties())
      {
        authClaims.Add(new Claim(x.Name, (x.GetValue(model) ?? "").ToString()));
      }

      switch ((int)loginType)
      {
        case 0:
          {
            authClaims.Add(new Claim("Role", "Admin"));
            break;
          }
        case 1:
          {
            authClaims.Add(new Claim("Role", "Driver"));
            break;
          }
        case 2:
          {
            authClaims.Add(new Claim("Role", "Student"));
            break;
          }
      }

      authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
      var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

      var token = new JwtSecurityToken(
          expires: DateTime.Now.AddDays(1),
          claims: authClaims,
          signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<Response> LoginGoogle(string idToken)
    {
      StudentViewModel result = null;
      string message = null;

      string uid = null;
      try
      {
        var decodedToken = await _firebaseAuth
            .VerifyIdTokenAsync(idToken);
        uid = decodedToken.Uid;
      }
      catch
      {
        // ignored
      }

      if (uid != null)
      {
        var user = await _unitOfWork.StudentRepository.Query()
            .Where(x => x.Uid == uid)
            .FirstOrDefaultAsync();

        if (user != null)
        {
          if (user.Status == 1)
          {
            result = new StudentViewModel()
            {
              StudentId = user.StudentId,
              FullName = user.FullName,
              Email = user.Email,
              Phone = user.Phone,
              PhotoUrl = user.PhotoUrl,
              AutomaticScheduling = user.AutomaticScheduling,
              Status = user.Status
            };
          }
          else
          {
            message = "The user doesn't have permission to access this resource";
          }
        }
        else
        {
          var userInfo = await _firebaseAuth.GetUserAsync(uid);
          var status = 0;

          var model = await _unitOfWork.StudentRepository.Query()
              .Where(x => x.Email == userInfo.Email)
              .FirstOrDefaultAsync();

          if (model != null)
          {
            model.FullName = userInfo.DisplayName;
            model.Email = userInfo.Email;
            model.Phone = userInfo.PhoneNumber;
            model.PhotoUrl = userInfo.PhotoUrl;
            model.Uid = userInfo.Uid;
            status = model.Status;

            _unitOfWork.StudentRepository.Update(model);

            result = new StudentViewModel()
            {
              FullName = userInfo.DisplayName,
              Email = userInfo.Email,
              Phone = userInfo.PhoneNumber,
              PhotoUrl = userInfo.PhotoUrl,
              AutomaticScheduling = model.AutomaticScheduling,
              Status = model.Status
            };
          }
          else
          {
            var studentModel = new Student()
            {
              StudentId = Guid.NewGuid(),
              FullName = userInfo.DisplayName,
              Email = userInfo.Email,
              Phone = userInfo.PhoneNumber,
              PhotoUrl = userInfo.PhotoUrl,
              AutomaticScheduling = false,
              Uid = userInfo.Uid,
              Status = 0
            };
            await _unitOfWork.StudentRepository.Add(studentModel);

            result = new StudentViewModel()
            {
              FullName = userInfo.DisplayName,
              Email = userInfo.Email,
              Phone = userInfo.PhoneNumber,
              PhotoUrl = userInfo.PhotoUrl,
              AutomaticScheduling = false,
              Status = 0
            };
          }


          await _unitOfWork.SaveChangesAsync();
        }
      }
      else
      {
        message = "Invalid ID token";
      }

      return new Response(200, GetToken((StudentViewModel)result, 2), message);
    }

    public async Task<Response> Register(LoginSearchModel model)
    {
      CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
      var admin = new Admin()
      {
        AdminId = Guid.NewGuid(),
        Password = passwordHash,
        Salt = passwordSalt,
        UserName = model.UserName
      };
      await _unitOfWork.AdminRepository.Add(admin);
      await _unitOfWork.SaveChangesAsync();
      return new Response(200, "Created admin");
    }
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using var hmac = new System.Security.Cryptography.HMACSHA512();
      passwordSalt = hmac.Key;
      passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

  }
}
