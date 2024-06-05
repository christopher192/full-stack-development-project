using API.DTO;
using System.Collections.Generic;
using API.Data;

namespace API.Services
{
    public interface ISchottWPSRepository
    {
        Task<List<UtgSasmMachineDTO>> GetAll();

        Task<ApiResult<AppDcMachineDTO>> GetMachineTable(ApiRequest request);

        Task<MutationApiResult<AppDcMachineDTO>> CreateMachine(AppDcMachineDTO appDcMachineDTO);

        Task<List<AppProcessWutgBreakingInspectionDTO>> GetAllTest();

        Task<MutationApiResult<AppDcMachineDTO>> EditMachine(AppDcMachineDTO appDcMachineDTO);

        Task<MutationApiResult<AppDcMachineDTO>> DeleteMachine(AppDcMachineDTO appDcMachineDTO);

        Task<List<AppDcTargetMachineDTO>> GetMachineTarget(string mode);

        Task<ApiResult<AppDcTargetDTO>> GetTargetTable(ApiRequest request);

        Task<MutationApiResult<AppDcTargetDTO>> CreateTarget(AppDcTargetDTO appDcTargetDTO);

        Task<MutationApiResult<AppDcTargetDTO>> EditTarget(AppDcTargetDTO appDcTargetDTO);

        Task<MutationApiResult<AppDcTargetDTO>> DeleteTarget(AppDcTargetDTO appDcTargetDTO);

        Task<ApiResult<AppDcGroupShiftDTO>> GetGroupShiftTable(ApiRequest request);

        Task<MutationApiResult<AppDcGroupShiftDTO>> CreateGroupShift(AppDcGroupShiftDTO appDcGroupShiftDTO);

        Task<MutationApiResult<AppDcGroupShiftDTO>> DeleteGroupShift(AppDcGroupShiftDTO appDcGroupShiftDTO);

        Task<ApiResult<AppDcGroupDTO>> GetGroupTable(ApiRequest request);

        Task<ApiResult<AppDcShiftDTO>> GetShiftTable(ApiRequest request);
    }
}
