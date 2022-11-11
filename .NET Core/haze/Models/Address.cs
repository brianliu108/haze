namespace haze.Models;

public class Address
{
    public int Id { get; set; }
    public string StreetAddress { get; set; }
    public string? UnitApt { get; set; }
    public string City { get; set; }
    public string PostalZipCode { get; set; }
    public string Phone { get; set; }
    public string Country { get; set; }
    public string ProvinceState { get; set; }
}