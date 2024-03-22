using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleAppSQWLite
{
    internal class Program
    {
        public static List<Option> options;
        private static DbContextOptions<ApplicationContext> conf;

        //  public static DbContextOptions<ApplicationContext> config;
        static void Main(string[] args)
        {
            conf = getOptionsFromJson();
            // Create options that you want your menu to have
            options = new List<Option>
            {
                new Option("Open data", (conf) => WriteTemporaryMessage("Hi",NewMethod)),
                new Option("Another thing", (conf) =>  WriteTemporaryMessage("How Are You",null)),
                new Option("Yet Another Thing", (conf) =>  WriteTemporaryMessage("Today",null)),
                new Option("Exit", (conf) => Environment.Exit(0)),
            };
            

           

            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            WriteMenu(options, options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke(conf);
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }

        private static DbContextOptions<ApplicationContext> getOptionsFromJson()
        {
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            var connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();                
            DbContextOptions<ApplicationContext> options1 = optionsBuilder.UseSqlite(connectionString).Options;
            return options1;          
           
        }

        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message,Action<DbContextOptions<ApplicationContext>> run)
        {
            Console.Clear();
            Console.WriteLine(message);
            run(conf);
           // Thread.Sleep(3000);
           Console.ReadKey(true);
            WriteMenu(options, options.First());
        }



        static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }

        }
    

        private static void NewMethod(DbContextOptions<ApplicationContext> ops)
        {
            if (ops == null) return;
           
            using (ApplicationContext db = new ApplicationContext(ops))
            {
                bool isCreated = db.Database.EnsureCreated();
                // bool isCreated2 = await db.Database.EnsureCreatedAsync();
                if (isCreated) Console.WriteLine("База данных была создана");
                else Console.WriteLine("База данных уже существует");
                // получаем объекты из бд и выводим на консоль
                var users = db.Users.ToList();
                Console.WriteLine("Список объектов:");
                foreach (User u in users)
                {
                    Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                }
            }
        }
    }

    public class Option
    {
        public string Name { get; }
        public Action<DbContextOptions<ApplicationContext>> Selected { get; }

        public Option(string name, Action<DbContextOptions<ApplicationContext>> selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}