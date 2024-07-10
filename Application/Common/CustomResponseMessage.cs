namespace Domain.Models
{
    public static class CustomResponseMessage
    {
        public static String Success(string operation)
        {
            return $"{operation} Sucessfull";
        }
        public static String Error(string operation)
        {
            return $"{operation} Error";
        }
    }
}
