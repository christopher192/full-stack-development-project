using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.JSONModel {
    public class FormListHistoryJSON
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; } = String.Empty;
        public string? FormDescription { get; set; }
        public string FormStatus { get; set; } = String.Empty;
        public string FormRevision { get; set; } = String.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = String.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string JSON { get; set; } = String.Empty;
    }
}
