using Newtonsoft.Json;

namespace Rtippo6Sivkov.Models;

public class Locality
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [JsonIgnore]
    public Organization Administration { get; set; }
    
    [JsonIgnore]
    public ICollection<Organization> OrganizationsWithThisLocality { get; set; }
}