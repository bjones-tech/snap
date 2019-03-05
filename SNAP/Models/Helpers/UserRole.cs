using System.Security.Principal;

namespace SNAP.Models.Helpers
{
    public class UserRole
    {
        public static bool IsDeveloper(IPrincipal user)
        {
            return user.Identity.Name == "NA\\bjones";
        }

        public static bool IsSNAPAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.Admins");
        }

        public static bool IsEmployeeAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.EmployeeAdmins");
        }

        public static bool IsContingentAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.ContingentAdmins");
        }

        public static bool IsEvalutionAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.EvaluationAdmins");
        }

        public static bool IsCandidateRecruiter(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.CandidateRecruiters");
        }

        public static bool IsITAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.ITAdmins");
        }

        public static bool IsPasswordAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.PasswordAdmins");
        }

        public static bool IsSoftwareUpdateAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.SoftwareUpdateAdmins");
        }

        public static bool IsGSCUserAdmin(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.GSCUserAdmins");
        }

        public static bool IsFacilities(IPrincipal user)
        {
            return user.IsInRole("AM.SNAP.Facilities");
        }
    }
}