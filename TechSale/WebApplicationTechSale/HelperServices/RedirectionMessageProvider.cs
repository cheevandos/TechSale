using System.Collections.Generic;

namespace WebApplicationTechSale.HelperServices
{
    public static class RedirectionMessageProvider
    {
        public static List<string> LotAcceptedMessages()
        {
            return new List<string>()
            {
                "Модерация прошла успешно",
                "Лот опубликован и теперь виден всем пользователям",
                "Сейчас вы будете перенаправлены на страницу с лотами"
            };
        }

        public static List<string> LotRejectedMessages()
        {
            return new List<string>()
            {
                "Вы отклонили публикацию лота",
                "Пользователь получит уведомление и сможет отредактировать данные для повторной публикации",
                "Сейчас вы будете перенаправлены на страницу c лотами",
            };
        }
    }
}
