using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class ModelStateExtensions
    {

        public static void AddModelErrorList(this ModelStateDictionary modelState,List<string> errors) //modelstatedictionary için yapılan extension bu.
        {

            errors.ForEach(error =>
            {
                modelState.AddModelError(string.Empty, error);  //signin signup methodlarında extension oluşturdul
            });
            
        }

    }
}
