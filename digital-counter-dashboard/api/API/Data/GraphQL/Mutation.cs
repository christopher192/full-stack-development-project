using Microsoft.EntityFrameworkCore;
using API.Data.Models;
using System.Data;
using HotChocolate.Subscriptions;
using API.DTO;
using API.Services;

namespace API.Data.GraphQL
{
    public class Mutation
    {
        [Serial]
        public async Task<MutationApiResult<AppDcMachineDTO>> AddMachine([Service] ISchottWPSRepository macService, AppDcMachineDTO appDcMachineDTO)
        {
            return await macService.CreateMachine(appDcMachineDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcMachineDTO>> EditMachine([Service] ISchottWPSRepository macService, AppDcMachineDTO appDcMachineDTO)
        {
            return await macService.EditMachine(appDcMachineDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcMachineDTO>> DeleteMachine([Service] ISchottWPSRepository macService, AppDcMachineDTO appDcMachineDTO)
        {
            return await macService.DeleteMachine(appDcMachineDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcTargetDTO>> AddTarget([Service] ISchottWPSRepository macService, AppDcTargetDTO appDcTargetDTO)
        {
            return await macService.CreateTarget(appDcTargetDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcTargetDTO>> EditTarget([Service] ISchottWPSRepository macService, AppDcTargetDTO appDcTargetDTO)
        {
            return await macService.EditTarget(appDcTargetDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcTargetDTO>> DeleteTarget([Service] ISchottWPSRepository macService, AppDcTargetDTO appDcTargetDTO)
        {
            return await macService.DeleteTarget(appDcTargetDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcGroupShiftDTO>> AddGroupShift([Service] ISchottWPSRepository macService, AppDcGroupShiftDTO appDcGroupShiftDTO)
        {
            return await macService.CreateGroupShift(appDcGroupShiftDTO);
        }

        [Serial]
        public async Task<MutationApiResult<AppDcGroupShiftDTO>> DeleteGroupShift([Service] ISchottWPSRepository macService, AppDcGroupShiftDTO appDcGroupShiftDTO)
        {
            return await macService.DeleteGroupShift(appDcGroupShiftDTO);
        }
    }
}
