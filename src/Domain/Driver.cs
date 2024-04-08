namespace Domain;

public class Driver
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public ICollection<Mission>? Missions { get; set; }
}
