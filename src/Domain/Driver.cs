namespace Domain;

public class Driver
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public string ApiKey { get; set; }

    public ICollection<Mission>? Missions { get; set; }

    public Driver(string name, string lastName)
    {
        Id = Guid.NewGuid().ToString();

        ApiKey = Guid.NewGuid().ToString();

        Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");

        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName), "LastName cannot be null.");
    }
}
