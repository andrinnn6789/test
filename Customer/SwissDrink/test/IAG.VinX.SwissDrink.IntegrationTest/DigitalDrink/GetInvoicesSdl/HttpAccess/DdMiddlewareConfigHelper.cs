namespace IAG.VinX.SwissDrink.IntegrationTest.DigitalDrink.GetInvoicesSdl.HttpAccess;

public static class DdMiddlewareConfigHelper
{
    private static readonly bool UseRealSystem = false;
    private static readonly string LiveDdMiddlewareUrl = "https://digitaldrink.net/api/v1/mid";
    private static readonly string UserName = "api_swissdrink";
    private static readonly string Password = "b8E5!h5pxivRaK^$nEdt";

    // Set data you want to test...
    private static readonly string CustomerNumber = "9999";
    private static readonly string InvoiceNumber = "9999";
    private static readonly string DeliveryNumber = "9999";


    public static DdMiddlewareConfig Config
    {
        get
        {
            var config = new DdMiddlewareConfig()
            {
                UseRealSystem = UseRealSystem,
                UserName = UserName,
                Password = Password
            };
            if (UseRealSystem)
            {
                config.BaseUrl = LiveDdMiddlewareUrl;
                config.CustomerNumber = CustomerNumber;
                config.InvoiceNumber = InvoiceNumber;
                config.DeliveryNumber = DeliveryNumber;
            }
            else
            {
                config.CustomerNumber = "theCustomerNumber";
            }

            return config;
        }
    }
}

public class DdMiddlewareConfig
{
    public bool UseRealSystem { get; init; }
    public string KestrelMockRequestJson { get; set; }
    public string BaseUrl { get; set; }
    public string UserName { get; init; }
    public string Password { get; init; }
    public string CustomerNumber { get; set; }
    public string InvoiceNumber { get; set; }
    public string DeliveryNumber { get; set; }
}