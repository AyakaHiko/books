namespace books.Models.ViewModels.SuperAdminViewModel
{
    public class UserAccessVm
    {
        public string Email { get; set; } = default!;
        public Access Access { get; set; } = default!;
    }

    public enum Access
    {
        Admin,
        Member,
        None
    }
}
