namespace Rtippo6Sivkov.Models;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Inn { get; set; }
    public string Kpp { get; set; }
    public string Address { get; set; }
    public bool IsPhysical { get; set; }
    public OrganizationType Type { get; set; }
    public Locality Locality { get; set; }
    public Locality? Administering { get; set; }
}