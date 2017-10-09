using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;
using Omu.ProDinner.WebUI.Utils;

namespace Omu.ProDinner.WebUI.Controllers.Awesome.AjaxList
{
    public class MealsAjaxListController : Controller
    {
        private readonly IRepo<Meal> repo;

        public MealsAjaxListController(IRepo<Meal> repo)
        {
            this.repo = repo;
        }

        public ActionResult Search(string search, int page, int? pageSize, int? pivot)
        {
            var ps = pageSize ?? 10;

            var list = repo.Where(o => o.Name.Contains(search), WebUtils.IsUserAdmin())
                        .OrderByDescending(o => o.Id);

            var items = (pivot.HasValue ? list.Where(o => o.Id <= pivot.Value) : list).Take(ps + 1).ToList();
            var isMore = items.Count > ps;

            var result = new AjaxListResult
            {
                Content = this.RenderPartialView("ListItems/Meal", items.Take(ps).ToList()),
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