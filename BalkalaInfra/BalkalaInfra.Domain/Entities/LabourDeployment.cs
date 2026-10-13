namespace BalkalaInfra.Domain.Entities;

public sealed class LabourDeployment
{
    public int LabourDeploymentId { get; set; }
    public int TenantId { get; set; }
    public int ProjectId { get; set; }
    public int? TaskId { get; set; }
    public int WorkerId { get; set; }
    public DateOnly WorkDate { get; set; }
    public int ShiftId { get; set; }
    public decimal? HoursWorked { get; set; }
    public decimal? QuantityCompleted { get; set; }
    public decimal? Rate { get; set; }
}
