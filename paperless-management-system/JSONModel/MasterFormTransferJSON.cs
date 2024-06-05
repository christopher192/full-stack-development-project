namespace WD_ERECORD_CORE.JSONModel
{
    public class MasterFormTransferJSON
    {
        public int Id { get; set; }
        public string? MasterFormName { get; set; }
        public string? MasterFormDescription { get; set; }
        public string? MasterFormRevision { get; set; }
        public string? MasterFormStatus { get; set; }
        public string? Owner { get; set; }
        public string? CreatedBy { get; set; }
    }
}
