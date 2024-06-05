namespace WD_ERECORD_CORE.JSONModel
{
    public class AllMasterFormHistoryJSON
    {
        public int Id { get; set; }
        public string MasterFormName { get; set; } = String.Empty;
        public string MasterFormDescription { get; set; } = String.Empty;
        public string MasterFormStatus { get; set; } = String.Empty;
        public string MasterFormRevision { get; set; } = String.Empty;
        public string JSON { get; set; } = String.Empty;
        public string ChangeLog { get; set; } = String.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = String.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; } = String.Empty;
    }
}
