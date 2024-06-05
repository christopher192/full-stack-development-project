using Dapper;
using API.DTO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using API.Data;
using System;
using HotChocolate;
using HotChocolate.Utilities;
using static HotChocolate.ErrorCodes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Services
{
    public class SchottWPSRepository : ISchottWPSRepository
    {
        private readonly IConfiguration _configuration;

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("SchottWPSConnection"));
            }
        }

        public SchottWPSRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<UtgSasmMachineDTO>> GetAll()
        {
            using (var conn = Connection)
            {
                var query = "SELECT * FROM app_Process_WUTG_SASM_Machine";
                var result = await conn.QueryAsync<UtgSasmMachineDTO>(query);
                
                return result.ToList();
            }
        }

        public async Task<List<AppProcessWutgBreakingInspectionDTO>> GetAllTest()
        {
            using (var conn = Connection)
            {
                var query = "SELECT * FROM app_Process_WUTG_Breaking_Inspection";
                var result = await conn.QueryAsync<AppProcessWutgBreakingInspectionDTO>(query);

                return result.ToList();
            }
        }

        public async Task<ApiResult<AppDcMachineDTO>> GetMachineTable(ApiRequest request)
        {
            using (var conn = Connection)
            {
                var apiResult = new ApiResult<AppDcMachineDTO>();

                var query = @"SELECT * FROM app_DC_Machine" + " ORDER BY " + request.SortCol + " " + request.SortDir.ToUpper();
                query += " OFFSET " + (request.Page - 1) * request.PerPage + " ROWS";
                query += " FETCH NEXT " + request.PerPage + " ROWS ONLY";

                var result = await conn.QueryAsync<AppDcMachineDTO>(query);

                apiResult.Result = result.ToList();

                var mssqlCount = await conn.QueryAsync<MSSQLCount>("SELECT COUNT(*) as Total FROM app_DC_Machine");
                apiResult.Total = mssqlCount.FirstOrDefault().Total;

                return apiResult;
            }
        }

        public async Task<MutationApiResult<AppDcMachineDTO>> CreateMachine(AppDcMachineDTO appDcMachineDTO)
        {
            var mutationResult = new MutationApiResult<AppDcMachineDTO>();
            mutationResult.Result = new AppDcMachineDTO();

            using (var conn = Connection)
            {
                var command = @"INSERT INTO app_DC_Machine(machine_name, dimension, date_created, status) VALUES (@Machine_Name, @Dimension, @Date_Created, @Status)";
                var saved = await conn.ExecuteAsync(command, param: appDcMachineDTO);

                if (saved == 1) {
                    mutationResult.Result = appDcMachineDTO;
                    mutationResult.Status = true;
                }

                return mutationResult;
            }
        }

        public async Task<MutationApiResult<AppDcMachineDTO>> EditMachine(AppDcMachineDTO appDcMachineDTO)
        {
            var mutationResult = new MutationApiResult<AppDcMachineDTO>();
            mutationResult.Result = new AppDcMachineDTO();

            using (var conn = Connection)
            {
                appDcMachineDTO.Last_Updated = DateTime.Now;

                var command = @"UPDATE app_DC_Machine SET machine_name = @Machine_Name, dimension = @Dimension, date_created = @Date_Created, status = @Status, last_updated = @Last_Updated WHERE Id = @Id";
                var saved = await conn.ExecuteAsync(command, param: appDcMachineDTO);

                if (saved == 1)
                {
                    mutationResult.Result = appDcMachineDTO;
                    mutationResult.Status = true;
                }

                return mutationResult;
            }
        }

        public async Task<MutationApiResult<AppDcMachineDTO>> DeleteMachine(AppDcMachineDTO appDcMachineDTO)
        {
            var mutationResult = new MutationApiResult<AppDcMachineDTO>();
            mutationResult.Result = new AppDcMachineDTO();

            using (var conn = Connection)
            {
                var command = @"DELETE FROM app_DC_Machine WHERE id = @Id";
                var saved = await conn.ExecuteAsync(command, param: appDcMachineDTO);

                if (saved == 1)
                {
                    mutationResult.Result = appDcMachineDTO;
                    mutationResult.Status = true;
                }

                return mutationResult;
            }
        }

        public async Task<List<AppDcTargetMachineDTO>> GetMachineTarget(string mode)
        {
            using (var conn = Connection)
            {
                var shiftQuery = "";
                
                if (mode == "Morning Shift")
                {
                    shiftQuery = "Morning";
                }
                else if (mode == "Afternoon Shift")
                {
                    shiftQuery = "Afternoon";
                }
                else if (mode == "Night Shift")
                {
                    shiftQuery = "Night";
                }

                var cte1Query = "SELECT machine, SUM(total_input) AS sum_total_input, SUM(good_operator_1) AS sum_good_operator_1, SUM(good_operator_2) AS sum_good_operator_2, SUM(total_good) AS sum_total_good FROM [SCHOTT_WPS].[dbo].[app_Process_WUTG_Breaking_Inspection] WHERE machine IS NOT NULL AND shift = '" + shiftQuery + "' AND start_end_time >= CONVERT(DATE, GETDATE()) AND start_end_time < CONVERT(DATE, DATEADD(DAY, 1, GETDATE())) GROUP BY machine";
                var cte2Query = "SELECT machine, SUM(total_good) AS sum_total_good_cutting FROM [SCHOTT_WPS].[dbo].[app_Process_WUTG_Bead_Cutting] WHERE machine IS NOT NULL AND shift = '"+ shiftQuery + "' AND start_end_time >= CONVERT(DATE, GETDATE()) AND start_end_time < CONVERT(DATE, DATEADD(DAY, 1, GETDATE())) GROUP BY machine";
                var cteCombined = "SELECT COALESCE(cte2.machine, cte1.machine) as machine_name, cte1.sum_total_input, cte1.sum_good_operator_1, cte1.sum_good_operator_2, cte1.sum_total_good, cte2.sum_total_good_cutting FROM cte1 FULL OUTER JOIN cte2 ON cte1.machine = cte2.machine";
                var lastQuery = "SELECT tar_tb.id, tar_tb.target_morning, tar_tb.target_afternoon, tar_tb.target_night, mac_tb.machine_name, mac_tb.dimension, mac_tb.status, cte_combined.sum_total_input, cte_combined.sum_good_operator_1, cte_combined.sum_good_operator_2, cte_combined.sum_total_good, cte_combined.sum_total_good_cutting FROM app_DC_Target tar_tb INNER JOIN app_DC_Machine mac_tb ON tar_tb.machine_id = mac_tb.id AND tar_tb.date >= CONVERT(DATE, GETDATE()) AND tar_tb.date < CONVERT(DATE, DATEADD(DAY, 1, GETDATE())) FULL OUTER JOIN cte_combined ON cte_combined.machine_name = mac_tb.machine_name;";
                
                // var query = "SELECT tar_tb.id, tar_tb.target_morning, tar_tb.target_afternoon, tar_tb.target_night, mac_tb.machine_name, mac_tb.dimension, mac_tb.status FROM app_DC_Target tar_tb INNER JOIN app_DC_Machine mac_tb ON tar_tb.machine_id =  mac_tb.id";
                var query = String.Format("WITH cte1 AS ({0}), cte2 AS ({1}), cte_combined AS ({2}){3}", cte1Query, cte2Query, cteCombined, lastQuery);
                var result = await conn.QueryAsync<AppDcTargetMachineDTO>(query);

                return result.OrderBy(x => x.Machine_Name).ToList();
            }
        }

        public async Task<ApiResult<AppDcTargetDTO>> GetTargetTable(ApiRequest request)
        {
            using (var conn = Connection)
            {
                var apiResult = new ApiResult<AppDcTargetDTO>();

                var query = @"SELECT * FROM app_DC_Target" + " ORDER BY " + request.SortCol + " " + request.SortDir.ToUpper();
                query += " OFFSET " + (request.Page - 1) * request.PerPage + " ROWS";
                query += " FETCH NEXT " + request.PerPage + " ROWS ONLY";

                var result = await conn.QueryAsync<AppDcTargetDTO>(query);

                apiResult.Result = result.ToList();

                var mssqlCount = await conn.QueryAsync<MSSQLCount>("SELECT COUNT(*) as Total FROM app_DC_Target");
                apiResult.Total = mssqlCount.FirstOrDefault().Total;

                return apiResult;
            }
        }

        public async Task<MutationApiResult<AppDcTargetDTO>> CreateTarget(AppDcTargetDTO appDcTargetDTO)
        {
            var mutationResult = new MutationApiResult<AppDcTargetDTO>();
            mutationResult.Result = new AppDcTargetDTO();

            using (var conn = Connection)
            {
                try
                {
                    var command = @"INSERT INTO app_DC_Target(machine_id, target_morning, target_afternoon, target_night, date) VALUES (@machine_id, @target_morning, @target_afternoon, @target_night, @date)";
                    var saved = await conn.ExecuteAsync(command, param: appDcTargetDTO);
                    var i = saved;

                    if (saved == 1)
                    {
                        mutationResult.Result = appDcTargetDTO;
                        mutationResult.Status = true;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }

            }
            return mutationResult;
        }

        public async Task<MutationApiResult<AppDcTargetDTO>> EditTarget(AppDcTargetDTO appDcTargetDTO)
        {
            var mutationResult = new MutationApiResult<AppDcTargetDTO>();
            mutationResult.Result = new AppDcTargetDTO();

            using (var conn = Connection)
            {

                var command = @"UPDATE app_DC_Target SET machine_id = @Machine_Id, date = @Date, target_morning = @Target_Morning, target_afternoon = @Target_Afternoon, target_night = @Target_Night WHERE Id = @Id";
                var saved = await conn.ExecuteAsync(command, param: appDcTargetDTO);

                if (saved == 1)
                {
                    mutationResult.Result = appDcTargetDTO;
                    mutationResult.Status = true;
                }

                return mutationResult;
            }
        }

        public async Task<MutationApiResult<AppDcTargetDTO>> DeleteTarget(AppDcTargetDTO appDcTargetDTO)
        {
            var mutationResult = new MutationApiResult<AppDcTargetDTO>();
            mutationResult.Result = new AppDcTargetDTO();

            using (var conn = Connection)
            {
                var command = @"DELETE FROM app_DC_Target WHERE id = @Id";
                var saved = await conn.ExecuteAsync(command, param: appDcTargetDTO);

                if (saved == 1)
                {
                    mutationResult.Result = appDcTargetDTO;
                    mutationResult.Status = true;
                }

                return mutationResult;
            }
        }

        public async Task<ApiResult<AppDcGroupShiftDTO>> GetGroupShiftTable(ApiRequest request)
        {
            using (var conn = Connection)
            {
                var apiResult = new ApiResult<AppDcGroupShiftDTO>();

                var query = @"SELECT * FROM app_DC_Group_Shift" + " ORDER BY " + request.SortCol + " " + request.SortDir.ToUpper();
                query += " OFFSET " + (request.Page - 1) * request.PerPage + " ROWS";
                query += " FETCH NEXT " + request.PerPage + " ROWS ONLY";

                var result = await conn.QueryAsync<AppDcGroupShiftDTO>(query);

                apiResult.Result = result.ToList();

                var mssqlCount = await conn.QueryAsync<MSSQLCount>("SELECT COUNT(*) as Total FROM app_DC_Group_Shift");
                apiResult.Total = mssqlCount.FirstOrDefault().Total;

                return apiResult;
            }
        }

        public async Task<MutationApiResult<AppDcGroupShiftDTO>> CreateGroupShift(AppDcGroupShiftDTO appDcGroupShiftDTO)
        {
            var mutationResult = new MutationApiResult<AppDcGroupShiftDTO>();
            mutationResult.Result = new AppDcGroupShiftDTO();

            using (var conn = Connection)
            {
                try
                {
                    var command = @"INSERT INTO app_DC_Group_Shift(date, shift_id, group_id, status) VALUES (@date, @shift_id, @group_id, @status)";
                    var saved = await conn.ExecuteAsync(command, param: appDcGroupShiftDTO);
                    var i = saved;

                    if (saved == 1)
                    {
                        mutationResult.Result = appDcGroupShiftDTO;
                        mutationResult.Status = true;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }

            }
            return mutationResult;
        }

        public async Task<MutationApiResult<AppDcGroupShiftDTO>> DeleteGroupShift(AppDcGroupShiftDTO appDcGroupShiftDTO)
        {
            var mutationResult = new MutationApiResult<AppDcGroupShiftDTO>();
            mutationResult.Result = new AppDcGroupShiftDTO();

            using (var conn = Connection)
            {
                var command = @"DELETE FROM app_DC_Group_Shift WHERE id = @Id";
                var saved = await conn.ExecuteAsync(command, param: appDcGroupShiftDTO);

                if (saved == 1)
                {
                    mutationResult.Result = appDcGroupShiftDTO;
                    mutationResult.Status = true;
                }

                return mutationResult;
            }
        }

        public async Task<ApiResult<AppDcGroupDTO>> GetGroupTable(ApiRequest request)
        {
            using (var conn = Connection)
            {
                var apiResult = new ApiResult<AppDcGroupDTO>();

                var query = @"SELECT * FROM app_DC_Group" + " ORDER BY " + request.SortCol + " " + request.SortDir.ToUpper();
                query += " OFFSET " + (request.Page - 1) * request.PerPage + " ROWS";
                query += " FETCH NEXT " + request.PerPage + " ROWS ONLY";

                var result = await conn.QueryAsync<AppDcGroupDTO>(query);

                apiResult.Result = result.ToList();

                var mssqlCount = await conn.QueryAsync<MSSQLCount>("SELECT COUNT(*) as Total FROM app_DC_Group");
                apiResult.Total = mssqlCount.FirstOrDefault().Total;

                return apiResult;
            }
        }

        public async Task<ApiResult<AppDcShiftDTO>> GetShiftTable(ApiRequest request)
        {
            using (var conn = Connection)
            {
                var apiResult = new ApiResult<AppDcShiftDTO>();

                var query = @"SELECT * FROM app_DC_Shift" + " ORDER BY " + request.SortCol + " " + request.SortDir.ToUpper();
                query += " OFFSET " + (request.Page - 1) * request.PerPage + " ROWS";
                query += " FETCH NEXT " + request.PerPage + " ROWS ONLY";

                var result = await conn.QueryAsync<AppDcShiftDTO>(query);

                apiResult.Result = result.ToList();

                var mssqlCount = await conn.QueryAsync<MSSQLCount>("SELECT COUNT(*) as Total FROM app_DC_Shift");
                apiResult.Total = mssqlCount.FirstOrDefault().Total;

                return apiResult;
            }
        }
    }
}
