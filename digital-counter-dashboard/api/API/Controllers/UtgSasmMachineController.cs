using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.DTO;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtgSasmMachineController : ControllerBase
    {
        private readonly ISchottWPSRepository _utgSasmMachine;

        public UtgSasmMachineController(ISchottWPSRepository utgSasmMachine)
        {
            _utgSasmMachine = utgSasmMachine;
        }

        [HttpGet]
        public async Task<List<AppDcTargetMachineDTO>> Get(string mode)
        {
            return await _utgSasmMachine.GetMachineTarget(mode);
        }
    }
}
