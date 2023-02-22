using System.Security.Claims;

namespace books.Authorization
{
    public static class Claims
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Member = "Member";

        public static readonly Claim MemberClaim = new(Member, Member);
        public static readonly Claim AdminClaim = new(Admin, Admin);
        public static readonly Claim SuperAdminClaim = new(SuperAdmin, SuperAdmin);
    }
}
