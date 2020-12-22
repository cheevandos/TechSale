namespace DataAccessLogic.HelperServices
{
    public static class ApplicationConstantsProvider
    {
        public static int GetPageSize()
        {
            return 6;
        }

        public static int GetShortRedirectionTime()
        {
            return 5;
        }

        public static int GetLongRedirectionTime()
        {
            return 10;
        }

        public static int GetMaxRedirectionTime()
        {
            return 15;
        }
    }
}
