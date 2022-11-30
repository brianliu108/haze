using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace haze.Models;

public class Friend
{
    public int Id { get; set; }
    public virtual User User1 { get; set; }
    public virtual User User2 { get; set; }
    public FriendStatus Status { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateAccepted { get; set; }
    public bool User1IsFamily { get; set; }
    public bool User2IsFamily { get; set; }
}