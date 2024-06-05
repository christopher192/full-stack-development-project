using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;
using static WD_ERECORD_CORE.Service.MasterFormEmailService;

namespace WD_ERECORD_CORE.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private UserManager<ApplicationUser> UserManager { get; set; }
        private readonly ApplicationDbContext _context;
        public DashboardViewModel dvm { get; set; } = new DashboardViewModel();

        public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            UserManager = userManager;
            _context = context;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> OnGet()
        {
            var User = await GetCurrentUser();

            dvm.User = User;
            dvm.Announcement = _context.AnnouncementLists.Where(x => x.Status == "Active").ToList();

            var CalendarLists = _context.CalendarLists.Where(x => x.UserId == User.UserName).ToList().Select(x => new { id = x.Id, title = (x.Title == string.Empty || x.Title == null ? "" : x.Title), description = (x.Description == string.Empty || x.Description == null ? "" : x.Description), start = x.StartDateTime.Date.ToString("yyyy-MM-dd") + "T" + x.StartDateTime.TimeOfDay.ToString(), end = x.EndDateTime.Date.ToString("yyyy-MM-dd") + "T" + x.EndDateTime.TimeOfDay.ToString(), className = "fc-event-light fc-event-solid-primary" });
            ViewData["CalendarData"] = CalendarLists;

            return Page();
        }

        public async Task<JsonResult> OnPostAcquireFormList()
        {
            var user = await GetCurrentUser();
            var data = _context.FormLists.Where(x => x.Owner == user.UserName && (x.FormStatus != "rejected" && x.FormStatus != "approved")).Select(x => new { x.Id, x.FormName, x.FormStatus }).ToList();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostAcquireFormApprovalList()
        {
            var user = await GetCurrentUser();
            var data = _context.FormLists.Include(x => x.FormListApprovalLevels).ThenInclude(x => x.FormListApprovers)
                .Where(x => x.FormStatus == "pending" && x.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending" && x.FormListApprovers.Where(x => x.ApproverStatus == "pending" && x.EmployeeId == user.UserName).Any()).Any())
                .Select(x => new { x.Id, x.FormName, x.FormStatus }).ToList();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostAcquireMasterFormApprovalList()
        {
            var user = await GetCurrentUser();
            var data = _context.MasterFormLists.Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers)
                .Where(x => x.MasterFormStatus == "pending" && x.MasterFormDepartments.Where(x => x.Status == "pending" && x.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "pending" && x.MasterFormCCBApprovers.Where(x => x.ApproverStatus == "pending" && x.EmployeeId == user.UserName).Any()).Any()).Any())
                .Select(x => new { x.Id, x.MasterFormName, x.MasterFormStatus }).ToList();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostAnnualSubmission()
        {
            var user = await GetCurrentUser();
            var AnnualSubmission = _context.FormLists.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.Owner == user.UserName).Select(x => new { x.FormName, x.CreatedDate }).AsEnumerable().GroupBy(g => new { FormName = g.FormName, Month = g.CreatedDate.Month }).OrderBy(o => o.Key.FormName).ThenBy(o => o.Key.Month).Select(x => new AnnualSubmission
            {
                FormName = x.Key.FormName,
                Month = x.Key.Month,
                Total = x.Count()
            });

            var getUniqueFormName = AnnualSubmission.Select(x => x.FormName).Distinct();
            var monthList = Enumerable.Range(1, 12).ToList();

            var completeAnnualSubmission = new List<AnnualSubmission>(getUniqueFormName.Count() * monthList.Count());

            foreach (var formName in getUniqueFormName)
            {
                foreach (var month in monthList)
                {
                    if (AnnualSubmission.Where(x => x.FormName == formName && x.Month == month).Any())
                    {
                        completeAnnualSubmission.Add(AnnualSubmission.Where(x => x.FormName == formName && x.Month == month).First());
                    }
                    else
                    {
                        completeAnnualSubmission.Add(new AnnualSubmission() { FormName = formName, Month = month, Total = 0 });
                    }
                }
            }

            var data = new List<AnnualSubmissionDataTransformation>();
            foreach (var formName in getUniqueFormName)
            {
                data.Add(new AnnualSubmissionDataTransformation() { name = formName, data = completeAnnualSubmission.Where(x => x.FormName == formName).Select(x => x.Total).ToList() });
            }

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostDailySubmission()
        {
            var user = await GetCurrentUser();
            var DailySubmission = _context.FormLists.Where(x => x.CreatedDate.Date == DateTime.Today && x.Owner == user.UserName).AsEnumerable().GroupBy(x => x.FormDescription).Select(x => new DailySubmission
            {
                Labels = x.Key,
                Series = x.Count()
            });

            var data = DailySubmission;

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostWeeklySubmission()
        {
            var user = await GetCurrentUser();

            DayOfWeek currentDay = DateTime.Now.DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            int daysTillEndDay = (currentDay - DayOfWeek.Sunday) - 1;
            DateTime currentWeekStartDate = DateTime.Now.AddDays(-daysTillCurrentDay);
            DateTime currentWeekEndDate = DateTime.Now.AddDays(daysTillEndDay);

            var WeeklySubmission = _context.FormLists.Where(x => (x.CreatedDate.Date >= currentWeekStartDate.Date && x.CreatedDate.Date <= currentWeekEndDate.Date) && x.Owner == user.UserName).AsEnumerable().GroupBy(x => x.FormDescription).Select(x => new WeeklySubmission
            {
                Labels = x.Key,
                Series = x.Count()
            });

            var data = WeeklySubmission;

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostMonthlySubmission()
        {
            var user = await GetCurrentUser();
            var MonthlySubmission = _context.FormLists.Where(x => x.CreatedDate.Month == DateTime.Now.Month && x.Owner == user.UserName).AsEnumerable().GroupBy(x => x.FormDescription).Select(x => new MonthlySubmission
            {
                Labels = x.Key,
                Series = x.Count()
            });

            var data = MonthlySubmission;

            return new JsonResult(data);
        }
    }
}