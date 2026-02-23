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
        UpdateFailed = 3052,

        InvalidAttendanceDate = 4001,
        DataNotFound = 4002,
        UpgradeLevelMismatch = 4003,

        NotEnoughCredit = 5001,

        UnKnownError = 9000,
        DBTransactionError = 9001,
    }
}
