using Microsoft.AspNetCore.Identity;
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


        //OVERLAD edicez yukarıdaki method list istiyor biz IEnumerable<IdenttyError> vercez bunun sebebi eski metoddan bakarsan metodun içine verdiğimiz IdentityResult.Error dan geliyor.
        //yukarıdaki metodda sürekli controllerda içine liste verip x.description şeklinde yazıyoduk bu kod kalabalığından kurtulcaz.
        public static void AddModelErrorList(this ModelStateDictionary modelState,IEnumerable<IdentityError> errors) //modelstatedictionary için yapılan extension bu.
        {
            //bu Identityerror nesnesinin errors instanceina git listele tek tek dön ve description yaz.
            errors.ToList().ForEach(error =>
            {
                modelState.AddModelError(string.Empty, error.Description);  //signin signup methodlarında extension oluşturdul
            });

        }

    }
}
