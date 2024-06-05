using Microsoft.AspNetCore.Mvc;

namespace WD_ERECORD_CORE.Components
{
    public class AsideViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string current)
        {
            return View("Default", current);
        }
    }
}
