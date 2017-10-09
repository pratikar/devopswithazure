using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;
using Omu.ProDinner.WebUI.Utils;

namespace Omu.ProDinner.WebUI.Controllers.Awesome.Lookup
{
    public class CountryIdLookupController : Controller
    {
        private readonly IRepo<Country> repo;

        public CountryIdLookupController(IRepo<Country> repo)
        {
            this.repo = repo;
        }

        public ActionResult SearchForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string search, int page, int? pivot)
        {
            var ps = 10;

            var list = repo.Where(o => o.Name.StartsWith(search), WebUtils.IsUserAdmin())
                .OrderByDescending(o => o.Id);


            var items = (pivot.HasValue ? list.Where(o => o.Id <= pivot.Value) : list).Take(ps + 1).ToList();
            var isMore = items.Count > ps;

            var result = new AjaxListResult
            {
                Content = this.RenderPartialView("ListItems/Country", items.Take(ps).ToList()),
                More = isMore
            };

            if (isMore)
            {
                result.Pivot = items.Last().Id;
            }

            return Json(result);
        }

        public ActionResult GetItem(int v)
        {
            var c = repo.Get(v) ?? new Country();
            return Json(new KeyContent(c.Id, c.Name));
        }
    }
}