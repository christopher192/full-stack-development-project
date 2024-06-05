namespace WD_ERECORD_CORE.JSONModel
{
    public class MasterFormCCBApprovalJSON
    {
        public int Id { get; set; }
        public string? ApproverName { get; set; }
        public int ApproverId { get; set; }
        public string? MasterFormName { get; set; }
        public string? MasterFormDescription { get; set; }
        public string? MasterFormRevision { get; set; }
        public string? MasterFormStatus { get; set; }
        public string? ApproverStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string? SubmittedBy { get; set; }
        public string? DepartmentName { get; set; }
        public string? ChangeLog { get; set; }
        public string? Remark { get; set; }
        public string? TimeLineData { get; set; }
    }
}
