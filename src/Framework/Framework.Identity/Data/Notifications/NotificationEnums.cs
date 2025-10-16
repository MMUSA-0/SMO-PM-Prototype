namespace Framework.Identity
{

    public enum EmailTemplateNames
    {
        Users_ForgotPasswordEmail,
        Users_AccountActivationEmail,
        Users_ResetPasswordEmail,
        Users_WelcomeEmail,
        Users_ChangeEmail,
        Users_EmailVerification,
        CompanyRequestAddEmail,        
        AccountRegistrationCreatedByAdmin,
        Users_ConfirmEmail,
        Submissions_VerifyProjectSubmissionEmail,
        Users_RegistrationPendingApprovalEmail,
        Users_RegistrationRejectionEmail,
        Submissions_ApprovedProjectSubmissionEmail,
        Submissions_RejectedProjectSubmissionEmail,
        Submissions_InProgressProjectSubmissionEmail,
        Submissions_ReturnedProjectSubmissionEmail,
        Workflow_RequestDetailsEmail,
        Users_LoginOTPEmail,
        Admin_LoginOTPEmail
    }

    public enum WebNotificationTemplate
    {
        Users_ConfirmEmail = 1,
    }


    public enum MobileTemplateNames
    {
    }

    public enum SmsTemplateNames
    {
        Users_LoginOTPSMS,
        Users_PhoneVerificationSMS,
        //Users_AccountActivationSMS,
        //Users_ResetPasswordSMS,
        Users_WelcomeSMS,
        CompanyRequestAddSMS,
        Users_RegistrationPendingApprovalSMS,
        Users_RegistrationRejectionSMS
    }

    public enum LoginStatusEnum
    {
        EmailNotRegistered = 1,
        EmailActivationSent = 2,
        EmailRegisteredAndActiveInternal = 3,
        EmailRegisteredAndActiveExternal = 4,
        ResetPasswordSent = 5,
        EmailTemporaryBlocked = 6,
        AdminLogin=7
    }
}
