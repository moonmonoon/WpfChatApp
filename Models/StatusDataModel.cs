using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfChatApp.Models
{
    public class StatusDataModel
    {
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }

        public Uri StatusImage { get; set; }

        // If we want to add our status
        public bool IsMeAddStatus { get; set; }

        /// <summary>
        /// To-Do: We will be covering in one of our upcoming videos
        /// </summary>
        //public string StatusMessage { get; set; }
    }
}
