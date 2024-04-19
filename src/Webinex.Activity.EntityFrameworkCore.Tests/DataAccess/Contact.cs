namespace Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

public class Contact
{
    public Phone PrimaryPhone { get; protected set; } = null!;
    public Phone? AdditionalPhone { get; protected set; }

    public Contact(Phone primaryPhone, Phone? additionalPhone)
    {
        PrimaryPhone = primaryPhone;
        AdditionalPhone = additionalPhone;
    }

    protected Contact()
    {
    }

    public void MutatePhone(Phone phone)
    {
        PrimaryPhone = phone;
    }

    public void MutatePhoneValue(string phone)
    {
        PrimaryPhone.MutateValue(phone);
    }

    public void MutateAdditionalPhone(Phone? phone)
    {
        AdditionalPhone = phone;
    }

    public Contact Clone() => new(PrimaryPhone.Clone(), AdditionalPhone?.Clone());

    public static Contact Random(bool additionalPhoneNull = false) =>
        new(Phone.Random(), additionalPhoneNull ? null : Phone.Random());

    public IDictionary<string, object?> ToDictionary()
    {
        return new Dictionary<string, object?>
        {
            [nameof(PrimaryPhone)] = PrimaryPhone.ToDictionary(),
            [nameof(AdditionalPhone)] = AdditionalPhone?.ToDictionary(),
        };
    }
}