using Newtonsoft.Json;

namespace Rtippo6Sivkov.Models;

public class OrganizationType
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [JsonIgnore]
    public ICollection<Organization> OrganizationsWithThisType { get; set; }
}