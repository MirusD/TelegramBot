using _102techBot.Domain.Entities;

namespace _102techBot.Common
{
    internal static class Buttons
    {
        public static List<Button> GetMainMenuBtns(User user)
        {
            var mainMenu = new List<Button>();

            switch (user.Role)
            {
                case UserRole.Administrator:
                    mainMenu.Add(new Button
                    {
                        Text = "Управление пользователями",
                        Data = "/users_control",
                        Ln = true
                    });                    
                    mainMenu.Add(new Button
                    {
                        Text = "Управление товарами",
                        Data = "/products_control",
                        Ln = true
                    });
                    mainMenu.Add(new Button
                    {
                        Text = "Добавить адресс",
                        Data = "/address_create",
                        Ln = true
                    });
                    break;

                case UserRole.SalesManager:
                    mainMenu.Add(new Button
                    {
                        Text = "Заявки на покупку",
                        Data = "/orders",
                        Ln = true
                    });
                    break;

                case UserRole.ServiceEngineer:
                    mainMenu.Add(new Button
                    {
                        Text = "Заявки на ремонт",
                        Data = "/repairs",
                        Ln = true
                    });
                    break;

                case UserRole.Client:
                case UserRole.Candidate:
                    mainMenu.Add(new Button
                    {
                        Text = "Каталог товаров",
                        Data = "/catalog",
                        Ln = true
                    });
                    mainMenu.Add(new Button
                    {
                        Text = "Корзина",
                        Data = "/cart",
                        Ln = true
                    });
                    mainMenu.Add(new Button
                    {
                        Text = "Запись на ремонт",
                        Data = "/repair_create",
                    });
                    mainMenu.Add(new Button
                    {
                        Text = "Обратный звонок",
                        Data = "/callback_create",
                        Ln = true
                    });
                    mainMenu.Add(new Button
                    {
                        Text = "Магазины и сервисы",
                        Data = "/shops_services",
                    });

                    if (user.Role != UserRole.Candidate)
                    {
                        mainMenu.Add(new Button
                        {
                            Text = "Ищу работу",
                            Data = "/search_job_add_candidate",
                            Ln = true
                        });
                    }
                    break;
            }

            return mainMenu;
        }
        public static List<Button> GetCategoryBtns(List<Category> categories)
        {
            var btnsList = new List<Button>();

            foreach (var category in categories)
            {
                btnsList.Add(new Button { Text = category.Name, Data = $"/category/{category.Id}", Ln = true });
            }

            return btnsList;
        }

        public static List<Button> GetCategoryBtns(string path, List<Category> categories)
        {
            var btnsList = new List<Button>();

            foreach (var category in categories)
            {
                btnsList.Add(new Button { Text = category.Name, Data = $"{path}_select_category/{category.Id}", Ln = true });
            }

            return btnsList;
        }

        public static List<Button> GetProductBtns(List<Product> products)
        {
            var btnsList = new List<Button>();

            foreach (var product in products)
            {
                btnsList.Add(new Button { Text = product.Name, Data = $"/model/{product.Id}", Ln = true });
            }

            return btnsList;
        }

        public static List<Button> GetProductBtns(string path, List<Product> products)
        {
            var btnsList = new List<Button>();

            foreach (var product in products)
            {
                btnsList.Add(new Button { Text = product.Name, Data = $"{path}_select_model/{product.Id}", Ln = true });
            }

            return btnsList;
        }

        public static List<Button> GetUserBtns(List<User> users)
        {
            var btnsList = new List<Button>();

            foreach (var user in users)
            {
                btnsList.Add(new Button { Text = $"{user.UserName}\n{user.FirstName}", Data = $"/user/{user.Id}", Ln = true });
            }

            return btnsList;
        }

        public static List<Button> GetConfirmBtns()
        {
            var buttons = new List<Button>
            {
                new Button
                {
                    Text = "Да",
                    Data = "/yes",
                },
                new Button
                {
                    Text = "Нет",
                    Data = "/no",
                },
            };

            return buttons;
        }

        public static List<Button> GetConfirmBtns(string opt)
        {
            var buttons = new List<Button>
            {
                new Button
                {
                    Text = "Да",
                    Data = $"{(!String.IsNullOrEmpty(opt) ? $"/{opt}" : "")}_yes",
                },
                new Button
                {
                    Text = "Нет",
                    Data = $"{(!String.IsNullOrEmpty(opt) ? $"/{opt}" : "")}_no",
                },
            };

            return buttons;
        }

        public static List<Button> GetWorkWithUsersBtns()
        {
            var btns = new List<Button>() {
                new Button
                {
                    Text = "Сотрудники",
                    Data = "/users/employes",
                    Ln = true
                },
                new Button
                {
                    Text = "Кандидаты",
                    Data = "/users/candidates",
                    Ln = true
                },
                new Button
                {
                    Text = "Клиенты",
                    Data = "/users/clients",
                    Ln = true
                }
            };

            return btns;
        }

        public static List<Button> GetWorkProductsBtns()
        {
            var btns = new List<Button>() {
                new Button
                {
                    Text = "Добавить категорию",
                    Data = "/add_category",
                    Ln = true
                },
                new Button
                {
                    Text = "Добавить товар",
                    Data = "/add_product",
                    Ln = true
                },
            };

            return btns;
        }

        public static List<Button> GetRolesBtns(string userId)
        {
            var btnsList = new List<Button>();

            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                btnsList.Add(new Button { Text = role.ToString(), Data = $"/select_role/{(int)role}/{userId}", Ln = true });
            }

            return btnsList;
        }
    }
}
