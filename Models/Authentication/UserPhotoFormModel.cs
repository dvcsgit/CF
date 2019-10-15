using System.Web;
using Utility.Models;

namespace Models.Authentication
{
    public class UserPhotoFormModel
    {
        public RequestResult RequestResult { get; set; }

        public HttpPostedFileBase Photo { get; set; }
    }
}
