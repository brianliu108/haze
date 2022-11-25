using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace haze.Models;

public class Friend
{
    public int Id { get; set; }
    /// <summary>
    /// The friend on the owner of this Friend object's friend list
    /// </summary>
    public virtual User User { get; set; }
    public bool Accepted { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateAccepted { get; set; }
    public bool IsFamily { get; set; }
}