// ViewModels/DashboardViewModel.cs
namespace CarService.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalServices { get; set; }
        public List<string> MonthLabels { get; set; } = new();
        public List<int> NewCustomersPerMonth { get; set; } = new();
        
        public List<string> ServiceTypeLabels { get; set; } = new();
        public List<int> ServiceTypeCounts { get; set; } = new();

        public string TopEmployeeName { get; set; } = "";
        public int TopEmployeeAppointments { get; set; }
        public string TopCustomerName { get; set; } = "";
        public int TopCustomerAppointments { get; set; }
    }
}
