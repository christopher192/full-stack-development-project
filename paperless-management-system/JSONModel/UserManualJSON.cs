namespace WD_ERECORD_CORE.JSONModel
{
    public class UserManualJSON
    {
        public int Id { get; set; }
        public string UserManualName { get; set; }
        public string UserManualDescription { get; set; }
        public string Status { get; set; }
        public string UserManualFilePath { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LatestUpdatedDate { get; set; }
        public string LatestUpdatedBy { get; set; }
        public bool AllowCUD { get; set; } = false;
    }
}
