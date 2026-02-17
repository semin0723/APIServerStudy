namespace APIServerStudy
{
    public enum ErrorCode : UInt16
    {
        None = 0,
        RegistSuccess = 1,
        InvalidUserID = 1000,
        InvalidPassword = 1001,
        UserAlreadyExists = 1002,

        AuthTokenNotMatch = 3001,

        RegisterFailed = 3051,

        InvalidAttendanceDate = 4001,

        UnKnownError = 9000,
    }
}
