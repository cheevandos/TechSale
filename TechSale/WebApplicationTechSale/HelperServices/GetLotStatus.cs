namespace WebApplicationTechSale.HelperServices
{
    public static class GetLotStatus
    {
        public static string GetOnModerationStatus()
        {
            return "На модерации";
        }

        public static string GetRejectedStatus()
        {
            return "Отклонен";
        }

        public static string GetAcceptedStatus()
        {
            return "Размещён";
        }
    }
}
