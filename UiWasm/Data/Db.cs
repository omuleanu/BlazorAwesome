using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omu.ValueInjecter;

namespace UiWasm.Data
{
    public static class Db
    {
        public static IList<Lunch> Lunches = new List<Lunch>();
        public static IList<Country> Countries = new List<Country>();
        public static IList<Chef> Chefs = new List<Chef>();
        public static IList<Meal> Meals = new List<Meal>();
        public static IList<Food> Foods = new List<Food>();
        public static IList<Category> Categories = new List<Category>();
        public static IList<Organisation> Organisations = new List<Organisation>();

        public static IEnumerable<Dinner> Dinners => dinners.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<ParentMeal> ParentMeals => parentMeals.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<Message> Messages => messages.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<Restaurant> Restaurants => restaurants.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<RestaurantAddress> RestaurantAddresses => restaurantAddresses.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<TreeNode> TreeNodes => treeNodes.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<Spreadsheet> Spreadsheets => spreadsheets.Values.Where(o => !o.IsDeleted);
        public static IEnumerable<Meeting> Meetings => meetings.Values.Where(o => !o.IsDeleted);

        private static readonly ConcurrentDictionary<int, Dinner> dinners = new ConcurrentDictionary<int, Dinner>();
        private static readonly ConcurrentDictionary<int, ParentMeal> parentMeals = new ConcurrentDictionary<int, ParentMeal>();
        private static readonly ConcurrentDictionary<int, Message> messages = new ConcurrentDictionary<int, Message>();
        private static readonly ConcurrentDictionary<int, Spreadsheet> spreadsheets = new ConcurrentDictionary<int, Spreadsheet>();
        private static readonly ConcurrentDictionary<int, Restaurant> restaurants = new ConcurrentDictionary<int, Restaurant>();
        private static readonly ConcurrentDictionary<int, RestaurantAddress> restaurantAddresses = new ConcurrentDictionary<int, RestaurantAddress>();
        private static readonly ConcurrentDictionary<int, TreeNode> treeNodes = new ConcurrentDictionary<int, TreeNode>();
        private static readonly ConcurrentDictionary<int, Meeting> meetings = new ConcurrentDictionary<int, Meeting>();

        private static int gid = 151;

        public static int NewId()
        {
            lock (lockObj)
            {
                return gid += 2;
            }
        }

        private static readonly object lockObj = new object();


        public static T Add<T>(T o) where T : Entity
        {
            return Insert(o);
        }

        public static T Insert<T>(T o) where T : Entity
        {
            lock (lockObj)
            {
                o.Id = gid += 2;
            }

            o.DateCreated = DateTime.UtcNow;

            var dict = (ConcurrentDictionary<int, T>)Dict<T>();

            if (dict != null)
            {
                if (dict.TryAdd(o.Id, o))
                {
                    return o;
                }
            }

            throw new Exception("can not add new item");
        }

        public static T Find<T>(int? id) where T : Entity
        {
            return Get<T>(id.Value);
        }

        public static T Get<T>(int? id) where T : Entity
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return Get<T>(id.Value);
        }

        public static Task<T> GetAsync<T>(int? id) where T : Entity
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return Task.Run(() => Get<T>(id.Value));
        }

        public static T Get<T>(int id) where T : Entity
        {
            T entity;
            var list = List<T>();
            if (list != null)
            {
                entity = ((IList<T>)list).SingleOrDefault(o => o.Id == id);
            }
            else
            {
                var dict = (ConcurrentDictionary<int, T>)Dict<T>();
                dict.TryGetValue(id, out entity);
            }

            if (entity == null || entity.IsDeleted) throw new Exception("this item doesn't exist anymore");
            return entity;
        }

        public static async Task<IQueryable<T>> Query<T>() where T : Entity
        {
            IEnumerable<T> set;
            var list = List<T>();
            if (list != null)
            {
                set = (IList<T>)list;
            }
            else
            {
                var dict = (ConcurrentDictionary<int, T>)Dict<T>();
                set = dict.Values;
            }

            await Task.Delay(1000);

            return set.Where(o => !o.IsDeleted).AsQueryable();
        }

        public static IEnumerable<T> Set<T>() where T : Entity
        {
            IEnumerable<T> set;
            var list = List<T>();
            if (list != null)
            {
                set = (IList<T>)list;
            }
            else
            {
                var dict = (ConcurrentDictionary<int, T>)Dict<T>();
                set = dict.Values;
            }

            return set;
        }

        public static void Update<T>(T o) where T : Entity
        {
            var entity = Get<T>(o.Id);
            entity.InjectFrom(o);
        }

        public static void Remove<T>(T entity) where T : Entity
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Delete<T>(entity.Id);
        }

        public static void Delete<T>(int id) where T : Entity
        {
            if (Set<T>().Count(o => !o.IsDeleted) < 10)
            {
                RestoreItems();
            }

            var ent = Get<T>(id);
            ent.DateDeleted = DateTime.UtcNow;
            ent.IsDeleted = true;
        }

        public static void RestoreItems()
        {
            Action<IEnumerable<Entity>> restore = entities =>
            {
                foreach (var o in entities)
                {
                    if (o.IsDeleted)
                    {
                        o.IsDeleted = false;
                    }
                }
            };

            restore(dinners.Values);
            restore(messages.Values);
            restore(meetings.Values);
            restore(restaurants.Values);
            restore(restaurantAddresses.Values);
            restore(spreadsheets.Values);
            restore(treeNodes.Values);
            restore(meetings.Values);
        }

        private static object List<T>()
        {
            return List(typeof(T));
        }

        private static object List(Type type)
        {
            if (type == typeof(Lunch)) return Lunches;
            if (type == typeof(Country)) return Countries;
            if (type == typeof(Chef)) return Chefs;
            if (type == typeof(Meal)) return Meals;
            if (type == typeof(Food)) return Foods;
            if (type == typeof(Category)) return Categories;
            return null;
        }

        private static object Dict<TEntity>() where TEntity : Entity
        {
            var type = typeof(TEntity);

            if (type == typeof(Dinner)) return dinners;
            if (type == typeof(ParentMeal)) return parentMeals;
            if (type == typeof(Message)) return messages;
            if (type == typeof(Spreadsheet)) return spreadsheets;
            if (type == typeof(Restaurant)) return restaurants;
            if (type == typeof(RestaurantAddress)) return restaurantAddresses;
            if (type == typeof(TreeNode)) return treeNodes;
            if (type == typeof(Meeting)) return meetings;

            return null;
        }

        private static void add<T>(T o) where T : Entity
        {
            o.DateCreated = DateTime.UtcNow;
            o.Id = gid += 2;
            var list = (IList<T>)List<T>();
            list.Add(o);
        }

        static Db()
        {
            var categories = new[] { "Fruits", "Legumes", "Vegetables", "Nuts", "Grains" };
            foreach (var category in categories)
            {
                add(new Category { Name = category });
            }

            add(new Meal { Name = "Mango", Category = Categories[0], Description = "The mango is a fleshy stone fruit belonging to the genus Mangifera" });
            add(new Meal { Name = "Apple", Category = Categories[0], Description = "The apple is the pomaceous fruit of the apple tree" });
            add(new Meal { Name = "Papaya", Category = Categories[0], Description = "The papaya is a large tree-like plant, with a single stem growing from 5 to 10 metres" });
            add(new Meal { Name = "Banana", Category = Categories[0], Description = "Bananas come in a variety of sizes and colors when ripe, including yellow, purple, and red." });
            add(new Meal { Name = "Cherry", Category = Categories[0], Description = "The cherry is the fruit of many plants of the genus Prunus, and is a fleshy stone fruit" });

            add(new Meal { Name = "Tomato", Category = Categories[1], Description = "The tomato fruit is consumed in diverse ways, including raw, as an ingredient in many dishes and sauces" });
            add(new Meal { Name = "Potato", Category = Categories[1], Description = "A potato is a starchy edible tuber native to South America and cultivated all over the world" });
            add(new Meal { Name = "Cucumber", Category = Categories[1], Description = "Like the tomato, the cucumber is a fruit. Botanically speaking, a fruit is the mature ovary of a flowering plant" });
            add(new Meal { Name = "Onion", Category = Categories[1], Description = " It is possible to chop the greens into small pieces for salads and as a garnish" });
            add(new Meal { Name = "Carrot", Category = Categories[1], Description = "The rich orange color of carrots comes from beta carotene, which also happens to be very good for optical health" });

            add(new Meal { Name = "Celery", Category = Categories[2], Description = "Although originally cultivated for its perceived medicinal qualities, celery has since made the jump into the daily diets" });
            add(new Meal { Name = "Broccoli", Category = Categories[2], Description = "Sometimes broccoli is compared to tiny trees" });
            add(new Meal { Name = "Artichoke", Category = Categories[2], Description = "The globe artichoke enjoys a long history of both lore and cooking preparation" });
            add(new Meal { Name = "Cauliflower", Category = Categories[2], Description = "As a general rule, the head is white, but variants of cauliflower come in purple and green as well" });
            add(new Meal { Name = "Lettuce", Category = Categories[2], Description = "Leaf lettuce is often very lightweight and ruffly, with a wrinkly surface and a soft, almost velvety texture" });

            add(new Meal { Name = "Hazelnuts", Category = Categories[3], Description = "Hazelnuts are produced by hazel trees" });
            add(new Meal { Name = "Chestnuts", Category = Categories[3], Description = "They have creamy white sweet flesh which appears in a number of cuisines" });
            add(new Meal { Name = "Walnuts", Category = Categories[3], Description = "A walnut is a seed from a tree in the genus Juglans" });
            add(new Meal { Name = "Almonds", Category = Categories[3], Description = "Almonds come in two varieties, sweet and bitter" });
            add(new Meal { Name = "Mongongo", Category = Categories[3], Description = "In addition to producing a highly useful lightweight, durable wood, the mongongo tree also yields a distinctive fruit" });

            add(new Meal { Name = "Rice", Category = Categories[4], Description = "Rice is a keystone of the grass family that produces a vast number of grains " });
            add(new Meal { Name = "Wheat", Category = Categories[4], Description = "Wheat is a type of grass grown all over the world for its highly nutritious and useful grain." });
            add(new Meal { Name = "Buckwheat", Category = Categories[4], Description = "Despite the name, buckwheat is not related to wheat" });
            add(new Meal { Name = "Oats", Category = Categories[4], Description = "They have been in cultivation for over 4,000 years" });
            add(new Meal { Name = "Barley", Category = Categories[4], Description = " a source of fermentable material for beer and certain distilled beverages, and as a component of various health foods" });

            var countryNames = new[]
                {
                    "Elwynn Forest", "Stormwind", "Loch Modan", "Westfall",
                    "Ironforge", "Orgrimmar", "Feralas", "Kitej", "Winterspring", "Teldrassil",
                    "La Croisette", "Goldshire", "Sylvanaar", "Redridge Mountains", "Stone Cutter",
                    "Regent", "Piccadilly", "Hatton Garden", "Greville", "Farringdon",
                    "Carpana", "Jisina", "Ghidrimesti"
                };

            var chefNames = new[]
            {
                new [] {"Naked", "Chef"},
                new [] {"Chef", "Chef"},
                new [] {"Bruce", "Nolan"},
                new [] {"Omu", "Man"},
                new [] {"Pepper", "Tomato"},
                new [] {"Charles", "Duchemin"},
                new [] {"Cheyenne", "Goldblum"},
                new [] {"Phoebe", "Phoebes"},
                new [] {"Casse", "Croute"},
                new [] {"Fromage", "Sandwich"},
                new [] {"Josh", "Baskin"},
                new [] {"Hercules", "Oat"},
                new [] {"Joanna", "Stan"},
                new [] {"Peter", "Gibbons"},
                new [] {"Tom", "Smykowski"},
                new [] {"Demeter", "Harvest"},
                new [] {"Hyperion", "Light"},
                new [] {"Gaia", "Earth"},
                new [] {"Chronos", "Timpus"},
            };

            foreach (var country in countryNames)
            {
                add(new Country { Name = country });
            }

            foreach (var nms in chefNames)
            {
                add(new Chef { FirstName = nms[0], LastName = nms[1], Country = Rnd(Countries) });
            }

            var dinnerNames = new[] {"hot chocolate", "believe in miracles", "out and about", "dinner tonight", "watch tv", "last night",
                "toasty", "great", "coffee for me", "dinner with friends", "school lunch",
                "eating at the pub", "cooking in the garden", "ninja dinner", "broccoli power", "eating out", "Awesome dinner",
                "Uber dinner", "Fruity dish", "Nice dish", "Nerds eating", "Pros eating", "Morning Coffee", "Muffin",
                "Picnic", "Dejeuner", "Breakfast", "petit dej", "Apero",
                "Snacks and movie", "Morning meal", "Morning Tea", "Cookies", "Apples", "Salad"};

            var ppl = new[]{
                "Aaron", "James", "Jeremy", "Richard", "Tommy", "Alan", "Charlie", "Jake", "Penny", "Sheldon", "Leonard", "Zazzles", "Howard", "Rajesh",
                "Phil", "Scott", "Bernadette", "Russell", "Timmy", "Jeff", "Adam", "Jennifer", "Audrey", "Claudia", "Brenda", "Tracy", "Alice", "Pierre",
                "Ryu", "Ken", "Irene", "Henry", "Lucas", "Maurice", "Luke", "Matt", "Trey", "David", "Patrick", "Hugo", "Sam", "Sebastian", "Ben", "Robert",
                "Oliver", "Justin", "Jamie", "Mateo", "Cheyenne", "Marcus", "Garrett", "Jonah", "Kelly", "Amy", "Glenn", "Dina", "Gabrielle", "Michelle",
                "Jason", "Joshua", "Benjamin", "Gordon", "Morgan", "Harry", "Daniel", "Jacob", "Sophie", "Emily", "Jessica", "Ella", "Mia", "Emma", "Megan",
                "Caitlin", "Amber", "Fernando", "Evelyn", "Lauren", "Nicole", "Paige", "Eve", "Iris", "Gracie", "Sarah", "Holly", "Elizabeth", "Rachel",
                "Courtney", "Owen", "Ruby", "Peter", "Michael", "Samir", "Joanna", "Stan", "Milton", "Tom", "Jerry", "William", "Carter", "Dolores", "Earl",
                "Francesco", "Mario", "Luigi", "Vincenzo", "Sylvain", "Nathan", "John", "Mary", "Maria", "Helena", "Linda", "Lucy", "Sergio", "Hillary",
                "Katarina", "Raquel", "Vanessa", "Miguel", "Pedro", "Carlos", "Paul"
            };

            var colors = new[] { "#5484ED", "#A4BDFC", "#7AE7BF", "#51B749", "#FBD75B", "#FFB878", "#FF887C", "#DC2127", "#DBADFF", "#E1E1E1" };

            var foodNames = new[] {"Banana", "Cheesecake", "Hot Beverage", "Apple", "Oat meal", "French toast", "Pizza", "Apple Pie", "Shepherd's pie",
            "Salad", "Soup"};
            var locations = new[] { "Home", "University", "Restaurant", "Visit", "Diner", "Central Perk", "Tavern" };

            var restaurantNames = new[] { "McDowell's", "Chotchkie's", "Chili's", "Flingers", "The Cheesecake Factory", "The Rolling Donut", "Big Kahuna Burger", "City Wok", "Cluckin' Bell",
                "Central Perk", "Island Diner", "Dream Cafe", "Cleveland's Deli", "Don't Drop Inn", "The Happy Sumo", "The Hungry Hun", "Krusty Burger", "Luigi's",
                "Big T Burgers and Fries", "Pizza on a stick" };

            foreach (var fname in foodNames)
            {
                add(new Food { Name = fname });
            }

            foreach (var food in foodNames)
            {
                AddDinner(food);
            }

            foreach (var o in dinnerNames)
            {
                AddDinner(o);
            }

            foreach (var restaurantName in restaurantNames.Reverse())
            {
                var restaurant = Insert(new Restaurant { Name = restaurantName, IsCreated = true });
                Insert(new RestaurantAddress { RestaurantId = restaurant.Id, Line1 = Rnd(ppl) + " square " + R.Next(10, 15), Chef = Rnd(Chefs) });
                Insert(new RestaurantAddress { RestaurantId = restaurant.Id, Line1 = Rnd(ppl) + " street " + R.Next(10, 35), Chef = Rnd(Chefs) });

                Organisations.Add(new Organisation { Id = Guid.NewGuid(), Name = restaurantName });
            }

            IDictionary<string, List<string>> personFood = ppl.ToDictionary(o => o, o => new List<string>());
            IDictionary<string, int> personTimes = ppl.ToDictionary(o => o, o => 0);
            var prices = new[] { 10, 20, 30, 50, 70, 23, 45, 100, 21, 79, 39, 18 };

            for (var i = 0; i < 750; i++)
            {
                var maxLunches = 750 / ppl.Length;
                var pplc = 750 / ppl.Length;

                var person = Rnd(ppl);
                if (Lunches.Count(o => o.Person == person) >= pplc) person = Rnd(ppl);

                if (personTimes[person] >= maxLunches)
                {
                    person = Rnd(ppl);
                }

                personTimes[person]++;

                string food;

                if (personFood[person].Count == 3)
                {
                    food = Rnd(personFood[person]);
                }
                else
                {
                    food = Rnd(foodNames);
                    personFood[person].Add(food);
                }

                add(new Lunch
                {
                    Date = RndDate(),
                    Food = Rnd(Foods),
                    Person = person,
                    Location = Rnd(locations),
                    Price = Rnd(prices),
                    Country = Rnd(Countries),
                    Chef = Rnd(Chefs),
                    Organic = R.Next(10) < 5,
                    Meals = RndMeals().ToList()
                });
            }

            var startDate = DateTime.UtcNow.AddDays(-30).Date;
            var endDate = DateTime.UtcNow.AddDays(27);
            var eventDurations = new[] { 15, 20, 30, 45, 60, 120, 450, 30, 15, 45, 60 };
            var lateDurations = new[] { 15, 20, 30, 45, 60, 30, 15 };

            while (startDate < endDate)
            {
                var eventsPerDay = R.Next(2, 4);
                if (startDate.DayOfWeek == DayOfWeek.Sunday) eventsPerDay = Math.Min(eventsPerDay, 1);

                for (var i = 0; i < eventsPerDay; i++)
                {
                    var ev = Insert(new Meeting { Start = startDate, Location = Rnd(locations) });
                    ev.Start = ev.Start.AddHours(R.Next(24));

                    if (R.Next(50) == 1)
                    {
                        ev.AllDay = true;
                        ev.End = ev.Start;
                    }
                    else
                    {
                        ev.End = ev.Start.AddMinutes(Rnd(ev.Start.Hour > 17 ? lateDurations : eventDurations));
                    }

                    ev.Title = "meeting at " + ev.Location;
                    ev.Color = Rnd(colors);
                }

                startDate = startDate.AddDays(1);
            }

            for (var i = 0; i < 100; i++)
            {
                Insert(new Message
                {
                    From = Rnd(ppl),
                    Subject = Rnd(foodNames),
                    DateReceived = RndDate(),
                    Body = RndMessage(new[] { ppl, foodNames, new[] { "bla bla bla", "and", "or", "it", "for", "something" } }),
                    IsRead = true
                });
            }

            foreach (var message in Messages.OrderByDescending(o => o.DateReceived).Take(5))
            {
                message.IsRead = false;
            }

            for (int i = 0; i < 97; i++)
            {
                Insert(new Spreadsheet
                {
                    Name = Rnd(ppl),
                    Location = Rnd(countryNames),
                    Meal = Rnd(Meals).Name
                });
            }

            for (int i = 0; i < 15; i++)
            {
                var node = Insert(new TreeNode());
                node.Name = "main node " + node.Id;
                FillNode(node, 0, R.Next(2, 5));
            }
        }

        private static readonly Random R = new Random();

        private static void AddDinner(string name)
        {
            if (Dinners.Any(d => d.Name == name)) { return; }

            Insert(new Dinner
            {
                Name = name,
                Date = RndDate(),
                Country = Rnd(Countries),
                Chef = Rnd(Chefs),
                Meals = RndMeals().ToList(),
                BonusMeal = Rnd(Meals),
                Organic = R.Next(10) > 3
            });
        }

        private static void FillNode(TreeNode parent, int depth, int maxDepth)
        {
            var maxNodes = (int)Math.Max(5 - depth * 0.7, 1);
            var nodeCount = R.Next(1, maxNodes);
            var leafNames = Meals.Select(o => o.Name).ToList();
            leafNames.Add("green leaf");
            leafNames.Add("leaf");

            for (int i = 0; i < nodeCount; i++)
            {
                if (depth >= maxDepth || R.Next(0, 2) == 1)
                {
                    var leaf = Insert(new TreeNode { Parent = parent, ParentId = parent.Id });
                    leaf.Name = leaf.Name = Rnd(leafNames) + " " + leaf.Id;
                }
                else
                {
                    var node = Insert(new TreeNode { Parent = parent, ParentId = parent.Id });
                    node.Name = "node " + node.Id;
                    FillNode(node, depth + 1, maxDepth);
                }
            }
        }

        private static string RndMessage(string[][] ph)
        {
            var s = "Hi \n ";
            for (int i = 0; i < R.Next(1000); i++)
            {
                s += " " + Rnd(Rnd(ph));
            }
            return s;
        }

        private static T Rnd<T>(ICollection<T> list)
        {
            return list.ToArray()[R.Next(0, list.Count)];
        }

        private static DateTime RndDate()
        {
            var day = R.Next(14) * 2 + 1;
            return new DateTime(R.Next(2009, 2020), R.Next(1, 12), day);
        }

        private static IEnumerable<Meal> RndMeals()
        {
            var list = new List<Meal>();
            var x = R.Next(1, 4);
            for (var i = 0; i < x; i++)
            {
                list.Add(Meals[R.Next(Meals.Count - 1)]);
            }

            return list.Distinct();
        }
    }
}