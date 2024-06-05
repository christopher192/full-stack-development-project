namespace WD_ERECORD_CORE.JSONModel
{
    public class MasterFormJSON
    {
        public int Id { get; set; }
        public string? MasterFormName { get; set; }
        public string? MasterFormDescription { get; set; }
        public string? MasterFormRevision { get; set; }
        public string? MasterFormStatus { get; set; }
        public string? Departments { get; set; }
        public string? Owner { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string? SubmittedBy { get; set; }
        public string? Editor { get; set; }
        public string? ChangeLog { get; set; }
        public string? TimeLineData { get; set; }
        public bool AllowUpRevision { get; set; }
    }
}
