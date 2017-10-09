using System.Linq;
using System.Threading;
using System.Web.Mvc;

using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;
using Omu.ProDinner.WebUI.Utils;

namespace Omu.ProDinner.WebUI.Controllers.Awesome.AjaxList
{
    public class DinnersAjaxListController : Controller
    {
        private readonly IRepo<Dinner> repo;

        public DinnersAjaxListController(IRepo<Dinner> repo)
        {
            this.repo = repo;
        }

        public ActionResult Search(string search, int? chef, int[] meals, int page, int? pivot)
        {
            var ps = 7;
            var list = repo.Where(o => o.Name.Contains(search), WebUtils.IsUserAdmin());
            
            if (chef.HasValue) list = list.Where(o => o.ChefId == chef.Value);
            if (meals != null) list = list.Where(o => meals.All(m => o.Meals.Select(g => g.Id).Contains(m)));
            list = list.OrderByDescending(o => o.Id);
            
            var items = (pivot.HasValue ? list.Where(o => o.Id <= pivot.Value) : list).Take(ps + 1).ToList();
            var isMore = items.Count > ps;

            var result = new AjaxListResult
            {
                Content = this.RenderPartialView("ListItems/Dinner", items.Take(ps).ToList()),
                More = isMore
            };

            if (isMore)
            {
                result.Pivot = items.Last().Id;
            }

            return Json(result);
        }
    }
}