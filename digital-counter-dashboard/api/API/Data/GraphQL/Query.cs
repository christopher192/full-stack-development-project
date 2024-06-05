using Microsoft.EntityFrameworkCore;
using API.Data.Models;
using HotChocolate.AspNetCore.Authorization;
using API.DTO;
using API.Services;

namespace API.Data.GraphQL
{
    public class Query
    {
        [Serial]
        public async Task<List<UtgSasmMachineDTO>> GetAllMachine([Service] ISchottWPSRepository macService)
        {
            return await macService.GetAll();
        }

        [Serial]
        public async Task<List<AppProcessWutgBreakingInspectionDTO>> GetAllTest2([Service] ISchottWPSRepository macService)
        {
            return await macService.GetAllTest();
        }

        [Serial]
/*        [UsePaging]
        [UseFiltering]
        [UseSorting]*/
        public async Task<List<AppDcTargetMachineDTO>> GetMachineTarget([Service] ISchottWPSRepository macService, string mode)
        {
            return await macService.GetMachineTarget(mode);
        }

        [Serial]
        public async Task<ApiResult<AppDcMachineDTO>> GetMachineTable([Service] ISchottWPSRepository macService, ApiRequest apiRequest)
        {
            return await macService.GetMachineTable(apiRequest);
        }


        [Serial]
        public async Task<ApiResult<AppDcTargetDTO>> GetTargetTable([Service] ISchottWPSRepository macService, ApiRequest apiRequest)
        {
            return await macService.GetTargetTable(apiRequest);
        }


        [Serial]
        public async Task<ApiResult<AppDcGroupShiftDTO>> GetGroupShiftTable([Service] ISchottWPSRepository macService, ApiRequest apiRequest)
        {
            return await macService.GetGroupShiftTable(apiRequest);
        }

        [Serial]
        public async Task<ApiResult<AppDcGroupDTO>> GetGroupTable([Service] ISchottWPSRepository macService, ApiRequest apiRequest)
        {
            return await macService.GetGroupTable(apiRequest);
        }

        [Serial]
        public async Task<ApiResult<AppDcShiftDTO>> GetShiftTable([Service] ISchottWPSRepository macService, ApiRequest apiRequest)
        {
            return await macService.GetShiftTable(apiRequest);
        }

    }
}
