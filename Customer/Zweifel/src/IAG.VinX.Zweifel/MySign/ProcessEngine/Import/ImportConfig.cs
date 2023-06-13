using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Import;

public class ImportConfig : JobConfig<ImportJob>
{
    public string ConnectionString { get; set; } = "$$sybaseConnection$";

    public string ImportFolder { get; set; } = "$$exchangeRoot$\\Import";

    public string BackupFolder { get; set; } = "$$exchangeRoot$\\Import\\Sicherung";

    public int ShippingCostRef { get; set; } = 2000026;

    public int PaymentMethodRef { get; set; } = 2;

    public int PamenyConditionRef { get; set; } = 3;
}