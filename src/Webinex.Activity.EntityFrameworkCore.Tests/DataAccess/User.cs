namespace Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

public class User
{
    private readonly List<Contact> _contacts = new();
    private readonly List<User> _related = new();

    public Guid Id { get; protected init; }
    public string Name { get; protected set; } = null!;
    public Contact Contact { get; protected set; } = null!;
    public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();
    public Guid? CreatedById { get; protected set; }
    public IReadOnlyCollection<User> Related => _related.AsReadOnly();

    public User(string name, Contact contact, IEnumerable<Contact> contacts, Guid? createdById, IEnumerable<User> related)
    {
        Id = Guid.NewGuid();
        Name = name;
        Contact = contact;
        _contacts.AddRange(contacts);
        CreatedById = createdById;
        _related.AddRange(related);
    }

    protected User()
    {
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void ReplacePhone(Phone phone)
    {
        Contact = new Contact(phone.Clone(), Contact.AdditionalPhone?.Clone());
    }

    public void MutateAdditionalPhone(Phone? additionalPhone)
    {
        Contact.MutateAdditionalPhone(additionalPhone?.Clone());
    }

    public void MutatePhoneValue(string value)
    {
        Contact.PrimaryPhone.MutateValue(value);
    }

    public void AddContact(Contact contact)
    {
        _contacts.Add(contact.Clone());
    }

    public void RemoveContact(int index)
    {
        _contacts.RemoveAt(index);
    }

    public void ReplaceContact(int index, Contact contact)
    {
        _contacts[index] = contact;
    }

    public void MutateContactPhone(int index, Phone value)
    {
        _contacts[index].MutatePhone(value);
    }

    public void MutateContactPhoneValue(int index, string value)
    {
        _contacts[index].MutatePhoneValue(value);
    }

    public void AddRelated(User user)
    {
        _related.Add(user);
    }

    public static User Random(int contactCount = 2, Guid? createdById = null, IEnumerable<User>? related = null) => new(
        name: Guid.NewGuid().ToString(),
        contact: Contact.Random(),
        contacts: Enumerable.Range(0, contactCount).Select(_ => Contact.Random()).ToArray(),
        createdById: createdById,
        related: related ?? Array.Empty<User>());

    public IDictionary<string, object?> Values()
    {
        return new Dictionary<string, object?>
        {
            [nameof(Id)] = Id,
            [nameof(Name)] = Name,
            [nameof(Contact)] = Contact.ToDictionary(),
            [nameof(Contacts)] = _contacts.Select(c => c.ToDictionary()).ToArray(),
            [nameof(CreatedById)] = CreatedById,
        };
    }

    public static EntityChange EntityChange(
        EntityChangeType type,
        Guid id,
        IDictionary<string, object?> values,
        IDictionary<string, object?>? originalValues)
    {
        return new EntityChange(
            type,
            nameof(User),
            new Dictionary<string, object?> { ["Id"] = id },
            values,
            originalValues);
    }
}