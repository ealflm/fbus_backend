using System.ComponentModel.DataAnnotations;

namespace FBus.Business.Authorization.SearchModel
{
    public class ModifiedPasswordModel
    {
        [Required] public string Username { get; set; }

        [Required] public string OldPassowrd { get; set; }

        [Required] public string NewPassword { get; set; }
    }
}