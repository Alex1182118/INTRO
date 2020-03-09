using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L1.Models;

namespace L1.Helpers
{
    public class Data<T>
    {
        private static Data<T> _instance = null;

        public static Data<T> Instance
        {
            get
            {
                if (_instance == null) _instance = new Data<T>();
                return _instance;
            }
        }

        public List<Drink> Drinks = new List<Drink>();
        
    }

}