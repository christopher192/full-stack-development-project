namespace WD_ERECORD_CORE.JSONModel
{
    public class FormApprovalJSON
    {
        public int Id { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; } = String.Empty;
        public string FormName { get; set; } = String.Empty;
        public string? FormDescription { get; set; }
        public string FormRevision { get; set; } = String.Empty;
        public string FormStatus { get; set; } = String.Empty;
        public string ApproverStatus { get; set; } = String.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = String.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; } = String.Empty;
        public string JSON { get; set; } = String.Empty;
    }
}
