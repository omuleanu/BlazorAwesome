using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Omu.BlazorAwesome.Models;
using System.Collections.Generic;
using System.Linq;
using UiServer.Models;
using UiServer.Utils;

namespace UiServer.Data
{
    public class CachedItems
    {
        private IDbContextFactory<MyContext> dbContextFactory;

        public CachedItems(IDbContextFactory<MyContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        private Task<IEnumerable<KeyContent>> meals;
        private Task<IEnumerable<KeyContent>> chefs;
        private Task<IEnumerable<KeyContent>> categories;
        private Task<IEnumerable<KeyContent>> foods;

        public async Task<IEnumerable<KeyContent>> Chefs()
        {
            return await getOrLoadField<Chef>(chefs, res => chefs = res, o => new(o.Id, o.FullName));
        }

        public async Task<IEnumerable<KeyContent>> Meals()
        {
            return await getOrLoadField<Meal>(
                meals,
                res => meals = res, 
                m => new ImgKeyContent
                {
                    Key = m.Id,
                    Content = m.Name,
                    Url = DemoUtils.MealsUrl + m.Name + ".jpg"
                });
        }

        public async Task<IEnumerable<KeyContent>> Foods()
        {
            var res = await getOrLoadField<Food>(
                foods,
                res => foods = res, 
                f => new ImgKeyContent
                {
                    Key = f.Id,
                    Content = f.Name,
                    Url = DemoUtils.FoodUrl + f.Pic
                });

            var lst = res.ToList();

            lst.Insert(0, new ImgKeyContent(null, "any", DemoUtils.FoodUrl + "pasta.png"));

            return lst;
        }

        public async Task<IEnumerable<KeyContent>> Categories()
        {
            return await getOrLoadField<Category>(categories, res => categories = res, o => new(o.Id, o.Name));
        }

        private async Task<IEnumerable<KeyContent>> getOrLoadField<T>(
            Task<IEnumerable<KeyContent>> field,
            Action<Task<IEnumerable<KeyContent>>> setField,
            Func<T, KeyContent> kcFunc) where T : Entity
        {
            if (field is not null)
            {
                return await field;
            }

            var getTask = getAll(kcFunc);

            setField(getTask);

            return await getTask;
        }

        private async Task<IEnumerable<KeyContent>> getAll<T>(Func<T, KeyContent> kcFunc) where T : Entity
        {
            using var cx = await dbContextFactory.CreateDbContextAsync();

            return (await cx.Set<T>().OrderBy(o => o.Id).ToArrayAsync()).Select(o => kcFunc(o));
        }
    }
}