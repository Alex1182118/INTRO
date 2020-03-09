using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using L1.Models;
using L1.Helpers;

namespace L1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        readonly BTree<Drink> NameTree = new BTree<Drink>(5);
        readonly BTree<Drink> FlavorTree = new BTree<Drink>(5);
        readonly BTree<Drink> VolumeTree = new BTree<Drink>(5);
        readonly BTree<Drink> PriceTree = new BTree<Drink>(5);
        readonly BTree<Drink> ManufacturerTree = new BTree<Drink>(5);
        readonly BTree<Drink> Tree = new BTree<Drink>(5);
        


        public void Get(List<Drink> SortedKeys)
        {
            

            var All = Tree.InOrden(,SortedKeys);
            return All;

            
        }
        void Get([FromBody] Drink nuevo)
        {
                       
            //var valorBuscado = Tree.HasItem(nuevo);
            
            //if (valorBuscado != false)
            //{
            //    encontrado = true;

            //}
            //else
            //{
            //    encontrado = false;
            //}
           
        }


        // POST: api/Drinks
        [HttpPost]
        public void Post([FromBody] Drink obj)
        {
            Tree.Insert(obj);
        }
    }
}
