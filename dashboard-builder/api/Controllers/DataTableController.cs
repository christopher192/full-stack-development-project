using Microsoft.AspNetCore.Mvc;
using Bogus;
using API.Data.Models;
using API.Data;
using API.Data.Datatable;
using Microsoft.EntityFrameworkCore;
using API.Extensions;
using HotChocolate.Authorization;
using NuGet.ContentModel;
using HotChocolate.Language;
using Bogus.DataSets;
using NuGet.Protocol.Plugins;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTableController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataTableController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMSSQLTable")]
        // [Authorize]
        public JsonResult GetMSSQLTable()
        {
            var allowed = new List<string>() { "Finances", "Projects", "Employees", "PurchaseOrderHistories" };
            var dict = new Dictionary<string, dynamic>();

            var tables = _context.Model.GetEntityTypes().Where(x => allowed.Contains(x.GetTableName()));

            foreach (var t in tables)
            {
                var tableColumns = t.GetProperties().Where(x => x.Name != "Id").ToList();
                var dict2 = new List<object>();

                foreach (var col in tableColumns)
                {
                   dict2.Push(new { ColName = col.GetColumnName(), Type = col.GetColumnType() });
                }

                dict[t.GetTableName()] = dict2;

                // dict[t.GetTableName()] = t.GetProperties().Where(x => x.Name != "Id").Select(x => x.Name);
            }

            return new JsonResult(dict);
        }


        [HttpPost]
        [Route("LoadPurchaseOrderHistoriesDataTable")]
        [Authorize]
        public async Task<JsonResult> LoadPurchaseOrderHistoriesDataTable([FromBody] DtParameters dtParameters)
        {
            var data = _context.PurchaseOrderHistories.Select(x => new {
                x.Id,
                x.DataId,
                x.SerielNumber,
                x.PurchaseId,
                x.Title,
                x.User,
                x.AssignedTo,
                x.CreatedBy,
                x.CreateDate,
                x.Status,
                x.Priority
            });

            var x1 = dtParameters.Columns.Where(x => x.Data == "id").FirstOrDefault()?.Search?.Value;
            var x2 = dtParameters.Columns.Where(x => x.Data == "dataId").FirstOrDefault()?.Search?.Value;
            var x3 = dtParameters.Columns.Where(x => x.Data == "serielNumber").FirstOrDefault()?.Search?.Value;
            var x4 = dtParameters.Columns.Where(x => x.Data == "purchaseId").FirstOrDefault()?.Search?.Value;
            var x5 = dtParameters.Columns.Where(x => x.Data == "title").FirstOrDefault()?.Search?.Value;
            var x6 = dtParameters.Columns.Where(x => x.Data == "user").FirstOrDefault()?.Search?.Value;
            var x7 = dtParameters.Columns.Where(x => x.Data == "assignedTo").FirstOrDefault()?.Search?.Value;
            var x8 = dtParameters.Columns.Where(x => x.Data == "createdBy").FirstOrDefault()?.Search?.Value;
            var x9 = dtParameters.Columns.Where(x => x.Data == "createDate").FirstOrDefault()?.Search?.Value;
            var x10 = dtParameters.Columns.Where(x => x.Data == "status").FirstOrDefault()?.Search?.Value;
            var x11 = dtParameters.Columns.Where(x => x.Data == "priority").FirstOrDefault()?.Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = data.AsQueryable();


            if (!string.IsNullOrEmpty(x1))
            {
                result = result.Where(r => r.Id != null && r.Id == int.Parse(x1));
            }

            if (!string.IsNullOrEmpty(x2))
            {
                result = result.Where(r => r.DataId != null && r.DataId.ToUpper().Contains(x2.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x3))
            {
                result = result.Where(r => r.SerielNumber != null && r.SerielNumber.ToUpper().Contains(x3.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x4))
            {
                result = result.Where(r => r.PurchaseId != null && r.PurchaseId.ToUpper().Contains(x4.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x5))
            {
                result = result.Where(r => r.Title != null && r.Title.Contains(x5.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x6))
            {
                result = result.Where(r => r.User != null && r.User.ToUpper().Contains(x6.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x7))
            {
                result = result.Where(r => r.AssignedTo != null && r.AssignedTo.ToUpper().Contains(x7.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x8))
            {
                result = result.Where(r => r.CreatedBy != null && r.CreatedBy.ToUpper().Contains(x8.ToUpper()));
            }

/*            if (!string.IsNullOrEmpty(x9))
            {
                result = result.Where(r => r.CreateDate != null && r.CreateDate.ToUpper().Contains(x9.ToUpper()));
            }*/

            if (!string.IsNullOrEmpty(x10))
            {
                result = result.Where(r => r.Status != null && r.Status.ToUpper().Contains(x10.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x11))
            {
                result = result.Where(r => r.Priority != null && r.Priority.ToUpper().Contains(x11.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await data.CountAsync();

            return new JsonResult(new DtResult<object>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        [Route("LoadProjectsDataTable")]
        [Authorize]
        public async Task<JsonResult> LoadProjectsDataTable([FromBody] DtParameters dtParameters)
        {
            var data = _context.Projects.Select(x => new {
                x.Id,
                x.Name,
                x.Description,
                x.Task,
                x.ClientName,
                x.AssignedTo,
                x.DueDate,
                x.Status,
                x.Priority
            });

            var x1 = dtParameters.Columns.Where(x => x.Data == "id").FirstOrDefault()?.Search?.Value;
            var x2 = dtParameters.Columns.Where(x => x.Data == "ame").FirstOrDefault()?.Search?.Value;
            var x3 = dtParameters.Columns.Where(x => x.Data == "description").FirstOrDefault()?.Search?.Value;
            var x4 = dtParameters.Columns.Where(x => x.Data == "task").FirstOrDefault()?.Search?.Value;
            var x5 = dtParameters.Columns.Where(x => x.Data == "clientName").FirstOrDefault()?.Search?.Value;
            var x6 = dtParameters.Columns.Where(x => x.Data == "assignedTo").FirstOrDefault()?.Search?.Value;
            var x7 = dtParameters.Columns.Where(x => x.Data == "dueDate").FirstOrDefault()?.Search?.Value;
            var x8 = dtParameters.Columns.Where(x => x.Data == "status").FirstOrDefault()?.Search?.Value;
            var x9 = dtParameters.Columns.Where(x => x.Data == "priority").FirstOrDefault()?.Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = data.AsQueryable();


            if (!string.IsNullOrEmpty(x1))
            {
                result = result.Where(r => r.Id != null && r.Id == int.Parse(x1));
            }

            if (!string.IsNullOrEmpty(x2))
            {
                result = result.Where(r => r.Name != null && r.Name.ToUpper().Contains(x2.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x3))
            {
                result = result.Where(r => r.Description != null && r.Description.ToUpper().Contains(x3.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x4))
            {
                result = result.Where(r => r.Task != null && r.Task.ToUpper().Contains(x4.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x5))
            {
                result = result.Where(r => r.ClientName != null && r.ClientName.ToUpper().Contains(x5.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x6))
            {
                result = result.Where(r => r.AssignedTo != null && r.AssignedTo.ToUpper().Contains(x6.ToUpper()));
            }

/*            if (!string.IsNullOrEmpty(x7))
            {
                result = result.Where(r => r.DueDate != null && r.DueDate.ToUpper().Contains(x7.ToUpper()));
            }*/

            if (!string.IsNullOrEmpty(x8))
            {
                result = result.Where(r => r.Status != null && r.Status.ToUpper().Contains(x8.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x9))
            {
                result = result.Where(r => r.Priority != null && r.Priority.ToUpper().Contains(x9.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await data.CountAsync();

            return new JsonResult(new DtResult<object>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        [Route("LoadFinancesDataTable")]
        [Authorize]
        public async Task<JsonResult> LoadFinancesDataTable([FromBody] DtParameters dtParameters)
        {
            var data = _context.Finances.Select(x => new { 
                x.Id,
                x.Account,
                x.AccountName,
                x.Amount,
                x.TransactionType,
                x.Currency,
                x.CreditCardNumber,
                x.CreditCardCvv,
                x.BitcoinAddress,
                x.EthereumAddress,
                x.RoutingNumber,
                x.Bic,
                x.Iban
            });

            var x1 = dtParameters.Columns.Where(x => x.Data == "id").FirstOrDefault()?.Search?.Value;
            var x2 = dtParameters.Columns.Where(x => x.Data == "account").FirstOrDefault()?.Search?.Value;
            var x3 = dtParameters.Columns.Where(x => x.Data == "accountName").FirstOrDefault()?.Search?.Value;
            var x4 = dtParameters.Columns.Where(x => x.Data == "transactionType").FirstOrDefault()?.Search?.Value;
            var x5 = dtParameters.Columns.Where(x => x.Data == "currency").FirstOrDefault()?.Search?.Value;
            var x6 = dtParameters.Columns.Where(x => x.Data == "creditCardNumber").FirstOrDefault()?.Search?.Value;
            var x7 = dtParameters.Columns.Where(x => x.Data == "creditCardCvv").FirstOrDefault()?.Search?.Value;
            var x8 = dtParameters.Columns.Where(x => x.Data == "bitcoinAddress").FirstOrDefault()?.Search?.Value;
            var x9 = dtParameters.Columns.Where(x => x.Data == "ethereumAddress").FirstOrDefault()?.Search?.Value;
            var x10 = dtParameters.Columns.Where(x => x.Data == "routingNumber").FirstOrDefault()?.Search?.Value;
            var x11 = dtParameters.Columns.Where(x => x.Data == "bic").FirstOrDefault()?.Search?.Value;
            var x12 = dtParameters.Columns.Where(x => x.Data == "iban").FirstOrDefault()?.Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = data.AsQueryable();


            if (!string.IsNullOrEmpty(x1))
            {
                result = result.Where(r => r.Id != null && r.Id == int.Parse(x1));
            }

            if (!string.IsNullOrEmpty(x2))
            {
                result = result.Where(r => r.Account != null && r.Account.ToUpper().Contains(x2.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x3))
            {
                result = result.Where(r => r.AccountName != null && r.AccountName.ToUpper().Contains(x3.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x4))
            {
                result = result.Where(r => r.TransactionType != null && r.TransactionType.ToUpper().Contains(x4.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x5))
            {
                result = result.Where(r => r.Currency != null && r.Currency.ToUpper().Contains(x5.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x6))
            {
                result = result.Where(r => r.CreditCardNumber != null && r.CreditCardNumber.ToUpper().Contains(x6.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x7))
            {
                result = result.Where(r => r.CreditCardCvv != null && r.CreditCardCvv.ToUpper().Contains(x7.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x8))
            {
                result = result.Where(r => r.BitcoinAddress != null && r.BitcoinAddress.ToUpper().Contains(x8.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x9))
            {
                result = result.Where(r => r.EthereumAddress != null && r.EthereumAddress.ToUpper().Contains(x9.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x10))
            {
                result = result.Where(r => r.RoutingNumber != null && r.RoutingNumber.ToUpper().Contains(x10.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x11))
            {
                result = result.Where(r => r.Bic != null && r.Bic.ToUpper().Contains(x11.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x12))
            {
                result = result.Where(r => r.Iban != null && r.Iban.ToUpper().Contains(x12.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await data.CountAsync();

            return new JsonResult(new DtResult<object>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        [Route("LoadEmployeesDataTable")]
        [Authorize]
        public async Task<JsonResult> LoadEmployeesDataTable([FromBody] DtParameters dtParameters)
        {
            var data = _context.Employees.Select(x => new {
                x.Id,
                x.FirstName,
                x.LastName,
                x.FullName,
                x.Position,
                x.Office,
                x.Age,
                x.Salary,
                x.StartDate,
                x.Sex
            });

            var x1 = dtParameters.Columns.Where(x => x.Data == "id").FirstOrDefault()?.Search?.Value;
            var x2 = dtParameters.Columns.Where(x => x.Data == "firstName").FirstOrDefault()?.Search?.Value;
            var x3 = dtParameters.Columns.Where(x => x.Data == "lastName").FirstOrDefault()?.Search?.Value;
            var x4 = dtParameters.Columns.Where(x => x.Data == "fullName").FirstOrDefault()?.Search?.Value;
            var x5 = dtParameters.Columns.Where(x => x.Data == "position").FirstOrDefault()?.Search?.Value;
            var x6 = dtParameters.Columns.Where(x => x.Data == "office").FirstOrDefault()?.Search?.Value;
            var x7 = dtParameters.Columns.Where(x => x.Data == "age").FirstOrDefault()?.Search?.Value;
            var x8 = dtParameters.Columns.Where(x => x.Data == "salary").FirstOrDefault()?.Search?.Value;
            var x9 = dtParameters.Columns.Where(x => x.Data == "startDate").FirstOrDefault()?.Search?.Value;
            var x10 = dtParameters.Columns.Where(x => x.Data == "sex").FirstOrDefault()?.Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = data.AsQueryable();


            if (!string.IsNullOrEmpty(x1))
            {
                result = result.Where(r => r.Id != null && r.Id == int.Parse(x1));
            }

            if (!string.IsNullOrEmpty(x2))
            {
                result = result.Where(r => r.FirstName != null && r.FirstName.ToUpper().Contains(x2.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x3))
            {
                result = result.Where(r => r.LastName != null && r.LastName.ToUpper().Contains(x3.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x4))
            {
                result = result.Where(r => r.FullName != null && r.FullName.ToUpper().Contains(x4.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x5))
            {
                result = result.Where(r => r.Position != null && r.Position.ToUpper().Contains(x5.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x6))
            {
                result = result.Where(r => r.Office != null && r.Office.ToUpper().Contains(x6.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x7))
            {
                result = result.Where(r => r.Age != null && r.Age.ToUpper().Contains(x7.ToUpper()));
            }

            if (!string.IsNullOrEmpty(x8))
            {
                result = result.Where(r => r.Salary != null && r.Salary.ToUpper().Contains(x8.ToUpper()));
            }

/*            if (!string.IsNullOrEmpty(x9))
            {
                result = result.Where(r => r.StartDate.Value.Date == DateTime.ParseExact(x9, "MM/dd/yyyy", null));
            }*/

            if (!string.IsNullOrEmpty(x10))
            {
                result = result.Where(r => r.Sex != null && r.Sex.ToUpper().Contains(x10.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await data.CountAsync();

            return new JsonResult(new DtResult<object>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        public enum Gender
        {
            Male,
            Female
        }

        [HttpGet]
        [Route("SeedEmployee")]
        public async Task<ActionResult> SeedEmployee()
        {
            var employees = new List<Employee>();

            var count = 100;
            var employeeFaker = new Faker<Employee>()
                .StrictMode(true)
                .RuleFor("Id", f => 0)
                .RuleFor("Sex", f => f.PickRandom<Gender>().ToString())
                .RuleFor("FirstName", (f, u) => f.Name.FirstName((Bogus.DataSets.Name.Gender?)Enum.Parse<Gender>(u.Sex)))
                .RuleFor("LastName", (f, u) => f.Name.LastName((Bogus.DataSets.Name.Gender?)Enum.Parse<Gender>(u.Sex)))
                .RuleFor("FullName", (f, u) => u.FirstName + " " + u.LastName)
                .RuleFor("Position", f => f.Name.JobTitle())
                .RuleFor("Office", f => f.Address.Country())
                .RuleFor("Age", f => f.Random.Number(20, 55).ToString())
                .RuleFor("StartDate", f => f.Date.Past())
                .RuleFor("Salary", f => f.Random.Number(1500, 10000).ToString());

            employees = employeeFaker.Generate(count);

            _context.Employees.AddRange(employees);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("SeedFinance")]
        public async Task<ActionResult> SeedFinance()
        {
            var finances = new List<API.Data.Models.Finance>();

            var count = 100;
            var financeFaker = new Faker<API.Data.Models.Finance>()
                .StrictMode(true)
                .RuleFor("Id", f => 0)
                .RuleFor("Account", f => f.Finance.Account())
                .RuleFor("AccountName", f => f.Finance.AccountName())
                .RuleFor("Amount", f => f.Finance.Account())
                .RuleFor("TransactionType", f => f.Finance.TransactionType())
                .RuleFor("CreditCardNumber", f => f.Finance.CreditCardNumber())
                .RuleFor("CreditCardCvv", f => f.Finance.CreditCardCvv())
                .RuleFor("BitcoinAddress", f => f.Finance.BitcoinAddress())
                .RuleFor("EthereumAddress", f => f.Finance.EthereumAddress())
                .RuleFor("RoutingNumber", f => f.Finance.RoutingNumber())
                .RuleFor("Bic", f => f.Finance.Bic())
                .RuleFor("Iban", f => f.Finance.Iban())
                .RuleFor("Currency", (f, o) => f.Finance.Currency().Code);

            finances = financeFaker.Generate(count);

            _context.Finances.AddRange(finances);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("SeedProject")]
        public async Task<ActionResult> SeedProject()
        {
            var projects = new List<API.Data.Models.Project>();

            var count = 100;
            var projectFaker = new Faker<API.Data.Models.Project>()
                .StrictMode(true)
                .RuleFor("Id", f => 0)
                .RuleFor("Name", f => f.Company.CompanyName())
                .RuleFor("Description", f => f.Commerce.ProductMaterial())
                .RuleFor("Task", f => f.Commerce.ProductName())
                .RuleFor("ClientName", f => f.Name.FullName())
                .RuleFor("AssignedTo", f => f.Name.FullName())
                .RuleFor("DueDate", f => f.Date.Future())
                .RuleFor("Status", f => f.PickRandom(new [] { "active", "disactive" }))
                .RuleFor("Priority", f => f.PickRandom(new [] { "high", "medium", "low" }));

            projects = projectFaker.Generate(count);

            _context.Projects.AddRange(projects);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        [Route("SeedPurchaseOrderHistory")]
        public async Task<ActionResult> SeedPurchaseOrderHistory()
        {
            var poh = new List<API.Data.Models.PurchaseOrderHistory>();

            var count = 100;
            var pohFaker = new Faker<API.Data.Models.PurchaseOrderHistory>()
                .StrictMode(true)
                .RuleFor("Id", f => 0)
                .RuleFor("DataId", f => new Guid().ToString())
                .RuleFor("SerielNumber", f => "PFX " + f.Commerce.Ean8())
                .RuleFor("PurchaseId", f => f.Commerce.Ean13())
                .RuleFor("Title", f => f.Company.CompanyName())
                .RuleFor("User", f => f.Name.FullName())
                .RuleFor("AssignedTo", f => f.Name.FullName())
                .RuleFor("CreatedBy", f => f.Name.FullName())
                .RuleFor("CreateDate", f => f.Date.Past())
                .RuleFor("Status", f => f.PickRandom(new[] { "active", "disactive" }))
                .RuleFor("Priority", f => f.PickRandom(new[] { "high", "medium", "low" }));

            poh = pohFaker.Generate(count);

            _context.PurchaseOrderHistories.AddRange(poh);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
