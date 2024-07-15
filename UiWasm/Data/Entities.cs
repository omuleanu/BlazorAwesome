using System;
using System.Collections.Generic;
using System.Linq;

namespace UiWasm.Data
{
    public class Entity
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateDeleted { get; set; }
    }

    public class Food : Entity
    {
        public string Name { get; set; }

        public string Pic
        {
            get
            {
                return Name is null ? null : Name.Replace(" ", "").Replace("'", "") + ".jpg";
            }
        }
    }

    public class Chef : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public Country Country { get; set; }
    }

    public class Country : Entity
    {
        public string Name { get; set; }
    }

    public class Category : Entity
    {
        public string Name { get; set; }
    }

    public class Meal : Entity
    {
        public Category Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // needed for EF
        public ICollection<Dinner> Dinners { get; set; }

        // needed for EF
        public ICollection<Lunch> Lunches { get; set; }
    }

    public class Restaurant : Entity
    {
        public string Name { get; set; }
    }

    public class Dinner : Entity
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Chef Chef { get; set; }
        public Country Country { get; set; }
        public ICollection<Meal> Meals { get; set; }
        public Meal BonusMeal { get; set; }
        public string Comments { get; set; }
        public bool Organic { get; set; }
        
        // used by master detail inline edit demo
        public Restaurant Restaurant { get; set; }
    }

    public class Lunch : Entity
    {
        public string Person { get; set; }
        public Food Food { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Price { get; set; }
        public Country Country { get; set; }
        public Chef Chef { get; set; }
        public bool Organic { get; set; }
        //public string FoodPic { get { return Food.Replace(" ", "").Replace("'", "") + ".jpg"; } }
        public ICollection<Meal> Meals { get; set; }
        public int MealsCount => Meals.Count();
    }

    public class Message : Entity
    {
        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime DateReceived { get; set; }

        public bool IsRead { get; set; }
    }

    public class Spreadsheet : Entity
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string Meal { get; set; }
    }

    public class TreeNode : Entity
    {
        public string Name { get; set; }

        public TreeNode Parent { get; set; }

        public int? ParentId { get; set; }
    }

    public class Meeting : Entity
    {
        public string Title { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Location { get; set; }

        public string Color { get; set; }

        public string Notes { get; set; }

        public bool AllDay { get; set; }
    }

    public class Organisation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class ParentMeal : Entity
    {
        public Category Category { get; set; }

        public Meal Meal { get; set; }
    }

    public enum WeatherType
    {
        Sunny = 1,
        Cloudy = 2,
        Foggy = 3,
        Windy = 4,
        ScottishSummer = 5
    }
}