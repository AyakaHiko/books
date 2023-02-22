namespace books.Authorization
{
    public class Policies
    {
        public const string SuperAdminAccessOnly = "SuperAdminAccessOnly";
        public const string AdminAndAboveAccess = "AdminAccess";
        public const string MemberAndAboveAccess = "MemberAccess";
    }
}
