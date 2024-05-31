using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omu.BlazorAwesome.Components.Internal;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// </summary>
    public class EditorState
    {
        /// <summary>        
        /// </summary>
        public Func<IDataOpt> GetOpt { private get; set; }

        private IDataOpt gopt => GetOpt != null ? GetOpt() : null;

        /// <summary>        
        /// </summary>
        public bool IsLoading { get; private set; }

        private IEnumerable<KeyContent> data;

        /// <summary>
        /// State data
        /// </summary>
        public IEnumerable<KeyContent> Data
        {
            get { return data ?? gopt?.Data; }
            set { data = value; }
        }

        /// <summary>
        /// Check value function
        /// </summary>
        public Func<Task> CheckVal { private get; set; }

        /// <summary>
        /// Load editor's data
        /// </summary>        
        public async Task LoadAsync()
        {
            if (gopt is null) return;            

            if (gopt.DataFunc is not null)
            {
                IsLoading = true;

                try
                {
                    Data = await gopt.DataFunc();
                }
                finally
                {
                    IsLoading = false;
                    await CheckVal();
                }
            }
        }
    }
}