namespace ClinicSystemTest.Enum
{
    public class SystemOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyUserId,
            MoreThanMax,
            ValidationError,



        }

        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        public string ErrorDescription
        {
            get;
            set;
        }

    }
}
