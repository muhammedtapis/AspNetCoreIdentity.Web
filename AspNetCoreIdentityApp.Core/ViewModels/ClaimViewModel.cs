namespace AspNetCoreIdentity.Core.ViewModels
{
    public class ClaimViewModel
    {
        //claim listeleme için
        public string Issuer { get; set; } = null!;   //claimi dağıtan kim üçünüc parti bi yerden de gelebilir.

        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}