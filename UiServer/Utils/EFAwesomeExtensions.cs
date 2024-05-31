using Microsoft.EntityFrameworkCore;
using Omu.BlazorAwesome.Models;
using System.Linq;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UiServer.Utils
{
    public static class EFAwesomeExtensions
    {
        /// <summary>
        /// setup EF for GridOpt
        /// </summary>
        public static void EFData<T, TContext>(
            this GridOpt<T> gopt, IDbContextFactory<TContext> cxf,
            Func<TContext, IQueryable<T>> getQuery)
            where TContext : DbContext
            where T : class
        {
            gopt.LoadData = async () =>
            {
                var state = gopt.State;
                using var cx = cxf.CreateDbContext();

                var q = getQuery(cx).AsQueryable();

                q = await state.ApplyFilters(q);
                
                state.SetCount(await q.CountAsync());
                
                state.Items = await state.QueryPage(q).ToArrayAsync();
            };
        }
    }
}
