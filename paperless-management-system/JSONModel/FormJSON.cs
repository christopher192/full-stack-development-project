namespace WD_ERECORD_CORE.JSONModel
{
    public class FormJSON
    {
        public int Id { get; set; }

        public string FormName { get; set; } = String.Empty;

        public string? FormDescription { get; set; }

        public string FormStatus { get; set; } = String.Empty;

        public string FormRevision { get; set; } = String.Empty;

        public string? JSON { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; } = String.Empty;

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
