using System.ComponentModel.DataAnnotations;


namespace FBus.Business.Authorization.SearchModel
{
    public class LoginSearchModel
    {
        [Required] public string UserName { get; set; }

        [Required] public string Password { get; set; }
    }
}
