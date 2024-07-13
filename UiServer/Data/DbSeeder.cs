using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace UiServer.Data
{
    public class DbSeeder
    {
        private readonly IDbContextFactory<MyContext> cxf;

        public DbSeeder(IDbContextFactory<MyContext> cxf)
        {
            this.cxf = cxf;
        }

        public void Seed(bool recreate = false)
        {
            using var mcx = cxf.CreateDbContext();

            void add<T>(T item) where T : class
            {
                mcx.Set<T>().Add(item);
            }
            if (recreate)
            {
                mcx.Database.EnsureDeleted();
            }

            mcx.Database.EnsureCreated();

            if (mcx.Meals.Any()) return;
            var categories = new[] { "Fruits", "Legumes", "Vegetables", "Nuts", "Grains" };

            var countryNames = new[]
            {
                "Elwynn Forest", "Stormwind", "Loch Modan", "Westfall",
                "Ironforge", "Orgrimmar", "Feralas", "Kitej", "Winterspring", "Teldrassil",
                "La Croisette", "Goldshire", "Sylvanaar", "Redridge Mountains", "Stone Cutter",
                "Regent", "Piccadilly", "Hatton Garden", "Greville", "Farringdon",
                "Carpana", "Jisina", "Ghidrimesti",
            };

            var ppl = new[]
            {
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

            var locations = new[] { "Home", "University", "Restaurant", "Visit", "Diner", "Central Perk", "Tavern" };

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

            var dinnerNames = new[] {"hot chocolate", "believe in miracles", "out and about", "dinner tonight", "watch tv", "last night",
                "toasty", "great", "coffee for me", "dinner with friends", "school lunch",
                "eating at the pub", "cooking in the garden", "ninja dinner", "broccoli power", "eating out", "Awesome dinner",
                "Uber dinner", "Fruity dish", "Nice dish", "Nerds eating", "Pros eating", "Morning Coffee", "Muffin",
                "Picnic", "Dejeuner", "Breakfast", "petit dej", "Apero",
                "Snacks and movie", "Morning meal", "Morning Tea", "Cookies", "Apples", "Salad"};

            var foodNames = new[] {"Banana", "Cheesecake", "Hot Beverage", "Apple", "Oat meal", "French toast", "Pizza", "Apple Pie", "Shepherd's pie",
            "Big Salad", "Soup"};

            var restaurantNames = new[] { "McDowell's", "Chotchkie's", "Chili's", "Flingers", "The Cheesecake Factory", "The Rolling Donut", "Big Kahuna Burger", "City Wok", "Cluckin' Bell",
                "Central Perk", "Island Diner", "Dream Cafe", "Cleveland's Deli", "Don't Drop Inn", "The Happy Sumo", "The Hungry Hun", "Krusty Burger", "Luigi's",
                "Big T Burgers and Fries", "Pizza on a stick" };

            #region Restaurants
            foreach (var restaurantName in restaurantNames.Reverse())
            {
                var restaurant = new Restaurant { Name = restaurantName };

                add(restaurant);
            }

            mcx.SaveChanges();
            #endregion

            #region Categories
            foreach (var category in categories)
            {
                add(new Category { Name = category });
            }

            mcx.SaveChanges();
            #endregion

            #region Foods
            foreach (var fname in foodNames)
            {
                add(new Food { Name = fname });
            }

            mcx.SaveChanges();
            #endregion

            #region Meals
            var Categories = mcx.Set<Category>().ToArray();

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

            #endregion

            #region Countries
            foreach (var country in countryNames)
            {
                add(new Country { Name = country });
            }

            mcx.SaveChanges(); 
            #endregion

            var Meals = mcx.Meals.ToArray();
            var Restaurants = mcx.Restaurants.ToArray();
            var Countries = mcx.Countries.ToArray();

            #region Countries
            foreach (var nms in chefNames)
            {
                add(new Chef { FirstName = nms[0], LastName = nms[1], Country = Rnd(Countries) });
            }

            mcx.SaveChanges(); 
            #endregion

            var Chefs = mcx.Chefs.ToArray();
            var Foods = mcx.Foods.ToArray();

            foreach (var food in foodNames)
            {
                AddDinner(food);
            }

            foreach (var o in dinnerNames)
            {
                AddDinner(o);
            }

            mcx.SaveChanges();


            IDictionary<string, List<Food>> personFood = ppl.ToDictionary(o => o, o => new List<Food>());
            IDictionary<string, int> personTimes = ppl.ToDictionary(o => o, o => 0);
            var prices = new[] { 10, 20, 30, 50, 70, 23, 45, 100, 21, 79, 39, 18 };
            List<Lunch> lunches = new();

            for (var i = 0; i < 750; i++)
            {
                var maxLunches = 750 / ppl.Length;
                var pplc = 750 / ppl.Length;

                var person = Rnd(ppl);
                if (lunches.Count(o => o.Person == person) >= pplc) person = Rnd(ppl);

                if (personTimes[person] >= maxLunches)
                {
                    person = Rnd(ppl);
                }

                personTimes[person]++;

                Food food;

                if (personFood[person].Count == 3)
                {
                    food = Rnd(personFood[person]);
                }
                else
                {
                    food = Rnd(Foods);
                    personFood[person].Add(food);
                }

                lunches.Add(new Lunch
                {
                    Date = RndDate(),
                    Food = food,
                    Person = person,
                    Location = Rnd(locations),
                    Price = Rnd(prices),
                    Country = Rnd(Countries),
                    Chef = Rnd(Chefs),
                    Organic = R.Next(10) < 5,
                    Meals = RndMeals().ToList()
                });
            }

            mcx.AddRange(lunches);
            mcx.SaveChanges();

            var nidn = 0;
            for (int i = 0; i < 15; i++)
            {
                var node = new TreeNode();
                node.Name = "main node " + nidn++;
                mcx.Add(node);

                FillNode(node, 0, R.Next(2, 5));
            }

            mcx.SaveChanges();


            void FillNode(TreeNode parent, int depth, int maxDepth)
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
                        var leaf = new TreeNode { Parent = parent, ParentId = parent.Id };
                        leaf.Name = leaf.Name = Rnd(leafNames) + " " + nidn++;
                        mcx.Add(leaf);
                    }
                    else
                    {
                        var node = new TreeNode { Parent = parent };
                        node.Name = "node " + nidn++;
                        mcx.Add(node);

                        FillNode(node, depth + 1, maxDepth);
                    }
                }
            }

            void AddDinner(string name)
            {
                if (mcx.Dinners.Any(d => d.Name == name)) return;

                add(new Dinner
                {
                    Name = name,
                    Date = RndDate(),
                    Country = Rnd(Countries),
                    Chef = Rnd(Chefs),
                    Meals = RndMeals().ToList(),
                    BonusMeal = Rnd(Meals),
                    Organic = R.Next(10) > 3,
                    Restaurant = R.Next(10) > 1 ? Rnd(Restaurants) : null
                });
            }

            IEnumerable<Meal> RndMeals()
            {
                var list = new List<Meal>();
                var x = R.Next(1, 4);
                for (var i = 0; i < x; i++)
                {
                    list.Add(Meals[R.Next(Meals.Length - 1)]);
                }

                return list.Distinct();
            }
        }

        private static T Rnd<T>(ICollection<T> list)
        {
            return list.ToArray()[R.Next(0, list.Count)];
        }

        private static DateTime RndDate()
        {
            var day = R.Next(14) * 2 + 1;
            return new DateTime(R.Next(2009, 2023), R.Next(1, 12), day);
        }

        private static readonly Random R = new Random();
    }
}
