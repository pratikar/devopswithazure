using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;
using Omu.ProDinner.Resources;
using Omu.ProDinner.WebUI.Utils;

namespace Omu.ProDinner.WebUI.Controllers.Awesome
{
    public class DataController : Controller
    {
        private readonly IUniRepo repo;

        public DataController(IUniRepo repo)
        {
            this.repo = repo;
        }

        public ActionResult GetChefs(bool? any)
        {
            var items = repo.GetAll<Chef>(WebUtils.IsUserAdmin()).ToArray()
                .Select(o => new KeyContent(o.Id, string.Format("{0} {1}", o.FirstName, o.LastName))).ToList();

            if (any.HasValue)
            {
                items.Insert(0, new KeyContent("", Mui.any));
            }

            return Json(items);
        }

        public ActionResult GetCountries()
        {
            var items = repo.GetAll<Country>(WebUtils.IsUserAdmin()).ToArray()
                .Select(o => new KeyContent(o.Id, o.Name));
            return Json(items);
        }

        public ActionResult GetDinners(string term, int? chef, int[] meals)
        {
            var query = repo.GetAll<Dinner>(WebUtils.IsUserAdmin())
                            .Where(o => o.Name.Contains(term));

            if (chef.HasValue) query = query.Where(o => o.ChefId == chef);
            if (meals != null) query = query.Where(o => meals.All(m => o.Meals.Select(g => g.Id).Contains(m)));

            var list = query.ToList();

            return Json(list.Select(i => new KeyContent { Content = i.Name, Key = i.Id }).Take(5));
        }
    }
}