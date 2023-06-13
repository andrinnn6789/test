namespace IAG.PerformX.CampusSursee.Dto.Registration;

public class RegistrationWithAddress
{
    public RegistrationPending Registration { get; set; }
    public RegistrationAddress RegistrationAddress { get; set; }
    public RegistrationAddress RegistrationCompanyAddress { get; set; }
    public RegistrationAddress RegistrationBillingAddress { get; set; }
}