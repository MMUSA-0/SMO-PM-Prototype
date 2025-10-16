

namespace Framework.Identity.Data
{
    public enum SystemUserRole
    {
        SuperAdmin = 1,
        LearningPartner = 2,
        Coaches = 3,
        Participant = 4,
    }

    public enum IdentityTypeEnum
    {
        National = 1,
        Resident = 2,
        Visitor = 3,
        VisitorOmrahVisa = 4,
        VisitorHajjVisa = 5
    }
    public enum LoginStatusEnum
    {
        EmailNotRegistered = 1,
        EmailActivationSent = 2,
        EmailRegisteredAndActiveInternal = 3,
        EmailRegisteredAndActiveExternal = 4,
        ResetPasswordSent = 5,
        EmailTemporaryBlocked = 6,
        EmailNotActive = 7,
        AccountValidToLogin = 8,
        AccountOtpSent = 9,
        AccountInValidToLogin = 10
    }


    public enum GenerationTokenEnum
    {
        ResetPassword = 1 ,
        ActivateAccount = 2,
        LoginOTP = 3
    }
}
