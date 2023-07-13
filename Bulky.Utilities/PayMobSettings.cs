using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.Utilities
{
	public class PayMobSettings
	{
/*        public PayMobSettings(IConfiguration configuration)
        {
            FirstStepUrl = configuration.GetValue<string>("ServiceUrls:PayMobFirst");
            SecondStepUrl = configuration.GetValue<string>("ServiceUrls:PayMobSecond");
            ThirdStepUrl = configuration.GetValue<string>("ServiceUrls:PayMobThird");
            API_Key = configuration.GetValue<string>("PayMob:API_Key");
        }*/
        public string FirstStepUrl { get; set; }
        public string SecondStepUrl { get; set; }
        public string ThirdStepUrl { get; set; }
        public string API_Key { get; set; }

    }
}
