namespace WD_ERECORD_CORE.JSONModel
{
    public class UserJSON
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeType { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string ManagerName { get; set; }
        public string ManagerId { get; set; }
        public string CostCenterName { get; set; }
        public string CostCenterId { get; set; }
        public string RoleName { get; set; }
        public List<string> SystemRoles { get; set; }
    }
}
