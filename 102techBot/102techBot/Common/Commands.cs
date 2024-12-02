namespace _102techBot.Common
{
    enum ClientCmd
    {
        Catalog,
        WaitEnterName,
        Introduced,
        InProcess,
        Completed,
        CreatingOrder,
        ConfirmingOrder,
        OrderCompleted
    }
    internal static class Commands
    {
        public static List<Command> GetDefaultMainMenuCommands()
        {
            return new List<Command>()
            {
                new Command()
                {
                    Cmd = "/start",
                    Description = "Начало общения с ботом"
                },
                new Command()
                {
                    Cmd = "/help",
                    Description = "Помощь"
                }
            };
        }

        public static List<Command> GetMainMenuCommandsIntroducedUser()
        {
            return new List<Command>()
            {
                new Command()
                {
                    Cmd = "/start",
                    Description = "Начало общения с ботом"
                },
                //new Command()
                //{
                //    Cmd = "/catalog",
                //    Description = "Каталог товаров"
                //},
                //new Command()
                //{
                //    Cmd = "/repair_create",
                //    Description = "Запись на ремонт"
                //},
                //new Command()
                //{
                //    Cmd = "/shops_services",
                //    Description = "Магазины и сервисы"
                //},
                //new Command()
                //{
                //    Cmd = "/callback_create",
                //    Description = "Обратный звонок"
                //}
            };
        }
    }

    internal class Command
    {
        public string Cmd { get; set; }
        public string Description { get; set; }
    }
}
