namespace CarService.Web.ViewModels
{
    public class AppointmentListViewModel
    {
        public int      Id { get; set; }
        public string   CustomerName   { get; set; } = string.Empty;
        public string   EmployeeName   { get; set; } = string.Empty;
        public string   ServiceType    { get; set; } = string.Empty;
        public DateTime AppointmentDate{ get; set; }
        public decimal  Price          { get; set; }
        public string   Status         { get; set; } = string.Empty;
    }
}
